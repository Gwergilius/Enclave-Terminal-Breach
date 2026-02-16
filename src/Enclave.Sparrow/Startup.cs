using Enclave.Echelon.Core.Services;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Models;
using Enclave.Sparrow.Phases;
using Enclave.Sparrow.Services;

namespace Enclave.Sparrow;

/// <summary>
/// Configures the SPARROW console application's dependency injection container.
/// </summary>
public static class Startup
{
    /// <summary>
    /// Registers all services required for the SPARROW UI: session state, I/O, solver, and phase components.
    /// </summary>
    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        // Session: shared state between data-input and hacking phases (one scope per run).
        services.AddScoped<IGameSession, GameSession>();

        // I/O: stdin/stdout abstraction for testability and sequential console I/O.
        services.AddSingleton<IConsoleIO, ConsoleIO>();

        // Solver: production tie-breaker strategy from Core.
        services.AddSingleton<IPasswordSolver, TieBreakerPasswordSolver>();

        // Phases: each phase is a separate component; order of execution is driven from Program.
        services.AddScoped<IStartupBadgePhase, StartupBadgePhase>();
        services.AddScoped<IDataInputPhase, DataInputPhase>();
        services.AddScoped<IHackingLoopPhase, HackingLoopPhase>();

        // Phase runner: executes phases in order.
        services.AddSingleton<IPhaseRunner>(sp => new PhaseRunner(
            sp.GetRequiredService<IServiceScopeFactory>(),
            [typeof(IStartupBadgePhase), typeof(IDataInputPhase), typeof(IHackingLoopPhase)]));

        return services;
    }
}
