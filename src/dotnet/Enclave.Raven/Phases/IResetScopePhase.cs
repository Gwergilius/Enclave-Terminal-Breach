using Enclave.Shared.Phases;

namespace Enclave.Raven.Phases;

/// <summary>
/// Phase that resets the current DI scope and then navigates to a configured target phase (e.g. DataInput).
/// Used before phases that require a fresh scoped lifetime (e.g. new <see cref="IGameSession"/>).
/// </summary>
public interface IResetScopePhase : IPhase
{
}