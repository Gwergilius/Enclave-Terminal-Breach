using Enclave.Phosphor;

namespace Enclave.Raven.Screens.HackingLoop;

/// <summary>
/// Renders the hacking loop screen: a sequential transcript of guesses, match counts, and final result.
/// The <see cref="Lines"/> list is built up by <see cref="HackingLoopViewModel"/> and rendered top-to-bottom.
/// </summary>
public sealed class HackingLoopComponent(Rectangle bounds) : IComponent
{
    /// <inheritdoc />
    public Rectangle Bounds { get; } = bounds;

    /// <summary>All lines to render, in display order. Built incrementally by the ViewModel.</summary>
    public List<string> Lines { get; } = [];

    /// <inheritdoc />
    public void Render(LayerWriter writer)
    {
        writer.MoveTo(0, 0);
        foreach (var line in Lines)
        {
            writer.Write(line);
            writer.Write("\n");
        }
    }
}
