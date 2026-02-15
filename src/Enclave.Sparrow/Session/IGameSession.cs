using Enclave.Echelon.Core.Models;

namespace Enclave.Sparrow.Session;

/// <summary>
/// Shared game state between the data-input phase and the hacking loop.
/// Both phases depend on this; the data-input phase fills candidates, the hacking phase reads and narrows them.
/// </summary>
public interface IGameSession
{
    /// <summary>
    /// Current list of password candidates. Data-input phase adds/removes; hacking phase replaces with narrowed list after each guess.
    /// </summary>
    IList<Password> Candidates { get; }

    /// <summary>
    /// Required word length (set when the first word is accepted in data-input). Null until then, or when the list is cleared (e.g. last candidate removed).
    /// </summary>
    int? WordLength { get; set; }
}
