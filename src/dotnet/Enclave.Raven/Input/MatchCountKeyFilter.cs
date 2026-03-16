namespace Enclave.Raven.Input;

/// <summary>
/// Key filter for match-count input: only digits 0–9 (max 2 characters), Backspace, Enter.
/// </summary>
public sealed class MatchCountKeyFilter : IReadLineKeyFilter
{
    private readonly int _maxLength;

    public MatchCountKeyFilter(int maxLength = 2)
    {
        _maxLength = maxLength;
    }

    /// <inheritdoc />
    public KeyFilterResult Handle(ConsoleKeyInfo key, string currentLine)
    {
        if (key.Key == ConsoleKey.Enter)
            return KeyFilterResult.Enter;
        if (key.Key == ConsoleKey.Backspace && currentLine.Length > 0)
            return KeyFilterResult.Backspace;
        if (key.KeyChar >= '0' && key.KeyChar <= '9' && currentLine.Length < _maxLength)
            return KeyFilterResult.AppendChar(key.KeyChar);
        return KeyFilterResult.Ignore;
    }
}
