using System.Threading;
using Enclave.Raven.Services;
using Enclave.Shared.IO;

namespace Enclave.Raven.Keyboard;

/// <summary>
/// Reads keys from the console and dispatches to subscribers by priority (lower number = higher priority).
/// Among equal priorities, registration order applies (strictly monotonically increasing sequence number).
/// Only one key is read per <see cref="GetNextKey"/>; if a subscriber sets <see cref="KeyPressedEventArgs.Handled"/>, the key is consumed and we read again (or return null when exit requested).
/// </summary>
public sealed class KeyboardService(IConsoleReader reader, IExitRequest exitRequest) : IKeyboardService
{
    private readonly List<(int Priority, long Sequence, Action<KeyPressedEventArgs> Handler)> _subscribers = [];
    private long _nextSequence;
    private readonly Lock _lock = new();

    /// <inheritdoc />
    public IDisposable Subscribe(int priority, Action<KeyPressedEventArgs> handler)
    {
        if (priority <= 0)
            throw new ArgumentOutOfRangeException(nameof(priority), priority, "Priority must be a positive integer.");
        long seq;
        lock (_lock)
        {
            seq = _nextSequence++;
            _subscribers.Add((priority, seq, handler));
            _subscribers.Sort((a, b) => a.Priority != b.Priority ? a.Priority.CompareTo(b.Priority) : a.Sequence.CompareTo(b.Sequence));
        }
        return new Subscription(this, seq);
    }

    internal void Unsubscribe(long sequence)
    {
        lock (_lock)
        {
            var i = _subscribers.FindIndex(t => t.Sequence == sequence);
            if (i >= 0)
                _subscribers.RemoveAt(i);
        }
    }

    /// <inheritdoc />
    public bool KbHit() => reader.KeyAvailable;

    /// <inheritdoc />
    public ConsoleKeyInfo? GetNextKey()
    {
        while (true)
        {
            var key = reader.ReadKey();
            if (key is null)
                return null;

            List<(int Priority, long Sequence, Action<KeyPressedEventArgs> Handler)> snapshot;
            lock (_lock)
                snapshot = [.. _subscribers];

            var e = new KeyPressedEventArgs(key.Value);
            foreach (var (_, _, h) in snapshot)
            {
                h(e);
                if (e.Handled)
                    break;
            }

            if (e.Handled)
            {
                if (exitRequest.IsExitRequested)
                    return null;
                continue;
            }

            return key;
        }
    }

    private sealed class Subscription(KeyboardService service, long sequence) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            service.Unsubscribe(sequence);
            _disposed = true;
        }
    }
}
