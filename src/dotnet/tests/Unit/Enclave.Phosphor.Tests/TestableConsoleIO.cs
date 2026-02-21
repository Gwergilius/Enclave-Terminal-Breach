using System.Collections.Concurrent;
using System.Text;
using Enclave.Common.Models;
using Enclave.Shared.IO;

namespace Enclave.Phosphor.Tests;

/// <summary>
/// Test double for <see cref="IConsoleIO"/> for AnsiPhosphorCanvas and PhosphorInputLoop unit tests.
/// Records all output, provides configurable dimensions, and allows key injection for input loop tests.
/// </summary>
internal sealed class TestableConsoleIO : IConsoleIO
{
    private readonly List<string> _written = new();
    private readonly BlockingCollection<ConsoleKeyInfo?> _keyQueue = new();

    /// <summary>All output from Write and WriteLine calls (each call is one element).</summary>
    public IReadOnlyList<string> Written => _written;

    /// <summary>Recorded semantic display calls (e.g. "HideCursor", "SetForegroundColor:#66FF66", "ClearScreen").</summary>
    public IReadOnlyList<string> SemanticCalls => _semanticCalls;

    private readonly List<string> _semanticCalls = new();

    /// <summary>Configurable dimensions for Initialize validation (default 80×24).</summary>
    public (int Width, int Height) Dimensions { get; set; } = (80, 24);

    /// <inheritdoc />
    public string Title { get; set; } = string.Empty;

    /// <inheritdoc />
    public Encoding OutputEncoding { get; set; } = Encoding.UTF8;

    /// <inheritdoc />
    public void Write(string? value) => _written.Add(value ?? string.Empty);

    /// <inheritdoc />
    public void WriteLine(string? value = null) => _written.Add(value ?? string.Empty);

    /// <inheritdoc />
    public string? ReadLine() => null;

    /// <inheritdoc />
    public ConsoleKeyInfo? ReadKey() => _keyQueue.Take();

    /// <summary>Injects a key for <see cref="ReadKey"/>. Use in PhosphorInputLoop tests. Call <see cref="SignalReadKeyEnd"/> to unblock with null.</summary>
    public void InjectKey(ConsoleKeyInfo key) => _keyQueue.Add(key);

    /// <summary>Signals end of key input (ReadKey will return null). Call to unblock the input loop.</summary>
    public void SignalReadKeyEnd() => _keyQueue.Add(null);

    /// <inheritdoc />
    public (int Width, int Height) GetDimensions() => Dimensions;

    /// <inheritdoc />
    public void Flush() { }

    /// <inheritdoc />
    public void HideCursor() => _semanticCalls.Add("HideCursor");

    /// <inheritdoc />
    public void ShowCursor() => _semanticCalls.Add("ShowCursor");

    /// <inheritdoc />
    public void SetForegroundColor(ColorValue color) => _semanticCalls.Add($"SetForegroundColor:{color.ToHex()}");

    /// <inheritdoc />
    public void SetBackgroundColor(ColorValue color) => _semanticCalls.Add($"SetBackgroundColor:{color.ToHex()}");

    /// <inheritdoc />
    public void ClearScreen() => _semanticCalls.Add("ClearScreen");

    /// <inheritdoc />
    public void ResetStyle() => _semanticCalls.Add("ResetStyle");
}
