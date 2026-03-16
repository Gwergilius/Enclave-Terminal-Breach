using Enclave.Phosphor;

namespace Enclave.Raven.Screens.Help;

/// <summary>
/// Renders a single version line (e.g. "RAVEN 1.2.3") with <see cref="CharStyle.Bright"/> emphasis.
/// Used by the info-mode pipeline when <c>-v</c> / <c>--version</c> is passed.
/// </summary>
public sealed class VersionComponent(Rectangle bounds) : IComponent
{
    /// <inheritdoc />
    public Rectangle Bounds { get; } = bounds;

    /// <summary>The version string to display (e.g. "RAVEN 1.2.3"). Empty string suppresses output.</summary>
    public string VersionLine { get; set; } = string.Empty;

    /// <summary>The row (0-based) at which to render the version line.</summary>
    public int Row { get; set; } = 0;

    /// <inheritdoc />
    public void Render(LayerWriter writer)
    {
        if (string.IsNullOrEmpty(VersionLine)) return;
        writer.MoveTo(0, Row);
        writer.Style = CharStyle.Bright;
        writer.Write(VersionLine);
    }
}
