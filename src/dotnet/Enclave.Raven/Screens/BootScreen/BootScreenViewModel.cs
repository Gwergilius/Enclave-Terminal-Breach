using System.Diagnostics;
using Enclave.Phosphor;
using Enclave.Raven.Configuration;
using Enclave.Shared.Phases;

namespace Enclave.Raven.Screens.BootScreen;

/// <summary>
/// Orchestrates the boot badge display: measures load time, sets component state, renders once, then navigates to DataInput.
/// </summary>
public sealed class BootScreenViewModel(
    IVirtualScreen screen,
    ICompositor compositor,
    INavigationService navigation,
    RavenOptions options,
    IProductInfo productInfo) : IScreenViewModel
{
    /// <inheritdoc />
    public string Name => "BootScreen";

    /// <inheritdoc />
    public Task RunAsync(CancellationToken ct = default)
    {
        var bounds = new Rectangle(0, 0, screen.Size.Width, screen.Size.Height);
        var layer = screen.AddLayer(bounds, zOrder: 0);
        try
        {
            var sw = Stopwatch.StartNew();
            var component = new BootScreenComponent(bounds);

            if (options.Startup.ShowBanner)
                component.ProductLine = $"{productInfo.Name} {productInfo.Version}";

            sw.Stop();

            if (options.Startup.ShowLoadTime)
                component.LoadTimeLine = $"Loading system profiles...{sw.ElapsedMilliseconds} ms";

            if (options.Startup.ShowBanner || options.Startup.ShowLoadTime)
            {
                var level = RavenIntelligence.Normalize(options.Intelligence);
                component.IntelligenceLine = $"Intelligence level: {RavenIntelligence.GetDisplayName(level)} ({level})";

                var dictSource = string.IsNullOrWhiteSpace(options.WordListPath) ? "manual" : options.WordListPath;
                component.DictionaryLine = $"Dictionary: {dictSource}";
            }

            RenderAndFlush(layer, component);
            navigation.NavigateTo("KeyPress", "Press any key to begin...", "DataInput");
        }
        finally
        {
            screen.RemoveLayer(layer);
        }

        return Task.CompletedTask;
    }

    private void RenderAndFlush(Layer layer, IComponent component)
    {
        layer.Clear();
        var writer = new LayerWriter(layer);
        component.Render(writer);
        screen.Invalidate(layer.Bounds);
        foreach (var region in screen.FlushDirtyRegions())
            compositor.Flush(region);
    }
}
