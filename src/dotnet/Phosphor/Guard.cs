namespace Enclave.Phosphor;

/// <summary>
/// Validation helpers for layer and writer input. Throws in all build configurations
/// (unlike <see cref="System.Diagnostics.Debug.Assert"/>, which is no-op in Release).
/// </summary>
internal static class Guard
{
    /// <summary>
    /// Ensures the cell is either empty or contains a printable character (code &gt;= space).
    /// C0 control characters (code &lt; U+0020) are rejected.
    /// </summary>
    /// <param name="cell">The cell to validate.</param>
    /// <exception cref="System.ArgumentException">Thrown when the cell has a control character and is not empty.</exception>
    public static void RequirePrintableForCell(VirtualCell cell)
    {
        if (cell.IsEmpty) return;
        if (cell.Character >= ' ') return;

        throw new ArgumentException(
            $"Control character U+{(int)cell.Character:X4} must not be stored in a VirtualCell. Use LayerWriter for streaming writes.",
            nameof(cell));
    }

    /// <summary>
    /// Ensures the character is printable (code &gt;= space). C0 control characters are rejected.
    /// Caller must handle <c>\n</c> separately before calling this.
    /// </summary>
    /// <param name="ch">The character to validate.</param>
    /// <exception cref="System.ArgumentException">Thrown when the character is a C0 control character.</exception>
    public static void RequirePrintableChar(char ch)
    {
        if (ch >= ' ') return;

        throw new ArgumentException(
            $"Control character U+{(int)ch:X4} in LayerWriter.Write — only printable characters and '\\n' are supported.",
            nameof(ch));
    }
}
