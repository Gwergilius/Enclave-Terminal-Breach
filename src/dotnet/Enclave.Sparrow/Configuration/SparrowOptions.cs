namespace Enclave.Sparrow.Configuration;

/// <summary>
/// Configuration options for the SPARROW application (maps to "Sparrow" section in appsettings.json).
/// </summary>
/// <remarks>See docs/Architecture/SPARROW-Requirements.md for property descriptions and defaults.</remarks>
public sealed class SparrowOptions
{
    /// <summary>
    /// Solver intelligence level: 0 = HOUSE gambit (random), 1 = Best-bucket (default), 2 = Tie-breaker.
    /// Accepts numeric string ("0"-"2") or aliases (e.g. "house", "bucket", "tie"). Invalid values fall back to 1.
    /// Stored as string so appsettings.json can use either numbers or aliases.
    /// </summary>
    public string Intelligence { get; set; } = "1";

    /// <summary>
    /// Optional path to a word list file. When set, candidates are loaded from file instead of manual input.
    /// </summary>
    public string? WordListPath { get; set; }

    /// <summary>
    /// Startup display options.
    /// </summary>
    public SparrowStartupOptions Startup { get; set; } = new();
}
