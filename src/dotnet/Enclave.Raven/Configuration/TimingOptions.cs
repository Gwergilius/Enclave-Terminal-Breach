using Enclave.Common.Extensions;
using Enclave.Phosphor;

namespace Enclave.Raven.Configuration;

/// <summary>
/// Timing options bound from the "Platform:Timing" section in appsettings.json.
/// Values are stored as strings (e.g. "10 ms", "150 ms") for binding and parsed to <see cref="TimeSpan"/> when accessed via <see cref="ITimingOptions"/> using <see cref="TimeSpanExtensions.ParseTimeUnit"/>.
/// </summary>
public sealed class TimingOptions : ITimingOptions
{
    /// <summary>
    /// Delay between normal characters (config: "CharDelay", e.g. "10 ms"). Used for binding; use <see cref="ITimingOptions.CharDelay"/> for <see cref="TimeSpan"/>.
    /// </summary>
    public string CharDelay { get; set; } = "10 ms";

    /// <summary>
    /// Fast character delay (config: "CharDelayFast", e.g. "2 ms"). Used for binding; use <see cref="ITimingOptions.CharDelayFast"/> for <see cref="TimeSpan"/>.
    /// </summary>
    public string CharDelayFast { get; set; } = "2 ms";

    /// <summary>
    /// Delay after newline (config: "LineDelay", e.g. "150 ms"). Used for binding; use <see cref="ITimingOptions.LineDelay"/> for <see cref="TimeSpan"/>.
    /// </summary>
    public string LineDelay { get; set; } = "150 ms";

    /// <summary>
    /// Slow delay (config: "SlowDelay", e.g. "500 ms"). Used for binding.
    /// </summary>
    public string SlowDelay { get; set; } = "500 ms";

    /// <summary>
    /// OK status pause (config: "OkStatusDelay", e.g. "250 ms"). Used for binding.
    /// </summary>
    public string OkStatusDelay { get; set; } = "250 ms";

    /// <summary>
    /// Progress update interval (config: "ProgressUpdate", e.g. "50 ms"). Used for binding.
    /// </summary>
    public string ProgressUpdate { get; set; } = "50 ms";

    /// <summary>
    /// Progress duration (config: "ProgressDuration", e.g. "800 ms"). Used for binding.
    /// </summary>
    public string ProgressDuration { get; set; } = "800 ms";

    /// <summary>
    /// Warning pause (config: "WarningPause", e.g. "1200 ms"). Used for binding.
    /// </summary>
    public string WarningPause { get; set; } = "1200 ms";

    /// <summary>
    /// Final pause (config: "FinalPause", e.g. "600 ms"). Used for binding.
    /// </summary>
    public string FinalPause { get; set; } = "600 ms";

    /// <inheritdoc />
    TimeSpan ITimingOptions.CharDelay => ParseDelayOrDefault(CharDelay, 10.Millisecs());

    /// <inheritdoc />
    TimeSpan ITimingOptions.CharDelayFast => ParseDelayOrDefault(CharDelayFast, 2.Millisecs());

    /// <inheritdoc />
    TimeSpan ITimingOptions.LineDelay => ParseDelayOrDefault(LineDelay, 150.Millisecs());

    /// <inheritdoc />
    TimeSpan ITimingOptions.SlowDelay => ParseDelayOrDefault(SlowDelay, 500.Millisecs());

    /// <inheritdoc />
    TimeSpan ITimingOptions.OkStatusDelay => ParseDelayOrDefault(OkStatusDelay, 250.Millisecs());

    /// <inheritdoc />
    TimeSpan ITimingOptions.ProgressUpdate => ParseDelayOrDefault(ProgressUpdate, 50.Millisecs());

    /// <inheritdoc />
    TimeSpan ITimingOptions.ProgressDuration => ParseDelayOrDefault(ProgressDuration, 800.Millisecs());

    /// <inheritdoc />
    TimeSpan ITimingOptions.WarningPause => ParseDelayOrDefault(WarningPause, 1200.Millisecs());

    /// <inheritdoc />
    TimeSpan ITimingOptions.FinalPause => ParseDelayOrDefault(FinalPause, 600.Millisecs());

    private static TimeSpan ParseDelayOrDefault(string? value, TimeSpan defaultValue)
    {
        try
        {
            return (value ?? string.Empty).Trim().ParseTimeUnit(defaultValue);
        }
        catch (FormatException)
        {
            return defaultValue;
        }
    }
}
