using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Best information score only; on tie, picks uniformly at random among the best (no worst-case tie-breaker).
/// Lore: RAVEN best-bucket strategy (Excel prototype behaviour). Uses base for <see cref="CalculateInformationScore"/> and <see cref="NarrowCandidates"/>; overrides <see cref="GetBestGuess"/> to pick randomly among best.
/// </summary>
/// <remarks>If no <see cref="Random"/> is provided, uses a new <see cref="Random"/> instance (non-deterministic).</remarks>
public class BestBucketPasswordSolver(Random? random=null) : PasswordSolverBase
{
    private readonly Random _random = random ?? new Random();

    /// <summary>Creates a solver that uses best score only; ties are broken randomly using the given seed.</summary>
    public BestBucketPasswordSolver(int seed) : this(new Random(seed)) { }

    /// <inheritdoc />
    public override Password? GetBestGuess(IEnumerable<Password> candidates)
    {
        var best = GetBestGuesses(candidates);
        switch(best.Count)
        {
            case 0: return null;
            case 1: return best[0];
            default: return best[_random.Next(best.Count)];
        }
    }

    /// <inheritdoc />
    public override IReadOnlyList<Password> GetBestGuesses(IEnumerable<Password> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);

        var remaining = candidates.ToList();
        if (remaining.Count <= 1) return remaining;

        var withScore = remaining
            .Select(p => (Password: p, Score: CalculateInformationScore(p, remaining)))
            .OrderByDescending(x => x.Score.Value)
            .ToList();

        var bestValue = withScore[0].Score.Value;
        return withScore
            .Where(x => x.Score.Value == bestValue)
            .Select(x => x.Password)
            .ToList();
    }
}
