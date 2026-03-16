using Enclave.Phosphor;

namespace Enclave.Raven.Screens.DataInput;

/// <summary>
/// Renders a single-row error/status line (e.g. validation messages).
/// Separate from the input component so that DataInput display and error display can vary independently
/// (e.g. input as popup, error as status bar at bottom).
/// </summary>
public sealed class DataInputErrorComponent(Rectangle bounds) : IComponent
{
    /// <inheritdoc />
    public Rectangle Bounds { get; } = bounds;

    /// <summary>Error or status message to display; null or empty clears the row.</summary>
    public string? ErrorMessage { get; set; }

    /// <inheritdoc />
    public void Render(LayerWriter writer)
    {
        writer.MoveTo(0, 0);
        writer.Write(ErrorMessage ?? string.Empty);
        writer.Write("\n");
    }
}
