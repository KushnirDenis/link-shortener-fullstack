using System.Security.Claims;
using LinkShortener.DAL;
using LinkShortener.Domain.Models;
using LinkShortener.Localization;
using LinkShortener.Models;
using LinkShortener.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Stravaig.ShortCode;

namespace LinkShortener.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]/")]
public class LinksController : ControllerBase
{
    private readonly AppDbContext _db;

    private readonly IStringLocalizer<ErrorMessages> _localizer;

    public LinksController(AppDbContext db,
        IStringLocalizer<ErrorMessages> localizer)
    {
        _db = db;
        _localizer = localizer;
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

    // Create a new link
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateLink([FromBody] NewLinkDto newLink)
    {
        // TODO: Если найдена удалённая ссылка (IsDeleted), то не создавать новую ссылку, а сделать IsDeleted = false
        var user = await Authenticate();

        if (user == null)
            return Unauthorized(new
            {
                message = _localizer["NoAccess"].Value
            });

        if (!UriValidator.ValidateUri(newLink.Url))
        {
            return BadRequest(new
            {
                message = _localizer["InvalidLink"].Value
            });
        }

        if (await _db.Links.AnyAsync(l => l.InitialLink == newLink.Url &&
                                          !l.IsDeleted
                                          && l.UserId == user.Id))
        {
            return BadRequest(new
            {
                message = _localizer["AlreadyShortenedLink"].Value
            });
        }


        string? code = ShortCode.GenerateRandomShortCode();

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

    // Update a link
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateLink([FromRoute] int id, [FromBody] NewLinkDto link)
    {
        var user = await Authenticate();

        if (user == null)
            return BadRequest(new
            {
                message = _localizer["NoAccess"].Value
            });
        
        if (!UriValidator.ValidateUri(link.Url))
        {
            return BadRequest(new
            {
                message = _localizer["InvalidLink"].Value
            });
        }

        var dbLink = await _db.Links.FirstOrDefaultAsync(l => l.Id == id &&
                                                              !l.IsDeleted &&
                                                              l.UserId == user.Id);

        if (dbLink == null)
            return NotFound(new
            {
                message = _localizer["LinkNotFound"].Value
            });

        if (dbLink.InitialLink == link.Url)
            return NoContent();

        dbLink.InitialLink = link.Url;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    // Delete a link
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLink([FromRoute] int id)
    {
        var user = await Authenticate();

        if (user == null)
            return BadRequest(new
            {
                message = _localizer["NoAccess"].Value
            });

        var link = await _db.Links.FirstOrDefaultAsync(l => l.Id == id
                                                            && !l.IsDeleted
                                                            && l.UserId == user.Id);

        if (link == null)
            return NotFound(new
            {
                message = _localizer["LinkNotFound"].Value
            });

        link.IsDeleted = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }


    // Get a link
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetLink([FromRoute] int id)
    {
        var link = await _db.Links
            .FirstOrDefaultAsync(l => l.Id == id &&
                                      !l.IsDeleted);

        if (link == null)
            return NotFound(new
            {
                message = _localizer["LinkNotFound"].Value
            });

        return Ok(new
        {
            initialUrl = link.InitialLink,
            shortCode = link.ShortCode
        });
    }

    [Authorize]
    [HttpGet("{linkId:int}/clicks")]
    public async Task<IActionResult> GetClicks([FromRoute] int linkId)
    {
        var user = await Authenticate();

        if (user == null)
            return BadRequest(new
            {
                message = _localizer["NoAccess"].Value
            });

        var link = await _db.Links.FirstOrDefaultAsync(l => l.Id == linkId && l.UserId == user.Id);

        if (link == null)
            return NotFound(new
            {
                message = _localizer["LinkNotFound"].Value
            });


        var clicks = await _db.Clicks.Where(c => c.LinkId == linkId)
            .Select(c => ClickDto.MapFromClick(c)).ToListAsync();

        if (clicks.Count == 0)
            return NotFound(new
            {
                message = _localizer["NoClickedOnLink"].Value
            });

        return Ok(clicks);
    }
}