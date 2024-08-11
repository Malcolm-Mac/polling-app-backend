using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using polling_app_backend.Repositories.Contracts;

namespace polling_app_backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PollController : ControllerBase
{
    private readonly IPollRepository _pollRepository;
    private readonly ILogger<PollController> _logger;

    public PollController(IPollRepository pollRepository, ILogger<PollController> logger)
    {
        _pollRepository = pollRepository;
        _logger = logger;
    }

    [HttpGet("{pollId}")]
    public async Task<IActionResult> GetPoll(int pollId)
    {
        _logger.LogInformation("Getting poll with ID {PollId}", pollId);

        var poll = await _pollRepository.GetPollByIdAsync(pollId);
        if (poll == null)
        {
            _logger.LogWarning("Poll with ID {PollId} not found", pollId);
            return NotFound();
        }

        _logger.LogInformation("Successfully retrieved poll with ID {PollId}", pollId);
        return Ok(poll);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPolls()
    {
        _logger.LogInformation("Getting all polls");
        var polls = await _pollRepository.GetAllPollsAsync();
        _logger.LogInformation("Successfully retrieved {PollCount} polls", polls.Count());
        return Ok(polls);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePoll([FromBody] Poll poll)
    {
        if (poll == null)
        {
            _logger.LogWarning("Attempted to create a null poll");
            return BadRequest("Poll cannot be null");
        }
        _logger.LogInformation("Creating a new poll with title {Title}", poll.Title);
        var createdPoll = await _pollRepository.CreatePollAsync(poll);

        _logger.LogInformation("Successfully created poll with ID {PollId}", createdPoll.PollId);
        return CreatedAtAction(nameof(GetPoll), new { pollId = createdPoll.PollId }, createdPoll);
    }

    [HttpPut("{pollId}")]
    public async Task<IActionResult> UpdatePoll(int pollId, [FromBody] Poll poll)
    {
        if (poll == null || poll.PollId != pollId)
        {
            _logger.LogWarning("Invalid poll data for ID {PollId}", pollId);
            return BadRequest("Poll data is invalid");
        }

        _logger.LogInformation("Updating poll with ID {PollId}", pollId);
        var updated = await _pollRepository.UpdatePollAsync(poll);
        if (!updated)
        {
            _logger.LogWarning("Poll with ID {PollId} not found for update", pollId);
            return NotFound();
        }

        _logger.LogInformation("Successfully updated poll with ID {PollId}", pollId);
        return NoContent();
    }

    [HttpDelete("{pollId}")]
    public async Task<IActionResult> DeletePoll(int pollId)
    {
        _logger.LogInformation("Deleting poll with ID {PollId}", pollId);

        var deleted = await _pollRepository.DeletePollAsync(pollId);
        if (!deleted)
        {
            _logger.LogWarning("Poll with ID {PollId} not found for deletion", pollId);
            return NotFound();
        }
        
        _logger.LogInformation("Successfully deleted poll with ID {PollId}", pollId);
        return NoContent();
    }
}
