namespace VotingSystem.ViewModels;

public class VoteCreatedViewModel
{
    public Guid VoteId { get; set; }
    public string Title { get; set; } = "";
    public string ShareUrl { get; set; } = "";
}
