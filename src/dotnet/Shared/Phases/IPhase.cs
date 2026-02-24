using FluentResults;

namespace Enclave.Shared.Phases;

/// <summary>
/// Common contract for all phases: a single executable step with <see cref="Run"/> and a unique <see cref="Name"/>.
/// </summary>
public interface IPhase
{
    /// <summary>Unique identifier for this phase (used by navigation).</summary>
    string Name { get; }

    /// <summary>Executes the phase. Returns <see cref="Result.Ok"/> on success, or <see cref="Result.Fail"/> / the result of <see cref="INavigationService.NavigateTo"/> to signal navigation or errors.</summary>
    /// <param name="args">Optional arguments passed from the previous phase via <see cref="INavigationService.NavigateTo(string, object[])"/> (may be empty).</param>
    Result Run(params object[] args);
}
