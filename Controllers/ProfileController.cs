using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Data;
using VotingSystem.ViewModels;

namespace VotingSystem.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Votes created by user
        var createdVotes = await _context.Votes
            .Where(v => v.CreatorUserId == userId)
            .OrderByDescending(v => v.StartTime)
            .ToListAsync();

        // Votes user participated in
        var participatedVotes = await _context.Ballots
            .Include(b => b.Vote)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.SubmittedAt)
            .Select(b => b.Vote)
            .Distinct()
            .ToListAsync();

        var vm = new ProfileViewModel
        {
            CreatedVotes = createdVotes,
            ParticipatedVotes = participatedVotes
        };

        return View(vm);
    }
}
