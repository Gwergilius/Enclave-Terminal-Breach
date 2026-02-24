using Enclave.Echelon.Core.Services;
using Enclave.Sparrow.Configuration;
using Enclave.Shared.IO;
using Enclave.Shared.Models;
using Enclave.Shared.Services;
using Enclave.Sparrow.Phases;
using Microsoft.Extensions.Configuration;

namespace Enclave.Sparrow;

/// <summary>
/// Configures the SPARROW console application's dependency injection container.
/// </summary>
public static class Startup
{
    /// <summary>
    /// Registers all services required for the SPARROW UI: configuration, session state, I/O, solver, and phase components.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Application configuration (command-line overrides + appsettings.json + defaults).</param>
    public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var sparrowSection = configuration.GetSection("Sparrow");
        var options = new SparrowOptions();
        sparrowSection.Bind(options);

        services.AddSingleton(options);
        services.AddSingleton<IConfiguration>(configuration);

        // Session: shared state between data-input and hacking phases (one scope per run).
        services.AddScoped<IGameSession, GameSession>();

        // I/O: stdin/stdout abstraction for testability and sequential console I/O.
        services.AddSingleton<IConsoleIO, ConsoleIO>();

        // Random: single instance for solver tie-breaking / random choice (non-security use).
        services.AddSingleton<IRandom, GameRandom>();

        // Solvers: each strategy registered as IPasswordSolver; factory selects by config.
        services.AddSingleton<IPasswordSolver, HouseGambitPasswordSolver>();
        services.AddSingleton<IPasswordSolver, BestBucketPasswordSolver>();
        services.AddSingleton<IPasswordSolver, TieBreakerPasswordSolver>();

        // Solver factory: resolves requested solver from the registered set using config (SparrowOptions).
        services.AddSingleton<ISolverConfiguration>(sp => sp.GetRequiredService<SparrowOptions>());
        services.AddSingleton<ISolverFactory, SolverFactory>();

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
