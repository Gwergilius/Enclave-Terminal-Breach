namespace Enclave.Raven.Services;

/// <summary>
/// Runs the configured phases in order within a service scope.
/// </summary>
public interface IPhaseRunner
{
    /// <summary>Executes all phases in sequence.</summary>
    void Run();
}
