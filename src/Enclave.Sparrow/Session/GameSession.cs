using Enclave.Echelon.Core.Models;

namespace Enclave.Sparrow.Session;

/// <summary>
/// Default implementation of <see cref="IGameSession"/>. Holds the mutable candidate list and word length for one console run.
/// </summary>
public sealed class GameSession : IGameSession
{
    /// <inheritdoc />
    public IList<Password> Candidates { get; } = [];

    /// <inheritdoc />
    public int? WordLength { get; set; }
}
