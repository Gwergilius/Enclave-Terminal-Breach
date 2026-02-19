using Enclave.Echelon.Core.Services;
using Enclave.Sparrow.Configuration;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Models;
using Enclave.Sparrow.Phases;
using Enclave.Sparrow.Services;
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

        // Solver: chosen by Intelligence (0 = HouseGambit, 1 = BestBucket, 2 = TieBreaker). Use raw config so CLI aliases (e.g. "house") work.
        const int seed = 17;
        var rawIntelligence = configuration["Sparrow:Intelligence"];
        var intelligence = SparrowIntelligence.Normalize(rawIntelligence ?? options.Intelligence);
        var solver = SolverByIntelligence.GetSolver(intelligence, seed);
        services.AddSingleton<IPasswordSolver>(_ => solver);

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
