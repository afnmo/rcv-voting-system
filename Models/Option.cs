namespace VotingSystem.Models;

public class Option
{
    public Guid OptionId { get; set; }

    public Guid VoteId { get; set; }
    public Vote Vote { get; set; } = null!;

    public string OptionText { get; set; } = string.Empty;
    
    public ICollection<BallotRanking> BallotRankings { get; set; } = new List<BallotRanking>();
}