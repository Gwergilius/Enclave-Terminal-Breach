using System.Diagnostics.CodeAnalysis;

namespace Enclave.Common.Drawing;

/// <summary>
/// A single character cell in the virtual screen buffer.
/// </summary>
/// <remarks>
/// A cell with <see cref="Character"/> == <c>'\0'</c> (default) is treated as transparent by the compositor —
/// it is skipped and whatever is on the layer below shows through.
/// Use <see cref="Space"/> for a visible empty cell.
/// </remarks>
[ExcludeFromCodeCoverage(Justification = "Simple data primitive — correctness verified by code review.")]
public readonly record struct VirtualCell(char Character, CharStyle Style = CharStyle.Normal)
{
    /// <summary>Transparent cell — compositor skips it, revealing the layer below.</summary>
    public static readonly VirtualCell Empty = new('\0');

    /// <summary>Visible space cell — paints the background at this position.</summary>
    public static readonly VirtualCell Space = new(' ');

    /// <summary>
    /// Returns <see langword="true"/> if this cell is transparent — the compositor skips it,
    /// revealing whatever is on the layer below.
    /// </summary>
    public bool IsEmpty => Character == '\0';
}
