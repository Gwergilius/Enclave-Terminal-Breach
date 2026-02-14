namespace Enclave.Sparrow.IO;

/// <summary>
/// Abstraction for sequential console input and output (stdin/stdout). Enables testability and aligns with SPARROW's no-cursor, no-colour constraint.
/// </summary>
public interface IConsoleIO
{
    /// <summary>Writes the given text to stdout (no newline).</summary>
    void Write(string? value);

    /// <summary>Writes a line to stdout (appends newline).</summary>
    void WriteLine(string? value = null);

    /// <summary>Reads one line from stdin. Returns null if no more input (e.g. Ctrl+C).</summary>
    string? ReadLine();
}
