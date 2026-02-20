using Enclave.Echelon.Core.Services;

namespace Enclave.Raven.Configuration;

/// <summary>
/// Configuration options for the RAVEN application (maps to "Raven" section in appsettings.json).
/// Also implements <see cref="ISolverConfiguration"/> so the solver factory can resolve the intelligence level from DI.
/// </summary>
/// <remarks>See docs/Architecture/RAVEN-Requirements.md for property descriptions and defaults.</remarks>
public sealed class RavenOptions : ISolverConfiguration
{
    /// <summary>
    /// Solver intelligence level: 0 = HOUSE gambit (random), 1 = Best-bucket (default), 2 = Tie-breaker.
    /// Accepts numeric string ("0"-"2") or aliases (e.g. "house", "bucket", "tie"). Invalid values fall back to 1.
    /// Stored as string so appsettings.json can use either numbers or aliases.
    /// </summary>
    public string Intelligence { get; set; } = "1";

    /// <inheritdoc />
    SolverLevel ISolverConfiguration.Level => SolverLevel.FromInt(RavenIntelligence.Normalize(Intelligence));

    /// <summary>
    /// Optional path to a word list file. When set, candidates are loaded from file instead of manual input.
    /// </summary>
    public string? WordListPath { get; set; }

    /// <summary>
    /// Startup display options.
    /// </summary>
    public RavenStartupOptions Startup { get; set; } = new();
}
