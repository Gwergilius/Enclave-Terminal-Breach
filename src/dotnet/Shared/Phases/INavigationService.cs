using FluentResults;

namespace Enclave.Shared.Phases;

/// <summary>
/// Service for phase-to-phase navigation. Phases call <see cref="NavigateTo"/> to transition and return its result from <see cref="IPhase.Run"/>;
/// the caller (e.g. Application) reads <see cref="NextPhase"/> after each phase run.
/// </summary>
public interface INavigationService
{
    /// <summary>Gets the name of the next phase to run (set by <see cref="NavigateTo"/>).</summary>
    string? NextPhase { get; }

    /// <summary>Gets the arguments to pass to the next phase's <see cref="IPhase.Run"/> (set by <see cref="NavigateTo"/>).</summary>
    IReadOnlyList<object> NextPhaseArgs { get; }

    /// <summary>
    /// Requests navigation to the given phase. For normal phases: sets <see cref="NextPhase"/> and <see cref="NextPhaseArgs"/> and returns <see cref="Result.Ok"/>.
    /// For <c>"Exit"</c>: the first argument may be a <see cref="Result"/> (e.g. a failed result to communicate exit reason); if present, that result is returned; otherwise returns <see cref="Result.Fail"/>(<see cref="ApplicationExit"/>). The runner can use <c>while (result.IsSuccess)</c> and treat a non-success result as exit (optionally dumping <see cref="Result.Errors"/> when the error is not <see cref="ApplicationExit"/>).
    /// </summary>
    /// <param name="phaseName">Unique name of the phase to run next (e.g. "DataInput"), or <c>"Exit"</c> to signal application exit.</param>
    /// <param name="args">Optional arguments: for normal phases, passed to the next phase's <see cref="IPhase.Run"/>; for <c>"Exit"</c>, the first argument may be a <see cref="Result"/> to return as the exit result.</param>
    /// <returns><see cref="Result.Ok"/> for normal navigation; for <c>"Exit"</c>, the optional result or <see cref="Result.Fail"/>(<see cref="ApplicationExit"/>).</returns>
    Result NavigateTo(string phaseName, params object[] args);
}