using VotingSystem.Models;

namespace VotingSystem.Services;

public class RcvCountingService
{
    public RcvResult CalculateWinner(Vote vote)
    {
        var result = new RcvResult();

        // All options start active
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

            // Initialize counts
            foreach (var optionId in activeOptions)
                round.VoteCounts[optionId] = 0;

            // Count votes
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

            // Total votes this round
            int totalVotes = round.VoteCounts.Values.Sum();

            // Majority check (> 50%)
            foreach (var kv in round.VoteCounts)
            {
                if (kv.Value > totalVotes / 2)
                {
                    result.WinningOptionId = kv.Key;
                    return result;
                }
            }

            // 🔴 TERMINAL TIE CHECK (ADD THIS)
            if (round.VoteCounts.Values.Distinct().Count() == 1)
            {
                // All remaining options have the same number of votes
                result.IsTie = true;
                result.TiedOptionIds = activeOptions.ToList();
                return result;
            }

            // Eliminate lowest
            var minVotes = round.VoteCounts.Min(v => v.Value);
            var eliminated = round.VoteCounts
                .First(v => v.Value == minVotes).Key;

            round.EliminatedOptionId = eliminated;
            activeOptions.Remove(eliminated);

            roundNumber++;
        }

        // Last remaining option wins
        result.WinningOptionId = activeOptions.First();
        return result;
    }
}