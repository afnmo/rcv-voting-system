namespace VotingSystem.Models;

public class Ballot
{
    public Guid BallotId { get; set; }

    public Guid VoteId { get; set; }
    public Vote Vote { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<BallotRanking> Rankings { get; set; } = new List<BallotRanking>();
}