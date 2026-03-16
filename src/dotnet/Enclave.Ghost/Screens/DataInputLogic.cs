using Enclave.Shared.Models;

namespace Enclave.Ghost.Screens;

/// <summary>
/// Presentation logic for the data-input screen: token parsing, session mutation, and state formatting.
/// Pure (no I/O, no rendering) — identical in behaviour to the Raven equivalent.
/// </summary>
internal static class DataInputLogic
{
    /// <summary>Splits <paramref name="line"/> into whitespace-delimited, trimmed, non-empty tokens.</summary>
    internal static IEnumerable<string> GetTokens(string line) =>
        line.Split((string[]?)null, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrEmpty(t));

    /// <summary>
    /// Applies all tokens in <paramref name="line"/> to <paramref name="session"/>:
    /// tokens starting with <c>-</c> remove the word that follows; all others add.
    /// </summary>
    /// <returns>The last error message if any operation failed, otherwise <see langword="null"/>.</returns>
    internal static string? ApplyLine(string line, IGameSession session)
    {
        var errors = ApplyLineCollectErrors(line, session);
        return errors.Count > 0 ? errors[^1] : null;
    }

    /// <summary>
    /// Applies all tokens in <paramref name="line"/> to <paramref name="session"/> and collects every error message.
    /// </summary>
    internal static IReadOnlyList<string> ApplyLineCollectErrors(string line, IGameSession session)
    {
        var errors = new List<string>();
        foreach (var token in GetTokens(line))
        {
            if (token.StartsWith('-'))
            {
                var result = session.Remove(token[1..].Trim());
                if (result.IsFailed) errors.Add(result.Errors[0].Message);
            }
            else
            {
                var result = session.Add(token);
                if (result.IsFailed) errors.Add(result.Errors[0].Message);
            }
        }
        return errors;
    }

    /// <summary>
    /// Returns the candidate count line and formatted candidate list text for display.
    /// <c>CandidateListText</c> is <see langword="null"/> when there are no candidates or word length is unknown.
    /// </summary>
    internal static (string CandidateCountLine, string? CandidateListText) GetCandidateState(IGameSession session)
    {
        var n   = session.Count;
        var len = session.WordLength ?? 0;
        return (
            $"{n} candidate(s):",
            (n > 0 && len > 0) ? CandidateListFormatter.Format(session, len) : null
        );
    }
}
