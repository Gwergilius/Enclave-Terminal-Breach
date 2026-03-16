using System.Diagnostics.CodeAnalysis;

namespace Enclave.Raven.Keyboard;

/// <summary>
/// Event data for <see cref="IKeyboardService.KeyPressed"/>.
/// Set <see cref="Handled"/> to <see langword="true"/> to consume the key so lower-priority subscribers are not notified.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "POCO, verify by review")]
public sealed class KeyPressedEventArgs
{
    public KeyPressedEventArgs(ConsoleKeyInfo key) => Key = key;

    /// <summary>The key that was pressed.</summary>
    public ConsoleKeyInfo Key { get; }

    /// <summary>Set to <see langword="true"/> to mark the key as handled; no further subscribers will receive it.</summary>
    public bool Handled { get; set; }
}
