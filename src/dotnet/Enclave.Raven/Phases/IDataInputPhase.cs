using Enclave.Shared.Models;

namespace Enclave.Raven.Phases;

/// <summary>
/// Runs the data-input phase: prompts for password candidates until empty line (RAVEN-Requirements §2).
/// Fills <see cref="IGameSession.Candidates"/> and sets <see cref="IGameSession.WordLength"/>.
/// </summary>
public interface IDataInputPhase : IPhase
{
}
