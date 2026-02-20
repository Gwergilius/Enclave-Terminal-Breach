namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Solver strategy level: which <see cref="IPasswordSolver"/> implementation to use (HOUSE gambit, Best-bucket, Tie-breaker).
/// Enum-like value object: fixed instances, <see cref="ToString()"/>, alias recognition, implicit int conversions.
/// </summary>
/// <remarks>SPARROW-Requirements: 0 = house/dumb/random/baseline, 1 = bucket/smart/tactical, 2 = tie/genius/optimal.</remarks>
public sealed class SolverLevel
{
    /// <summary>Random pick from candidates (SPARROW HOUSE gambit).</summary>
    public static readonly SolverLevel HouseGambit = new(0, "HouseGambit");

    /// <summary>Best information score; random tie-break (RAVEN best-bucket). Default when config is invalid.</summary>
    public static readonly SolverLevel BestBucket = new(1, "BestBucket");

    /// <summary>Best score then smallest worst-case bucket (ECHELON tie-breaker).</summary>
    public static readonly SolverLevel TieBreaker = new(2, "TieBreaker");

    /// <summary>Level used when configuration is missing or invalid.</summary>
    public static SolverLevel Default => BestBucket;

    private static readonly Dictionary<int, SolverLevel> _levelsByValue = new()
    {
        [0] = HouseGambit,
        [1] = BestBucket,
        [2] = TieBreaker,
    };

    private static readonly Dictionary<int, string[]> _aliasesByLevel = new()
    {
        [0] = ["house", "dumb", "algorithm:random", "military:baseline"],
        [1] = ["smart", "algorithm:bucket", "military:tactical"],
        [2] = ["genius", "algorithm:tie", "military:optimal"],
    };

    /// <summary>Default prefix for <see cref="ToString(string?)"/> (military display: baseline, tactical, optimal).</summary>
    public const string DefaultAliasPrefix = "military";

    private readonly int _value;
    private readonly string _name;

    private SolverLevel(int value, string name)
    {
        _value = value;
        _name = name;
    }

    /// <summary>Numeric value (0, 1, 2 for the defined levels).</summary>
    public int Value => _value;

    /// <summary>Maps 0–2 to the corresponding level; values outside range return <see cref="Default"/>.</summary>
    public static SolverLevel FromInt(int value) =>
        value switch { 0 => HouseGambit, 1 => BestBucket, 2 => TieBreaker, _ => Default };

    /// <summary>Returns the level for the given value; for 0–2 returns the canonical instance, otherwise <see cref="Default"/>.</summary>
    public static SolverLevel FromValue(int value) =>
        _levelsByValue.TryGetValue(value, out var level) ? level : Default;

    /// <summary>Parses "0"/"1"/"2", level name, or any registered alias (case-insensitive). Suffix after ":" is matched (e.g. "baseline", "tactical").</summary>
    public static bool TryParse(string? value, out SolverLevel level)
    {
        level = Default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        var s = value.Trim().ToLowerInvariant();
        if (int.TryParse(s, out var i) && i >= 0 && i <= 2)
        {
            level = FromInt(i);
            return true;
        }
        if (s.Equals(nameof(HouseGambit), StringComparison.OrdinalIgnoreCase)) { level = HouseGambit; return true; }
        if (s.Equals(nameof(BestBucket), StringComparison.OrdinalIgnoreCase)) { level = BestBucket; return true; }
        if (s.Equals(nameof(TieBreaker), StringComparison.OrdinalIgnoreCase)) { level = TieBreaker; return true; }
        var (Level, alias) = _aliasesByLevel
            .SelectMany(kv => kv.Value.Select(alias => (Level: kv.Key, Alias: alias)))
            .FirstOrDefault(pair => GetAliasSuffix(pair.Alias).Equals(s, StringComparison.OrdinalIgnoreCase));
        if (alias != null)
        {
            level = FromInt(Level);
            return true;
        }
        return false;
    }

    /// <summary>Whether this value is one of the defined levels (0–2).</summary>
    public static bool IsDefined(SolverLevel level) =>
        level._value >= 0 && level._value <= 2;

    /// <summary>Returns the default alias (military: baseline, tactical, optimal).</summary>
    public override string ToString() => ToString(DefaultAliasPrefix);

    /// <summary>
    /// Returns the alias for this level with the given prefix. Empty string = alias with no prefix (e.g. house, smart, genius);
    /// <see cref="DefaultAliasPrefix"/> ("military") = e.g. baseline, tactical, optimal. No match → internal name (e.g. HouseGambit).
    /// </summary>
    /// <param name="aliasPrefix">Prefix of the alias to select (e.g. "military", "algorithm"); "" = no prefix.</param>
    /// <remarks>The constructor is private; _value is always 0, 1 or 2. We maintain AliasesByLevel; every level has at least one military alias.</remarks>
    public string ToString(string? aliasPrefix)
    {
        var prefix = aliasPrefix ?? DefaultAliasPrefix;
        var aliases = _aliasesByLevel[_value];
        string? match = null;
        if(prefix.Length == 0)
        { 
            // If no prefix, prefer the first alias without a colon (e.g. "house" over "algorithm:random").
            match = aliases.FirstOrDefault(a => !a.Contains(':'));
        }
        else
        {
            // If prefix is specified, prefer the first alias with that prefix (e.g. "military:tactical" over "algorithm:tactical").
            prefix += ":";
            match = aliases.FirstOrDefault(a => a.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
        // If no alias matches the prefix, or if no prefix and all aliases have a prefix, fallback to the internal name (e.g. "HouseGambit").
        // This ensures we always return an alias if any exist.
        match ??= _name;
        match = GetAliasSuffix(match);
        return match;
    }

    private static string GetAliasSuffix(string categoryValue)
    {
        var colon = categoryValue.IndexOf(':');
        return colon >= 0 ? categoryValue[(colon + 1)..] : categoryValue;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is SolverLevel other && _value == other._value;

    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>Implicit conversion to int (the numeric value).</summary>
    public static implicit operator int(SolverLevel level) => level._value;

    /// <summary>Implicit conversion from int (uses <see cref="FromInt"/>: invalid values become <see cref="Default"/>).</summary>
    public static implicit operator SolverLevel(int value) => FromInt(value);
}
