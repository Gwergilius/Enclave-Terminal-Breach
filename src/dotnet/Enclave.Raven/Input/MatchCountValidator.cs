using System.Globalization;
using Enclave.Raven.Screens.HackingLoop;

namespace Enclave.Raven.Input;

/// <summary>
/// Validator for match-count input: only allows empty (full match) or a number in the allowed list (0..wordLength).
/// </summary>
public sealed class MatchCountValidator(int wordLength, IReadOnlyList<int> allowedMatchCounts) : IReadLineValidator
{
    /// <inheritdoc />
    public (bool IsValid, string? ErrorMessage) Validate(string line)
    {
        if (HackingLoopLogic.TryParseMatchCount(line, wordLength, allowedMatchCounts, out _))
            return (true, null);
        return (false, string.Format(CultureInfo.InvariantCulture,
            "Enter one of {0} or Enter for {1}.", string.Join(", ", allowedMatchCounts), wordLength));
    }
}
