using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Enclave.Echelon.Core.Services;
using Enclave.Phosphor;
using Enclave.Raven.Configuration;
using Enclave.Raven.Input;
using Enclave.Raven.IO;
using Enclave.Raven.Screens;
using Enclave.Raven.Screens.BootScreen;
using Enclave.Raven.Screens.DataInput;
using Enclave.Raven.Screens.HackingLoop;
using Enclave.Raven.Keyboard;
using Enclave.Raven.Screens.KeyPress;
using Enclave.Raven.Services;
using Enclave.Shared.IO;
using Enclave.Shared.Models;
using Enclave.Shared.Phases;
using Microsoft.Extensions.Configuration;

namespace Enclave.Raven;

/// <summary>
/// Configures the RAVEN console application's dependency injection container.
/// </summary>
[SupportedOSPlatform("windows")]
[ExcludeFromCodeCoverage(Justification = "Thin wrapper around Console.Write/ReadLine; testing would only verify BCL behavior.")]
public static class Startup
{
    /// <summary>
    /// Registers all services required for the RAVEN UI: configuration, session state, I/O, Phosphor, solver, and screens.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Application configuration (command-line overrides + appsettings.json + defaults).</param>
    public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var systemSection = configuration.GetSection("System");
        var options = new RavenOptions();
        systemSection.Bind(options);

        var timingSection = configuration.GetSection("Platform:Timing");
        var timingOptions = new TimingOptions();
        timingSection.Bind(timingOptions);

        services.AddSingleton(options);
        services.AddSingleton(timingOptions);
        services.AddSingleton<ITimingOptions>(sp => sp.GetRequiredService<TimingOptions>());
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IProductInfo>(_ => ProductInfo.GetCurrent());

        // Session: shared state between data-input and hacking screens (one scope per run).
        services.AddScoped<IGameSession, GameSession>();

        // I/O: low-level console for Phosphor and for input (ReadLine, ReadInt).
        services.AddSingleton<IConsoleIO, ConsoleIO>();

        // Phosphor 1.0: AnsiPhosphorCanvas used directly for terminal init (Initialize/Dispose) by Application.
        // IPhosphorWriter points to AnsiPhosphorCanvas for direct (non-typewriter) writes used by Compositor.
        services.AddSingleton<IScreenOptions>(_ => new ScreenOptions());
        services.AddSingleton(_ => PhosphorThemeFactory.Create(options.Theme));
        services.AddSingleton<AnsiPhosphorCanvas>();
        services.AddSingleton<IPhosphorWriter>(sp => sp.GetRequiredService<AnsiPhosphorCanvas>());

        // Phosphor 2.0: VirtualScreen + Compositor for layer-based rendering.
        services.AddSingleton<IVirtualScreen>(sp =>
        {
            var consoleIO = sp.GetRequiredService<IConsoleIO>();
            var (w, h) = consoleIO.GetDimensions();
            return new VirtualScreen(new Size(Math.Max(w, 80), Math.Max(h, 24)));
        });
        services.AddSingleton<IPhosphorCursor>(sp =>
            new AnsiPhosphorCursor(sp.GetRequiredService<IConsoleIO>()));
        services.AddSingleton(sp => new Compositor(
            sp.GetRequiredService<IVirtualScreen>(),
            sp.GetRequiredService<IPhosphorWriter>(),
            sp.GetRequiredService<IPhosphorCursor>()));
        services.AddSingleton<ICompositor>(sp => sp.GetRequiredService<Compositor>());

        // Keyboard: low-level reader; then central keyboard service with priority-based KeyPressed subscribers.
        services.AddSingleton<IPhosphorReader, ConsoleKeyboardHandler>();
        services.AddSingleton<IConsoleReader>(sp => sp.GetRequiredService<IPhosphorReader>());
        services.AddSingleton<IKeyboardService, KeyboardService>();
        services.AddSingleton<ExitService>();

        // Fixed-region read-line: key-by-key input in a fixed screen area (DataInput, MatchCount); uses IKeyboardService + PushBack.
        services.AddSingleton<IFixedRegionReadLine, FixedRegionReadLine>();

        // Random: single instance for solver tie-breaking / random choice (non-security use).
        services.AddSingleton<IRandom, GameRandom>();

        // Solvers: each strategy registered as IPasswordSolver; factory selects by config.
        services.AddSingleton<IPasswordSolver, HouseGambitPasswordSolver>();
        services.AddSingleton<IPasswordSolver, BestBucketPasswordSolver>();
        services.AddSingleton<IPasswordSolver, TieBreakerPasswordSolver>();

        // Solver factory: resolves requested solver from the registered set using config (RavenOptions).
        services.AddSingleton<ISolverConfiguration>(sp => sp.GetRequiredService<RavenOptions>());
        services.AddSingleton<ISolverFactory, SolverFactory>();

        // Navigation and scope: screen names and current scope for resolution.
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ICurrentScope, CurrentScopeHolder>();
        services.AddScoped<ViewModelRegistry>();
        services.AddScoped<IViewModelRegistry>(sp => sp.GetRequiredService<ViewModelRegistry>());

        // Screens: each screen ViewModel is scoped so each round gets a fresh instance.
        services.AddScoped<BootScreenViewModel>();
        services.AddScoped<IScreenViewModel>(sp => sp.GetRequiredService<BootScreenViewModel>());
        services.AddScoped<DataInputViewModel>();
        services.AddScoped<IScreenViewModel>(sp => sp.GetRequiredService<DataInputViewModel>());
        services.AddScoped<HackingLoopViewModel>();
        services.AddScoped<IScreenViewModel>(sp => sp.GetRequiredService<HackingLoopViewModel>());
        services.AddScoped<KeyPressViewModel>();
        services.AddScoped<IScreenViewModel>(sp => sp.GetRequiredService<KeyPressViewModel>());

        // Exit signal (e.g. Ctrl+C); shared between Program and Application.
        services.AddSingleton<IExitRequest, ExitRequest>();

        // Main application runner.
        services.AddSingleton<Application>();

        return services;
    }
}
