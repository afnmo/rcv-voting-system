namespace VotingSystem.Services;

public class RcvResult
{
    public Guid? WinningOptionId { get; set; }
    public bool IsTie { get; set; }
    public List<Guid> TiedOptionIds { get; set; } = new();
    public List<RcvRound> Rounds { get; set; } = new();
}


public class RcvRound
{
    public int RoundNumber { get; set; }
    public Dictionary<Guid, int> VoteCounts { get; set; } = new();
    public Guid? EliminatedOptionId { get; set; }
}