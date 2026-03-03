using Enclave.Common;
using Enclave.Common.Helpers;
using Enclave.Common.Test.Core;
using Enclave.Phosphor;

namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="PhosphorTypewriter"/>.
/// </summary>
[UnitTest, TestOf(nameof(PhosphorTypewriter))]
public class PhosphorTypewriterTests
{
    private static ITimingOptions ZeroTiming()
    {
        return new ZeroTimingOptions();
    }

    private static Waiter NoWaitWaiter() => new((_, _) => Task.CompletedTask);

    [Fact]
    public void Write_EnqueuesCharacters_ThenDispose_FlushesToInner()
    {
        // Arrange
        var inner = new TestPhosphorWriter();
        var timing = ZeroTiming();
        var waiter = NoWaitWaiter();
        using var typewriter = new PhosphorTypewriter(inner, timing, waiter);

        // Act
        typewriter.Write("Hi");

        // Dispose completes the channel and waits for the loop to flush
        typewriter.Dispose();

        // Assert
        inner.Recorded.Count.ShouldBe(2);
        inner.Recorded[0].Text.ShouldBe("H");
        inner.Recorded[0].Style.ShouldBe(CharStyle.Normal);
        inner.Recorded[1].Text.ShouldBe("i");
        inner.Recorded[1].Style.ShouldBe(CharStyle.Normal);
    }

    [Fact]
    public void WriteLine_EnqueuesNewline_FlushedToInner()
    {
        var inner = new TestPhosphorWriter();
        using var typewriter = new PhosphorTypewriter(inner, ZeroTiming(), NoWaitWaiter());

        typewriter.WriteLine();
        typewriter.Dispose();

        inner.Recorded.Count.ShouldBe(1);
        inner.Recorded[0].Text.ShouldBe("\n");
    }

    [Fact]
    public void Style_IsCapturedPerCharacter()
    {
        var inner = new TestPhosphorWriter();
        using var typewriter = new PhosphorTypewriter(inner, ZeroTiming(), NoWaitWaiter());

        typewriter.Write("A");
        typewriter.Style = CharStyle.Bright;
        typewriter.Write("B");
        typewriter.Dispose();

        inner.Recorded.Count.ShouldBe(2);
        inner.Recorded[0].Style.ShouldBe(CharStyle.Normal);
        inner.Recorded[1].Style.ShouldBe(CharStyle.Bright);
    }

    [Fact]
    public void Style_GetterReturnsDefaultValue()
    {
        using var typewriter = new PhosphorTypewriter(new TestPhosphorWriter(), ZeroTiming(), NoWaitWaiter());

        typewriter.Style.ShouldBe(CharStyle.Normal);
    }

    [Fact]
    public void Dispose_WithNoCharactersWritten_CompletesCleanly()
    {
        var inner = new TestPhosphorWriter();
        var typewriter = new PhosphorTypewriter(inner, ZeroTiming(), NoWaitWaiter());

        typewriter.Dispose(); // channel is empty — await foreach loop body never runs

        inner.Recorded.ShouldBeEmpty();
    }

    [Fact]
    public async Task Write_WhenDelayIsPositive_SleepAsyncIsCalled()
    {
        // The first character is always sent immediately (lastSendTime is pre-seeded to
        // DateTime.UtcNow - CharDelay). The second character arrives before CharDelay
        // has elapsed, so remaining > TimeSpan.Zero and SleepAsync must be called.
        //
        // We wait for the TCS signal before calling Dispose() to avoid a race: Dispose()
        // calls _cts.Cancel() which causes WaitToReadAsync to throw OCE before the
        // second character is dequeued, masking the delay path entirely.
        var inner = new TestPhosphorWriter();
        // RunContinuationsAsynchronously prevents a deadlock: TrySetResult() is called on the
        // RunAsync thread inside the SleepAsync delegate. Without this flag, the test's await
        // continuation would run inline on that same thread, reach Dispose() → GetAwaiter().GetResult(),
        // and deadlock because _runTask can never complete while its thread is blocked.
        var tcs   = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var trackingWaiter = new Waiter((_, _) => { tcs.TrySetResult(); return Task.CompletedTask; });
        using var typewriter = new PhosphorTypewriter(
            inner, new FixedCharDelayTimingOptions(TimeSpan.FromMinutes(10)), trackingWaiter);

        typewriter.Write("AB");

        // Block until the background loop reaches SleepAsync, then let Dispose drain cleanly.
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        typewriter.Dispose();

        tcs.Task.IsCompletedSuccessfully.ShouldBeTrue();
    }

    private sealed class ZeroTimingOptions : ITimingOptions
    {
        public TimeSpan CharDelay => TimeSpan.Zero;
        public TimeSpan CharDelayFast => TimeSpan.Zero;
        public TimeSpan LineDelay => TimeSpan.Zero;
        public TimeSpan SlowDelay => TimeSpan.Zero;
        public TimeSpan OkStatusDelay => TimeSpan.Zero;
        public TimeSpan ProgressUpdate => TimeSpan.Zero;
        public TimeSpan ProgressDuration => TimeSpan.Zero;
        public TimeSpan WarningPause => TimeSpan.Zero;
        public TimeSpan FinalPause => TimeSpan.Zero;
    }

    private sealed class FixedCharDelayTimingOptions(TimeSpan charDelay) : ITimingOptions
    {
        public TimeSpan CharDelay        => charDelay;
        public TimeSpan CharDelayFast    => charDelay;
        public TimeSpan LineDelay        => charDelay;
        public TimeSpan SlowDelay        => TimeSpan.Zero;
        public TimeSpan OkStatusDelay    => TimeSpan.Zero;
        public TimeSpan ProgressUpdate   => TimeSpan.Zero;
        public TimeSpan ProgressDuration => TimeSpan.Zero;
        public TimeSpan WarningPause     => TimeSpan.Zero;
        public TimeSpan FinalPause       => TimeSpan.Zero;
    }
}
