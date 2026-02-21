namespace Enclave.Phosphor.Tests;

/// <summary>
/// Test double for <see cref="IPhosphorWriter"/> that records all Write/WriteLine calls.
/// </summary>
public sealed class TestPhosphorWriter : IPhosphorWriter
{
    private readonly List<(string Text, CharStyle Style)> _recorded = new();

    /// <summary>
    /// All recorded Write/WriteLine calls as (Text, Style) pairs.
    /// Style is captured at the moment of each call.
    /// </summary>
    public IReadOnlyList<(string Text, CharStyle Style)> Recorded => _recorded;

    /// <inheritdoc />
    public CharStyle Style { get; set; } = CharStyle.Normal;

    /// <inheritdoc />
    public void Write(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        _recorded.Add((text, Style));
    }

    /// <inheritdoc />
    public void WriteLine(string? text = null)
    {
        _recorded.Add((text ?? string.Empty, Style));
    }

    /// <summary>Clears all recorded output.</summary>
    public void Clear() => _recorded.Clear();
}
