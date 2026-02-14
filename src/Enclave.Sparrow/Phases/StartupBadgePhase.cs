using System.Diagnostics;
using System.Reflection;
using Enclave.Sparrow.IO;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Prints SPARROW banner and load time (SPARROW-Requirements §1).
/// </summary>
public sealed class StartupBadgePhase : IStartupBadgePhase
{
    private readonly IConsoleIO _console;

    public StartupBadgePhase(IConsoleIO console) => _console = console ?? throw new ArgumentNullException(nameof(console));

    /// <inheritdoc />
    public void Run()
    {
        var sw = Stopwatch.StartNew();
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString(3) ?? "0.0.0";
        var product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "SPARROW";

        _console.WriteLine($"{product} {version}");
        sw.Stop();
        _console.WriteLine($"Loading system profiles...{sw.ElapsedMilliseconds} ms");
        _console.WriteLine();
    }
}
