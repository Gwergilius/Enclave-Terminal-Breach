using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Enclave.Raven.Configuration;
using Enclave.Shared.IO;

namespace Enclave.Raven.Phases;

/// <summary>
/// Prints RAVEN banner and load time when enabled in options (RAVEN-Requirements §1).
/// </summary>
public sealed class StartupBadgePhase(
    [NotNull] IConsoleIO console,
    [NotNull] RavenOptions options) : IStartupBadgePhase
{
    private readonly IConsoleIO _console = console;
    private readonly RavenOptions _options = options;

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
            var level = RavenIntelligence.Normalize(_options.Intelligence);
            var displayName = RavenIntelligence.GetDisplayName(level);
            _console.WriteLine($"Intelligence level: {displayName} ({level})");

            var dictionarySource = string.IsNullOrWhiteSpace(_options.WordListPath) ? "manual" : _options.WordListPath;
            _console.WriteLine($"Dictionary: {dictionarySource}");

            _console.WriteLine();
        }
    }
}
