using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Enclave.Phosphor;
using Enclave.Raven.Configuration;
using Enclave.Shared.Phases;
using FluentResults;

namespace Enclave.Raven.Phases;

/// <summary>
/// Prints RAVEN banner and load time when enabled in options (RAVEN-Requirements §1).
/// </summary>
public sealed class StartupBadgePhase(
    [NotNull] IPhosphorWriter writer,
    [NotNull] RavenOptions options,
    [NotNull] INavigationService navigation,
    [NotNull] IProductInfo productInfo) : IStartupBadgePhase
{
    private readonly IPhosphorWriter _writer = writer;
    private readonly RavenOptions _options = options;
    private readonly INavigationService _navigation = navigation;
    private readonly IProductInfo _productInfo = productInfo;

    /// <inheritdoc />
    public string Name => "StartupBadge";

    /// <inheritdoc />
    public Result Run(params object[] args)
    {
        var sw = Stopwatch.StartNew();

        if (_options.Startup.ShowBanner)
        {
            _writer.WriteLine($"{_productInfo.Name} {_productInfo.Version}");
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

        return _navigation.NavigateTo("ResetScope", "DataInput");
    }
}
