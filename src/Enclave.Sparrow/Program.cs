using Enclave.Sparrow.Phases;

namespace Enclave.Sparrow;

internal static class Program
{
    private static readonly Type[] _phases =
    [
        typeof(IStartupBadgePhase),
        typeof(IDataInputPhase),
        typeof(IHackingLoopPhase),
    ];

    private static int Main(string[] args)
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        foreach (var phaseType in _phases)
        {
            var phase = scope.ServiceProvider.GetRequiredService(phaseType) as IPhase
                ?? throw new InvalidOperationException($"Phase {phaseType.Name} does not implement {nameof(IPhase)}.");
            phase.Run();
        }

        return 0;
    }
}
