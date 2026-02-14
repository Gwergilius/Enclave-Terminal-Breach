namespace Enclave.Sparrow.IO;

/// <summary>
/// Standard console implementation: <see cref="Console"/> stdin/stdout.
/// </summary>
public sealed class ConsoleIO : IConsoleIO
{
    /// <inheritdoc />
    public void Write(string? value) => Console.Write(value);

    /// <inheritdoc />
    public void WriteLine(string? value = null) => Console.WriteLine(value);

    /// <inheritdoc />
    public string? ReadLine() => Console.ReadLine();
}
