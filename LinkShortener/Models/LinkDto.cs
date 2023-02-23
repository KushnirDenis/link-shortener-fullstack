using LinkShortener.Domain.Models;

namespace LinkShortener.Models;

public class LinkDto
{
    public int Id { get; set; }
    public string InitialLink { get; set; }
    public string ShortCode { get; set; }

    public static LinkDto MapFromLink(Link link)
    {
        return new LinkDto()
        {
            Id = link.Id,
            InitialLink = link.InitialLink,
            ShortCode = link.ShortCode,
        };
    }
}