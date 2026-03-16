using System.Diagnostics.CodeAnalysis;
using Enclave.Phosphor;
using Enclave.Raven.Services;
using Enclave.Shared.Phases;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave.Raven;

/// <summary>
/// Main application runner: initializes the terminal, then executes screens in a loop by resolving the next screen
/// from <see cref="INavigationService"/> and <see cref="IViewModelRegistry"/> until navigation reaches "Exit".
/// Screens are resolved from <see cref="ICurrentScope"/>; scope is reset before each "DataInput" navigation
/// so that the next round gets a fresh <see cref="Enclave.Shared.Models.IGameSession"/>.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Orchestration only; screens and I/O are tested separately.")]
public sealed class Application(
    AnsiPhosphorCanvas canvas,
    ICurrentScope currentScope,
    INavigationService navigation,
    IProductInfo productInfo)
{
    /// <summary>Runs the application until navigation reaches "Exit" or <paramref name="ct"/> is cancelled.</summary>
    public async Task<Result> RunAsync(CancellationToken ct = default)
    {
        var title = $"{productInfo.Name} {productInfo.Version} – ENCLAVE SIGINT";
        canvas.Initialize(title);

        try
        {
            navigation.NavigateTo("BootScreen");

            while (!ct.IsCancellationRequested)
            {
                var nextScreen = navigation.NextPhase;
                if (string.IsNullOrEmpty(nextScreen))
                    break;
                if (string.Equals(nextScreen, "Exit", StringComparison.OrdinalIgnoreCase))
                    break;

                // Scope reset before DataInput ensures a fresh IGameSession for each round.
                if (string.Equals(nextScreen, "DataInput", StringComparison.OrdinalIgnoreCase))
                    currentScope.ResetScope();

                var vmRegistry = currentScope.CurrentScope.ServiceProvider
                    .GetRequiredService<IViewModelRegistry>();
                var getVmResult = vmRegistry.GetViewModel(nextScreen);
                if (getVmResult.IsFailed)
                    return Result.Fail(getVmResult.Errors);

                await getVmResult.Value.RunAsync(ct);
            }

            return Result.Fail(new ApplicationExit());
        }
        finally
        {
            canvas.Dispose();
        }
    }
}
