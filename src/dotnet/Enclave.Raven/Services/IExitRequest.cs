namespace Enclave.Raven.Services;

/// <summary>
/// Allows the composition root to signal that the application should exit (e.g. on Ctrl+C).
/// </summary>
public interface IExitRequest
{
    /// <summary>Gets whether exit has been requested.</summary>
    bool IsExitRequested { get; }

    /// <summary>Signals that the application should exit.</summary>
    void RequestExit();
}
