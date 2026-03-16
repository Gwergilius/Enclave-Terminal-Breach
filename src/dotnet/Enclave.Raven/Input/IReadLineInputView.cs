using Enclave.Phosphor;

namespace Enclave.Raven.Input;

/// <summary>
/// View contract for the fixed-region read-line: prompt, current line, cursor row, and render.
/// Implemented by e.g. <see cref="Screens.DataInput.DataInputInputComponent"/>.
/// </summary>
public interface IReadLineInputView
{
    string Prompt { get; set; }
    string CurrentLineContent { get; set; }
    int NextRow { get; }
    void Render(LayerWriter writer);
}
