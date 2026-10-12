namespace CampusEats.Api.Models;

// Minimal user record carried over from Lab 04 (auth).
// Only the password HASH is ever stored -- never the password.
public class AppUser
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
