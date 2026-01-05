using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Data;
using VotingSystem.DTOs;
using VotingSystem.Models;

using System.Security.Claims;


namespace VotingSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/votes/{voteId}/ballots")]
public class BallotsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BallotsController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpPost]
    public async Task<IActionResult> Submit(Guid voteId, SubmitBallotDto dto)
    {
        
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        // Vote exists & open
        var vote = await _context.Votes
            .Include(v => v.Options)
            .FirstOrDefaultAsync(v => v.VoteId == voteId);

        if (vote == null)
            return NotFound("Vote not found");

        if (vote.Status != VoteStatus.Open)
            return BadRequest("Voting is not open");
        
        // One ballot per user per vote
        bool alreadyVoted = await _context.Ballots
            .AnyAsync(b => b.VoteId == voteId && b.UserId == userId);

        if (alreadyVoted)
            return BadRequest("User already voted in this vote");

        // Validate options belong to vote
        var validOptionIds = vote.Options
            .Select(o => o.OptionId)
            .ToHashSet();

        if (dto.Rankings.Any(r => !validOptionIds.Contains(r.OptionId)))
            return BadRequest("One or more options are invalid for this vote");

        // Validate ranks (no duplicates, start at 1)
        var ranks = dto.Rankings.Select(r => r.RankNumber).ToList();

        if (ranks.Count != ranks.Distinct().Count())
            return BadRequest("Duplicate rank numbers are not allowed");

        if (ranks.Min() != 1)
            return BadRequest("Ranking must start at 1");

        // Create ballot
        var ballot = new Ballot
        {
            BallotId = Guid.NewGuid(),
            VoteId = voteId,
            UserId = userId,
            SubmittedAt = DateTime.UtcNow,
            Rankings = dto.Rankings.Select(r => new BallotRanking
            {
                BallotRankingId = Guid.NewGuid(),
                OptionId = r.OptionId,
                RankNumber = r.RankNumber
            }).ToList()
        };

        _context.Ballots.Add(ballot);
        await _context.SaveChangesAsync();

        return Ok("Ballot submitted successfully");
    }
}
