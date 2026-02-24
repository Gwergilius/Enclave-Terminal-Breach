namespace Enclave.Phosphor;

/// <summary>
/// Timing values for terminal and application behaviour (e.g. typewriter delays, pauses).
/// Typically bound from the "Platform:Timing" configuration section.
/// </summary>
public interface ITimingOptions
{
    /// <summary>
    /// Delay between normal characters when using typewriter-style output. Default typically 10 ms.
    /// </summary>
    TimeSpan CharDelay { get; }

    /// <summary>
    /// Shorter delay for "fast" characters (e.g. box-drawing). Default typically 2 ms.
    /// </summary>
    TimeSpan CharDelayFast { get; }

    /// <summary>
    /// Delay after a newline when using typewriter-style output. Default typically 150 ms.
    /// </summary>
    TimeSpan LineDelay { get; }

    /// <summary>
    /// Duration for slow / emphasised effects. Default typically 500 ms.
    /// </summary>
    TimeSpan SlowDelay { get; }

    /// <summary>
    /// Pause when showing an OK or success status. Default typically 250 ms.
    /// </summary>
    TimeSpan OkStatusDelay { get; }

    /// <summary>
    /// Interval between progress updates. Default typically 50 ms.
    /// </summary>
    TimeSpan ProgressUpdate { get; }

    /// <summary>
    /// Total duration of a progress animation. Default typically 800 ms.
    /// </summary>
    TimeSpan ProgressDuration { get; }

    /// <summary>
    /// Pause when showing a warning. Default typically 1200 ms.
    /// </summary>
    TimeSpan WarningPause { get; }

    /// <summary>
    /// Pause at the end of a sequence. Default typically 600 ms.
    /// </summary>
    TimeSpan FinalPause { get; }
}
