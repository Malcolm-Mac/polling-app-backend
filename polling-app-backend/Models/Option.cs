namespace polling_app_backend;

public class Option
{
    public int OptionId { get; set; }
    public int QuestionId { get; set; }
    public required string OptionText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required Question Question { get; set; }
    public required ICollection<Vote> Votes { get; set; }
}
