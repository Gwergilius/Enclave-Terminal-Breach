using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Enclave.Phosphor;
using Enclave.Raven.Services;
using Enclave.Shared.Phases;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave.Raven;

/// <summary>
/// Main application phase: initializes the terminal canvas, then runs phases in a loop by resolving the next phase from <see cref="INavigationService"/> and <see cref="IPhaseRegistry"/> until the phase is "Exit".
/// Phases are resolved from <see cref="ICurrentScope"/> so that scope resets (e.g. before DataInput) provide fresh scoped services.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Orchestration only; phases and I/O are tested separately.")]
public sealed class Application(
    IPhosphorCanvas canvas,
    IPhosphorWriter writer,
    ICurrentScope currentScope,
    INavigationService navigation,
    IProductInfo productInfo) : IPhase
{
    /// <inheritdoc />
    public string Name => "Application";

    /// <inheritdoc />
    public Result Run(params object[] args)
    {
        var title = $"{productInfo.Name} {productInfo.Version} – ENCLAVE SIGINT";
        canvas.Initialize(title);

        try
        {
            var result = navigation.NavigateTo("StartupBadge");

            while (result.IsSuccess)
            {
                var nextPhase = navigation.NextPhase;
                if (string.IsNullOrEmpty(nextPhase))
                    break;

                var phaseRegistry = currentScope.CurrentScope.ServiceProvider.GetRequiredService<IPhaseRegistry>();
                var getPhaseResult = phaseRegistry.GetPhase(nextPhase);
                if (getPhaseResult.IsFailed)
                {
                    result = Result.Fail(getPhaseResult.Errors);
                    break;
                }

                var phase = getPhaseResult.Value;
                var nextArgs = navigation.NextPhaseArgs?.ToArray() ?? [];
                result = phase.Run(nextArgs);
            }

            if (result.IsFailed && result.Errors.Any(e => e is not ApplicationExit))
            {
                foreach (var error in result.Errors)
                    canvas.WriteLine(error.Message);
            }

            return result;
        }
        finally
        {
            if (writer is IDisposable writerDisposable)
                writerDisposable.Dispose();
            canvas.Dispose();
        }
    }
}