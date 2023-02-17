namespace LinkShortener.Domain.Models;

public class Link: BaseEntity
{
    public int Id { get; set; }
    public string InitialLink { get; set; }
    public string ShortLink { get; set; }
    public bool IsDeleted { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    
}