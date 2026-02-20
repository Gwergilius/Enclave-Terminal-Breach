using Enclave.Raven.Models;

namespace Enclave.Raven.Phases;

/// <summary>
/// Runs the hacking loop: suggest guess, read match count, narrow candidates until win or exit (RAVEN-Requirements §3).
/// Reads and updates <see cref="IGameSession.Candidates"/>.
/// </summary>
public interface IHackingLoopPhase : IPhase
{
}
