using Enclave.Echelon.Core.Models;

namespace Enclave.Raven.Models;

/// <summary>
/// Shared game state between the data-input phase and the hacking loop.
/// Both phases depend on this; the data-input phase fills candidates, the hacking phase reads and narrows them.
/// </summary>
public interface IGameSession: IList<Password>
{
    /// <summary>
    /// Required word length (set when the first word is accepted in data-input). Null until then, or when the list is cleared (e.g. last candidate removed).
    /// </summary>
    int? WordLength { get; set; }

    /// <summary>
    /// Adds a valid word to the candidate list. Validates word format, length consistency, and duplicate check.
    /// </summary>
    /// <param name="word">The word to add.</param>
    /// <returns>A successful result if added; a failed result with message if validation fails.</returns>
    FluentResults.Result Add(string word);

    /// <summary>
    /// Removes a candidate by exact word match (case-insensitive).
    /// </summary>
    /// <param name="word">The word to remove.</param>
    /// <returns>Result.Ok if removed successfully; Result with NotFoundError if word is empty or not in list.</returns>
    FluentResults.Result Remove(string word);
}
