namespace VotingSystem.Models;

public class User
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Vote> CreatedVotes { get; set; } = new List<Vote>();
    public ICollection<Ballot> Ballots { get; set; } = new List<Ballot>();
}