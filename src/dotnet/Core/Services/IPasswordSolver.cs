using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Defines the contract for a service that suggests optimal password guesses.
/// </summary>
public interface IPasswordSolver
{
    /// <summary>
    /// Gets the best password guess from the remaining passwords.
    /// The best guess is the one that provides the most information, i.e., produces
    /// the highest number of distinct match count results when compared against other remaining passwords.
    /// If multiple passwords have the same score, returns the first one found.
    /// </summary>
    /// <param name="candidates">Collection of the remaining candidate passwords.</param>
    /// <returns>The recommended password to guess, or null if no passwords remain.</returns>
    /// <exception cref="ArgumentNullException">Thrown when session is null.</exception>
    Password? GetBestGuess(IEnumerable<Password> candidates);

    /// <summary>
    /// Gets all passwords that have the highest information score (best guesses).
    /// Multiple passwords may have the same optimal score, allowing the user to choose based on personal preference.
    /// </summary>
    /// <param name="candidates">Collection of the remaining candidate passwords.</param>
    /// <returns>A collection of passwords with the highest information score, or empty if no passwords remain.</returns>
    /// <exception cref="ArgumentNullException">Thrown when session is null.</exception>
    IReadOnlyList<Password> GetBestGuesses(IEnumerable<Password> candidates);

    /// <summary>
    /// Calculates the "information score" for a specific password.
    /// Higher scores indicate that guessing this password will provide more information.
    /// </summary>
    /// <param name="password">The password to evaluate.</param>
    /// <param name="candidates">The list of candidate passwords to compare against.</param>
    /// <returns>A ScoreInfo containing the password, match count distribution, information score, and worst-case group size.</returns>
    /// <exception cref="ArgumentNullException">Thrown when password or candidates is null.</exception>
    ScoreInfo CalculateInformationScore(Password password, IEnumerable<Password> candidates);

    /// <summary>
    /// Narrows the candidate list to those consistent with the terminal response:
    /// keeps only candidates whose match count with the guess equals the reported response.
    /// </summary>
    /// <param name="candidates">Current candidate passwords.</param>
    /// <param name="guess">The password that was guessed.</param>
    /// <param name="matchCount">The match count returned by the terminal for this guess.</param>
    /// <returns>Candidates for which <see cref="Password.GetMatchCount"/> with the guess equals <paramref name="matchCount"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when candidates or guess is null.</exception>
    IReadOnlyList<Password> NarrowCandidates(IEnumerable<Password> candidates, Password guess, int matchCount);
}
