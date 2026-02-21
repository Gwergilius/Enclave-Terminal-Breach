using Enclave.Shared.IO;

namespace Enclave.Phosphor;

/// <summary>
/// Keyboard input: reading keys/lines and handling key events. Extends <see cref="IConsoleReader"/> so that
/// console, Blazor and MAUI can share the same reader contract. Register with <see cref="IPhosphorInputLoop"/> for event handling.
/// </summary>
public interface IPhosphorReader : IConsoleReader
{
    /// <summary>
    /// Called when the user presses a key (e.g. from <see cref="IPhosphorInputLoop.Run"/>).
    /// </summary>
    /// <param name="key">The key information.</param>
    /// <returns>True to consume the event (stop further processing); false to pass through.</returns>
    bool OnKeyPressed(ConsoleKeyInfo key);
}
