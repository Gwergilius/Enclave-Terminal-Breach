using Enclave.Common.Extensions;
using Enclave.Common.Helpers;
using Enclave.Common.Test.Core;

namespace Enclave.Common.Tests;

/// <summary>
/// Unit tests for <see cref="Waiter"/> (injectable delay; extension methods are excluded from coverage and verified by code review).
/// </summary>
[UnitTest, TestOf(nameof(Waiter))]
public class WaiterTests
{
    [Fact]
    public async Task Constructor_WithNullDelay_UsesDefaultTaskDelay()
    {
        // Arrange: parameterless uses Task.Delay; verify by waiting 10s and measuring elapsed time
        var waiter = new Waiter();
        var cts = new CancellationTokenSource();
        var delay = 10.Seconds();

        // Act
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await waiter.SleepAsync(delay, cts.Token);
        sw.Stop();

        // Assert: must have waited at least 10s (proves real delay), and not more than 15s (proves we use delay, not e.g. infinite wait)
        sw.Elapsed.ShouldBeGreaterThanOrEqualTo(TimeSpan.FromSeconds(10));
        sw.Elapsed.ShouldBeLessThanOrEqualTo(TimeSpan.FromSeconds(15));
    }

    [Fact]
    public async Task SleepAsync_WithPositiveDelay_InvokesDelayAndCompletes()
    {
        // Arrange
        TimeSpan? capturedDelay = null;
        CancellationToken? capturedToken = null;
        var waiter = new Waiter((d, ct) =>
        {
            capturedDelay = d;
            capturedToken = ct;
            return Task.CompletedTask;
        });
        var delay = 80.Millisecs();
        var cts = new CancellationTokenSource();

        // Act
        await waiter.SleepAsync(delay, cts.Token);

        // Assert
        capturedDelay.ShouldBe(delay);
        capturedToken.ShouldBe(cts.Token);
    }

    [Fact]
    public async Task SleepAsync_WhenDelayThrowsTaskCanceledException_ReturnsWithoutException()
    {
        // Arrange
        var waiter = new Waiter((_, _) => Task.FromException(new TaskCanceledException()));
        var cts = new CancellationTokenSource();

        // Act
        await waiter.SleepAsync(100.Millisecs(), cts.Token);

        // Assert: completes without throwing
    }

    [Fact]
    public async Task SleepAsync_WhenTokenCanceledDuringWait_ReturnsWithoutException()
    {
        // Arrange: delay func returns a task that completes when the token is cancelled
        var waiter = new Waiter((_, ct) =>
        {
            var tcs = new TaskCompletionSource();
            ct.Register(() => tcs.SetResult());
            return tcs.Task;
        });
        var cts = new CancellationTokenSource();
        var sleepTask = waiter.SleepAsync(10.Seconds(), cts.Token);

        // Act
        await Task.Delay(30, CancellationToken.None);
        cts.Cancel();
        await sleepTask;

        // Assert: task completes without throwing
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SleepAsync_WithZeroOrNegativeDelay_DoesNotInvokeDelay(int milliseconds)
    {
        // Arrange
        var delayInvoked = false;
        var waiter = new Waiter((_, _) =>
        {
            delayInvoked = true;
            return Task.CompletedTask;
        });
        var delay = TimeSpan.FromMilliseconds(milliseconds);
        var cts = new CancellationTokenSource();

        // Act
        await waiter.SleepAsync(delay, cts.Token);

        // Assert
        delayInvoked.ShouldBeFalse("should exit immediately without calling delay");
    }

    [Fact]
    public async Task SleepAsync_WithAlreadyCanceledToken_DoesNotInvokeDelay()
    {
        // Arrange
        var delayInvoked = false;
        var waiter = new Waiter((_, _) =>
        {
            delayInvoked = true;
            return Task.CompletedTask;
        });
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        await waiter.SleepAsync(1000.Millisecs(), cts.Token);

        // Assert
        delayInvoked.ShouldBeFalse("should not wait when token is already cancelled");
    }
}
