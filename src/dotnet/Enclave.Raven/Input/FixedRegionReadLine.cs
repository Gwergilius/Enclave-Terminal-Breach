using System.Collections.Generic;
using Enclave.Phosphor;
using Enclave.Raven.Keyboard;

namespace Enclave.Raven.Input;

/// <summary>
/// Reads one line in a fixed screen region key-by-key (no console scroll).
/// Keys come from <see cref="IKeyboardService.GetNextKey"/> (after higher-priority handlers such as <see cref="ExitService"/>).
/// Supports <see cref="PushBack"/> for one-character lookahead (or multiple keys pushed by e.g. TAB completion).
/// </summary>
public sealed class FixedRegionReadLine(
    IKeyboardService keyboard,
    IVirtualScreen screen,
    ICompositor compositor,
    IPhosphorCursor cursor) : IFixedRegionReadLine
{
    private readonly Queue<ConsoleKeyInfo> _pushBack = new();

    /// <inheritdoc />
    public string? ReadLine(ReadLineParams p)
    {
        while (true)
        {
            var currentLine = "";
            p.InputView.CurrentLineContent = "";
            RenderInput(p);
            cursor.MoveTo(0, p.InputView.NextRow);

            while (true)
            {
                var key = GetNextKey();
                if (key is null)
                    return null;

                var result = p.KeyFilter.Handle(key.Value, currentLine);
                var rePrompt = false;

                switch (result.Kind)
                {
                    case KeyFilterKind.Enter:
                        if (p.Validator != null)
                        {
                            var (isValid, errorMessage) = p.Validator.Validate(currentLine);
                            if (!isValid)
                            {
                                p.OnInvalidInput?.Invoke(errorMessage ?? "Invalid input.");
                                p.InputView.CurrentLineContent = "";
                                RenderInput(p);
                                cursor.MoveTo(0, p.InputView.NextRow);
                                rePrompt = true;
                                break;
                            }
                        }
                        return currentLine;
                    case KeyFilterKind.Backspace when currentLine.Length > 0:
                        currentLine = currentLine[..^1];
                        p.InputView.CurrentLineContent = currentLine;
                        RenderInput(p);
                        cursor.MoveTo(currentLine.Length, p.InputView.NextRow);
                        break;
                    case KeyFilterKind.Append:
                        currentLine += result.Char;
                        p.InputView.CurrentLineContent = currentLine;
                        RenderInput(p);
                        cursor.MoveTo(currentLine.Length, p.InputView.NextRow);
                        break;
                    case KeyFilterKind.Special:
                        p.OnSpecialKey?.Invoke(key.Value);
                        p.RenderExtra?.Invoke();
                        cursor.MoveTo(currentLine.Length, p.InputView.NextRow);
                        break;
                    default:
                        break;
                }

                if (rePrompt)
                    break;
            }
        }
    }

    /// <inheritdoc />
    public bool KbHit() => _pushBack.Count > 0 || keyboard.KbHit();

    /// <inheritdoc />
    public ConsoleKeyInfo? ReadChar() => GetNextKey();

    /// <inheritdoc />
    public void PushBack(ConsoleKeyInfo key) => _pushBack.Enqueue(key);

    /// <summary>Gets the next key: from pushback buffer first, then from the keyboard service.</summary>
    private ConsoleKeyInfo? GetNextKey()
    {
        if (_pushBack.Count > 0)
            return _pushBack.Dequeue();
        return keyboard.GetNextKey();
    }

    private void RenderInput(ReadLineParams p)
    {
        p.InputLayer.Clear();
        p.InputView.Render(new LayerWriter(p.InputLayer));
        screen.Invalidate(p.InputLayer.Bounds);
        foreach (var region in screen.FlushDirtyRegions())
            compositor.Flush(region);
    }
}
