namespace Enclave.Phosphor;

/// <summary>
/// Low-level primitive for sequential text output with style support.
/// All rendering in PHOSPHOR 1.0 is top-to-bottom, left-to-right.
/// </summary>
public interface IPhosphorWriter
{
    /// <summary>
    /// Current character style. All subsequent Write/WriteLine calls use this style
    /// until it is changed. Defaults to <see cref="CharStyle.Normal"/>.
    /// </summary>
    CharStyle Style { get; set; }

    /// <summary>Writes text at the current cursor position using <see cref="Style"/>.</summary>
    /// <param name="text">Text to write.</param>
    void Write(string text);

    /// <summary>
    /// Writes text followed by a newline using <see cref="Style"/>.
    /// When called with no argument, advances to the next line only.
    /// </summary>
    /// <param name="text">Text to write, or null to write only a newline.</param>
    void WriteLine(string? text = null);
}
