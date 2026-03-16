using Enclave.Phosphor;
using Enclave.Raven.Input;

namespace Enclave.Raven.Screens.DataInput;

/// <summary>
/// Renders the input region (fixed 2 rows): prompt (row 0), input line (row 1).
/// The input line shows <see cref="CurrentLineContent"/> and is padded with spaces so the row is always fully overwritten (no leftover text).
/// Error display is a separate <see cref="DataInputErrorComponent"/>.
/// Implements <see cref="IReadLineInputView"/> for use with <see cref="IFixedRegionReadLine"/>.
/// </summary>
public sealed class DataInputInputComponent(Rectangle bounds) : IComponent, IReadLineInputView
{
    private const int PromptRow = 0;
    private const int InputLineRow = 1;

    /// <inheritdoc />
    public Rectangle Bounds { get; } = bounds;

    public string Prompt { get; set; } = string.Empty;

    /// <summary>Current line being typed; rendered on row 1 and padded with spaces to clear the full row.</summary>
    public string CurrentLineContent { get; set; } = string.Empty;

    /// <summary>Absolute screen row for the input line (cursor position).</summary>
    public int NextRow => Bounds.Top + InputLineRow;

    /// <inheritdoc />
    public void Render(LayerWriter writer)
    {
        writer.MoveTo(0, PromptRow);
        writer.Write(Prompt);
        writer.Write("\n");

        writer.MoveTo(0, InputLineRow);
        var pad = Bounds.Width - CurrentLineContent.Length;
        writer.Write(CurrentLineContent);
        if (pad > 0) writer.Write(new string(' ', pad));
        writer.Write("\n");
    }
}
