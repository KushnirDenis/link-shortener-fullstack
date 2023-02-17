namespace LinkShortener.Domain.Models;

public class User: BaseEntity
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public List<Link> Links { get; set; }
}