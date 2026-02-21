using System.Text;
using Enclave.Common.Models;
using Enclave.Shared.IO;

namespace Enclave.Shared.Tests.IO;

/// <summary>
/// Test double for IConsoleIO that returns configurable ReadLine values and dimensions.
/// ReadInt delegates to ConsoleIntReader for real behavior testing.
/// </summary>
internal sealed class TestConsoleIO : IConsoleIO
{
    private readonly Queue<string?> _readLineResponses = new();
    private readonly List<string> _writtenLines = new();
    private readonly List<string> _written = new();

    public IReadOnlyList<string> WrittenLines => _writtenLines;
    public IReadOnlyList<string> Written => _written;

    /// <summary>Configurable dimensions for PHOSPHOR tests (default 80×24).</summary>
    public (int Width, int Height) Dimensions { get; set; } = (80, 24);

    public void AddReadLineResponse(string? response)
    {
        _readLineResponses.Enqueue(response);
    }

    /// <inheritdoc />
    public string Title { get; set; } = string.Empty;

    /// <inheritdoc />
    public Encoding OutputEncoding { get; set; } = Encoding.UTF8;

    public string? ReadLine()
    {
        return _readLineResponses.Count > 0 ? _readLineResponses.Dequeue() : null;
    }

    /// <inheritdoc />
    public ConsoleKeyInfo? ReadKey() => null;

    public void Write(string? value)
    {
        _written.Add(value ?? "");
    }

    public void WriteLine(string? value = null)
    {
        _writtenLines.Add(value ?? "");
    }

    /// <inheritdoc />
    public (int Width, int Height) GetDimensions() => Dimensions;

    /// <inheritdoc />
    public void Flush() { }

    /// <inheritdoc />
    public void HideCursor() { }

    /// <inheritdoc />
    public void ShowCursor() { }

    /// <inheritdoc />
    public void SetForegroundColor(ColorValue color) { }

    /// <inheritdoc />
    public void SetBackgroundColor(ColorValue color) { }

    /// <inheritdoc />
    public void ClearScreen() { }

    /// <inheritdoc />
    public void ResetStyle() { }

    public int ReadInt(int min, int max, int defaultValue, string prompt = "Enter value: ", string? errorMessage = null)
        => ConsoleIntReader.Read(this, min, max, defaultValue, prompt, errorMessage);
}
