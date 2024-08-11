namespace polling_app_backend;

public class Question
{
public int QuestionId { get; set; }
    public int PollId { get; set; }
    public required string QuestionText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required Poll Poll { get; set; }
    public required ICollection<Option> Options { get; set; }
}
