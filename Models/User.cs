using Microsoft.AspNetCore.Identity;

namespace VotingSystem.Models;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Vote> CreatedVotes { get; set; } = new List<Vote>();
    public ICollection<Ballot> Ballots { get; set; } = new List<Ballot>();
}