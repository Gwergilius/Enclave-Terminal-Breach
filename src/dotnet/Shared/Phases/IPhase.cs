namespace Enclave.Shared.Phases;

/// <summary>
/// Common contract for all phases: a single executable step with <see cref="Run"/>.
/// </summary>
public interface IPhase
{
    /// <summary>Executes the phase.</summary>
    void Run();
}
