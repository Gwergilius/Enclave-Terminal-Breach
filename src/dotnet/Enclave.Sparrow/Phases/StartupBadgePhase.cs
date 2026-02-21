using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Enclave.Shared.IO;
using Enclave.Sparrow.Configuration;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Prints SPARROW banner and load time when enabled in options (SPARROW-Requirements §1).
/// </summary>
public sealed class StartupBadgePhase(
    [NotNull] IConsoleIO console,
    [NotNull] SparrowOptions options) : IStartupBadgePhase
{
    private readonly IConsoleIO _console = console;
    private readonly SparrowOptions _options = options;

    /// <inheritdoc />
    public void Run()
    {
        var sw = Stopwatch.StartNew();

        if (_options.Startup.ShowBanner)
        {
            var info = ProductInfo.GetCurrent();
            _console.WriteLine($"{info.Name} {info.Version}");
        }

        sw.Stop();
        if (_options.Startup.ShowLoadTime)
            _console.WriteLine($"Loading system profiles...{sw.ElapsedMilliseconds} ms");

        if (_options.Startup.ShowBanner || _options.Startup.ShowLoadTime)
        {
            var level = SparrowIntelligence.Normalize(_options.Intelligence);
            var displayName = SparrowIntelligence.GetDisplayName(level);
            _console.WriteLine($"Intelligence level: {displayName} ({level})");

            var dictionarySource = string.IsNullOrWhiteSpace(_options.WordListPath) ? "manual" : _options.WordListPath;
            _console.WriteLine($"Dictionary: {dictionarySource}");

            _console.WriteLine();
        }
    }
}
