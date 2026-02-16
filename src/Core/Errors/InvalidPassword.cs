using FluentResults;

namespace Enclave.Echelon.Core.Errors;

/// <summary>
/// Error indicating that a word is not valid as a password candidate.
/// </summary>
/// <param name="word">The invalid word.</param>
/// <param name="message">Human-readable reason (e.g. validation failure, length mismatch).</param>
public class InvalidPassword(string word, string message) : Error(message), IError
{
    /// <summary>
    /// Gets the word that failed validation.
    /// </summary>
    public string Word { get; } = word;
}
