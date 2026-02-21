using System.Diagnostics.CodeAnalysis;
using Enclave.Phosphor;
using Enclave.Shared.IO;

namespace Enclave.Raven.IO;

/// <summary>
/// Default production implementation of <see cref="IPhosphorReader"/> using <see cref="IConsoleIO"/>.
/// Delegates <see cref="ReadKey"/> and <see cref="ReadLine"/> to the console; <see cref="OnKeyPressed"/> returns false (pass through).
/// </summary>
public sealed class ConsoleKeyboardHandler([NotNull] IConsoleIO console) : IPhosphorReader
{
    private readonly IConsoleIO _console = console;

    /// <inheritdoc />
    public string? ReadLine() => _console.ReadLine();

    /// <inheritdoc />
    public ConsoleKeyInfo? ReadKey() => _console.ReadKey();

    /// <inheritdoc />
    public bool OnKeyPressed(ConsoleKeyInfo key) => false;
}
