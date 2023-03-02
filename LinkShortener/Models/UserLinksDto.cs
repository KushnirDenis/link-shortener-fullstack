namespace LinkShortener.Models;

public class UserLinksDto
{
    public List<LinkDto> Links { get; set; }
    public int Cursor { get; set; }
}