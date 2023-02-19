using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LinkShortener.DAL;
using LinkShortener.Domain.Models;
using LinkShortener.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stravaig.ShortCode;

namespace LinkShortener.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]/")]
public class LinksController : ControllerBase
{
    private AppDbContext _db;

    public LinksController(AppDbContext db)
    {
        _db = db;
    }

    private async Task<User?> Authenticate()
    {
        var userIdStr = User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr == null)
            return null;

        int userId;

        if (!int.TryParse(userIdStr, out userId))
            return null;

        return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Links([FromBody] NewLinkDto newLink)
    {
        // TODO: Если найдена удалённая ссылка (IsDeleted), то не создавать новую ссылку, а сделать IsDeleted = false
        var user = await Authenticate();

        if (user == null)
            return Unauthorized(new
            {
                message = "Нет доступа"
            });

        if (await _db.Links.AnyAsync(l => l.InitialLink == newLink.Url &&
                                          !l.IsDeleted
                                          && l.UserId == user.Id))
        {
            return BadRequest(new
            {
                message = "Вы уже сократили эту ссылку"
            });
        }


        string? code = ShortCode.GenerateRandomShortCode();
        ;
        Link? dbLink = await _db.Links.FirstOrDefaultAsync(l => l.ShortCode == code &&
                                                                !l.IsDeleted);

        while (dbLink != null)
        {
            code = ShortCode.GenerateRandomShortCode();
            dbLink = await _db.Links.FirstOrDefaultAsync(l => l.ShortCode == code &&
                                                              !l.IsDeleted);
        }


        var link = new Link()
        {
            InitialLink = newLink.Url,
            ShortCode = code,
            User = user
        };

        await _db.Links.AddAsync(link);
        await _db.SaveChangesAsync();

        return Created($"/api/v{1}/links/{link.Id}", null);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Link([FromRoute] int id)
    {
        var link = await _db.Links.FirstOrDefaultAsync(l => l.Id == id
                                                            && !l.IsDeleted);

        if (link == null)
            return BadRequest(new
            {
                message = "Ссылка не найдена"
            });

        var user = await Authenticate();

        if (user == null ||
            link.UserId != user.Id)
            return BadRequest(new
            {
                message = "Нет доступа"
            });

        link.IsDeleted = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> Links([FromRoute] int id)
    {
        var link = await _db.Links
            .FirstOrDefaultAsync(l => l.Id == id &&
                !l.IsDeleted);

        if (link == null)
            return NotFound(new
            {
                message = "Ссылка не найдена"
            });

        return Ok(new
        {
            initialUrl = link.InitialLink,
            shortCode = link.ShortCode
        });
    }
}