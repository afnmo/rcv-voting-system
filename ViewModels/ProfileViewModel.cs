using VotingSystem.Models;

namespace VotingSystem.ViewModels;

public class ProfileViewModel
{
    public List<Vote> CreatedVotes { get; set; } = new();
    public List<Vote> ParticipatedVotes { get; set; } = new();
}
