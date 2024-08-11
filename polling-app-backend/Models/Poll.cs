namespace polling_app_backend;

public class Poll
{
    public int PollId { get; set; }
    public int UserId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public required User User { get; set; }
    public required ICollection<Question> Questions { get; set; }
}
