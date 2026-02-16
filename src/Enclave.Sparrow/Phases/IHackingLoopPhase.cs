using Enclave.Sparrow.Models;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Runs the hacking loop: suggest guess, read match count, narrow candidates until win or exit (SPARROW-Requirements §3).
/// Reads and updates <see cref="IGameSession.Candidates"/>.
/// </summary>
public interface IHackingLoopPhase : IPhase
{
}
