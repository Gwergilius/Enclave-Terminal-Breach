namespace Enclave.Phosphor;

/// <summary>
/// Polling-based keyboard input loop. Registers handlers and dispatches key events.
/// </summary>
public interface IPhosphorInputLoop
{
    /// <summary>Registers a keyboard handler. Handlers are called in registration order.</summary>
    /// <param name="handler">Handler to register.</param>
    void Register(IPhosphorKeyboardHandler handler);

    /// <summary>
    /// Starts the blocking input loop. Returns when <see cref="Stop"/> is called.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for graceful shutdown (e.g. Ctrl+C).</param>
    void Run(CancellationToken cancellationToken = default);

    /// <summary>Signals the input loop to exit cleanly.</summary>
    void Stop();
}
