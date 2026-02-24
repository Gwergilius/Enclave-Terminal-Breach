using Enclave.Shared.Phases;

namespace Enclave.Raven.Phases;

/// <summary>
/// Phase that shows "Press any key to play again...", waits for a key, clears the screen, then navigates to the next phase (e.g. ResetScope) or Exit.
/// </summary>
public interface IPlayAgainPhase : IPhase
{
}