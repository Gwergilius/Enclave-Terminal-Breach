namespace Enclave.Phosphor;

/// <summary>
/// Default implementation of <see cref="IVirtualScreen"/>.
/// </summary>
/// <remarks>
/// Creates a virtual screen of the given <paramref name="size"/>.
/// </remarks>
public sealed class VirtualScreen(Size size) : IVirtualScreen
{
    private readonly List<Layer> _layers = [];
    private readonly DirtyRegionTracker _dirtyTracker = new();
    private readonly Lock _layersLock = new();

    /// <inheritdoc />
    public Size Size { get; } = size;

    /// <inheritdoc />
    public Layer AddLayer(Rectangle bounds, int zOrder)
    {
        var layer = new Layer(bounds, zOrder);
        lock (_layersLock)
        {
            _layers.Add(layer);
        }
        return layer;
    }

    /// <inheritdoc />
    public void RemoveLayer(Layer layer)
    {
        Rectangle vacated;
        lock (_layersLock)
        {
            vacated = layer.Bounds;
            _layers.Remove(layer);
        }
        _dirtyTracker.Invalidate(vacated);
    }

    /// <inheritdoc />
    public void Invalidate(Rectangle region) => _dirtyTracker.Invalidate(region);

    /// <inheritdoc />
    public bool HasDirtyRegions => _dirtyTracker.HasRegions;

    /// <inheritdoc />
    public IReadOnlyList<Rectangle> FlushDirtyRegions() => _dirtyTracker.Flush();

    /// <inheritdoc />
    public IEnumerable<Layer> GetLayersInRegion(Rectangle region)
    {
        lock (_layersLock)
        {
            return [.. _layers
                .Where(l => l.Bounds.IntersectsWith(region))
                .OrderBy(l => l.ZOrder)]; // materialise under lock
        }
    }
}
