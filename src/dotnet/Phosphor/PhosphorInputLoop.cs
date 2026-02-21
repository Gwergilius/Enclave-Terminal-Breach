using Enclave.Shared.IO;

namespace Enclave.Phosphor;

/// <summary>
/// Default implementation of <see cref="IPhosphorInputLoop"/> using <see cref="IConsoleIO.ReadKey"/>.
/// </summary>
public sealed class PhosphorInputLoop : IPhosphorInputLoop
{
    private readonly IConsoleIO _console;
    private readonly List<IPhosphorKeyboardHandler> _handlers = new();
    private volatile bool _stopRequested;

    /// <summary>
    /// Initializes a new instance of <see cref="PhosphorInputLoop"/> with the given console.
    /// </summary>
    /// <param name="console">Console abstraction for reading keys.</param>
    public PhosphorInputLoop(IConsoleIO console)
    {
        ArgumentNullException.ThrowIfNull(console);
        _console = console;
    }

    /// <inheritdoc />
    public void Register(IPhosphorKeyboardHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        lock (_handlers)
        {
            _handlers.Add(handler);
        }
    }

    /// <inheritdoc />
    public void Run(CancellationToken cancellationToken = default)
    {
        _stopRequested = false;
        while (!_stopRequested && !cancellationToken.IsCancellationRequested)
        {
            var key = _console.ReadKey();
            if (key is null)
                break;

            if (cancellationToken.IsCancellationRequested) break;

            IReadOnlyList<IPhosphorKeyboardHandler> snapshot;
            lock (_handlers)
            {
                snapshot = _handlers.ToList();
            }

            foreach (var handler in snapshot)
            {
                if (handler.OnKeyPressed(key.Value))
                    break;
            }
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        _stopRequested = true;
    }
}
