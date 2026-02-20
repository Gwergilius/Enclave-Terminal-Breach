namespace Enclave.Sparrow.Phases;

/// <summary>
/// Common contract for all SPARROW phases: a single executable step with <see cref="Run"/>.
/// </summary>
public interface IPhase
{
    /// <summary>Executes the phase.</summary>
    void Run();
}
