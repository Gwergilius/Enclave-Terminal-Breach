using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Enclave.Phosphor;
using Enclave.Raven.Configuration;

namespace Enclave.Raven.Phases;

/// <summary>
/// Prints RAVEN banner and load time when enabled in options (RAVEN-Requirements §1).
/// </summary>
public sealed class StartupBadgePhase(
    [NotNull] IPhosphorWriter writer,
    [NotNull] RavenOptions options) : IStartupBadgePhase
{
    private readonly IPhosphorWriter _writer = writer;
    private readonly RavenOptions _options = options;

    /// <inheritdoc />
    public void Run()
    {
        var sw = Stopwatch.StartNew();

        if (_options.Startup.ShowBanner)
        {
            var info = ProductInfo.GetCurrent();
            _writer.WriteLine($"{info.Name} {info.Version}");
        }

        sw.Stop();
        if (_options.Startup.ShowLoadTime)
            _writer.WriteLine($"Loading system profiles...{sw.ElapsedMilliseconds} ms");

        if (_options.Startup.ShowBanner || _options.Startup.ShowLoadTime)
        {
            var level = RavenIntelligence.Normalize(_options.Intelligence);
            var displayName = RavenIntelligence.GetDisplayName(level);
            _writer.WriteLine($"Intelligence level: {displayName} ({level})");

            var dictionarySource = string.IsNullOrWhiteSpace(_options.WordListPath) ? "manual" : _options.WordListPath;
            _writer.WriteLine($"Dictionary: {dictionarySource}");

            _writer.WriteLine();
        }
    }
}
