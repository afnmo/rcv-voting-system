using VotingSystem.Models;

namespace VotingSystem.ViewModels;

public class VoteListItemViewModel
{
    public Vote Vote { get; set; } = default!;
    public bool AlreadyVoted { get; set; }

}