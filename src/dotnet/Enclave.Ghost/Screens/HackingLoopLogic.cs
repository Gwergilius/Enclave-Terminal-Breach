using System.Globalization;
using Enclave.Echelon.Core.Models;
using Enclave.Shared.Models;

namespace Enclave.Ghost.Screens;

/// <summary>
/// Presentation logic for the hacking-loop screen.
/// Pure (no I/O, no rendering) — identical in behaviour to the Raven equivalent.
/// </summary>
internal static class HackingLoopLogic
{
    /// <summary>
    /// Returns the distinct match counts that actually occur between <paramref name="guess"/> and the current candidates.
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
    internal static string FormatMatchCountPrompt(int wordLength, IReadOnlyList<int> possibleMatchCounts)
    {
        var values = string.Join(", ", possibleMatchCounts.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        return string.Format(CultureInfo.InvariantCulture,
            "Match count ({0} or Enter for {1}):", values, wordLength);
    }

    /// <summary>
    /// Tries to parse a match count from raw user input.
    /// Blank/whitespace input is treated as a full-match shortcut (returns <paramref name="wordLength"/>).
    /// </summary>
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

    /// <summary>Replaces the session candidates with the narrowed list.</summary>
    internal static void ApplyNarrowedCandidates(IGameSession session, IReadOnlyList<Password> narrowed)
    {
        session.Clear();
        foreach (var p in narrowed)
            session.Add(p);
    }
}
