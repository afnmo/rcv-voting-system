namespace VotingSystem.Models;

public class BallotRanking
{
    public Guid BallotRankingId { get; set; }

    public Guid BallotId { get; set; }
    public Ballot Ballot { get; set; } = null!;

    public Guid OptionId { get; set; }
    public Option Option { get; set; } = null!;

    public int RankNumber { get; set; }
}