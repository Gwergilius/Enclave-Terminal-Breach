using Enclave.Phosphor;

namespace Enclave.Raven.Input;

/// <summary>
/// Parameters for a single fixed-region read-line: view, layer, key filter, optional validator, and callbacks.
/// </summary>
public sealed class ReadLineParams
{
    /// <summary>Layer that contains the prompt + input line (cleared and re-rendered on each update).</summary>
    public required Layer InputLayer { get; init; }

    /// <summary>View to update (Prompt, CurrentLineContent) and render.</summary>
    public required IReadLineInputView InputView { get; init; }

    /// <summary>Key filter: which keys append, backspace, enter, or special.</summary>
    public required IReadLineKeyFilter KeyFilter { get; init; }

    /// <summary>Optional. If set, on Enter the line is validated; if invalid, <see cref="OnInvalidInput"/> is called and input continues.</summary>
    public IReadLineValidator? Validator { get; init; }

    /// <summary>Called when Enter is pressed and validation fails. Callback should show the error (e.g. add a line to content) and re-render; runner then continues with empty line.</summary>
    public Action<string>? OnInvalidInput { get; init; }

    /// <summary>Called when the key filter returns <see cref="KeyFilterKind.Special"/> (e.g. Up/Down for error paging).</summary>
    public Action<ConsoleKeyInfo>? OnSpecialKey { get; init; }

    /// <summary>Called after <see cref="OnSpecialKey"/> to re-render extra layers (e.g. error bar).</summary>
    public Action? RenderExtra { get; init; }
}
