using System.Diagnostics.CodeAnalysis;
using Enclave.Phosphor;
using Enclave.Raven.Services;
using Enclave.Shared.Phases;
using FluentResults;

namespace Enclave.Raven.Phases;

/// <summary>
/// Shows "Press any key to play again...", waits for a key, clears the screen, then navigates to "ResetScope" or "Exit" if exit was requested.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Thin I/O and navigation; behaviour covered by integration.")]
public sealed class PlayAgainPhase(
    IPhosphorCanvas canvas,
    IPhosphorReader reader,
    IExitRequest exitRequest,
    INavigationService navigation) : IPlayAgainPhase
{
    /// <inheritdoc />
    public string Name => "PlayAgain";

    /// <inheritdoc />
    public Result Run(params object[] args)
    {
        if (exitRequest.IsExitRequested)
            return navigation.NavigateTo("Exit");

        canvas.WriteLine();
        canvas.WriteLine("Press any key to play again...");
        reader.ReadKey();

        if (exitRequest.IsExitRequested)
            return navigation.NavigateTo("Exit");

        canvas.ClearScreen();
        return navigation.NavigateTo("ResetScope", "DataInput");
    }
}