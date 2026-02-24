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
}
