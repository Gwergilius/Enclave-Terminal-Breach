namespace Enclave.Common.Helpers;

/// <summary>
/// Encapsulates wait logic so the delay implementation can be replaced (e.g. for testing).
/// </summary>
/// <remarks>
/// Creates a waiter that uses the given function for delays, or <see cref="Task.Delay(TimeSpan, CancellationToken)"/> when <paramref name="delay"/> is null.
/// </remarks>
/// <param name="delay">Optional. Function that waits for the given delay, honouring the cancellation token. May throw <see cref="OperationCanceledException"/> or <see cref="TaskCanceledException"/> when cancelled. Defaults to <see cref="Task.Delay(TimeSpan, CancellationToken)"/>.</param>
public sealed class Waiter(Func<TimeSpan, CancellationToken, Task>? delay = null)
{
    private readonly Func<TimeSpan, CancellationToken, Task> _delay = delay ?? ((d, ct) => Task.Delay(d, ct));

    /// <summary>
    /// Waits for <paramref name="delay"/> unless the token is already cancelled or the delay is zero or negative.
    /// Swallows <see cref="TaskCanceledException"/> so callers get a clean return instead of an exception.
    /// </summary>
    /// <param name="delay">How long to wait.</param>
    /// <param name="cancellationToken">Token to cancel the wait.</param>
    public async Task SleepAsync(TimeSpan delay, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested || delay <= TimeSpan.Zero)
        {
            return;
        }
        try
        {
            await _delay(delay, cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            // Swallow cancellation exceptions to allow graceful exit
        }
    }
}
