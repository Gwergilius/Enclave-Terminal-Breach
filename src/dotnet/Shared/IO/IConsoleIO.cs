namespace Enclave.Shared.IO;

/// <summary>
/// Abstraction for sequential console input and output (stdin/stdout). Enables testability and aligns with PHOSPHOR/RAVEN no-cursor, no-colour constraint.
/// </summary>
public interface IConsoleIO
{
    /// <summary>Writes the given text to stdout (no newline).</summary>
    void Write(string? value);

    /// <summary>Writes a line to stdout (appends newline).</summary>
    void WriteLine(string? value = null);

    /// <summary>Reads one line from stdin. Returns null if no more input (e.g. Ctrl+C).</summary>
    string? ReadLine();

    /// <summary>Prompts until a valid integer in [min, max] is entered. Returns <paramref name="defaultValue"/> when ReadLine returns null.</summary>
    /// <param name="min">Minimum allowed value (inclusive).</param>
    /// <param name="max">Maximum allowed value (inclusive).</param>
    /// <param name="defaultValue">Value returned when input is null (e.g. EOF).</param>
    /// <param name="prompt">Optional prompt text before reading.</param>
    /// <param name="errorMessage">Optional error message when input is invalid.</param>
    /// <returns>An integer in [min, max].</returns>
    int ReadInt(int min, int max, int defaultValue, string prompt = "Enter value: ", string? errorMessage = null);
}
