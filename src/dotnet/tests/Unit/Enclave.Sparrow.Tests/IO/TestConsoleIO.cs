using Enclave.Sparrow.IO;

namespace Enclave.Sparrow.Tests.IO;

/// <summary>
/// Test double for IConsoleIO that returns configurable ReadLine values.
/// ReadInt delegates to ConsoleIntReader for real behavior testing.
/// </summary>
internal sealed class TestConsoleIO : IConsoleIO
{
    private readonly Queue<string?> _readLineResponses = new();
    private readonly List<string> _writtenLines = new();
    private readonly List<string> _written = new();

    public IReadOnlyList<string> WrittenLines => _writtenLines;
    public IReadOnlyList<string> Written => _written;

    public void AddReadLineResponse(string? response)
    {
        _readLineResponses.Enqueue(response);
    }

    public string? ReadLine()
    {
        return _readLineResponses.Count > 0 ? _readLineResponses.Dequeue() : null;
    }

    public void Write(string? value)
    {
        _written.Add(value ?? "");
    }

    public void WriteLine(string? value = null)
    {
        _writtenLines.Add(value ?? "");
    }

    public int ReadInt(int min, int max, int defaultValue, string prompt = "Enter value: ", string? errorMessage = null)
        => ConsoleIntReader.Read(this, min, max, defaultValue, prompt, errorMessage);
}
