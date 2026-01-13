using VotingSystem.Models;
using VotingSystem.Services;

namespace VotingSystem.ViewModels;

public class PublicResultsViewModel
{
    public Vote Vote { get; set; } = null!;
    public RcvResult Result { get; set; } = null!;
}