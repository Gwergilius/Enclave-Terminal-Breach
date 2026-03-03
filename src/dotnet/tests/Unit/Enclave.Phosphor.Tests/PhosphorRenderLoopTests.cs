namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="PhosphorRenderLoop"/> — cancellation, key dispatch,
/// and dirty-region flushing behaviour.
/// </summary>
[UnitTest, TestOf(nameof(PhosphorRenderLoop))]
public sealed class PhosphorRenderLoopTests
{
    // --- Test doubles -------------------------------------------------------

    private sealed class RecordingCursor : IPhosphorCursor
    {
        public void MoveTo(int col, int row) { }
    }

    /// <summary>
    /// Minimal <see cref="IPhosphorReader"/> that invokes a callback on each key press
    /// and passes the event through (returns false).
    /// </summary>
    private sealed class ActionReader(Action<ConsoleKeyInfo> onKey) : IPhosphorReader
    {
        public string?       ReadLine() => null;
        public ConsoleKeyInfo? ReadKey() => null;
        public bool OnKeyPressed(ConsoleKeyInfo key) { onKey(key); return false; }
    }

    // --- Helpers ------------------------------------------------------------

    private static (
        FakeVirtualScreen    Screen,
        TestPhosphorWriter   Writer,
        TestPhosphorInputLoop Input,
        PhosphorRenderLoop   Sut)
    CreateSut(int width = 20, int height = 5)
    {
        var screen     = new FakeVirtualScreen(new Size(width, height));
        var writer     = new TestPhosphorWriter();
        var cursor     = new RecordingCursor();
        var compositor = new Compositor(screen, writer, cursor);
        var input      = new TestPhosphorInputLoop();
        var sut        = new PhosphorRenderLoop(screen, compositor, input);
        return (screen, writer, input, sut);
    }

    private static ConsoleKeyInfo AnyKey() =>
        new('\0', ConsoleKey.Enter, false, false, false);

    /// <summary>
    /// Runs the loop on a background thread. Catches <see cref="OperationCanceledException"/>
    /// so that tests which cancel while blocking do not result in a faulted task.
    /// </summary>
    private static Task StartLoop(PhosphorRenderLoop sut, CancellationTokenSource cts) =>
        Task.Run(() =>
        {
            try { sut.Run(cts.Token); }
            catch (OperationCanceledException) { } // acceptable when cancelled while blocking
        });

    // --- Cancellation -------------------------------------------------------

    [Fact]
    public async Task Run_PreCancelled_ExitsImmediately()
    {
        var (_, _, _, sut) = CreateSut();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // The while condition is false from the start — Run() returns without ever reading a key.
        await Task.Run(() => sut.Run(cts.Token))
                  .WaitAsync(TimeSpan.FromSeconds(2));
    }

    // --- Key dispatch -------------------------------------------------------

    [Fact]
    public async Task Run_KeyInjected_DispatchedToRegisteredHandler()
    {
        var (_, _, input, sut) = CreateSut();
        using var cts = new CancellationTokenSource();
        ConsoleKeyInfo? received = null;

        // The handler captures the key and cancels the loop so the test terminates.
        input.Register(new ActionReader(k => { received = k; cts.Cancel(); }));

        var task    = StartLoop(sut, cts);
        var injected = AnyKey();
        input.InjectKey(injected);
        await task.WaitAsync(TimeSpan.FromSeconds(2));

        received.ShouldBe(injected);
    }

    // --- Dirty region flushing ----------------------------------------------

    [Fact]
    public async Task Run_DirtyRegionAfterDispatch_CompositorFlushes()
    {
        var (screen, writer, input, sut) = CreateSut();
        using var cts = new CancellationTokenSource();

        var layer = screen.AddLayer(new Rectangle(0, 0, 3, 1), zOrder: 0);
        layer.SetCell(0, 0, new VirtualCell('A'));
        screen.Invalidate(new Rectangle(0, 0, 3, 1));

        // The handler cancels the loop. Execution order in Run() guarantees that
        // the dirty-region flush happens *after* Dispatch() returns but *before*
        // the while condition is re-evaluated — so the flush is always reached.
        input.Register(new ActionReader(_ => cts.Cancel()));

        var task = StartLoop(sut, cts);
        input.InjectKey(AnyKey());
        await task.WaitAsync(TimeSpan.FromSeconds(2));

        writer.Recorded.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Run_NoDirtyRegions_CompositorNotInvoked()
    {
        var (_, writer, input, sut) = CreateSut();
        using var cts = new CancellationTokenSource();

        input.Register(new ActionReader(_ => cts.Cancel()));

        var task = StartLoop(sut, cts);
        input.InjectKey(AnyKey());
        await task.WaitAsync(TimeSpan.FromSeconds(2));

        writer.Recorded.ShouldBeEmpty();
    }

    [Fact]
    public async Task Run_MultipleDirtyRegions_AllFlushed()
    {
        var (screen, writer, input, sut) = CreateSut(width: 20, height: 5);
        using var cts = new CancellationTokenSource();

        var layerA = screen.AddLayer(new Rectangle(0, 0, 3, 1), zOrder: 0);
        layerA.SetCell(0, 0, new VirtualCell('A'));
        screen.Invalidate(new Rectangle(0, 0, 3, 1));

        var layerB = screen.AddLayer(new Rectangle(10, 0, 3, 1), zOrder: 0);
        layerB.SetCell(10, 0, new VirtualCell('B'));
        screen.Invalidate(new Rectangle(10, 0, 3, 1));

        input.Register(new ActionReader(_ => cts.Cancel()));

        var task = StartLoop(sut, cts);
        input.InjectKey(AnyKey());
        await task.WaitAsync(TimeSpan.FromSeconds(2));

        var allWritten = string.Concat(writer.Recorded.Select(r => r.Text));
        allWritten.ShouldContain("A");
        allWritten.ShouldContain("B");
    }
}
