namespace Enclave.Raven.Input;

/// <summary>
/// Filters keys in the read-line loop: which keys append a character, backspace, submit, or trigger special handling.
/// </summary>
public interface IReadLineKeyFilter
{
    /// <summary>
    /// Process a key. Return <see cref="KeyFilterResult.AppendChar"/> to append, <see cref="KeyFilterResult.Backspace"/>,
    /// <see cref="KeyFilterResult.Enter"/>, <see cref="KeyFilterResult.Ignore"/>, or <see cref="KeyFilterResult.Special"/>.
    /// </summary>
    /// <param name="key">The key that was pressed.</param>
    /// <param name="currentLine">Current line content (e.g. to enforce max length).</param>
    KeyFilterResult Handle(ConsoleKeyInfo key, string currentLine);
}
