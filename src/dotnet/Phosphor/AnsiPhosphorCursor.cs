using System.Diagnostics.CodeAnalysis;
using Enclave.Shared.IO;

namespace Enclave.Phosphor;

/// <summary>
/// Positions the terminal cursor using ANSI escape sequences.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Thin ANSI escape wrapper — correctness verified by code review.")]
public sealed class AnsiPhosphorCursor(IConsoleWriter writer) : IPhosphorCursor
{
    /// <inheritdoc />
    /// <remarks>
    /// Emits <c>ESC[{row+1};{col+1}H</c> (ANSI CUP — Cursor Position, 1-based).
    /// </remarks>
    public void MoveTo(int col, int row)
    {
        // ANSI CUP: ESC [ row ; col H  (1-based)
        writer.Write($"\x1b[{row + 1};{col + 1}H");
    }
}
