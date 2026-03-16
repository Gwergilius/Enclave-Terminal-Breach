using System.Runtime.InteropServices;

namespace Enclave.Phosphor;

/// <summary>
/// A rectangular region of the virtual screen owned by a single UI component.
/// Layers are ordered by <see cref="ZOrder"/>; higher values render on top.
/// </summary>
/// <remarks>
/// Creates a new layer occupying <paramref name="bounds"/> at the given <paramref name="zOrder"/>.
/// All cells are initialised to <see cref="VirtualCell.Empty"/> (transparent).
/// </remarks>
public sealed class Layer(Rectangle bounds, int zOrder)
{
    /// <summary>Z-order of this layer. Higher values render on top of lower values.</summary>
    public int ZOrder { get; } = zOrder;

    /// <summary>Absolute screen bounds of this layer.</summary>
    public Rectangle Bounds { get; private set; } = bounds;

    /// <summary>When false, the compositor skips this layer during recomposition.</summary>
    public bool IsVisible { get; set; } = true;

    // Internal buffer indexed as [relRow, relCol] (relative to Bounds.Location)
    private readonly VirtualCell[,] _buffer = new VirtualCell[bounds.Height, bounds.Width];

    /// <summary>
    /// Returns the cell at absolute screen position (<paramref name="col"/>, <paramref name="row"/>).
    /// </summary>
    public VirtualCell GetCell(int col, int row) =>
        _buffer[row - Bounds.Top, col - Bounds.Left];

    /// <summary>
    /// Sets the cell at absolute screen position (<paramref name="col"/>, <paramref name="row"/>).
    /// </summary>
    public void SetCell(int col, int row, VirtualCell cell)
    {
        Guard.RequirePrintableForCell(cell);
        _buffer[row - Bounds.Top, col - Bounds.Left] = cell;
    }

    /// <summary>
    /// Fills the entire buffer with <see cref="VirtualCell.Empty"/> (transparent).
    /// </summary>
    public void Clear()
    {
        // MemoryMarshal: treats 2D array as flat Span, with a single fill
        MemoryMarshal
            .CreateSpan(ref _buffer[0, 0], _buffer.Length)
            .Fill(VirtualCell.Empty);
    }

    /// <summary>
    /// Moves the layer to <paramref name="newLocation"/>; the buffer contents are preserved.
    /// </summary>
    public void MoveTo(Point newLocation)
    {
        Bounds = new Rectangle(newLocation, Bounds.Dimension);
    }
}
