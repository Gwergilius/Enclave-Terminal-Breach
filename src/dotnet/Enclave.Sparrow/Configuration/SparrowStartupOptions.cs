namespace Enclave.Sparrow.Configuration;

/// <summary>
/// Startup display options for SPARROW (Sparrow:Startup section).
/// </summary>
public sealed class SparrowStartupOptions
{
    /// <summary>
    /// Whether to display the startup banner (product name and version). Default: true.
    /// </summary>
    public bool ShowBanner { get; set; } = true;

    /// <summary>
    /// Whether to display load time in the banner. Default: true.
    /// </summary>
    public bool ShowLoadTime { get; set; } = true;
}
