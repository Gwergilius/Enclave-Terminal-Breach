using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Base class for <see cref="IPasswordSolver"/> implementations. Provides default (virtual) behaviour for
/// <see cref="GetBestGuess"/>, <see cref="CalculateInformationScore"/>, and <see cref="NarrowCandidates"/>.
/// Subclasses implement <see cref="GetBestGuesses"/> and override the others only when strategy differs.
/// </summary>
public abstract class PasswordSolverBase : IPasswordSolver
{
    /// <inheritdoc />
    /// <remarks>Default: returns the first element of <see cref="GetBestGuesses"/>, or null if empty.</remarks>
    public virtual Password? GetBestGuess(IEnumerable<Password> candidates)
    {
        var bestGuesses = GetBestGuesses(candidates);
        return bestGuesses.Count > 0 ? bestGuesses[0] : null;
    }

    /// <summary>
    /// Gets all passwords considered best by this strategy. Each implementation defines its own selection logic.
    /// </summary>
    public abstract IReadOnlyList<Password> GetBestGuesses(IEnumerable<Password> candidates);

    /// <inheritdoc />
    /// <remarks>
    /// Default: number of distinct match-count values when comparing the password to all candidates; used for information-score strategies.
    /// </remarks>
    public virtual ScoreInfo CalculateInformationScore(Password password, IEnumerable<Password> candidates)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(candidates);

        var scoreInfo = new ScoreInfo(password);

        foreach (var candidate in candidates)
        {
            var matchCount = password.GetMatchCount(candidate);
            scoreInfo[matchCount]++;
        }

        return scoreInfo;
    }

    /// <inheritdoc />
    /// <remarks>Default: keeps only candidates whose match count with the guess equals the reported response.</remarks>
    public virtual IReadOnlyList<Password> NarrowCandidates(IEnumerable<Password> candidates, Password guess, int matchCount)
    {
        ArgumentNullException.ThrowIfNull(candidates, nameof(candidates));
        ArgumentNullException.ThrowIfNull(guess, nameof(guess));

        return [.. candidates.Where(c => guess.GetMatchCount(c) == matchCount)];
    }
}
