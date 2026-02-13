using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Picks a guess uniformly at random from the candidates (no score used).
/// Uses the same <see cref="CalculateInformationScore"/> and <see cref="NarrowCandidates"/> logic as <see cref="PasswordSolver"/> for compatibility.
/// </summary>
/// <remarks>Creates a solver that picks guesses at random using the given generator.</remarks>
public class RandomGuessSolver(Random random) : IPasswordSolver
{
    private readonly Random _random = random ?? throw new ArgumentNullException(nameof(random));
    private readonly PasswordSolver _inner = new();

    /// <summary>Creates a solver that picks guesses at random using the given seed for reproducibility.</summary>
    public RandomGuessSolver(int seed) : this(new Random(seed)) { }

    /// <inheritdoc />
    public Password? GetBestGuess(IEnumerable<Password> candidates)
    {
        var list = candidates?.ToList() ?? throw new ArgumentNullException(nameof(candidates));
        if (list.Count == 0) return null;
        return list[_random.Next(list.Count)];
    }

    /// <inheritdoc />
    public IReadOnlyList<Password> GetBestGuesses(IEnumerable<Password> candidates)
    {
        var guess = GetBestGuess(candidates);
        return guess == null ? [] : [guess];
    }

    /// <inheritdoc />
    public ScoreInfo CalculateInformationScore(Password password, IEnumerable<Password> candidates)
        => _inner.CalculateInformationScore(password, candidates);

    /// <inheritdoc />
    public IReadOnlyList<Password> NarrowCandidates(IEnumerable<Password> candidates, Password guess, int matchCount)
        => _inner.NarrowCandidates(candidates, guess, matchCount);
}
