using Enclave.Echelon.Core.Services;

namespace Enclave.Raven.Configuration;

/// <summary>
/// Maps intelligence level from config (numeric or alias) to solver index 0 (HOUSE), 1 (Best-bucket), 2 (Tie-breaker).
/// Delegates to <see cref="SolverLevel"/> (Core) for alias parsing and display names.
/// </summary>
/// <remarks>RAVEN-Requirements: 0 = house/dumb/random/baseline, 1 = bucket/smart/tactical, 2 = tie/genius/optimal.</remarks>
public static class RavenIntelligence
{
    /// <summary>
    /// Normalizes the raw config value (int or string) to 0, 1, or 2. Invalid values return <see cref="SolverLevel.Default"/> value (1).
    /// Uses <see cref="SolverLevel.TryParse"/> for alias recognition.
    /// </summary>
    public static int Normalize(object? value)
    {
        if (value is int i && i >= 0 && i <= 2)
            return i;
        if (value != null && SolverLevel.TryParse(value.ToString(), out var level))
            return level.Value;
        return SolverLevel.Default.Value;
    }

    /// <summary>
    /// Returns the military display name for the level (baseline, tactical, optimal). Used for consistent badge output.
    /// Invalid level (not 0–2) returns the number as string.
    /// </summary>
    public static string GetDisplayName(int level) =>
        level >= 0 && level <= 2
            ? SolverLevel.FromInt(level).ToString(SolverLevel.DefaultAliasPrefix)
            : level.ToString();
}
