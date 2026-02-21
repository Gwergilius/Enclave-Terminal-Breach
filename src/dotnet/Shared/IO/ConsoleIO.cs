using System.Diagnostics.CodeAnalysis;

namespace Enclave.Shared.IO;

/// <summary>
/// Standard console implementation: <see cref="Console"/> stdin/stdout.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Thin wrapper around Console.Write/ReadLine; testing would only verify BCL behavior.")]
public sealed class ConsoleIO : IConsoleIO
{
    /// <inheritdoc />
    public void Write(string? value) => Console.Write(value);

    /// <inheritdoc />
    public void WriteLine(string? value = null) => Console.WriteLine(value);

    /// <inheritdoc />
    public string? ReadLine() => Console.ReadLine();

    /// <inheritdoc />
    public int ReadInt(int min, int max, int defaultValue, string prompt = "Enter value: ", string? errorMessage = null)
        => ConsoleIntReader.Read(this, min, max, defaultValue, prompt, errorMessage);
}
