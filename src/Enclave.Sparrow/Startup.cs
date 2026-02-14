using Enclave.Echelon.Core.Services;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Phases;
using Enclave.Sparrow.Session;

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

        return services;
    }
}
