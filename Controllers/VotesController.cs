using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Data;
using VotingSystem.Models;
using System.Security.Claims;
using VotingSystem.DTOs;
using VotingSystem.Services;
using VotingSystem.ViewModels;

namespace VotingSystem.Controllers;

[Authorize]
public class VotesController : Controller
{
    private readonly AppDbContext _context;

    private readonly RcvCountingService _rcvService;

    public VotesController(AppDbContext context, RcvCountingService rcvService)
    {
        _context = context;
        _rcvService = rcvService;
    }
    
    
    private Guid? GetUserIdOrNull()
    {
        if (User.Identity?.IsAuthenticated != true)
            return null;

        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return string.IsNullOrWhiteSpace(idStr) ? null : Guid.Parse(idStr);
    }


    [Authorize]
    public async Task<IActionResult> Details(Guid id)
    {
        var vote = await _context.Votes
            .Include(v => v.Options)
            .Include(v => v.Ballots)
            .FirstOrDefaultAsync(v => v.VoteId == id);

        if (vote == null)
            return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (vote.CreatorUserId != userId)
            return Forbid();

        return View(vote);
    }

    
    

    public IActionResult Create()
    {
        return View();
    }
    


    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        var userId = GetUserIdOrNull();

        var query = _context.Votes.AsQueryable();

        if (userId == null)
        {
            query = query.Where(v => v.Visibility == VoteVisibility.Public);
        }
        else
        {
            query = query.Where(v =>
                v.Visibility == VoteVisibility.Public ||
                (v.Visibility == VoteVisibility.Private && v.CreatorUserId == userId)
            );
        }

        var openVotes = await query
            .Where(v =>
                v.StartTime <= now &&
                v.EndTime > now &&
                v.Status == VoteStatus.Open
            )
            .OrderBy(v => v.EndTime)
            .ToListAsync();

        if (userId == null)
        {
            var guestVm = openVotes.Select(v => new VotingSystem.ViewModels.VoteListItemViewModel
            {
                Vote = v,
                AlreadyVoted = false,

            }).ToList();

            return View(guestVm);
        }

        var votedVoteIds = await _context.Ballots
            .Where(b => b.UserId == userId)
            .Select(b => b.VoteId)
            .ToHashSetAsync();

        var vm = openVotes.Select(v => new VotingSystem.ViewModels.VoteListItemViewModel
        {
            Vote = v,
            AlreadyVoted = votedVoteIds.Contains(v.VoteId),

        }).ToList();

        return View(vm);
    }

    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVoteViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Question))
        {
            ModelState.AddModelError("", "السؤال مطلوب");
        }

        var validOptions = model.Options
            .Where(o => !string.IsNullOrWhiteSpace(o))
            .ToList();

        if (validOptions.Count < 2)
        {
            ModelState.AddModelError("", "لازم خيارين على الأقل");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var now = DateTime.UtcNow;

        var vote = new Vote
        {
            VoteId = Guid.NewGuid(),
            Title = model.Question,
            CreatorUserId = userId,
            StartTime = now,
            EndTime = now.AddHours(model.DurationHours),
            Status = VoteStatus.Open,

            Visibility = model.Visibility
        };


        foreach (var optionText in validOptions)
        {
            vote.Options.Add(new Option
            {
                OptionId = Guid.NewGuid(),
                OptionText = optionText
            });
        }

        _context.Votes.Add(vote);
        await _context.SaveChangesAsync();

        return RedirectToAction("Created", new { id = vote.VoteId });

    }
    [HttpGet]
    public async Task<IActionResult> Created(Guid id)
    {
        var vote = await _context.Votes
            .Include(v => v.Options)
            .FirstOrDefaultAsync(v => v.VoteId == id);

        if (vote == null) return NotFound();

        var vm = new VoteCreatedViewModel
        {
            VoteId = vote.VoteId,
            Title = vote.Title,
            ShareUrl = Url.Action("Rank", "Votes", new { id = vote.VoteId }, Request.Scheme)!
        };

        return View(vm);
    }



    [Authorize]
    public async Task<IActionResult> Rank(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var vote = await _context.Votes
            .Include(v => v.Options)
            .FirstOrDefaultAsync(v => v.VoteId == id);

        if (vote == null)
            return NotFound();

        if (vote.Visibility == VoteVisibility.Private && vote.CreatorUserId != userId)
            return Forbid();

        bool isClosed = vote.Status != VoteStatus.Open || DateTime.UtcNow >= vote.EndTime;

        var ballot = await _context.Ballots
            .Include(b => b.Rankings)
            .ThenInclude(r => r.Option)
            .FirstOrDefaultAsync(b => b.VoteId == id && b.UserId == userId);

        var vm = new VoteRankViewModel
        {
            Vote = vote,
            AlreadyVoted = ballot != null,
            IsOwner = (vote.CreatorUserId == userId)

        };
        
        if (ballot != null)
        {
            vm.MyRankings = ballot.Rankings
                .OrderBy(r => r.RankNumber)
                .Select(r => new UserRankingItem
                {
                    RankNumber = r.RankNumber,
                    OptionId = r.OptionId,
                    OptionText = r.Option!.OptionText 
                })
                .ToList();
        }

        ViewBag.IsClosed = isClosed;

        return View(vm);
    }

    

    [AllowAnonymous]
    public async Task<IActionResult> Results(Guid id)
    {
        var vote = await _context.Votes
            .Include(v => v.Options)
            .Include(v => v.Ballots)
            .ThenInclude(b => b.Rankings)
            .FirstOrDefaultAsync(v => v.VoteId == id);

        if (vote == null)
            return NotFound();

        if (vote.Status != VoteStatus.Closed && vote.Status != VoteStatus.Completed)
            return View("ResultsNotReady", vote); 

        var result = _rcvService.CalculateWinner(vote);

        if (vote.Status != VoteStatus.Completed)
        {
            vote.WinningOptionId = result.WinningOptionId;
            vote.Status = VoteStatus.Completed;
            vote.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        var vm = new PublicResultsViewModel
        {
            Vote = vote,
            Result = result
        };

        return View(vm);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(Guid id)
    {
        var vote = await _context.Votes
            .FirstOrDefaultAsync(v => v.VoteId == id);

        if (vote == null)
            return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (vote.CreatorUserId != userId)
            return Forbid();

        if (vote.Status == VoteStatus.Closed || vote.Status == VoteStatus.Completed)
            return RedirectToAction("Details", new { id });

        vote.Status = VoteStatus.Closed;
        vote.EndTime = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction("Results", new { id });
    }

    

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public async Task<IActionResult> Rank(Guid id, SubmitBallotDto dto)
{
    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    var vote = await _context.Votes
        .Include(v => v.Options)
        .FirstOrDefaultAsync(v => v.VoteId == id);

    if (vote == null)
        return NotFound();

    if (vote.Visibility == VoteVisibility.Private && vote.CreatorUserId != userId)
        return Forbid();

    if (vote.Status != VoteStatus.Open || DateTime.UtcNow >= vote.EndTime)
    {
        TempData["ToastError"] = "التصويت مغلق أو انتهى وقته.";
        return RedirectToAction("Rank", new { id });
    }

    bool alreadyVoted = await _context.Ballots
        .AnyAsync(b => b.VoteId == id && b.UserId == userId);

    if (alreadyVoted)
    {
        TempData["ToastError"] = "أنت صوّتت بالفعل في هذا التصويت ✅";
        return RedirectToAction("Rank", new { id });
    }

    var validOptionIds = vote.Options
        .Select(o => o.OptionId)
        .ToHashSet();

    if (dto.Rankings == null || !dto.Rankings.Any())
    {
        TempData["ToastError"] = "لازم تختار ترتيب واحد على الأقل.";
        return RedirectToAction("Rank", new { id });
    }

    if (dto.Rankings.Any(r => !validOptionIds.Contains(r.OptionId)))
    {
        TempData["ToastError"] = "يوجد خيار غير صحيح ضمن الاختيارات.";
        return RedirectToAction("Rank", new { id });
    }

    var ranks = dto.Rankings.Select(r => r.RankNumber).ToList();

    if (ranks.Count != ranks.Distinct().Count())
    {
        TempData["ToastError"] = "لا يمكن تكرار نفس الرقم في الترتيب.";
        return RedirectToAction("Rank", new { id });
    }

    if (ranks.Min() != 1)
    {
        TempData["ToastError"] = "الترتيب لازم يبدأ من رقم 1.";
        return RedirectToAction("Rank", new { id });
    }

    var ballot = new Ballot
    {
        BallotId = Guid.NewGuid(),
        VoteId = id,
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

    TempData["ToastSuccess"] = "تم تسجيل صوتك بنجاح ✅";
    return RedirectToAction("Rank", new { id });
}


    public IActionResult Preview()
    {
        return View("Results", null); 
    }


}