namespace Enclave.Phosphor;

/// <summary>
/// Translates dirty screen regions from the virtual buffer into physical console writes.
/// </summary>
public interface ICompositor
{
    /// <summary>Recomposes, diffs, and emits the minimum set of console writes for <paramref name="region"/>.</summary>
    void Flush(Rectangle region);
}
