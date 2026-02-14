namespace Enclave.Sparrow.Phases;

/// <summary>
/// Runs the data-input phase: prompts for password candidates until empty line (SPARROW-Requirements §2).
/// Fills <see cref="Session.IGameSession.Candidates"/> and sets <see cref="Session.IGameSession.WordLength"/>.
/// </summary>
public interface IDataInputPhase : IPhase
{
}
