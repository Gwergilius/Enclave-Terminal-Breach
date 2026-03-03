using System.Diagnostics;

namespace Enclave.Phosphor;

/// <summary>
/// A stateful writer that renders a stream of characters into a <see cref="Layer"/>,
/// interpreting <c>\n</c> as a CRLF-style line break (moves to column 0 of the next row).
/// </summary>
/// <remarks>
/// Maintains an internal cursor position and <see cref="Style"/> within the layer's bounds.
/// Characters written past the right edge are silently clipped; the internal column counter
/// still advances so that a subsequent <c>\n</c> resets it correctly.
/// Writes that exceed the bottom of the layer are discarded — scroll behaviour is not
/// implemented and is deferred to PHOSPHOR 2.1.
/// </remarks>
public sealed class LayerWriter(Layer layer)
{
    private int _col;
    private int _row;

    /// <summary>Width of the layer in columns.</summary>
    public int Width => layer.Bounds.Width;

    /// <summary>Height of the layer in rows.</summary>
    public int Height => layer.Bounds.Height;

    /// <summary>The character style applied to subsequent <see cref="Write"/> calls.</summary>
    public CharStyle Style { get; set; } = CharStyle.Normal;

    /// <summary>
    /// Moves the internal cursor to (<paramref name="col"/>, <paramref name="row"/>) relative
    /// to the layer's top-left corner.
    /// </summary>
    public void MoveTo(int col, int row)
    {
        _col = col;
        _row = row;
    }

    /// <summary>
    /// Writes <paramref name="text"/> into the layer at the current cursor position.
    /// <c>\n</c> moves to column 0 of the next row. Only printable characters and <c>\n</c>
    /// are supported; other control characters are rejected by a <see cref="Debug.Assert"/>.
    /// </summary>
    public void Write(string text)
    {
        foreach (var ch in text)
        {
            if (ch == '\n')
            {
                _col = 0;
                _row++;
                continue;
            }

            Debug.Assert(!char.IsControl(ch),
                $"Control character U+{(int)ch:X4} in LayerWriter.Write — " +
                "only printable characters and '\\n' are supported.");

            if (_row >= Height) return;

            if (_col < Width)
                layer.SetCell(layer.Bounds.Left + _col, layer.Bounds.Top + _row,
                    new VirtualCell(ch, Style));

            // Advance the column even past the right edge so '\n' still resets correctly.
            _col++;
        }
    }

    /// <summary>
    /// Sets a single cell at a position relative to the layer's top-left corner,
    /// bypassing the internal cursor. Writes outside the layer bounds are silently ignored.
    /// </summary>
    public void SetCell(int relCol, int relRow, VirtualCell cell)
    {
        if (relCol < 0 || relCol >= Width || relRow < 0 || relRow >= Height) return;
        layer.SetCell(layer.Bounds.Left + relCol, layer.Bounds.Top + relRow, cell);
    }
}
