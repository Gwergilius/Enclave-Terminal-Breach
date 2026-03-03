namespace Enclave.Phosphor;

/// <summary>
/// Event-driven render loop for the virtual screen compositor.
/// </summary>
/// <remarks>
/// The loop blocks on keyboard input. Each keypress may cause a component to update its layer
/// (write cells and call <see cref="IVirtualScreen.Invalidate"/>). After each dispatch the
/// compositor recomposes only the dirty regions — no timer polling, no full-screen redraw.
/// </remarks>
public sealed class PhosphorRenderLoop(
    IVirtualScreen screen,
    Compositor compositor,
    IPhosphorInputLoop input)
{
    /// <summary>
    /// Runs the render loop until <paramref name="ct"/> is cancelled.
    /// </summary>
    public void Run(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var key = input.ReadKey(ct);
            input.Dispatch(key);

            if (screen.HasDirtyRegions)
            {
                foreach (var region in screen.FlushDirtyRegions())
                    compositor.Flush(region);
            }
        }
    }
}
