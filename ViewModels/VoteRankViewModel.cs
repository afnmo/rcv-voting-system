using VotingSystem.Models;

namespace VotingSystem.ViewModels;

public class VoteRankViewModel
{
    public Vote Vote { get; set; } = default!;
    public bool AlreadyVoted { get; set; }
    
    public bool IsOwner { get; set; }

    public List<UserRankingItem> MyRankings { get; set; } = new();
}

public class UserRankingItem
{
    public int RankNumber { get; set; }
    public Guid OptionId { get; set; }
    public string OptionText { get; set; } = "";
}