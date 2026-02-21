using System.Text;
using Enclave.Common.Models;

namespace Enclave.Shared.IO;

/// <summary>
/// Abstraction for sequential console input and output (stdin/stdout). Enables testability and aligns with PHOSPHOR/RAVEN no-cursor, no-colour constraint.
/// Extended with PHOSPHOR requirements: Title, OutputEncoding, GetDimensions, Flush, and semantic display operations (cursor, colours, clear).
/// Implementations decide how to achieve these (e.g. ANSI escape sequences on a real terminal).
/// </summary>
public interface IConsoleIO
{
    /// <summary>Gets or sets the console window title.</summary>
    string Title { get; set; }

    /// <summary>Gets or sets the output encoding (e.g. UTF-8 for box-drawing characters).</summary>
    Encoding OutputEncoding { get; set; }

    /// <summary>Writes the given text to stdout (no newline).</summary>
    void Write(string? value);

    /// <summary>Writes a line to stdout (appends newline).</summary>
    void WriteLine(string? value = null);

    /// <summary>Reads one line from stdin. Returns null if no more input (e.g. Ctrl+C).</summary>
    string? ReadLine();

    /// <summary>Reads a single key. Blocks until a key is available. Returns null when input is not available (e.g. no console).</summary>
    ConsoleKeyInfo? ReadKey();

    /// <summary>Returns the terminal dimensions (columns, rows).</summary>
    (int Width, int Height) GetDimensions();

    /// <summary>Flushes any buffered output.</summary>
    void Flush();

    /// <summary>Hides the system cursor.</summary>
    void HideCursor();

    /// <summary>Shows the system cursor.</summary>
    void ShowCursor();

    /// <summary>Sets the foreground colour for subsequent output.</summary>
    void SetForegroundColor(ColorValue color);

    /// <summary>Sets the background colour for subsequent output.</summary>
    void SetBackgroundColor(ColorValue color);

    /// <summary>Clears the entire screen and moves the cursor to the top-left.</summary>
    void ClearScreen();

    /// <summary>Resets colour and other display attributes to default.</summary>
    void ResetStyle();

    /// <summary>Prompts until a valid integer in [min, max] is entered. Returns <paramref name="defaultValue"/> when ReadLine returns null.</summary>
    /// <param name="min">Minimum allowed value (inclusive).</param>
    /// <param name="max">Maximum allowed value (inclusive).</param>
    /// <param name="defaultValue">Value returned when input is null (e.g. EOF).</param>
    /// <param name="prompt">Optional prompt text before reading.</param>
    /// <param name="errorMessage">Optional error message when input is invalid.</param>
    /// <returns>An integer in [min, max].</returns>
    int ReadInt(int min, int max, int defaultValue, string prompt = "Enter value: ", string? errorMessage = null);
}
