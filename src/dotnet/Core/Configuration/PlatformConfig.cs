namespace Enclave.Raven.Core.Configuration;

/// <summary>
/// Platform-specific configuration for ECHELON boot sequence and project information.
/// Maps to "Platform" section in appsettings.json.
/// </summary>
public class PlatformConfig
{
    /// <summary>
    /// Project codename (e.g., "RAVEN", "ECHELON", "GHOST")
    /// </summary>
    public string ProjectCodename { get; set; } = string.Empty;

    /// <summary>
    /// Version string (e.g., "v0.3.1", "v2.1.7")
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Platform name (e.g., "ROBCO TERMINAL NX-12", "Pip-Boy 3000 Mark IV")
    /// </summary>
    public string PlatformName { get; set; } = string.Empty;

    /// <summary>
    /// Brief description of this platform variant
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// System modules to load during boot (Phase 1)
    /// </summary>
    public string[] SystemModules { get; set; } = [];

    /// <summary>
    /// Applications to load during boot (Phase 3)
    /// </summary>
    public string[] Applications { get; set; } = [];

    /// <summary>
    /// Boot sequence timing configuration
    /// </summary>
    public TimingConfig Timing { get; set; } = new();

    /// <summary>
    /// Font configuration (font name => file path)
    /// Only used by MAUI platform
    /// </summary>
    public Dictionary<string, string> Fonts { get; set; } = new();

    /// <summary>
    /// Default font family name
    /// MAUI: Uses registered font alias (e.g., "FixedsysExcelsior")
    /// Blazor: Uses CSS font-family value (e.g., "'Fixedsys', 'Courier New', monospace")
    /// </summary>
    public string? DefaultFont { get; set; }
}

/// <summary>
/// Boot sequence timing configuration.
/// All values should be specified with units (e.g., "150 ms", "1 sec").
/// Parsed using TimeSpanExtensions.ParseTimeUnit().
/// </summary>
public class TimingConfig
{
    /// <summary>
    /// Normal line load time (e.g., "150 ms")
    /// </summary>
    public string LineDelay { get; set; } = "150 ms";

    /// <summary>
    /// Important/dramatic lines delay (e.g., "400 ms")
    /// </summary>
    public string SlowDelay { get; set; } = "400 ms";

    /// <summary>
    /// SleepAsync before showing "OK" status (e.g., "200 ms")
    /// </summary>
    public string OkStatusDelay { get; set; } = "200 ms";

    /// <summary>
    /// Progress bar refresh rate (e.g., "50 ms")
    /// Set to "0 ms" for platforms without progress bars
    /// </summary>
    public string ProgressUpdate { get; set; } = "50 ms";

    /// <summary>
    /// Full progress bar animation duration (e.g., "800 ms")
    /// Set to "0 ms" for platforms without progress bars
    /// </summary>
    public string ProgressDuration { get; set; } = "800 ms";

    /// <summary>
    /// Warning screen pause duration (e.g., "1 sec")
    /// </summary>
    public string WarningPause { get; set; } = "1 sec";

    /// <summary>
    /// Final pause before main app (e.g., "500 ms")
    /// </summary>
    public string FinalPause { get; set; } = "500 ms";

    /// <summary>
    /// SleepAsync between characters for typewriter effect (e.g., "20 ms")
    /// <</summary>
    public string CharDelay { get; set; } = "";

    /// <summary>
    /// SleepAsync for fast typewriter (e.g., "5 ms") for box drawing characters
    /// </summary>
    public string CharDelayFast { get; set; } = "";
}
