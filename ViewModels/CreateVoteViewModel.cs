using VotingSystem.Models;

namespace VotingSystem.ViewModels;

public class CreateVoteViewModel
{
    public string Question { get; set; } = string.Empty;

    public List<string> Options { get; set; } = new();
    
    public int DurationHours { get; set; }

    public int? MaxParticipants { get; set; }
    
    public VoteVisibility Visibility { get; set; } = VoteVisibility.Public;

}