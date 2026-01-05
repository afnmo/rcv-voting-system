using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Data;
using VotingSystem.Services;
using VotingSystem.Models;

namespace VotingSystem.Controllers;

[ApiController]
[Route("api/votes/{voteId}/results")]
public class ResultsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly RcvCountingService _rcvService;

    public ResultsController(AppDbContext context, RcvCountingService rcvService)
    {
        _context = context;
        _rcvService = rcvService;
    }

    [HttpGet]
    public async Task<IActionResult> GetResults(Guid voteId)
    {
        var vote = await _context.Votes
            .Include(v => v.Options)
            .Include(v => v.Ballots)
            .ThenInclude(b => b.Rankings)
            .FirstOrDefaultAsync(v => v.VoteId == voteId);

        if (vote == null)
            return NotFound("Vote not found");

        if (vote.Status != VoteStatus.Closed && vote.Status != VoteStatus.Completed)
            return BadRequest("Vote must be closed before viewing results");

        if (!vote.Ballots.Any())
        {
            vote.Status = VoteStatus.Completed;
            vote.WinningOptionId = null;
            vote.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                vote.VoteId,
                Result = "NoVotes"
            });
        }


        var result = _rcvService.CalculateWinner(vote);

        if (vote.Status != VoteStatus.Completed)
        {
            vote.WinningOptionId = result.WinningOptionId;
            vote.Status = VoteStatus.Completed;
            vote.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return Ok(new
        {
            vote.VoteId,
            WinnerOptionId = result.WinningOptionId,
            Rounds = result.Rounds
        });
    }
}
