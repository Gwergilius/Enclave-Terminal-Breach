using Enclave.Phosphor;
using Enclave.Raven.Keyboard;
using Enclave.Shared.Phases;

namespace Enclave.Raven.Screens.KeyPress;

/// <summary>
/// Displays a configurable prompt at the bottom of the screen and waits for a keypress
/// before navigating to the next screen. Renders only the prompt row — the previous
/// screen's content remains visible in the physical terminal until the next VM renders.
/// Uses <see cref="IKeyboardService.GetNextKey"/> so Ctrl+C/Alt+F4 are handled by <see cref="ExitService"/>.
/// </summary>
/// <remarks>
/// The preceding screen passes prompt and next-screen name as <see cref="INavigationService.NavigateTo"/>
/// arguments: <c>NavigateTo("KeyPress", "Press any key...", "NextScreenName")</c>.
/// </remarks>
public sealed class KeyPressViewModel(
    IKeyboardService keyboard,
    IVirtualScreen screen,
    ICompositor compositor,
    INavigationService navigation) : IScreenViewModel
{
    /// <inheritdoc />
    public string Name => "KeyPress";

    /// <inheritdoc />
    public Task RunAsync(CancellationToken ct = default)
    {
        var args   = navigation.NextPhaseArgs;
        var prompt = args.Count > 0 && args[0] is string s ? s : "Press any key to continue...";
        var next   = args.Count > 1 && args[1] is string n ? n : "Exit";

        var bounds    = new Rectangle(0, 0, screen.Size.Width, screen.Size.Height);
        var promptRow = screen.Size.Height - 1;

        // Drain any dirty regions left by the preceding screen's RemoveLayer call.
        // We skip recomposing them: the physical terminal already shows the correct content
        // from the preceding screen's own flush, and we intentionally preserve it.
        screen.FlushDirtyRegions();

        var layer     = screen.AddLayer(bounds, zOrder: 0);
        try
        {
            var component  = new KeyPressComponent(bounds) { Prompt = prompt, Row = promptRow };
            var promptArea = new Rectangle(0, promptRow, screen.Size.Width, 1);

            var writer = new LayerWriter(layer);
            component.Render(writer);
            screen.Invalidate(promptArea);
            foreach (var region in screen.FlushDirtyRegions())
                compositor.Flush(region);

            keyboard.GetNextKey();
        }
        finally
        {
            screen.RemoveLayer(layer);
        }

        navigation.NavigateTo(next);
        return Task.CompletedTask;
    }
}
