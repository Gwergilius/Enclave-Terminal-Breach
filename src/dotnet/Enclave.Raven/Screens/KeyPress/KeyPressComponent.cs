using Enclave.Phosphor;

namespace Enclave.Raven.Screens.KeyPress;

/// <summary>
/// Renders a single prompt line at a configurable row.
/// State is set by <see cref="KeyPressViewModel"/> before each render.
/// </summary>
public sealed class KeyPressComponent(Rectangle bounds) : IComponent
{
    /// <inheritdoc />
    public Rectangle Bounds { get; } = bounds;

    /// <summary>The prompt text to display (e.g., "Press any key to continue...").</summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>The row (0-based) at which to render the prompt.</summary>
    public int Row { get; set; } = 0;

    /// <inheritdoc />
    public void Render(LayerWriter writer)
    {
        writer.MoveTo(0, Row);
        writer.Write(Prompt);
    }
}
