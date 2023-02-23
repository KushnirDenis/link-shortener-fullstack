using LinkShortener.Domain.Models;

namespace LinkShortener.Models;

public class ClickDto
{
    public string IPAddress { get; set; }
    public DateTime ClickedAt { get; set; }

    public static ClickDto MapFromClick(Click click)
    {
        return new ClickDto()
        {
            IPAddress = click.IPAddress,
            ClickedAt = click.CreatedAt
        };
    }
}