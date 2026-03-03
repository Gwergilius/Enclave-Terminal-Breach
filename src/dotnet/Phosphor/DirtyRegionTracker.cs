namespace Enclave.Phosphor;

/// <summary>
/// Tracks rectangular regions that require recomposition. Thread-safe.
/// </summary>
/// <remarks>
/// Adjacent or overlapping regions are merged into one bounding rectangle,
/// trading a slightly larger recomposition area for fewer compositor passes.
/// </remarks>
internal sealed class DirtyRegionTracker
{
    private readonly List<Rectangle> _regions = [];
    private readonly Lock _lock = new();

    public bool HasRegions
    {
        get { lock (_lock) return _regions.Count > 0; }
    }

    /// <summary>
    /// Marks <paramref name="region"/> as dirty. If it overlaps an existing dirty region,
    /// the two are merged into their bounding union; otherwise the region is appended.
    /// </summary>
    public void Invalidate(Rectangle region)
    {
        if (region.IsEmpty) return;

        lock (_lock)
        {
            for (int i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].IntersectsWith(region))
                {
                    _regions[i] = _regions[i].Union(region);
                    return;
                }
            }
            _regions.Add(region);
        }
    }

    /// <summary>
    /// Returns all pending dirty regions and clears the tracker.
    /// </summary>
    public IReadOnlyList<Rectangle> Flush()
    {
        lock (_lock)
        {
            var snapshot = _regions.ToList();
            _regions.Clear();
            return snapshot;
        }
    }
}
