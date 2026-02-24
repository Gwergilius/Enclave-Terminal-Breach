using Enclave.Echelon.Core.Services;
using Enclave.Raven.Configuration;
using Enclave.Raven.IO;
using Enclave.Shared.IO;
using Enclave.Shared.Models;
using Enclave.Shared.Phases;
using Enclave.Shared.Services;
using Enclave.Raven.Phases;
using Enclave.Phosphor;
using Microsoft.Extensions.Configuration;
using Enclave.Raven.Services;

namespace Enclave.Raven;

/// <summary>
/// Configures the RAVEN console application's dependency injection container.
/// </summary>
public static class Startup
{
    /// <summary>
    /// Registers all services required for the RAVEN UI: configuration, session state, I/O, Phosphor, solver, and phase components.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Application configuration (command-line overrides + appsettings.json + defaults).</param>
    public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var ravenSection = configuration.GetSection("Raven");
        var options = new RavenOptions();
        ravenSection.Bind(options);

        services.AddSingleton(options);
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IProductInfo>(_ => ProductInfo.GetCurrent());

        // Session: shared state between data-input and hacking phases (one scope per run).
        services.AddScoped<IGameSession, GameSession>();

        // I/O: low-level console for Phosphor and for input (ReadLine, ReadInt).
        services.AddSingleton<IConsoleIO, ConsoleIO>();

        // Phosphor 1.0: terminal output and canvas (RAVEN uses IPhosphorWriter for output).
        services.AddSingleton<IScreenOptions>(_ => new ScreenOptions());
        services.AddSingleton(_ => PhosphorThemeFactory.Create("green"));
        services.AddSingleton<AnsiPhosphorCanvas>();
        services.AddSingleton<IPhosphorCanvas>(sp => sp.GetRequiredService<AnsiPhosphorCanvas>());
        services.AddSingleton<IPhosphorWriter>(sp => sp.GetRequiredService<AnsiPhosphorCanvas>());

        // Keyboard: default handler delegates to IConsoleIO (ReadKey, ReadLine).
        services.AddSingleton<IPhosphorReader, ConsoleKeyboardHandler>();

        // Random: single instance for solver tie-breaking / random choice (non-security use).
        services.AddSingleton<IRandom, GameRandom>();

        // Solvers: each strategy registered as IPasswordSolver; factory selects by config.
        services.AddSingleton<IPasswordSolver, HouseGambitPasswordSolver>();
        services.AddSingleton<IPasswordSolver, BestBucketPasswordSolver>();
        services.AddSingleton<IPasswordSolver, TieBreakerPasswordSolver>();

        // Solver factory: resolves requested solver from the registered set using config (RavenOptions).
        services.AddSingleton<ISolverConfiguration>(sp => sp.GetRequiredService<RavenOptions>());
        services.AddSingleton<ISolverFactory, SolverFactory>();

        // Navigation and scope: phase names and current scope for resolution.
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ICurrentScope, CurrentScopeHolder>();
        services.AddScoped<PhaseRegistry>();
        services.AddScoped<IPhaseRegistry>(sp => sp.GetRequiredService<PhaseRegistry>());

        // Phases: each phase is a separate component; PhaseRegistry receives them via IEnumerable<IPhase>.
        services.AddScoped<IStartupBadgePhase, StartupBadgePhase>();
        services.AddScoped<IPhase>(sp => sp.GetRequiredService<IStartupBadgePhase>());
        services.AddScoped<IResetScopePhase, ResetScopePhase>();
        services.AddScoped<IPhase>(sp => sp.GetRequiredService<IResetScopePhase>());
        services.AddScoped<IDataInputPhase, DataInputPhase>();
        services.AddScoped<IPhase>(sp => sp.GetRequiredService<IDataInputPhase>());
        services.AddScoped<IHackingLoopPhase, HackingLoopPhase>();
        services.AddScoped<IPhase>(sp => sp.GetRequiredService<IHackingLoopPhase>());
        services.AddScoped<IPlayAgainPhase, PlayAgainPhase>();
        services.AddScoped<IPhase>(sp => sp.GetRequiredService<IPlayAgainPhase>());

        // Phase runner: executes phases in order (legacy; used by tests or Sparrow).
        services.AddSingleton<IPhaseRunner>(sp => new PhaseRunner(
            sp.GetRequiredService<IServiceScopeFactory>(),
            [typeof(IStartupBadgePhase), typeof(IDataInputPhase), typeof(IHackingLoopPhase)]));

        // Exit signal (e.g. Ctrl+C); shared between Program and Application.
        services.AddSingleton<IExitRequest, ExitRequest>();

        // Main application phase: resolve and call Run() to execute the navigation loop.
        services.AddSingleton<Application>();

        return services;
    }
}
