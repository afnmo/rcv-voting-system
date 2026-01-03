namespace VotingSystem.Models;

public class Vote
{
    public Guid VoteId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid CreatorUserId { get; set; }
    public User Creator { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public VoteStatus Status { get; set; }
    
    public Guid? WinningOptionId { get; set; }
    public Option? WinningOption { get; set; }

    public DateTime? CompletedAt { get; set; }
    
    public ICollection<Option> Options { get; set; } = new List<Option>();
    public ICollection<Ballot> Ballots { get; set; } = new List<Ballot>();
}