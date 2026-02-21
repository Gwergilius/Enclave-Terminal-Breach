using System.Diagnostics.CodeAnalysis;
using Enclave.Phosphor;
using Enclave.Raven.Phases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave.Raven;

[ExcludeFromCodeCoverage(Justification = "Straightforward composition root; test by review.")]
internal static class Program
{
    private static volatile bool s_exitRequested;

    private static int Main(string[] args)
    {
        if (TryHandleHelpOrVersion(args, out var exitCode))
            return exitCode;

        var configuration = BuildConfiguration(args);
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, configuration);
        var provider = services.BuildServiceProvider();

        var canvas = provider.GetRequiredService<IPhosphorCanvas>();
        var title = $"{ProductInfo.GetCurrent().Name} {ProductInfo.GetCurrent().Version} – ENCLAVE SIGINT";
        canvas.Initialize(title);

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            s_exitRequested = true;
        };

        try
        {
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            var writer = provider.GetRequiredService<IPhosphorWriter>();
            var reader = provider.GetRequiredService<IPhosphorReader>();

            // Run startup badge once.
            using (var badgeScope = scopeFactory.CreateScope())
            {
                var badge = badgeScope.ServiceProvider.GetRequiredService<IStartupBadgePhase>();
                badge.Run();
            }

            // Replay loop: DataInput → HackingLoop → wait key → clear → repeat. Exit via Ctrl+C (normal exit).
            while (!s_exitRequested)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dataInput = scope.ServiceProvider.GetRequiredService<IDataInputPhase>();
                    var hackingLoop = scope.ServiceProvider.GetRequiredService<IHackingLoopPhase>();
                    dataInput.Run();
                    hackingLoop.Run();
                }

                if (s_exitRequested)
                    break;

                writer.WriteLine();
                writer.WriteLine("Press any key to play again...");
                reader.ReadKey();
                if (s_exitRequested)
                    break;
                canvas.ClearScreen();
            }
        }
        finally
        {
            canvas.Dispose();
        }

        return 0;
    }

    private static bool TryHandleHelpOrVersion(string[] args, out int exitCode)
    {
        exitCode = 0;
        foreach (var a in args)
        {
            if (a is "-h" or "--help")
            {
                PrintHelp();
                return true;
            }
            if (a is "-v" or "--version")
            {
                PrintVersion();
                return true;
            }
        }
        return false;
    }

    private static void PrintHelp()
    {
        var help = """
            raven [options]

            Options:
              -i, --intelligence <level>    Solver intelligence level (default: 1)
                                              0 = Random (HOUSE gambit)
                                              1 = Smart (Best-bucket)
                                              2 = Genius (Tie-breaker)
                                              Aliases: house, bucket, tie

              -w, --words <file>            Word list file path (optional)
                                              Load candidates from file; when omitted, startup shows Dictionary: manual

              -h, --help                    Show help message and exit
              -v, --version                 Show version information and exit

            Examples:
              raven                         Use default settings (intelligence: 1)
              raven -i 0                    Use random solver (HOUSE gambit)
              raven -i house                Same as above (alias)
              raven -i 2                    Use optimized solver (Tie-breaker)
              raven -w words.txt            Load candidates from file
              raven -i 2 -w words.txt       Combine options

            When using dotnet run, pass application options after -- so they reach the app:
              dotnet run --project Enclave.Raven.csproj -- --help
              dotnet run --project Enclave.Raven.csproj -- -i 2
            """;
        Console.WriteLine(help);
    }

    private static void PrintVersion()
    {
        var info = ProductInfo.GetCurrent();
        Console.WriteLine($"{info.Name} {info.Version}");
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
    /// Parses -i/--intelligence and -w/--words from args. Later config sources override earlier; CLI is added last so it wins.
    /// </summary>
    private static Dictionary<string, string?> ParseCommandLineArgs(string[] args)
    {
        var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        int i = 0;
        while( i < args.Length ) {
            var arg = args[i++];
            if ((arg is "-i" or "--intelligence") && i < args.Length)
            {
                dict["Raven:Intelligence"] = args[i++];
            }
            else if ((arg is "-w" or "--words") && i < args.Length)
            {
                dict["Raven:WordListPath"] = args[i++];
            }
        }
        return dict;
    }
}
