namespace LinkShortener.Domain.Models;

public class Click
{
    public int Id { get; set; }
    public int LinkId { get; set; }
    public Link Link { get; set; }
    public string IPAddress { get; set; }
}