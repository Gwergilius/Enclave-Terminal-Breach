using Enclave.Common.Models;

namespace Enclave.Phosphor;

/// <summary>
/// Creates built-in <see cref="PhosphorTheme"/> instances.
/// </summary>
public static class PhosphorThemeFactory
{
    private static readonly IReadOnlyDictionary<CharStyle, ColorValue> GreenPalette = new Dictionary<CharStyle, ColorValue>
    {
        [CharStyle.Background] = ColorValue.FromHex("#0C190C"),
        [CharStyle.Dark] = ColorValue.FromHex("#1A4D1A"),
        [CharStyle.Normal] = ColorValue.FromHex("#339933"),
        [CharStyle.Bright] = ColorValue.FromHex("#66FF66")
    };

    private static readonly IReadOnlyDictionary<CharStyle, ColorValue> AmberPalette = new Dictionary<CharStyle, ColorValue>
    {
        [CharStyle.Background] = ColorValue.FromHex("#190C00"),
        [CharStyle.Dark] = ColorValue.FromHex("#653811"),
        [CharStyle.Normal] = ColorValue.FromHex("#996600"),
        [CharStyle.Bright] = ColorValue.FromHex("#FFBB33")
    };

    private static readonly IReadOnlyDictionary<CharStyle, ColorValue> WhitePalette = new Dictionary<CharStyle, ColorValue>
    {
        [CharStyle.Background] = ColorValue.FromHex("#0A0A0A"),
        [CharStyle.Dark] = ColorValue.FromHex("#4D4D4D"),
        [CharStyle.Normal] = ColorValue.FromHex("#999999"),
        [CharStyle.Bright] = ColorValue.FromHex("#E6E6E6")
    };

    private static readonly IReadOnlyDictionary<CharStyle, ColorValue> BluePalette = new Dictionary<CharStyle, ColorValue>
    {
        [CharStyle.Background] = ColorValue.FromHex("#000519"),
        [CharStyle.Dark] = ColorValue.FromHex("#1A417B"),
        [CharStyle.Normal] = ColorValue.FromHex("#3366CC"),
        [CharStyle.Bright] = ColorValue.FromHex("#66BBFF")
    };

    /// <summary>
    /// Creates a theme by key. Supported keys: "green", "amber", "white", "blue".
    /// </summary>
    /// <param name="key">Theme key.</param>
    /// <returns>The <see cref="PhosphorTheme"/> for the given key.</returns>
    /// <exception cref="ArgumentException">When <paramref name="key"/> is not a known theme.</exception>
    public static PhosphorTheme Create(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var normalized = key.Trim().ToLowerInvariant();
        return normalized switch
        {
            "green" => new PhosphorTheme("green", GreenPalette),
            "amber" => new PhosphorTheme("amber", AmberPalette),
            "white" => new PhosphorTheme("white", WhitePalette),
            "blue" => new PhosphorTheme("blue", BluePalette),
            _ => throw new ArgumentException($"Unknown theme key: '{key}'. Supported: green, amber, white, blue.", nameof(key))
        };
    }

    /// <summary>
    /// Returns the default theme (Classic Green Phosphor).
    /// </summary>
    public static PhosphorTheme Default => Create("green");
}
