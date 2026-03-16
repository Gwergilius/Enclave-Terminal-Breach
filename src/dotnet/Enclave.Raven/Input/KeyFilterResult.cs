using System.Diagnostics.CodeAnalysis;

namespace Enclave.Raven.Input;

/// <summary>
/// Result of processing a key in the read-line loop.
/// </summary>
public enum KeyFilterKind
{
    /// <summary>Append this character to the current line (see <see cref="ReadLineParams.KeyFilter"/>).</summary>
    Append,
    /// <summary>Remove the last character (backspace).</summary>
    Backspace,
    /// <summary>Submit the current line (Enter).</summary>
    Enter,
    /// <summary>Ignore this key.</summary>
    Ignore,
    /// <summary>Special handling (e.g. Up/Down for error paging); <see cref="ReadLineParams.OnSpecialKey"/> is invoked.</summary>
    Special
}

/// <summary>
/// Result of a key filter: action kind and, for <see cref="Append"/>, the character to append.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Straightforward, verify by review")]
public readonly record struct KeyFilterResult(KeyFilterKind Kind, char Char = '\0')
{
    public static KeyFilterResult AppendChar(char c) => new(KeyFilterKind.Append, c);
    public static KeyFilterResult Backspace => new(KeyFilterKind.Backspace);
    public static KeyFilterResult Enter => new(KeyFilterKind.Enter);
    public static KeyFilterResult Ignore => new(KeyFilterKind.Ignore);
    public static KeyFilterResult Special => new(KeyFilterKind.Special);
}
