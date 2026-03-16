namespace Enclave.Raven.Input;

/// <summary>
/// Reads one line in a fixed screen region key-by-key (no console scroll).
/// Uses <see cref="ReadLineParams"/> for prompt, key filter, optional validator, and callbacks.
/// Keys are obtained via <see cref="Enclave.Raven.Keyboard.IKeyboardService"/> (priority order); Ctrl+C/Alt+F4 are handled by <see cref="Enclave.Raven.Keyboard.ExitService"/>.
/// </summary>
public interface IFixedRegionReadLine
{
    /// <summary>
    /// True when a key is available: either in the pushback buffer or from the keyboard (next <see cref="ReadChar"/> or <see cref="ReadLine"/> will not block).
    /// </summary>
    bool KbHit();

    /// <summary>
    /// Reads a single key from the input buffer: first from pushback (any <see cref="PushBack"/>-ed keys), then from the keyboard service. Returns null if none available (e.g. stream closed, exit requested).
    /// </summary>
    ConsoleKeyInfo? ReadChar();

    /// <summary>
    /// Reads keys until Enter (or special handling). Validates if <see cref="ReadLineParams.Validator"/> is set; on invalid, calls <see cref="ReadLineParams.OnInvalidInput"/> and continues.
    /// </summary>
    /// <param name="params">Prompt, layer, view, key filter, validator, and callbacks.</param>
    /// <returns>The entered line, or null if input was cancelled (e.g. Ctrl+C, Alt+F4, exit requested).</returns>
    string? ReadLine(ReadLineParams @params);

    /// <summary>
    /// Pushes a key back into the input buffer (lookahead). The next <see cref="ReadChar"/> or key read inside <see cref="ReadLine"/> will be this key.
    /// Used e.g. by higher-priority subscribers (TAB completion) to inject characters before the normal key stream.
    /// </summary>
    void PushBack(ConsoleKeyInfo key);
}
