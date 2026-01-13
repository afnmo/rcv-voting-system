using VotingSystem.Models;

namespace VotingSystem.Services;

public class RcvCountingService
{
    public RcvResult CalculateWinner(Vote vote)
    {
        var result = new RcvResult();

        var activeOptions = vote.Options
            .Select(o => o.OptionId)
            .ToHashSet();

        var roundNumber = 1;

        while (activeOptions.Count > 1)
        {
            var round = new RcvRound
            {
                RoundNumber = roundNumber
            };

            foreach (var optionId in activeOptions)
                round.VoteCounts[optionId] = 0;

            foreach (var ballot in vote.Ballots)
            {
                var preferred = ballot.Rankings
                    .Where(r => activeOptions.Contains(r.OptionId))
                    .OrderBy(r => r.RankNumber)
                    .FirstOrDefault();

                if (preferred != null)
                    round.VoteCounts[preferred.OptionId]++;
            }

            result.Rounds.Add(round);

            int totalVotes = round.VoteCounts.Values.Sum();

            foreach (var kv in round.VoteCounts)
            {
                if (kv.Value > totalVotes / 2)
                {
                    result.WinningOptionId = kv.Key;
                    return result;
                }
            }

            if (round.VoteCounts.Values.Distinct().Count() == 1)
            {
                result.IsTie = true;
                result.TiedOptionIds = activeOptions.ToList();
                return result;
            }

            var minVotes = round.VoteCounts.Min(v => v.Value);
            var eliminated = round.VoteCounts
                .First(v => v.Value == minVotes).Key;

            round.EliminatedOptionId = eliminated;
            activeOptions.Remove(eliminated);

            roundNumber++;
        }

        result.WinningOptionId = activeOptions.First();
        return result;
    }
}