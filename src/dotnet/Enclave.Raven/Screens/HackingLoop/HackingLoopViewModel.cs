using Enclave.Echelon.Core.Models;
using Enclave.Echelon.Core.Services;
using Enclave.Phosphor;
using Enclave.Raven.Input;
using Enclave.Raven.Screens.DataInput;
using Enclave.Raven.Services;
using Enclave.Shared.Models;
using Enclave.Shared.Phases;

namespace Enclave.Raven.Screens.HackingLoop;

/// <summary>
/// Orchestrates the hacking loop: suggests guesses, reads match counts, narrows candidates until solved,
/// then shows a "press any key to play again" prompt.
/// Uses <see cref="IFixedRegionReadLine"/> with <see cref="MatchCountKeyFilter"/> and <see cref="MatchCountValidator"/> so the input stays fixed.
/// </summary>
public sealed class HackingLoopViewModel(
    IGameSession session,
    IVirtualScreen screen,
    ICompositor compositor,
    IPhosphorCursor cursor,
    INavigationService navigation,
    IExitRequest exitRequest,
    ISolverFactory solverFactory,
    IFixedRegionReadLine fixedRegionReadLine) : IScreenViewModel
{
    private const int InputRegionRows = 2;

    /// <inheritdoc />
    public string Name => "HackingLoop";

    /// <inheritdoc />
    public Task RunAsync(CancellationToken ct = default)
    {
        var width = screen.Size.Width;
        var height = screen.Size.Height;
        var contentBounds = new Rectangle(0, 0, width, height - InputRegionRows);
        var inputBounds = new Rectangle(0, height - InputRegionRows, width, InputRegionRows);

        var contentLayer = screen.AddLayer(contentBounds, zOrder: 0);
        var inputLayer = screen.AddLayer(inputBounds, zOrder: 1);
        try
        {
            var contentComponent = new HackingLoopComponent(contentBounds);
            var inputComponent = new DataInputInputComponent(inputBounds);
            var solver = solverFactory.GetSolver();
            var wordLength = session.WordLength ?? 0;

            if (wordLength <= 0 || session.Count == 0)
            {
                contentComponent.Lines.Add("No candidates. Exiting.");
                RenderAndFlush(contentLayer, inputLayer, contentComponent, inputComponent);
            }
            else
            {
                RunSolverLoop(contentLayer, inputLayer, contentComponent, inputComponent, solver, wordLength);
            }

            WaitForReplay();
        }
        finally
        {
            screen.RemoveLayer(contentLayer);
            screen.RemoveLayer(inputLayer);
        }

        return Task.CompletedTask;
    }

    private void RunSolverLoop(
        Layer contentLayer,
        Layer inputLayer,
        HackingLoopComponent contentComponent,
        DataInputInputComponent inputComponent,
        IPasswordSolver solver,
        int wordLength)
    {
        while (true)
        {
            var guess = solver.GetBestGuess(session);
            if (guess == null)
            {
                contentComponent.Lines.Add("No candidates left. Exiting.");
                RenderAndFlush(contentLayer, inputLayer, contentComponent, inputComponent);
                return;
            }

            if (RunGuess(contentLayer, inputLayer, contentComponent, inputComponent, solver, guess, wordLength))
                return;
        }
    }

    private bool RunGuess(
        Layer contentLayer,
        Layer inputLayer,
        HackingLoopComponent contentComponent,
        DataInputInputComponent inputComponent,
        IPasswordSolver solver,
        Password guess,
        int wordLength)
    {
        contentComponent.Lines.Add(string.Empty);
        contentComponent.Lines.Add($"Suggested guess: `{guess.Word}`");
        var possibleMatchCounts = HackingLoopLogic.GetPossibleMatchCounts(guess, session);
        inputComponent.Prompt = HackingLoopLogic.FormatMatchCountPrompt(wordLength, possibleMatchCounts);
        inputComponent.CurrentLineContent = "";
        RenderAndFlush(contentLayer, inputLayer, contentComponent, inputComponent);
        cursor.MoveTo(0, inputComponent.NextRow);

        var line = fixedRegionReadLine.ReadLine(new ReadLineParams
        {
            InputLayer = inputLayer,
            InputView = inputComponent,
            KeyFilter = new MatchCountKeyFilter(),
            Validator = new MatchCountValidator(wordLength, possibleMatchCounts),
            OnInvalidInput = errorMsg =>
            {
                contentComponent.Lines.Add(errorMsg);
                RenderAndFlush(contentLayer, inputLayer, contentComponent, inputComponent);
                cursor.MoveTo(0, inputComponent.NextRow);
            }
        });
        if (line == null)
        {
            contentComponent.Lines.Add("Input closed.");
            RenderAndFlush(contentLayer, inputLayer, contentComponent, inputComponent);
            return true;
        }
        HackingLoopLogic.TryParseMatchCount(line, wordLength, possibleMatchCounts, out var matchCount);

        if (matchCount == wordLength)
        {
            contentComponent.Lines.Add(string.Empty);
            contentComponent.Lines.Add("Correct. Terminal cracked.");
            inputComponent.Prompt = "";
            inputComponent.CurrentLineContent = "";
            RenderAndFlush(contentLayer, inputLayer, contentComponent, inputComponent);
            return true;
        }

        var narrowed = solver.NarrowCandidates(session, guess, matchCount);
        HackingLoopLogic.ApplyNarrowedCandidates(session, narrowed);

        contentComponent.Lines.Add(string.Empty);
        contentComponent.Lines.Add($"{session.Count} candidate(s):");
        contentComponent.Lines.Add(CandidateListFormatter.Format(session, wordLength));
        RenderAndFlush(contentLayer, inputLayer, contentComponent, inputComponent);
        return false;
    }

    private void WaitForReplay()
    {
        if (exitRequest.IsExitRequested)
            navigation.NavigateTo("Exit");
        else
            navigation.NavigateTo("KeyPress", "Press any key to play again...", "DataInput");
    }

    private void RenderAndFlush(
        Layer contentLayer,
        Layer inputLayer,
        HackingLoopComponent contentComponent,
        DataInputInputComponent inputComponent)
    {
        contentLayer.Clear();
        contentComponent.Render(new LayerWriter(contentLayer));
        inputLayer.Clear();
        inputComponent.Render(new LayerWriter(inputLayer));

        screen.Invalidate(contentLayer.Bounds);
        screen.Invalidate(inputLayer.Bounds);
        foreach (var region in screen.FlushDirtyRegions())
            compositor.Flush(region);
    }
}
