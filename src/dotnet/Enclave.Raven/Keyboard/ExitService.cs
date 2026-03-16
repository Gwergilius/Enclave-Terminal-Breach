using Enclave.Raven.Services;

namespace Enclave.Raven.Keyboard;

/// <summary>
/// Subscribes to the keyboard with high priority (1–10) and requests exit on Ctrl+C or Alt+F4.
/// </summary>
public sealed class ExitService(IKeyboardService keyboard, IExitRequest exitRequest)
{
    private const int Priority = 5; // High priority (1–10)
    private IDisposable? _subscription;

    /// <summary>
    /// Starts listening for Ctrl+C and Alt+F4. Call once at startup (e.g. from constructor or application init).
    /// </summary>
    public void Start()
    {
        _subscription ??= keyboard.Subscribe(Priority, OnKeyPressed);
    }

    private void OnKeyPressed(KeyPressedEventArgs e)
    {
        if (!IsExitKey(e.Key))
            return;
        exitRequest.RequestExit();
        e.Handled = true;
    }

    private static bool IsExitKey(ConsoleKeyInfo key)
    {
        if (key.KeyChar == '\x03') // Ctrl+C (ETX)
            return true;
        if (key.Key == ConsoleKey.C && (key.Modifiers & ConsoleModifiers.Control) != 0)
            return true;
        if (key.Key == ConsoleKey.F4 && (key.Modifiers & ConsoleModifiers.Alt) != 0)
            return true;
        return false;
    }
}
