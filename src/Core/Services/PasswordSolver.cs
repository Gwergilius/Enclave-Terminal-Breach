using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Services;

public class PasswordSolver : IPasswordSolver
{
    /// <inheritdoc />
    /// <example>
    /// <code>
    /// var solver = new PasswordSolver();
    /// var candidates = new[] { new Password("TERMS"), new Password("TEXAS"), new Password("TIRES"), new Password("TANKS") };
    /// var bestGuess = solver.GetBestGuess(candidates);
    /// Console.WriteLine($"Best guess: {bestGuess?.Word}");
    /// </code>
    /// </example>
    public Password? GetBestGuess(IEnumerable<Password> candidates)
    {
        var bestGuesses = GetBestGuesses(candidates);
        return bestGuesses.Count > 0 ? bestGuesses[0] : null;
    }

    /// <inheritdoc />
    /// <example>
    /// <code>
    /// var solver = new PasswordSolver();
    /// var session = new GameSession(new[] { "TERMS", "TEXAS", "TIRES", "TANKS" });
    /// var bestGuesses = solver.GetBestGuesses(session);
    /// Console.WriteLine($"Best guesses: {string.Join(", ", bestGuesses.Select(p => p.Word))}");
    /// // User can choose any of the equally optimal guesses
    /// </code>
    /// </example>
    public IReadOnlyList<Password> GetBestGuesses(IEnumerable<Password> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates, nameof(candidates));

        var remaining = candidates.ToList();

        if (remaining.Count == 0)
        {
            return [];
        }

        if (remaining.Count == 1)
        {
            return remaining;
        }

        // Calculate scores for all passwords using two criteria:
        // 1. Primary: Number of distinct match count values (higher is better)
        // 2. Secondary: Size of largest group (lower is better - minimax strategy)
        var passwordsWithScores = remaining
            .Select(password => CalculateInformationScore(password, remaining))
            .OrderByDescending(s => s.Value)
            .ThenBy(s => s.WorstCase)
            .ToList();

        // First, find the best (highest) distinct group count
        var bestDistinctGroups = passwordsWithScores[0].Value;

        // Filter to only those with the best distinct group count
        var bestByGroups = passwordsWithScores
            .Where(x => x.Value == bestDistinctGroups)
            .OrderBy(x => x.WorstCase)
            .ToList();

        // Among those, find the best (lowest) worst-case group size
        var bestWorstCase = bestByGroups[0].WorstCase;

        // Return all passwords with both the best distinct groups AND the best worst-case
        return bestByGroups
            .Where(x => x.WorstCase == bestWorstCase)
            .Select(x => x.Password)
            .ToList();
    }

    /// <summary>
    /// Calculates the "information score" for a specific password.
    /// The score is the number of distinct match count values when comparing this password
    /// against all other candidates. A higher score means guessing this password will
    /// narrow down the possibilities more effectively.
    /// </summary>
    /// <param name="password">The password to evaluate.</param>
    /// <param name="candidates">The list of candidate passwords to compare against.</param>
    /// <returns>The information score (number of distinct match count values).</returns>
    /// <exception cref="ArgumentNullException">Thrown when password or candidates is null.</exception>
    /// <example>
    /// <code>
    /// var solver = new PasswordSolver();
    /// var password = new Password("TERMS");
    /// var candidates = new[] { new Password("TEXAS"), new Password("TIRES"), new Password("TANKS") };
    /// var score = solver.CalculateInformationScore(password, candidates);
    /// </code>
    /// </example>
    public ScoreInfo CalculateInformationScore(Password password, IEnumerable<Password> candidates)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));
        ArgumentNullException.ThrowIfNull(candidates, nameof(candidates));

        var scoreInfo = new ScoreInfo(password);

        foreach (var candidate in candidates)
        {
            // Note: We don't skip self-comparison because:
            // 1. GetMatchCount is cached (O(1)), while Equals would add overhead per iteration
            // 2. Self-comparison adds word length to every password equally, so it doesn't affect ranking
            var matchCount = password.GetMatchCount(candidate);
            scoreInfo[matchCount]++;
        }

        return scoreInfo;
    }

    /// <inheritdoc />
    public IReadOnlyList<Password> NarrowCandidates(IEnumerable<Password> candidates, Password guess, int matchCount)
    {
        ArgumentNullException.ThrowIfNull(candidates, nameof(candidates));
        ArgumentNullException.ThrowIfNull(guess, nameof(guess));

        return candidates.Where(c => guess.GetMatchCount(c) == matchCount).ToList();
    }
}
