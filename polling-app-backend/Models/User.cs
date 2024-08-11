using Microsoft.AspNetCore.Identity;

namespace polling_app_backend;

public class User : IdentityUser
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required ICollection<Poll> Polls { get; set; }
}
