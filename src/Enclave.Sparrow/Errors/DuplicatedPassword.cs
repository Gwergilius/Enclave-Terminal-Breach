using FluentResults;

namespace Enclave.Sparrow.Errors;

/// <summary>
/// Error indicating that a password candidate is already in the candidate list.
/// </summary>
/// <param name="word">The duplicated word.</param>
public class DuplicatedPassword(string word) : Error($"Already in list (ignored): {word}"), IError
{
    /// <summary>
    /// Gets the duplicated word.
    /// </summary>
    public string Word { get; } = word;
}
