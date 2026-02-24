namespace Enclave.Common.Models;

/// <summary>
/// Platform-agnostic immutable color representation for cross-platform use (MAUI, Blazor, console).
/// </summary>
/// <param name="R">Red component (0–255).</param>
/// <param name="G">Green component (0–255).</param>
/// <param name="B">Blue component (0–255).</param>
/// <param name="A">Alpha component (0–255). Defaults to 255 (opaque).</param>
public record ColorValue(byte R, byte G, byte B, byte A = 255)
{
    /// <summary>Transparent color (fully transparent black).</summary>
    public static ColorValue Transparent { get; } = new(0, 0, 0, 0);

    /// <summary>White color.</summary>
    public static ColorValue White { get; } = new(255, 255, 255);

    /// <summary>Black color.</summary>
    public static ColorValue Black { get; } = new(0, 0, 0);

    /// <summary>
    /// Parses a hex color string (e.g. "#66FF66", "#66FF66FF", "66FF66").
    /// </summary>
    /// <param name="hex">Hex string with or without leading #. Supports 6 (RGB) or 8 (RGBA) digits.</param>
    /// <returns>Parsed <see cref="ColorValue"/>.</returns>
    /// <exception cref="ArgumentException">When <paramref name="hex"/> format is invalid.</exception>
    public static ColorValue FromHex(string hex)
    {
        ArgumentNullException.ThrowIfNull(hex);
        var s = hex.TrimStart('#');
        if (s.Length != 6 && s.Length != 8)
            throw new ArgumentException($"Invalid hex color length: expected 6 or 8 characters, got {s.Length}.", nameof(hex));
        var r = Convert.ToByte(s.Substring(0, 2), 16);
        var g = Convert.ToByte(s.Substring(2, 2), 16);
        var b = Convert.ToByte(s.Substring(4, 2), 16);
        var a = s.Length == 8 ? Convert.ToByte(s.Substring(6, 2), 16) : (byte)255;
        return new ColorValue(r, g, b, a);
    }

    /// <summary>
    /// Converts to hex string (e.g. "#66FF66").
    /// </summary>
    /// <param name="includeAlpha">When true, includes alpha channel (8 digits).</param>
    /// <returns>Hex string with leading #.</returns>
    public string ToHex(bool includeAlpha = false)
    {
        if (includeAlpha)
            return $"#{R:X2}{G:X2}{B:X2}{A:X2}";
        return $"#{R:X2}{G:X2}{B:X2}";
    }

    /// <summary>
    /// Converts to CSS rgba string (e.g. "rgba(102, 255, 102, 1.0)").
    /// </summary>
    public string ToCssRgba() => $"rgba({R}, {G}, {B}, {A / 255.0:F1})";

    /// <summary>
    /// Converts to CSS rgb string (e.g. "rgb(102, 255, 102)").
    /// </summary>
    public string ToCssRgb() => $"rgb({R}, {G}, {B})";
}
