using System.Text;
using Enclave.Common.Models;

namespace Enclave.Shared.IO;

/// <summary>
/// Abstraction for sequential console input and output (stdin/stdout). Enables testability and aligns with PHOSPHOR/RAVEN no-cursor, no-colour constraint.
/// Extends <see cref="IConsoleReader"/> (ReadLine, ReadKey) and adds output, dimensions, and semantic display operations.
/// Use <see cref="ConsoleReaderExtensions.ReadInt"/> for validated integer input.
/// </summary>
public interface IConsoleIO : IConsoleReader, IConsoleWriter
{
    /// <summary>Gets or sets the console window title.</summary>
    string Title { get; set; }

    /// <summary>Returns the terminal dimensions (columns, rows).</summary>
    (int Width, int Height) GetDimensions();
    Encoding OutputEncoding { get; set; }

    void ClearScreen();
    void Flush();
    void HideCursor();
    void ResetStyle();
    void SetBackgroundColor(ColorValue color);
    void SetForegroundColor(ColorValue color);
    void ShowCursor();
}
