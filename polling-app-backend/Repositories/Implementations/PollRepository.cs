using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using polling_app_backend.Repositories.Contracts;

namespace polling_app_backend.Repositories.Implentations;

public class PollRepository : IPollRepository
{
    private readonly ConcurrentDictionary<int, Poll> _polls = new();
    private int _nextId = 1;
    public async Task<Poll?> GetPollByIdAsync(int pollId)
    {
        _polls.TryGetValue(pollId, out var poll);
        return await Task.FromResult(poll);
    }
    public async Task<IEnumerable<Poll>> GetAllPollsAsync()
    {
        var polls = _polls.Values;
        return await Task.FromResult(polls);
    }

    public async Task<Poll> CreatePollAsync(Poll poll)
    {
        poll.PollId = _nextId++;
        _polls[poll.PollId] = poll;
        return await Task.FromResult(poll);
    }
    public async Task<bool> UpdatePollAsync(Poll poll)
    {
        if (_polls.ContainsKey(poll.PollId))
        {
            _polls[poll.PollId] = poll;
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }
    public async Task<bool> DeletePollAsync(int pollId)
    {
        return await Task.FromResult(_polls.TryRemove(pollId, out _));
    }
}
