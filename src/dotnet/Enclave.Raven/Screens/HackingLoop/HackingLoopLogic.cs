using System.Globalization;
using System.Linq;
using Enclave.Echelon.Core.Models;
using Enclave.Shared.Models;

namespace Enclave.Raven.Screens.HackingLoop;

/// <summary>
/// Presentation logic for the hacking-loop screen.
/// Pure (no I/O, no rendering) — injected into <see cref="HackingLoopViewModel"/> and tested independently.
/// </summary>
internal static class HackingLoopLogic
{
    /// <summary>
    /// Returns the distinct match counts that actually occur between <paramref name="guess"/> and the current candidates in <paramref name="session"/>.
    /// Sorted ascending for display.
    /// </summary>
    internal static IReadOnlyList<int> GetPossibleMatchCounts(Password guess, IGameSession session)
    {
        if (session == null || session.Count == 0)
            return [];
        var set = new HashSet<int>();
        foreach (Password candidate in session)
        {
            if (candidate != null)
                set.Add(guess.GetMatchCount(candidate));
        }
        return [.. set.OrderBy(x => x)];
    }

    /// <summary>
    /// Formats the match-count prompt including the allowed values and "Enter for full match".
    /// </summary>
    /// <param name="wordLength">Used for "or Enter for {wordLength}".</param>
    /// <param name="possibleMatchCounts">Allowed numeric values (from <see cref="GetPossibleMatchCounts"/>).</param>
    internal static string FormatMatchCountPrompt(int wordLength, IReadOnlyList<int> possibleMatchCounts)
    {
        var values = string.Join(", ", possibleMatchCounts.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        return string.Format(CultureInfo.InvariantCulture,
            "Match count ({0} or Enter for {1})? ", values, wordLength);
    }

    /// <summary>
    /// Tries to parse a match count from raw user input.
    /// Blank/whitespace input is treated as a full-match shortcut (returns <paramref name="wordLength"/>).
    /// Non-empty input must be a number in <paramref name="allowedMatchCounts"/> (0..wordLength).
    /// </summary>
    /// <returns><see langword="true"/> if the input was valid; <see langword="false"/> if re-prompt is needed.</returns>
    internal static bool TryParseMatchCount(string? line, int wordLength, IReadOnlyList<int> allowedMatchCounts, out int value)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            value = wordLength;
            return true;
        }

        if (int.TryParse(line.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
            && value >= 0 && value <= wordLength
            && allowedMatchCounts.Contains(value))
            return true;

        value = 0;
        return false;
    }

    /// <summary>
    /// Reads a valid match count by repeatedly calling <paramref name="readLine"/> until a valid value is entered.
    /// Only accepts values in <paramref name="allowedMatchCounts"/> or empty (full match).
    /// Calls <paramref name="onInvalidInput"/> when the input needs re-prompting.
    /// </summary>
    internal static int CollectMatchCount(
        int wordLength,
        IReadOnlyList<int> allowedMatchCounts,
        Func<string?> readLine,
        Action onInvalidInput)
    {
        while (true)
        {
            var line = readLine();
            if (TryParseMatchCount(line, wordLength, allowedMatchCounts, out var v))
                return v;
            onInvalidInput();
        }
    }

    /// <summary>Replaces the session candidates with the narrowed list.</summary>
    internal static void ApplyNarrowedCandidates(IGameSession session, IReadOnlyList<Password> narrowed)
    {
        session.Clear();
        foreach (var p in narrowed)
            session.Add(p);
    }
}
