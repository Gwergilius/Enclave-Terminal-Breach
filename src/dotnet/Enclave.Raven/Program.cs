using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Enclave.Phosphor;
using Enclave.Raven.Configuration;
using Enclave.Raven.Keyboard;
using Enclave.Raven.Screens.Help;
using Enclave.Raven.Services;
using Enclave.Shared.IO;
using Microsoft.Extensions.Configuration;

namespace Enclave.Raven;

[ExcludeFromCodeCoverage(Justification = "Straightforward composition root; test by review.")]
[SupportedOSPlatform("windows")]
internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var configuration = BuildConfiguration(args);

        var showHelp    = configuration.GetValue<bool>("Info:ShowHelp");
        var showVersion = configuration.GetValue<bool>("Info:ShowVersion");
        if (showHelp || showVersion)
            return RunInfoMode(configuration, showVersion, showHelp);

        var services = new ServiceCollection();
        Startup.ConfigureServices(services, configuration);
        var provider = services.BuildServiceProvider();

        using var cts = new CancellationTokenSource();
        var exitRequest = provider.GetRequiredService<IExitRequest>();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            exitRequest.RequestExit();
            cts.Cancel();
        };

        // Start keyboard exit handling (Ctrl+C / Alt+F4) with high priority so FixedRegionReadLine and others see exit.
        provider.GetRequiredService<ExitService>().Start();

        var app = provider.GetRequiredService<Application>();
        var result = await app.RunAsync(cts.Token);

        return result.IsSuccess ? 0 : 1;
    }

    private static int RunInfoMode(IConfiguration configuration, bool showVersion, bool showHelp)
    {
        var options = new RavenOptions();
        configuration.GetSection("System").Bind(options);

        var theme     = PhosphorThemeFactory.Create(options.Theme);
        var consoleIO = new ConsoleIO();
        var canvas    = new AnsiPhosphorCanvas(theme, consoleIO, new ScreenOptions());
        canvas.Initialize("RAVEN");

        try
        {
            var screenSize = new Size(canvas.Width, canvas.Height);
            var screen     = new VirtualScreen(screenSize);
            var compositor = new Compositor(screen, canvas, new AnsiPhosphorCursor(consoleIO));

            var bounds = new Rectangle(0, 0, screenSize.Width, screenSize.Height);
            var layer  = screen.AddLayer(bounds, zOrder: 0);
            var writer = new LayerWriter(layer);
            var currentRow = 0;

            if (showVersion)
            {
                var info = ProductInfo.GetCurrent();
                new VersionComponent(bounds) { VersionLine = $"{info.Name} {info.Version}", Row = currentRow }.Render(writer);
                currentRow += 2;
            }

            if (showHelp)
                new HelpScreenComponent(bounds) { Row = currentRow }.Render(writer);

            screen.Invalidate(bounds);
            foreach (var region in screen.FlushDirtyRegions())
                compositor.Flush(region);
        }
        finally
        {
            canvas.Dispose();
        }

        return 0;
    }

    private static IConfiguration BuildConfiguration(string[] args)
    {
        var cliOverrides = ParseCommandLineArgs(args);

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddInMemoryCollection(cliOverrides)
            .Build();
    }

    /// <summary>
    /// Parses CLI options and stores them under configuration keys.
    /// -i/--intelligence → System:Intelligence
    /// -t/--theme        → System:Theme
    /// -w/--words        → System:WordListPath
    /// -h/--help         → Info:ShowHelp
    /// -v/--version      → Info:ShowVersion
    /// CLI is added last in the config builder chain so it wins over appsettings.json.
    /// </summary>
    private static Dictionary<string, string?> ParseCommandLineArgs(string[] args)
    {
        var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        int i = 0;
        while (i < args.Length)
        {
            var arg = args[i++];
            if ((arg is "-i" or "--intelligence") && i < args.Length)
                dict["System:Intelligence"] = args[i++];
            else if ((arg is "-t" or "--theme") && i < args.Length)
                dict["System:Theme"] = args[i++];
            else if ((arg is "-w" or "--words") && i < args.Length)
                dict["System:WordListPath"] = args[i++];
            else if (arg is "-h" or "--help")
                dict["Info:ShowHelp"] = "true";
            else if (arg is "-v" or "--version")
                dict["Info:ShowVersion"] = "true";
        }
        return dict;
    }
}
