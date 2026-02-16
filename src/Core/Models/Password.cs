using Enclave.Echelon.Core.Extensions;
using Enclave.Echelon.Core.Validators;

namespace Enclave.Echelon.Core.Models;

/// <summary>
/// Represents a potential password in the terminal hacking mini-game.
/// Match count results are cached for O(1) subsequent lookups.
/// </summary>
public class Password
{
    private static readonly PasswordValidator _validator = new();
    
    /// <summary>
    /// Cache for match count results. Thread-safe for concurrent access.
    /// Key is the other password's word (uppercase), value is the match count.
    /// </summary>
    private readonly ConcurrentDictionary<string, int> _matchCountCache = new();

    /// <summary>
    /// Gets the word value of the password.
    /// </summary>
    public string Word { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this password has been eliminated as a possibility.
    /// </summary>
    public bool IsEliminated { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Password"/> class.
    /// </summary>
    /// <param name="word">The word value of the password.</param>
    /// <exception cref="ArgumentNullException">Thrown when word is null.</exception>
    /// <exception cref="ArgumentException">Thrown when word is empty, whitespace, or contains non-letter characters.</exception>
    public Password(string word)
    {
        _validator.ValidateAndThrowArgumentException(word);

        Word = word.ToUpperInvariant();
        IsEliminated = false;
    }

    /// <summary>
    /// Calculates the number of matching characters at the same positions between this password and another password.
    /// Results are cached bidirectionally for O(1) subsequent lookups.
    /// </summary>
    /// <param name="other">The other password to compare against.</param>
    /// <returns>The count of characters that match at the same position.</returns>
    /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
    /// <example>
    /// <code>
    /// var password1 = new Password("RELEASED");
    /// var password2 = new Password("DETECTOR");
    /// int matches = password1.GetMatchCount(password2); // Returns 2 (E at positions 2 and 4)
    /// // Subsequent calls use cached value - O(1)
    /// </code>
    /// </example>
    public int GetMatchCount(Password other)
    {
        ArgumentNullException.ThrowIfNull(other);

        // Check cache first - O(1) lookup
        if (_matchCountCache.TryGetValue(other.Word, out var cached))
        {
            return cached;
        }

        // Calculate match count - O(n) where n is word length
        var matchCount = CalculateMatchCount(other);

        // Cache bidirectionally (GetMatchCount is symmetric)
        _matchCountCache[other.Word] = matchCount;
        other._matchCountCache[Word] = matchCount;

        return matchCount;
    }

    /// <summary>
    /// Checks if the match count with another password is already cached.
    /// </summary>
    /// <param name="other">The other password to check.</param>
    /// <returns>True if the match count is cached; otherwise, false.</returns>
    public bool HasCachedMatchCount(Password other)
    {
        return _matchCountCache.ContainsKey(other.Word);
    }

    /// <summary>
    /// Gets the number of cached match counts for this password.
    /// </summary>
    public int CacheSize => _matchCountCache.Count;

    /// <summary>
    /// Calculates the match count without caching.
    /// </summary>
    private int CalculateMatchCount(Password other)
    {
        var minLength = Math.Min(Word.Length, other.Word.Length);
        var matchCount = 0;

        for (var i = 0; i < minLength; i++)
        {
            if (Word[i] == other.Word[i])
            {
                matchCount++;
            }
        }

        return matchCount;
    }

    /// <summary>
    /// Returns the word value of this password.
    /// </summary>
    /// <returns>The word value.</returns>
    public override string ToString() => Word;

    /// <summary>
    /// Determines whether the specified object is equal to the current password.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is Password other)
        {
            return Word.Equals(other.Word, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    /// <summary>
    /// Returns a hash code for this password.
    /// </summary>
    /// <returns>A hash code for the current password.</returns>
    public override int GetHashCode() => Word.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Calculates the difference (number of non-matching characters) between two passwords.
    /// Null values are treated as empty strings.
    /// </summary>
    /// <param name="left">The first password (null treated as empty).</param>
    /// <param name="right">The second password (null treated as empty).</param>
    /// <returns>The count of characters that do not match at the same position.</returns>
    /// <example>
    /// <code>
    /// var p1 = new Password("TERMS");
    /// var p2 = new Password("TEXAS");
    /// int diff = p1 - p2;    // Returns 2 (R≠X at pos 2, M≠A at pos 3)
    /// int diff2 = p1 - null; // Returns 5 (all characters differ from empty)
    /// </code>
    /// </example>
    public static int operator -(Password? left, Password? right)
    {
        var leftLength = left?.Word.Length ?? 0;
        var rightLength = right?.Word.Length ?? 0;

        if (left is null || right is null)
        {
            return Math.Max(leftLength, rightLength);
        }

        var maxLength = Math.Max(leftLength, rightLength);
        return maxLength - left.GetMatchCount(right);
    }

    public static implicit operator string(Password password) => password.Word;
    public static implicit operator Password(string word) => new(word);
}



