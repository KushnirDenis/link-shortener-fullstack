using System.Security.Claims;
using LinkShortener.DAL;
using LinkShortener.Domain.Models;
using LinkShortener.Localization;
using LinkShortener.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace LinkShortener.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IStringLocalizer<ErrorMessages> _localizer;
    private const int _linksPerPage = 20;

    public UsersController(AppDbContext db,
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cursor">the cursor points to the id of the last received item</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("{userId:int}/links")]
    public async Task<IActionResult> Links([FromRoute] int userId, [FromQuery] int cursor)
    {
        var user = await Authenticate();

        if (user == null || user.Id != userId)
            return BadRequest(new
            {
                message = _localizer["NoAccess"].Value
            });

        if (cursor == 1)
            return NoContent();

        // if cursor == 0, select all links
        // if cursor != 0, select links where link.Id < cursor
        var links = await _db.Links
            .OrderByDescending(l => l.CreatedAt)
            .Where(l => l.UserId == userId &&
                        !l.IsDeleted &&
                        (cursor == 0 || l.Id < cursor))
            .Take(_linksPerPage)
            .Select(l => LinkDto.MapFromLink(l))
            .ToListAsync();

        var newCursor = links.Count > 0 ? links[^1].Id : 1;
        return Ok(new UserLinksDto()
        {
            Links = links,
            Cursor = newCursor
        });
    }
}