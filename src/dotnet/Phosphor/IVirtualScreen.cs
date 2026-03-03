namespace Enclave.Phosphor;

/// <summary>
/// The single source of truth for the intended screen state.
/// Maintains an ordered collection of <see cref="Layer"/>s and tracks which
/// regions need recomposition after a change.
/// </summary>
public interface IVirtualScreen
{
    /// <summary>Screen dimensions (columns × rows).</summary>
    Size Size { get; }

    /// <summary>
    /// Registers a new layer occupying <paramref name="bounds"/> at <paramref name="zOrder"/>.
    /// The layer is inserted into the Z-order at the correct position.
    /// </summary>
    Layer AddLayer(Rectangle bounds, int zOrder);

    /// <summary>
    /// Removes <paramref name="layer"/> and invalidates the region it occupied,
    /// so the layers below it are recomposed on the next flush.
    /// </summary>
    void RemoveLayer(Layer layer);

    /// <summary>
    /// Marks <paramref name="region"/> as requiring recomposition.
    /// Called by components after they write to their layer.
    /// </summary>
    void Invalidate(Rectangle region);

    /// <summary>Returns <c>true</c> if any dirty regions are pending recomposition.</summary>
    bool HasDirtyRegions { get; }

    /// <summary>Returns pending dirty regions and clears the tracker.</summary>
    IReadOnlyList<Rectangle> FlushDirtyRegions();

    /// <summary>
    /// Returns the layers whose bounds intersect <paramref name="region"/>, in ascending Z-order.
    /// </summary>
    IEnumerable<Layer> GetLayersInRegion(Rectangle region);
}
