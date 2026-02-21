namespace Enclave.Shared.IO;

/// <summary>
/// Abstraction for reading line and key input. Implemented by <see cref="IConsoleIO"/> and keyboard handlers
/// so that Blazor/MAUI and console can share the same <see cref="ConsoleReaderExtensions.ReadInt"/> behavior.
/// </summary>
public interface IConsoleReader
{
    /// <summary>Reads one line from input. Returns null if no more input (e.g. EOF, Ctrl+C).</summary>
    string? ReadLine();

    /// <summary>Reads a single key. Blocks until a key is available. Returns null when input is not available.</summary>
    ConsoleKeyInfo? ReadKey();
}
