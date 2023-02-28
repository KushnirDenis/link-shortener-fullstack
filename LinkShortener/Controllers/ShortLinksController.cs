using LinkShortener.DAL;
using LinkShortener.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Controllers;

[Route("/l/")]
public class ShortLinksController : ControllerBase
{
    private readonly AppDbContext _db;

    public ShortLinksController(AppDbContext db)
    {
        _db = db;
    }
    
    [HttpGet("{shortCode}")]
    public async Task<IActionResult> Handle([FromRoute] string shortCode)
    {
        var dbLink = await _db.Links.FirstOrDefaultAsync(l => l.ShortCode == shortCode);

        if (dbLink == null)
            return NotFound();
        
        

        var click = new Click()
        {
            IPAddress = Request.HttpContext.Connection.RemoteIpAddress?
                .MapToIPv4()
                .ToString() ?? "",
            LinkId = dbLink.Id
        };
        
        _db.Clicks.Add(click);
        
        await _db.SaveChangesAsync();
        
        return Redirect(dbLink.InitialLink);
    }
    
}