using Enclave.Phosphor;
using Enclave.Raven.Configuration;
using Enclave.Raven.Input;
using Enclave.Shared.Models;
using Enclave.Shared.Phases;

namespace Enclave.Raven.Screens.DataInput;

/// <summary>
/// Orchestrates data input: loads candidates from file when <see cref="RavenOptions.WordListPath"/> is set,
/// otherwise prompts interactively until an empty line is entered.
/// Uses separate layers for candidates (top), input (2 rows: prompt + input line), and error (1 row).
/// Uses <see cref="IFixedRegionReadLine"/> with <see cref="DataInputKeyFilter"/> so the input stays fixed; Up/Down page through errors.
/// </summary>
public sealed class DataInputViewModel(
    IGameSession session,
    IVirtualScreen screen,
    ICompositor compositor,
    IPhosphorCursor cursor,
    INavigationService navigation,
    RavenOptions options,
    IFixedRegionReadLine fixedRegionReadLine) : IScreenViewModel
{
    private const int InputRegionRows = 2;
    private const int ErrorRegionRows = 1;
    private const int BottomRegionRows = InputRegionRows + ErrorRegionRows;

    /// <inheritdoc />
    public string Name => "DataInput";

    /// <summary>Collected validation errors from the last applied line (empty after clear).</summary>
    internal IReadOnlyList<string> LastErrors { get; set; } = [];

    /// <summary>Index into <see cref="LastErrors"/> for the currently displayed message (used with Up/Down).</summary>
    internal int CurrentErrorIndex { get; set; }

    /// <inheritdoc />
    public Task RunAsync(CancellationToken ct = default)
    {
        var width = screen.Size.Width;
        var height = screen.Size.Height;
        var candidatesBounds = new Rectangle(0, 0, width, height - BottomRegionRows);
        var inputBounds = new Rectangle(0, height - BottomRegionRows, width, InputRegionRows);
        var errorBounds = new Rectangle(0, height - ErrorRegionRows, width, ErrorRegionRows);

        var candidatesLayer = screen.AddLayer(candidatesBounds, zOrder: 0);
        var inputLayer = screen.AddLayer(inputBounds, zOrder: 1);
        var errorLayer = screen.AddLayer(errorBounds, zOrder: 2);
        try
        {
            var candidatesComponent = new DataInputCandidatesComponent(candidatesBounds);
            var inputComponent = new DataInputInputComponent(inputBounds);
            var errorComponent = new DataInputErrorComponent(errorBounds);

            if (!string.IsNullOrWhiteSpace(options.WordListPath))
                RunFileMode(candidatesLayer, inputLayer, errorLayer, candidatesComponent, inputComponent, errorComponent, options.WordListPath!);
            else
                RunInteractiveMode(candidatesLayer, inputLayer, errorLayer, candidatesComponent, inputComponent, errorComponent);

            navigation.NavigateTo("HackingLoop");
        }
        finally
        {
            screen.RemoveLayer(candidatesLayer);
            screen.RemoveLayer(inputLayer);
            screen.RemoveLayer(errorLayer);
        }

        return Task.CompletedTask;
    }

    private void RunFileMode(
        Layer candidatesLayer,
        Layer inputLayer,
        Layer errorLayer,
        DataInputCandidatesComponent candidatesComponent,
        DataInputInputComponent inputComponent,
        DataInputErrorComponent errorComponent,
        string path)
    {
        if (!File.Exists(path))
            LastErrors = ["Word list file not found: " + path];
        else
        {
            var fileErrors = new List<string>();
            foreach (var line in File.ReadLines(path))
            {
                var errors = DataInputLogic.ApplyLineCollectErrors(line, session);
                fileErrors.AddRange(errors);
            }
            LastErrors = fileErrors;
        }
        CurrentErrorIndex = 0;
        UpdateCandidateState(candidatesComponent);
        RenderAndFlush(candidatesLayer, inputLayer, errorLayer, candidatesComponent, inputComponent, errorComponent);
    }

    private void RunInteractiveMode(
        Layer candidatesLayer,
        Layer inputLayer,
        Layer errorLayer,
        DataInputCandidatesComponent candidatesComponent,
        DataInputInputComponent inputComponent,
        DataInputErrorComponent errorComponent)
    {
        inputComponent.Prompt = "Enter password candidates:";
        inputComponent.CurrentLineContent = "";
        LastErrors = [];
        RenderAndFlush(candidatesLayer, inputLayer, errorLayer, candidatesComponent, inputComponent, errorComponent);
        cursor.MoveTo(0, inputComponent.NextRow);

        string? line;
        while ((line = ReadLineWithParams(inputLayer, errorLayer, inputComponent, errorComponent)) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            LastErrors = [];
            CurrentErrorIndex = 0;
            inputComponent.CurrentLineContent = "";
            RenderAndFlush(candidatesLayer, inputLayer, errorLayer, candidatesComponent, inputComponent, errorComponent);

            LastErrors = DataInputLogic.ApplyLineCollectErrors(line, session);
            CurrentErrorIndex = 0;
            UpdateCandidateState(candidatesComponent);
            inputComponent.Prompt = "Enter more password candidates (empty line to finish):";
            inputComponent.CurrentLineContent = "";
            RenderAndFlush(candidatesLayer, inputLayer, errorLayer, candidatesComponent, inputComponent, errorComponent);
            cursor.MoveTo(0, inputComponent.NextRow);
        }
    }

    private string? ReadLineWithParams(
        Layer inputLayer,
        Layer errorLayer,
        DataInputInputComponent inputComponent,
        DataInputErrorComponent errorComponent)
    {
        var p = new ReadLineParams
        {
            InputLayer = inputLayer,
            InputView = inputComponent,
            KeyFilter = new DataInputKeyFilter(),
            OnSpecialKey = key =>
            {
                if (key.Key == ConsoleKey.UpArrow && LastErrors.Count > 1)
                    CurrentErrorIndex = (CurrentErrorIndex - 1 + LastErrors.Count) % LastErrors.Count;
                if (key.Key == ConsoleKey.DownArrow && LastErrors.Count > 1)
                    CurrentErrorIndex = (CurrentErrorIndex + 1) % LastErrors.Count;
                UpdateErrorDisplay(errorComponent);
            },
            RenderExtra = () =>
            {
                errorLayer.Clear();
                errorComponent.Render(new LayerWriter(errorLayer));
                screen.Invalidate(errorLayer.Bounds);
                foreach (var region in screen.FlushDirtyRegions())
                    compositor.Flush(region);
            }
        };
        return fixedRegionReadLine.ReadLine(p);
    }

    private void UpdateErrorDisplay(DataInputErrorComponent errorComponent)
    {
        if (LastErrors.Count == 0)
            errorComponent.ErrorMessage = null;
        else
        {
            var msg = LastErrors[CurrentErrorIndex];
            errorComponent.ErrorMessage = LastErrors.Count > 1 ? "↑↓ " + msg : msg;
        }
    }

    private void UpdateCandidateState(DataInputCandidatesComponent component)
    {
        var (count, list) = DataInputLogic.GetCandidateState(session);
        component.CandidateCountLine = count;
        component.CandidateListText  = list;
    }

    private void RenderAndFlush(
        Layer candidatesLayer,
        Layer inputLayer,
        Layer errorLayer,
        DataInputCandidatesComponent candidatesComponent,
        DataInputInputComponent inputComponent,
        DataInputErrorComponent errorComponent)
    {
        UpdateErrorDisplay(errorComponent);

        candidatesLayer.Clear();
        candidatesComponent.Render(new LayerWriter(candidatesLayer));
        inputLayer.Clear();
        inputComponent.Render(new LayerWriter(inputLayer));
        errorLayer.Clear();
        errorComponent.Render(new LayerWriter(errorLayer));

        screen.Invalidate(candidatesLayer.Bounds);
        screen.Invalidate(inputLayer.Bounds);
        screen.Invalidate(errorLayer.Bounds);
        foreach (var region in screen.FlushDirtyRegions())
            compositor.Flush(region);
    }
}
