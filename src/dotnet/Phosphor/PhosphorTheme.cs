using Enclave.Common.Models;

namespace Enclave.Phosphor;

/// <summary>
/// Maps each <see cref="CharStyle"/> to a concrete <see cref="ColorValue"/> for terminal rendering.
/// </summary>
/// <param name="Key">Theme identifier (e.g. "green", "amber", "white", "blue").</param>
/// <param name="Palette">Mapping from <see cref="CharStyle"/> to <see cref="ColorValue"/>.</param>
public sealed record PhosphorTheme(
    string Key,
    IReadOnlyDictionary<CharStyle, ColorValue> Palette);
