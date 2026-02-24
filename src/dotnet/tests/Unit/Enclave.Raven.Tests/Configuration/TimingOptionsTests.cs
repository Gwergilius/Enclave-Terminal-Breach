using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.Configuration;

namespace Enclave.Raven.Tests.Configuration;

/// <summary>
/// Unit tests for <see cref="TimingOptions"/> (binding and <see cref="ITimingOptions"/> parsing).
/// </summary>
[UnitTest, TestOf(nameof(TimingOptions))]
public class TimingOptionsTests
{
    [Theory]
    [InlineData("10 ms", 10)]
    [InlineData("150 ms", 150)]
    [InlineData("2 ms", 2)]
    [InlineData("0 ms", 0)]
    public void ITimingOptions_CharDelay_WithValidMsString_ReturnsMilliseconds(string value, int expectedMs)
    {
        var options = new TimingOptions { CharDelay = value };
        var timing = (ITimingOptions)options;
        timing.CharDelay.ShouldBe(TimeSpan.FromMilliseconds(expectedMs));
    }

    [Theory]
    [InlineData("1 s", 1000)]
    [InlineData("2 s", 2000)]
    public void ITimingOptions_LineDelay_WithValidSecondsString_ReturnsSeconds(string value, int expectedMs)
    {
        var options = new TimingOptions { LineDelay = value };
        var timing = (ITimingOptions)options;
        timing.LineDelay.ShouldBe(TimeSpan.FromMilliseconds(expectedMs));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ITimingOptions_CharDelay_WithNullOrWhitespace_ReturnsDefault(string? value)
    {
        var options = new TimingOptions { CharDelay = value! };
        var timing = (ITimingOptions)options;
        timing.CharDelay.ShouldBe(TimeSpan.FromMilliseconds(10));
    }

    [Fact]
    public void ITimingOptions_CharDelay_WithInvalidString_ReturnsDefault()
    {
        var options = new TimingOptions { CharDelay = "invalid" };
        var timing = (ITimingOptions)options;
        timing.CharDelay.ShouldBe(TimeSpan.FromMilliseconds(10));
    }

    [Fact]
    public void ITimingOptions_CharDelay_WithNumberOnly_ReturnsDefault()
    {
        var options = new TimingOptions { CharDelay = "10" };
        var timing = (ITimingOptions)options;
        timing.CharDelay.ShouldBe(TimeSpan.FromMilliseconds(10));
    }

    [Fact]
    public void DefaultInstance_ExposesExpectedTimeSpansViaITimingOptions()
    {
        var options = new TimingOptions();
        var timing = (ITimingOptions)options;

        timing.CharDelay.ShouldBe(TimeSpan.FromMilliseconds(10));
        timing.CharDelayFast.ShouldBe(TimeSpan.FromMilliseconds(2));
        timing.LineDelay.ShouldBe(TimeSpan.FromMilliseconds(150));
        timing.SlowDelay.ShouldBe(TimeSpan.FromMilliseconds(500));
        timing.OkStatusDelay.ShouldBe(TimeSpan.FromMilliseconds(250));
        timing.ProgressUpdate.ShouldBe(TimeSpan.FromMilliseconds(50));
        timing.ProgressDuration.ShouldBe(TimeSpan.FromMilliseconds(800));
        timing.WarningPause.ShouldBe(TimeSpan.FromMilliseconds(1200));
        timing.FinalPause.ShouldBe(TimeSpan.FromMilliseconds(600));
    }

    [Fact]
    public void AfterSettingStringProperties_ITimingOptionsReturnsParsedValues()
    {
        var options = new TimingOptions
        {
            CharDelay = "25 ms",
            LineDelay = "300 ms"
        };
        var timing = (ITimingOptions)options;

        timing.CharDelay.ShouldBe(TimeSpan.FromMilliseconds(25));
        timing.LineDelay.ShouldBe(TimeSpan.FromMilliseconds(300));
    }
}
