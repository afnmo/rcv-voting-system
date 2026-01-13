// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using VotingSystem.Data;
// using VotingSystem.DTOs;
// using VotingSystem.Models;
//
// namespace VotingSystem.Controllers;
//
// [Authorize]
// [ApiController]
// [Route("api/votes/{voteId}/options")]
// public class OptionsController : ControllerBase
// {
//     private readonly AppDbContext _context;
//
//     public OptionsController(AppDbContext context)
//     {
//         _context = context;
//     }
//
//     // POST: api/votes/{voteId}/options
//     [HttpPost]
//     public async Task<IActionResult> Create(Guid voteId, CreateOptionDto dto)
//     {
//         var vote = await _context.Votes
//             .Include(v => v.Options)
//             .FirstOrDefaultAsync(v => v.VoteId == voteId);
//
//         if (vote == null)
//             return NotFound("Vote not found");
//
//         if (vote.Status != VoteStatus.Draft)
//             return BadRequest("Options can only be added while vote is in Draft");
//
//         bool duplicate = vote.Options
//             .Any(o => o.OptionText.ToLower() == dto.OptionText.ToLower());
//
//         if (duplicate)
//             return BadRequest("Option already exists for this vote");
//
//         var option = new Option
//         {
//             OptionId = Guid.NewGuid(),
//             VoteId = voteId,
//             OptionText = dto.OptionText
//         };
//
//         _context.Options.Add(option);
//         await _context.SaveChangesAsync();
//
//         return Ok(new
//         {
//             option.OptionId,
//             option.OptionText,
//             option.VoteId
//         });
//
//     }
//
//     // GET: api/votes/{voteId}/options
//     [HttpGet]
//     public async Task<IActionResult> GetByVote(Guid voteId)
//     {
//         var options = await _context.Options
//             .Where(o => o.VoteId == voteId)
//             .Select(o => new
//             {
//                 o.OptionId,
//                 o.OptionText
//             })
//             .ToListAsync();
//
//         return Ok(options);
//     }
// }