namespace Enclave.Phosphor.Tests;

/// <summary>
/// Test double for <see cref="IVirtualScreen"/>.
/// Records <see cref="Invalidate"/> calls and stores added/removed layers.
/// </summary>
public sealed class FakeVirtualScreen : IVirtualScreen
{
    private readonly List<Layer> _layers = [];
    private readonly List<Rectangle> _invalidatedRegions = [];

    public FakeVirtualScreen(Size size) => Size = size;

    /// <inheritdoc />
    public Size Size { get; }

    /// <summary>All regions passed to <see cref="Invalidate"/> since the last <see cref="ClearInvalidations"/>.</summary>
    public IReadOnlyList<Rectangle> InvalidatedRegions => _invalidatedRegions;

    /// <summary>Currently registered layers.</summary>
    public IReadOnlyList<Layer> Layers => _layers;

    /// <inheritdoc />
    public Layer AddLayer(Rectangle bounds, int zOrder)
    {
        var layer = new Layer(bounds, zOrder);
        _layers.Add(layer);
        return layer;
    }

    /// <inheritdoc />
    public void RemoveLayer(Layer layer)
    {
        var vacated = layer.Bounds;
        _layers.Remove(layer);
        _invalidatedRegions.Add(vacated);
    }

    /// <inheritdoc />
    public void Invalidate(Rectangle region)
    {
        if (!region.IsEmpty)
            _invalidatedRegions.Add(region);
    }

    /// <inheritdoc />
    public bool HasDirtyRegions => _invalidatedRegions.Count > 0;

    /// <inheritdoc />
    public IReadOnlyList<Rectangle> FlushDirtyRegions()
    {
        var snapshot = _invalidatedRegions.ToList();
        _invalidatedRegions.Clear();
        return snapshot;
    }

    /// <inheritdoc />
    public IEnumerable<Layer> GetLayersInRegion(Rectangle region) =>
        _layers.Where(l => l.Bounds.IntersectsWith(region)).OrderBy(l => l.ZOrder);

    /// <summary>Clears the recorded invalidation list without flushing dirty state.</summary>
    public void ClearInvalidations() => _invalidatedRegions.Clear();
}
