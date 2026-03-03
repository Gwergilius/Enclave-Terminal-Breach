namespace Enclave.Phosphor;

/// <summary>
/// Keyboard input loop. Registers handlers and dispatches key events.
/// </summary>
public interface IPhosphorInputLoop
{
    /// <summary>Registers a keyboard handler. Handlers are called in registration order.</summary>
    void Register(IPhosphorReader handler);

    /// <summary>
    /// Blocks until a key is available, then returns it.
    /// Throws <see cref="OperationCanceledException"/> if <paramref name="ct"/> is cancelled.
    /// </summary>
    ConsoleKeyInfo ReadKey(CancellationToken ct);

    /// <summary>
    /// Dispatches <paramref name="key"/> to registered handlers in registration order.
    /// Stops at the first handler that returns <c>true</c> (key consumed).
    /// </summary>
    void Dispatch(ConsoleKeyInfo key);

    /// <summary>
    /// Starts the blocking input loop. Returns when <see cref="Stop"/> is called or
    /// <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    void Run(CancellationToken cancellationToken = default);

    /// <summary>Signals the input loop to exit cleanly.</summary>
    void Stop();
}
