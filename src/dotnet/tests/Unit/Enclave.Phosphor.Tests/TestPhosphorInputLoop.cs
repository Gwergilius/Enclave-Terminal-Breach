using System.Collections.Concurrent;

namespace Enclave.Phosphor.Tests;

/// <summary>
/// Test double for <see cref="IPhosphorInputLoop"/> that allows programmatic key injection.
/// </summary>
public sealed class TestPhosphorInputLoop : IPhosphorInputLoop
{
    private readonly BlockingCollection<ConsoleKeyInfo?> _keyQueue = new();
    private readonly List<IPhosphorKeyboardHandler> _handlers = new();

    /// <inheritdoc />
    public void Register(IPhosphorKeyboardHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _handlers.Add(handler);
    }

    /// <inheritdoc />
    public void Run(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            ConsoleKeyInfo? key;
            try
            {
                key = _keyQueue.Take(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (key is null)
                break; // Sentinel for Stop

            foreach (var handler in _handlers)
            {
                if (handler.OnKeyPressed(key.Value))
                    break;
            }
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        _keyQueue.Add(null);
    }

    /// <summary>
    /// Injects a key press to simulate user input. Call from tests.
    /// </summary>
    /// <param name="key">The key to simulate.</param>
    public void InjectKey(ConsoleKeyInfo key)
    {
        _keyQueue.Add(key);
    }

    /// <summary>
    /// Disposes the underlying queue. Call when done with the test.
    /// </summary>
    public void Dispose() => _keyQueue.Dispose();
}
