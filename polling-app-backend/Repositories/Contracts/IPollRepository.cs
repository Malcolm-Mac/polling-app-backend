namespace polling_app_backend.Repositories.Contracts;

public interface IPollRepository
{
    Task<Poll?> GetPollByIdAsync(int pollId);
    Task<IEnumerable<Poll>> GetAllPollsAsync();
    Task<Poll> CreatePollAsync(Poll poll);
    Task<bool> UpdatePollAsync(Poll poll);
    Task<bool> DeletePollAsync(int pollId);
}
