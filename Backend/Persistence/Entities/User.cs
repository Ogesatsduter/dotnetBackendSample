namespace Backend.Persistence.Entities;

public class User
{
    public long Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Salt { get; set; } = string.Empty;
}