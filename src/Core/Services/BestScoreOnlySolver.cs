using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Picks a guess with the best information score only. On tie, picks uniformly at random among the best (no worst-case tie-breaker).
/// Matches the Excel prototype behaviour. Uses <see cref="PasswordSolver"/> for score calculation and narrowing.
/// </summary>
/// <remarks>Creates a solver that uses best score only; ties are broken randomly with the given generator.</remarks>
public class BestScoreOnlySolver(Random random) : IPasswordSolver
{
    private readonly Random _random = random ?? throw new ArgumentNullException(nameof(random));
    private readonly PasswordSolver _inner = new();

    /// <summary>Creates a solver that uses best score only; ties are broken randomly using the given seed.</summary>
    public BestScoreOnlySolver(int seed) : this(new Random(seed)) { }

    /// <inheritdoc />
    public Password? GetBestGuess(IEnumerable<Password> candidates)
    {
        var best = GetBestGuesses(candidates);
        return best.Count == 0 ? null : best[_random.Next(best.Count)];
    }

    /// <inheritdoc />
    public IReadOnlyList<Password> GetBestGuesses(IEnumerable<Password> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates, nameof(candidates));

        var remaining = candidates.ToList();
        if (remaining.Count == 0) return [];
        if (remaining.Count == 1) return remaining;

        var withScore = remaining
            .Select(p => (Password: p, Score: _inner.CalculateInformationScore(p, remaining)))
            .OrderByDescending(x => x.Score.Value)
            .ToList();

        var bestValue = withScore[0].Score.Value;
        return withScore
            .Where(x => x.Score.Value == bestValue)
            .Select(x => x.Password)
            .ToList();
    }

    /// <inheritdoc />
    public ScoreInfo CalculateInformationScore(Password password, IEnumerable<Password> candidates)
        => _inner.CalculateInformationScore(password, candidates);

    /// <inheritdoc />
    public IReadOnlyList<Password> NarrowCandidates(IEnumerable<Password> candidates, Password guess, int matchCount)
        => _inner.NarrowCandidates(candidates, guess, matchCount);
}
