namespace VotingSystem.DTOs;

public class SubmitBallotDto
{
    public List<RankingDto> Rankings { get; set; } = new();
}