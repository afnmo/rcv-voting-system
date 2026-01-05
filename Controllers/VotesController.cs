using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Data;
using VotingSystem.DTOs;
using VotingSystem.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace VotingSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/votes")]
public class VotesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VotesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/votes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var votes = await _context.Votes.ToListAsync();
        return Ok(votes);
    }

    // GET: api/votes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {

        var vote = await _context.Votes.FindAsync(id);

        if (vote == null)
            return NotFound();

        return Ok(new
        {
            vote.VoteId,
            vote.Title,
            vote.Description,
            vote.Status,
            vote.StartTime,
            vote.EndTime,
            vote.WinningOptionId,
            vote.CompletedAt,
            Options = vote.Options.Select(o => new
            {
                o.OptionId,
                o.OptionText
            })
        });

    }

    // POST: api/votes
    [HttpPost]
    public async Task<IActionResult> Create(CreateVoteDto dto)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );
        var vote = new Vote
        {
            VoteId = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = VoteStatus.Draft,
            CreatorUserId = userId
        };

        _context.Votes.Add(vote);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = vote.VoteId }, vote);
    }


    // DELETE: api/votes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var vote = await _context.Votes.FindAsync(id);

        if (vote == null)
            return NotFound();

        _context.Votes.Remove(vote);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    
    // PUT: api/votes/{id}/open
    [HttpPut("{id}/open")]
    public async Task<IActionResult> Open(Guid id)
    {
        var vote = await _context.Votes
            .Include(v => v.Options)
            .FirstOrDefaultAsync(v => v.VoteId == id);

        if (vote == null)
            return NotFound("Vote not found");

        if (vote.Status != VoteStatus.Draft)
            return BadRequest("Only Draft votes can be opened");

        if (!vote.Options.Any())
            return BadRequest("Cannot open vote without options");

        vote.Status = VoteStatus.Open;
        await _context.SaveChangesAsync();

        return Ok("Vote is now open");
    }
    
    
    // PUT: api/votes/{id}/close
    [HttpPut("{id}/close")]
    public async Task<IActionResult> Close(Guid id)
    {
        var vote = await _context.Votes.FindAsync(id);

        if (vote == null)
            return NotFound("Vote not found");

        if (vote.Status != VoteStatus.Open)
            return BadRequest("Only Open votes can be closed");

        vote.Status = VoteStatus.Closed;
        vote.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok("Vote is now closed");
    }


}