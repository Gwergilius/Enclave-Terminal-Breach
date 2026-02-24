using Enclave.Shared.Phases;
using FluentResults;

namespace Enclave.Raven.Services;

/// <summary>
/// Provides the phase instance for a given phase name (from the current scope).
/// </summary>
public interface IPhaseRegistry
{
    /// <summary>Gets the phase instance for the given phase name (e.g. "DataInput").</summary>
    /// <param name="phaseName">Unique phase name.</param>
    /// <returns><see cref="Result{T}.Value"/> with the phase when found; <see cref="Result{T}.IsFailed"/> with <see cref="NotFoundError"/> when the phase is not registered.</returns>
    Result<IPhase> GetPhase(string phaseName);
}
