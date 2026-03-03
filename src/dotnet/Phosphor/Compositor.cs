namespace Enclave.Phosphor;

/// <summary>
/// Translates dirty regions from the virtual screen into physical console writes.
/// </summary>
/// <remarks>
/// Three phases per dirty region:
/// <list type="number">
///   <item><term>Recompose</term><description>Painter's algorithm: layers painted bottom-to-top into a composite buffer.</description></item>
///   <item><term>Diff</term><description>Composite buffer compared with last-written physical buffer; only changed cells are queued.</description></item>
///   <item><term>Emit</term><description>Changed cells grouped into same-style runs per row and written via <see cref="IPhosphorWriter"/>.</description></item>
/// </list>
/// </remarks>
public sealed class Compositor
{
    private readonly IVirtualScreen _screen;
    private readonly IPhosphorWriter _writer;
    private readonly IPhosphorCursor _cursor;

    // Buffers are allocated once for the full screen size.
    private readonly VirtualCell[,] _compositeBuffer;
    private readonly VirtualCell[,] _physicalBuffer;

    /// <summary>
    /// Creates a compositor for <paramref name="screen"/> that emits output via <paramref name="writer"/>
    /// and positions the cursor via <paramref name="cursor"/>.
    /// </summary>
    public Compositor(IVirtualScreen screen, IPhosphorWriter writer, IPhosphorCursor cursor)
    {
        ArgumentNullException.ThrowIfNull(screen);
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(cursor);

        _screen = screen;
        _writer = writer;
        _cursor = cursor;

        _compositeBuffer = new VirtualCell[screen.Size.Height, screen.Size.Width];
        _physicalBuffer  = new VirtualCell[screen.Size.Height, screen.Size.Width];

        // Initialise physical buffer to Space so the first full flush always emits everything.
        FillBuffer(_physicalBuffer, VirtualCell.Space);
    }

    /// <summary>
    /// Recomposes <paramref name="region"/>, diffs against the last physical state,
    /// and emits the minimum set of writes to the console.
    /// </summary>
    public void Flush(Rectangle region)
    {
        Recompose(region);
        var writes = Diff(region);
        Emit(writes);
    }

    // --- Phase 1: Recompose -------------------------------------------------

    private void Recompose(Rectangle region)
    {
        FillRegion(_compositeBuffer, region, VirtualCell.Space);

        foreach (var layer in _screen.GetLayersInRegion(region).Where(l => l.IsVisible))
        {
            var overlap = layer.Bounds.Intersect(region);
            if (overlap.IsEmpty) continue;

            for (int row = overlap.Top; row <= overlap.Bottom; row++)
            for (int col = overlap.Left; col <= overlap.Right; col++)
            {
                var cell = layer.GetCell(col, row);
                if (!cell.IsEmpty)
                    _compositeBuffer[row, col] = cell;
            }
        }
    }

    // --- Phase 2: Diff ------------------------------------------------------

    private List<CellWrite> Diff(Rectangle region)
    {
        var writes = new List<CellWrite>();
        for (int row = region.Top; row <= region.Bottom; row++)
        for (int col = region.Left; col <= region.Right; col++)
        {
            var composed = _compositeBuffer[row, col];
            if (composed != _physicalBuffer[row, col])
            {
                writes.Add(new CellWrite(col, row, composed));
                _physicalBuffer[row, col] = composed;
            }
        }
        return writes;
    }

    // --- Phase 3: Emit ------------------------------------------------------

    private void Emit(List<CellWrite> writes)
    {
        // Track the physical cursor position so consecutive same-row runs need no repositioning.
        // null = position unknown (first write always emits MoveTo).
        int? cursorCol = null;
        int? cursorRow = null;

        foreach (var rowGroup in writes.GroupBy(w => w.Row).OrderBy(g => g.Key))
        {
            var run = new System.Text.StringBuilder();
            CharStyle? runStyle = null;
            int? runStartCol = null;
            int? prevCol = null;

            foreach (var write in rowGroup.OrderBy(w => w.Col))
            {
                bool sameStyle = write.Cell.Style == runStyle;
                bool adjacent  = prevCol.HasValue && write.Col == prevCol.Value + 1;

                if (!sameStyle || !adjacent)
                {
                    FlushRun(runStartCol, rowGroup.Key, runStyle, run, ref cursorCol, ref cursorRow);
                    run.Clear();
                    runStyle    = write.Cell.Style;
                    runStartCol = write.Col;
                }

                run.Append(write.Cell.Character);
                prevCol = write.Col;
            }

            FlushRun(runStartCol, rowGroup.Key, runStyle, run, ref cursorCol, ref cursorRow);
        }
    }

    private void FlushRun(
        int? col, int row, CharStyle? style,
        System.Text.StringBuilder text,
        ref int? cursorCol, ref int? cursorRow)
    {
        if (text.Length == 0) return;

        if (col!.Value != cursorCol || row != cursorRow)
            _cursor.MoveTo(col.Value, row);

        _writer.Style = style!.Value;
        _writer.Write(text.ToString());

        // After the write the terminal cursor sits right after the last written character.
        cursorCol = col.Value + text.Length;
        cursorRow = row;
    }

    // --- Helpers ------------------------------------------------------------

    private static void FillBuffer(VirtualCell[,] buffer, VirtualCell cell)
    {
        for (int r = 0; r < buffer.GetLength(0); r++)
            for (int c = 0; c < buffer.GetLength(1); c++)
                buffer[r, c] = cell;
    }

    private static void FillRegion(VirtualCell[,] buffer, Rectangle region, VirtualCell cell)
    {
        for (int row = region.Top; row <= region.Bottom; row++)
            for (int col = region.Left; col <= region.Right; col++)
                buffer[row, col] = cell;
    }

    // --- Internal record ----------------------------------------------------

    private readonly record struct CellWrite(int Col, int Row, VirtualCell Cell);
}
