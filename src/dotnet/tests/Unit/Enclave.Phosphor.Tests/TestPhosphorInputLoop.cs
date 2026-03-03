using System.Collections.Concurrent;

namespace Enclave.Phosphor.Tests;

/// <summary>
/// Test double for <see cref="IPhosphorInputLoop"/> that allows programmatic key injection.
/// </summary>
public sealed class TestPhosphorInputLoop : IPhosphorInputLoop
{
    private readonly BlockingCollection<ConsoleKeyInfo?> _keyQueue = new();
    private readonly List<IPhosphorReader> _handlers = new();

    /// <inheritdoc />
    public void Register(IPhosphorReader handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _handlers.Add(handler);
    }

    /// <inheritdoc />
    public ConsoleKeyInfo ReadKey(CancellationToken ct)
    {
        ConsoleKeyInfo? key;
        try
        {
            key = _keyQueue.Take(ct);
        }
        catch (OperationCanceledException)
        {
            throw;
        }

        return key ?? throw new InvalidOperationException("Stop sentinel received.");
    }

    /// <inheritdoc />
    public void Dispatch(ConsoleKeyInfo key)
    {
        foreach (var handler in _handlers)
        {
            if (handler.OnKeyPressed(key))
                break;
        }
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

            Dispatch(key.Value);
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        _keyQueue.Add(null);
    }

    /// <summary>Injects a key press to simulate user input.</summary>
    public void InjectKey(ConsoleKeyInfo key)
    {
        _keyQueue.Add(key);
    }

    /// <summary>Disposes the underlying queue.</summary>
    public void Dispose() => _keyQueue.Dispose();
}
