namespace Enclave.Raven.Input;

/// <summary>
/// Key filter for password-candidate input: printable characters (letters, digits, punctuation, space, minus), Backspace, Enter; Up/Down as Special.
/// </summary>
public sealed class DataInputKeyFilter : IReadLineKeyFilter
{
    /// <inheritdoc />
    public KeyFilterResult Handle(ConsoleKeyInfo key, string currentLine)
    {
        if (key.Key == ConsoleKey.UpArrow)
            return KeyFilterResult.Special;
        if (key.Key == ConsoleKey.DownArrow)
            return KeyFilterResult.Special;
        if (key.Key == ConsoleKey.Enter)
            return KeyFilterResult.Enter;
        if (key.Key == ConsoleKey.Backspace && currentLine.Length > 0)
            return KeyFilterResult.Backspace;
        if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || key.KeyChar == ' ' || key.KeyChar == '-')
            return KeyFilterResult.AppendChar(key.KeyChar);
        return KeyFilterResult.Ignore;
    }
}
