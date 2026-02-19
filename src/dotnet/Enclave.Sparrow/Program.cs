using System.Diagnostics.CodeAnalysis;
using Enclave.Sparrow.Configuration;
using Enclave.Sparrow.Services;
using Microsoft.Extensions.Configuration;

namespace Enclave.Sparrow;

[ExcludeFromCodeCoverage(Justification = "Straightforward composition root; test by review.")]
internal static class Program
{
    private static int Main(string[] args)
    {
        if (TryHandleHelpOrVersion(args, out var exitCode))
            return exitCode;

        var configuration = BuildConfiguration(args);
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, configuration);
        var provider = services.BuildServiceProvider();

        var runner = provider.GetRequiredService<IPhaseRunner>();
        runner.Run();

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
            sparrow [options]

            Options:
              -i, --intelligence <level>    Solver intelligence level (default: 1)
                                              0 = Random (HOUSE gambit)
                                              1 = Smart (Best-bucket)
                                              2 = Genius (Tie-breaker)
                                              Aliases: house, bucket, tie

              -w, --words <file>            Word list file path (optional)
                                              Load candidates from file instead of manual input

              -h, --help                    Show help message and exit
              -v, --version                 Show version information and exit

            Examples:
              sparrow                       Use default settings (intelligence: 1)
              sparrow -i 0                  Use random solver (HOUSE gambit)
              sparrow -i house              Same as above (alias)
              sparrow -i 2                  Use optimized solver (Tie-breaker)
              sparrow -w words.txt          Load candidates from file
              sparrow -i 2 -w words.txt     Combine options

            When using dotnet run, pass application options after -- so they reach the app:
              dotnet run --project Enclave.Sparrow.csproj -- --help
              dotnet run --project Enclave.Sparrow.csproj -- -i 2
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
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if ((arg is "-i" or "--intelligence") && i + 1 < args.Length)
            {
                dict["Sparrow:Intelligence"] = args[i + 1];
                i++;
            }
            else if ((arg is "-w" or "--words") && i + 1 < args.Length)
            {
                dict["Sparrow:WordListPath"] = args[i + 1];
                i++;
            }
        }
        return dict;
    }
}
