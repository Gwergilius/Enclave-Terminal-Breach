namespace Enclave.Phosphor;

/// <summary>
/// Handler for keyboard events. Register with <see cref="IPhosphorInputLoop"/>.
/// </summary>
public interface IPhosphorKeyboardHandler
{
    /// <summary>
    /// Called when the user presses a key.
    /// </summary>
    /// <param name="key">The key information.</param>
    /// <returns>True to consume the event (stop further processing); false to pass through.</returns>
    bool OnKeyPressed(ConsoleKeyInfo key);
}
