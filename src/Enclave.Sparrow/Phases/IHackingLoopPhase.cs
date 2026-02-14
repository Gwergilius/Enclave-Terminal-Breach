namespace Enclave.Sparrow.Phases;

/// <summary>
/// Runs the hacking loop: suggest guess, read match count, narrow candidates until win or exit (SPARROW-Requirements §3).
/// Reads and updates <see cref="Session.IGameSession.Candidates"/>.
/// </summary>
public interface IHackingLoopPhase : IPhase
{
}
