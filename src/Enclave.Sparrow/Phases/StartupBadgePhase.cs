using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Enclave.Sparrow.IO;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Prints SPARROW banner and load time (SPARROW-Requirements §1).
/// </summary>
public sealed class StartupBadgePhase([NotNull] IConsoleIO console) : IStartupBadgePhase
{
    private readonly IConsoleIO _console = console;

    /// <inheritdoc />
    public void Run()
    {
        var sw = Stopwatch.StartNew();
        var info = ProductInfo.GetCurrent();
        _console.WriteLine($"{info.Name} {info.Version}");
        sw.Stop();

        _console.WriteLine($"Loading system profiles...{sw.ElapsedMilliseconds} ms");
        _console.WriteLine();
    }
}
