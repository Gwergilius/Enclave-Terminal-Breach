using System.Threading.Channels;
using Enclave.Common;
using Enclave.Common.Helpers;
using Enclave.Shared.IO;

namespace Enclave.Phosphor;

/// <summary>
/// Decorator around <see cref="IPhosphorWriter"/> that outputs characters one-by-one with configurable delays (typewriter effect).
/// Characters are enqueued from <see cref="Write"/>/<see cref="WriteLine"/> and sent by a background loop that waits
/// before each character: <see cref="ITimingOptions.LineDelay"/> for '\n', otherwise <see cref="ITimingOptions.CharDelay"/>.
/// Waits only the remaining time since the previous character was sent (no extra delay if the caller already paused).
/// </summary>
public sealed class PhosphorTypewriter : IPhosphorWriter, IDisposable
{
    private readonly IPhosphorWriter _inner;
    private readonly ITimingOptions _timing;
    private readonly Waiter _waiter;
    private readonly Channel<(char C, CharStyle Style)> _channel;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _runTask;
    private CharStyle _style = CharStyle.Normal;
    private bool _disposed;

    /// <summary>
    /// Current character style used for subsequent characters written. Captured per character when enqueueing.
    /// </summary>
    public CharStyle Style
    {
        get => _style;
        set => _style = value;
    }

    /// <summary>
    /// Creates a typewriter that decorates <paramref name="inner"/> and uses <paramref name="timing"/> for delays.
    /// </summary>
    /// <param name="inner">The underlying writer that actually outputs to the terminal.</param>
    /// <param name="timing">Delay values (CharDelay, LineDelay).</param>
    /// <param name="waiter">Optional. Used for waiting; defaults to <see cref="Waiter"/> (Task.Delay).</param>
    public PhosphorTypewriter(IPhosphorWriter inner, ITimingOptions timing, Waiter? waiter = null)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _timing = timing ?? throw new ArgumentNullException(nameof(timing));
        _waiter = waiter ?? new Waiter();
        _channel = Channel.CreateUnbounded<(char, CharStyle)>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
        _runTask = RunAsync(_cts.Token);
    }

    /// <inheritdoc />
    public void Write(string? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (value is null) return;
        foreach (var c in value)
            _channel.Writer.TryWrite((c, _style));
    }

    /// <inheritdoc />
    public void WriteLine(string? value = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (value is not null)
            foreach (var c in value)
                _channel.Writer.TryWrite((c, _style));
        _channel.Writer.TryWrite(('\n', _style));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _channel.Writer.Complete();
        _cts.Cancel();
        try
        {
            _runTask.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
            // Expected when we cancel.
        }
        _cts.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var lastSendTime = DateTime.UtcNow - _timing.CharDelay; // First character can be sent immediately.
        try
        {
            await foreach (var (c, style) in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                var delay = c == '\n' ? _timing.LineDelay : _timing.CharDelay;
                var deadline = lastSendTime + delay;
                var remaining = deadline - DateTime.UtcNow;
                if (remaining > TimeSpan.Zero)
                    await _waiter.SleepAsync(remaining, cancellationToken).ConfigureAwait(false);

                _inner.Style = style;
                _inner.Write(c.ToString());
                lastSendTime = DateTime.UtcNow;
            }
        }
        catch (OperationCanceledException)
        {
            // Normal on Dispose.
        }
    }
}
