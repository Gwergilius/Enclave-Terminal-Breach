using Enclave.Phosphor;

namespace Enclave.Raven.Screens.BootScreen;

/// <summary>
/// Renders the startup badge: product name, load time, intelligence level and dictionary source.
/// State is set by <see cref="BootScreenViewModel"/> before each render.
/// </summary>
public sealed class BootScreenComponent(Rectangle bounds) : IComponent
{
    /// <inheritdoc />
    public Rectangle Bounds { get; } = bounds;

    public string? ProductLine { get; set; }
    public string? LoadTimeLine { get; set; }
    public string? IntelligenceLine { get; set; }
    public string? DictionaryLine { get; set; }

    /// <inheritdoc />
    public void Render(LayerWriter writer)
    {
        writer.MoveTo(0, 0);
        if (ProductLine is not null)     { writer.Write(ProductLine);     writer.Write("\n"); }
        if (LoadTimeLine is not null)    { writer.Write(LoadTimeLine);    writer.Write("\n"); }
        if (IntelligenceLine is not null){ writer.Write(IntelligenceLine); writer.Write("\n"); }
        if (DictionaryLine is not null)  { writer.Write(DictionaryLine);  writer.Write("\n"); }
    }
}
