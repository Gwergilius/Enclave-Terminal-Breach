namespace Enclave.Raven.Keyboard;

/// <summary>
/// Central keyboard service: reads keys and notifies subscribers in priority order (lower number = higher priority).
/// Subscribers can mark a key as handled so that lower-priority subscribers do not receive it.
/// </summary>
public interface IKeyboardService
{
    /// <summary>
    /// Subscribe to key events. Lower <paramref name="priority"/> = higher priority (e.g. 1 before 100).
    /// Priority must be a positive integer.
    /// </summary>
    /// <param name="priority">Positive integer; lower value is higher priority (e.g. 1–10 for exit, 100 for input).</param>
    /// <param name="handler">Called when a key is read; set <see cref="KeyPressedEventArgs.Handled"/> to consume the key.</param>
    /// <returns>A disposable to unsubscribe.</returns>
    IDisposable Subscribe(int priority, Action<KeyPressedEventArgs> handler);

    /// <summary>
    /// True when a key is available; the next <see cref="GetNextKey"/> call will not block (key comes from the underlying input).
    /// </summary>
    bool KbHit();

    /// <summary>
    /// Blocks until the next key is read, notifies subscribers in priority order, then returns the key if no one handled it (or null if stream closed / exit requested after a handled key).
    /// If a subscriber sets <see cref="KeyPressedEventArgs.Handled"/>, the key is consumed and this method loops to read the next key (or returns null when exit was requested).
    /// </summary>
    ConsoleKeyInfo? GetNextKey();
}
