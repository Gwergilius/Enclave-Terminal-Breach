using Enclave.Sparrow.Phases;

namespace Enclave.Sparrow.Services;

/// <summary>
/// Executes phases in order, each within a shared service scope.
/// </summary>
public sealed class PhaseRunner(IServiceScopeFactory scopeFactory, Type[] phaseTypes) : IPhaseRunner
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    private readonly Type[] _phaseTypes = phaseTypes ?? throw new ArgumentNullException(nameof(phaseTypes));

    /// <inheritdoc />
    public void Run()
    {
        using var scope = _scopeFactory.CreateScope();
        foreach (var phaseType in _phaseTypes)
        {
            var phase = scope.ServiceProvider.GetRequiredService(phaseType) as IPhase
                ?? throw new InvalidOperationException($"Phase {phaseType.Name} does not implement {nameof(IPhase)}.");
            phase.Run();
        }
    }
}
