using System.Diagnostics.CodeAnalysis;
using System.Text;
using Enclave.Common.Models;

namespace Enclave.Shared.IO;

/// <summary>
/// Standard console implementation: <see cref="Console"/> stdin/stdout.
/// Semantic display operations (cursor, colours, clear) are implemented via ANSI escape sequences.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Thin wrapper around Console.Write/ReadLine; testing would only verify BCL behavior.")]
public sealed class ConsoleIO : IConsoleIO
{
    private static void WriteAnsi(string code) => Console.Write(code);
    /// <inheritdoc />
    public string Title
    {
        get => Console.Title;
        set => Console.Title = value;
    }

    /// <inheritdoc />
    public Encoding OutputEncoding
    {
        get => Console.OutputEncoding;
        set => Console.OutputEncoding = value;
    }

    /// <inheritdoc />
    public void Write(string? value)
    {
        if (value is null)
            return;
        Console.Write(value);
    }

    /// <inheritdoc />
    public void WriteLine(string? value = null) => Console.WriteLine(value);

    /// <inheritdoc />
    public string? ReadLine()
    {
        ShowCursor();
        try
        {
            return Console.ReadLine();
        }
        finally
        {
            HideCursor();
        }
    }

    /// <inheritdoc />
    public ConsoleKeyInfo? ReadKey()
    {
        ShowCursor();
        try
        {
            return Console.ReadKey(intercept: true);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        finally
        {
            HideCursor();
        }
    }

    /// <inheritdoc />
    public (int Width, int Height) GetDimensions()
    {
        try
        {
            return (Console.WindowWidth, Console.WindowHeight);
        }
        catch
        {
            return (80, 24);
        }
    }

    /// <inheritdoc />
    public void Flush() => Console.Out.Flush();

    /// <inheritdoc />
    public void HideCursor() => WriteAnsi("\x1b[?25l");

    /// <inheritdoc />
    public void ShowCursor() => WriteAnsi("\x1b[?25h");

    /// <inheritdoc />
    public void SetForegroundColor(ColorValue color) => WriteAnsi($"\x1b[38;2;{color.R};{color.G};{color.B}m");

    /// <inheritdoc />
    public void SetBackgroundColor(ColorValue color) => WriteAnsi($"\x1b[48;2;{color.R};{color.G};{color.B}m");

    /// <inheritdoc />
    public void ClearScreen() => WriteAnsi("\x1b[2J\x1b[H");

    /// <inheritdoc />
    public void ResetStyle() => WriteAnsi("\x1b[0m");
}
