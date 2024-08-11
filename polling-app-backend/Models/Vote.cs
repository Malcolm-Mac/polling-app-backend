namespace polling_app_backend;

public class Vote
{
    public int VoteId { get; set; }
    public int OptionId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required Option Option { get; set; }
    public required User User { get; set; }
}
