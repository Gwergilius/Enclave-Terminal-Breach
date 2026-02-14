using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Production solver: best information score, then tie-break by smallest worst-case bucket size (Lore: ECHELON tie-breaker strategy).
/// Uses default base behaviour for <see cref="GetBestGuess"/>, <see cref="CalculateInformationScore"/>, and <see cref="NarrowCandidates"/>.
/// </summary>
public class TieBreakerPasswordSolver : PasswordSolverBase
{
    /// <inheritdoc />
    public override IReadOnlyList<Password> GetBestGuesses(IEnumerable<Password> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates, nameof(candidates));

        var remaining = candidates.ToList();

        if (remaining.Count <= 1) return remaining;

        // 1. Primary: distinct match-count values (higher is better); 2. Secondary: smallest worst-case bucket (minimax)
        var passwordsWithScores = remaining
            .Select(password => CalculateInformationScore(password, remaining))
            .OrderByDescending(s => s.Value)
            .ThenBy(s => s.WorstCase)
            .ToList();

        var bestDistinctGroups = passwordsWithScores[0].Value;
        var bestByGroups = passwordsWithScores
            .Where(x => x.Value == bestDistinctGroups)
            .OrderBy(x => x.WorstCase)
            .ToList();

        var bestWorstCase = bestByGroups[0].WorstCase;
        return bestByGroups
            .Where(x => x.WorstCase == bestWorstCase)
            .Select(x => x.Password)
            .ToList();
    }
}
