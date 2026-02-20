namespace Enclave.Echelon.Core.Models;

/// <summary>
/// Contains the scoring information for a password candidate in the terminal hacking mini-game.
/// Used to determine the optimal guess based on information theory and minimax strategy.
/// </summary>
/// <remarks>
/// Creates a new ScoreInfo for the specified password.
/// </remarks>
/// <param name="password">The password being evaluated.</param>
public class ScoreInfo(Password password)
{
    /// <summary>
    /// Array mapping match count values to the number of candidates with that match count.
    /// Index: match count (0 to word length), Value: number of words with that match count.
    /// </summary>
    private readonly int[] _matchCounts = new int[password.Word.Length + 1];

    /// <summary>
    /// The password being evaluated.
    /// </summary>
    public Password Password { get; } = password;

    /// <summary>
    /// Gets or sets the count for a specific match count value.
    /// </summary>
    /// <param name="matchCount">The match count (0 to word length).</param>
    /// <returns>The number of words with that match count.</returns>
    public int this[int matchCount]
    {
        get => _matchCounts[matchCount];
        set => _matchCounts[matchCount] = value;
    }

    /// <summary>
    /// The number of distinct match count groups (higher is better - more information gained).
    /// </summary>
    public int Value => _matchCounts.Count(c => c > 0);

    /// <summary>
    /// The size of the largest group (lower is better - minimizes worst-case remaining candidates).
    /// </summary>
    public int WorstCase => _matchCounts.Max();
}
