using Enclave.Common.Errors;
using Enclave.Shared.Phases;
using FluentResults;

namespace Enclave.Raven.Services;

/// <summary>
/// Default implementation of <see cref="IPhaseRegistry"/>: holds phases from the current scope (injected via <see cref="IEnumerable{IPhase}"/>)
/// and returns the matching instance by <see cref="IPhase.Name"/>.
/// </summary>
public sealed class PhaseRegistry : IPhaseRegistry
{
    private readonly IReadOnlyDictionary<string, IPhase> _nameToPhase;

    /// <summary>Initializes with the phases from the current scope (DI injects all registered <see cref="IPhase"/> implementations).</summary>
    /// <param name="phases">All phase instances from the current scope.</param>
    public PhaseRegistry(IEnumerable<IPhase> phases)
    {
        ArgumentNullException.ThrowIfNull(phases);
        _nameToPhase = phases.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public Result<IPhase> GetPhase(string phaseName)
    {
        if (_nameToPhase.TryGetValue(phaseName, out var phase))
            return Result.Ok(phase);
        return Result.Fail<IPhase>(new NotFoundError($"Phase '{phaseName}' is not registered."));
    }
}
