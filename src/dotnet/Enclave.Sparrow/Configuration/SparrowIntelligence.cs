namespace Enclave.Sparrow.Configuration;

/// <summary>
/// Maps intelligence level from config (numeric or alias) to solver index 0 (HOUSE), 1 (Best-bucket), 2 (Tie-breaker).
/// Uses a single alias map; military aliases (baseline, tactical, optimal) are used for display.
/// </summary>
/// <remarks>SPARROW-Requirements: 0 = house/dumb/random/baseline, 1 = bucket/smart/tactical, 2 = tie/genius/optimal.</remarks>
public static class SparrowIntelligence
{
    private static readonly Dictionary<int, HashSet<string>> _aliases = new()
    {
        [0] = ["raw:0", "friendly:house", "friendly:dumb", "algorithm:random", "military:baseline"],
        [1] = ["raw:1", "algorithm:bucket", "friendly:smart", "military:tactical"],
        [2] = ["raw:2", "algorithm:tie", "friendly:genius", "military:optimal"],
    };

    private const string MilitaryPrefix = "military:";

    /// <summary>
    /// Normalizes the raw config value (int or string) to 0, 1, or 2. Invalid values return 1 (Best-bucket).
    /// Matches input against the suffix (part after "category:") of any alias in the map.
    /// </summary>
    public static int Normalize(object? value)
    {
        if (value is int i && i >= 0 && i <= 2)
            return i;

        var s = value?.ToString()?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(s)) return 1;

        foreach (var (level, set) in _aliases)
        {
            foreach (var alias in set)
            {
                var suffix = GetSuffix(alias);
                if (suffix.Equals(s, StringComparison.OrdinalIgnoreCase))
                    return level;
            }
        }

        return int.TryParse(s, out var n) && n >= 0 && n <= 2 ? n : 1;
    }

    /// <summary>
    /// Returns the military display name for the level (baseline, tactical, optimal). Used for consistent badge output.
    /// </summary>
    public static string GetDisplayName(int level)
    {
        if (!_aliases.TryGetValue(level, out var set))
            return level.ToString();

        foreach (var alias in set)
        {
            if (alias.StartsWith(MilitaryPrefix, StringComparison.OrdinalIgnoreCase))
                return GetSuffix(alias);
        }

        return level.ToString();
    }

    private static string GetSuffix(string categoryValue)
    {
        var colon = categoryValue.IndexOf(':');
        return colon >= 0 ? categoryValue[(colon + 1)..] : categoryValue;
    }
}
