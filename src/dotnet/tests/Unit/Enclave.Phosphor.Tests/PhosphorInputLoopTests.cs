using Enclave.Common.Extensions;
using Enclave.Common.Test.Core;

namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="PhosphorInputLoop"/> (production implementation).
/// Run() is tested with <see cref="TestableConsoleIO"/> key injection.
/// </summary>
[UnitTest, TestOf(nameof(PhosphorInputLoop))]
public sealed class PhosphorInputLoopTests: TestBase
{
    // --- ReadKey ------------------------------------------------------------

    [Fact]
    public void ReadKey_PreCancelledToken_ThrowsWithoutReadingConsole()
    {
        // First ThrowIfCancellationRequested() fires immediately; _console.ReadKey() is never called.
        var console = new TestableConsoleIO();
        var loop    = new PhosphorInputLoop(console);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Should.Throw<OperationCanceledException>(() => loop.ReadKey(cts.Token));
        // No key was needed: the console queue is still empty.
    }

    [Fact]
    public async Task ReadKey_TokenCancelledAfterConsoleReturns_ThrowsOperationCancelled()
    {
        // Second ThrowIfCancellationRequested() fires: token is cancelled while
        // _console.ReadKey() is blocking, then a key unblocks it.
        var console = new TestableConsoleIO();
        var loop    = new PhosphorInputLoop(console);
        using var cts = new CancellationTokenSource();

        var readTask = Task.Run(() => loop.ReadKey(cts.Token));

        // Give the background thread enough time to enter _console.ReadKey() / BlockingCollection.Take().
        await Sleep(50);

        // Cancel first so the second check fires once the key unblocks the read.
        cts.Cancel();
        console.InjectKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));

        await Should.ThrowAsync<OperationCanceledException>(
            () => readTask.WaitAsync(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public async Task ReadKey_ConsoleReturnsNull_ThrowsInvalidOperationException()
    {
        var console = new TestableConsoleIO();
        var loop    = new PhosphorInputLoop(console);

        var readTask = Task.Run(() => loop.ReadKey(CancellationToken.None));
        console.SignalReadKeyEnd(); // injects null — simulates closed input stream

        await Should.ThrowAsync<InvalidOperationException>(
            () => readTask.WaitAsync(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public async Task ReadKey_ValidKey_ReturnsIt()
    {
        var console = new TestableConsoleIO();
        var loop = new PhosphorInputLoop(console);
        var expected = new ConsoleKeyInfo('z', ConsoleKey.Z, false, false, false);

        var readTask = Task.Run(() => loop.ReadKey(CancellationToken.None));
        console.InjectKey(expected);
        var result = await WaitTask(readTask, 2.Seconds());
        result.ShouldBe(expected);
    }

    // --- Constructor / Register ---------------------------------------------

    [Fact]
    public void Constructor_NullConsole_ThrowsArgumentNullException()
    {
        var ex = Should.Throw<ArgumentNullException>(() => new PhosphorInputLoop(null!));
        ex.ParamName.ShouldBe("console");
    }

    [Fact]
    public void Register_NullHandler_ThrowsArgumentNullException()
    {
        var console = new TestableConsoleIO();
        var loop = new PhosphorInputLoop(console);

        var ex = Should.Throw<ArgumentNullException>(() => loop.Register(null!));
        ex.ParamName.ShouldBe("handler");
    }

    [Fact]
    public async Task Run_DispatchesKeyToHandler()
    {
        var console = new TestableConsoleIO();
        var loop = new PhosphorInputLoop(console);
        var handler = new RecordingKeyboardHandler();
        loop.Register(handler);

        var key = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false);
        var runTask = RunTask(() => loop.Run());
        console.InjectKey(key);
        console.SignalReadKeyEnd();
        await WaitTask(runTask, 2.Seconds());

        handler.ReceivedKeys.ShouldContain(key);
    }

    [Fact]
    public async Task Run_WhenHandlerReturnsTrue_StopsPropagation()
    {
        var console = new TestableConsoleIO();
        var loop = new PhosphorInputLoop(console);
        var consumingHandler = new ConsumingKeyboardHandler();
        var secondHandler = new RecordingKeyboardHandler();
        loop.Register(consumingHandler);
        loop.Register(secondHandler);

        var key = new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false);
        var runTask = RunTask(() => loop.Run());
        console.InjectKey(key);
        console.SignalReadKeyEnd();
        await WaitTask(runTask, 2.Seconds());

        consumingHandler.ReceivedKeys.ShouldContain(key);
        secondHandler.ReceivedKeys.ShouldBeEmpty();
    }

    [Fact]
    public void Run_PreCancelledToken_ExitsWithoutReadingKey()
    {
        // while condition fires immediately; _console.ReadKey() is never called.
        var console = new TestableConsoleIO();
        var loop    = new PhosphorInputLoop(console);
        var handler = new RecordingKeyboardHandler();
        loop.Register(handler);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        loop.Run(cts.Token); // must return without blocking

        handler.ReceivedKeys.ShouldBeEmpty();
        // No key was needed — the console queue is still empty.
    }

    [Fact]
    public async Task Run_TokenCancelledWhileBlocking_ExitsWithoutDispatching()
    {
        // Run() is blocking on _console.ReadKey(). We cancel the token then inject
        // a key to unblock it. The post-ReadKey cancellation check fires before
        // Dispatch, so the key must NOT reach any handler.
        var console = new TestableConsoleIO();
        var loop    = new PhosphorInputLoop(console);
        var handler = new RecordingKeyboardHandler();
        loop.Register(handler);
        using var cts = new CancellationTokenSource();
        var key = new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false);

        var runTask = RunTask(() => loop.Run(cts.Token));

        // Give the background thread enough time to enter _console.ReadKey() / BlockingCollection.Take().
        await Sleep(50);

        cts.Cancel();
        console.InjectKey(key); // unblocks ReadKey — post-ReadKey check exits the loop

        await WaitTask(runTask, 2.Seconds());

        handler.ReceivedKeys.ShouldBeEmpty(); // key was NOT dispatched
    }

    [Fact]
    public async Task Stop_SignalsRunToExit()
    {
        var console = new TestableConsoleIO();
        var loop = new PhosphorInputLoop(console);
        loop.Register(new RecordingKeyboardHandler());

        var runTask = RunTask(() => loop.Run());
        loop.Stop();
        console.SignalReadKeyEnd();
        await WaitTask(runTask, 2.Seconds());

        var completed = runTask.IsCompletedSuccessfully || runTask.IsCanceled;
        completed.ShouldBeTrue();
    }

    private sealed class RecordingKeyboardHandler : IPhosphorReader
    {
        private readonly List<ConsoleKeyInfo> _keys = [];
        public IReadOnlyList<ConsoleKeyInfo> ReceivedKeys => _keys;

        public bool KeyAvailable => false;
        public string? ReadLine() => null;

        public ConsoleKeyInfo? ReadKey() => null;

        public bool OnKeyPressed(ConsoleKeyInfo key)
        {
            _keys.Add(key);
            return false;
        }
    }

    private sealed class ConsumingKeyboardHandler : IPhosphorReader
    {
        private readonly List<ConsoleKeyInfo> _keys = [];
        public IReadOnlyList<ConsoleKeyInfo> ReceivedKeys => _keys;

        public bool KeyAvailable => false;
        public string? ReadLine() => null;

        public ConsoleKeyInfo? ReadKey() => null;

        public bool OnKeyPressed(ConsoleKeyInfo key)
        {
            _keys.Add(key);
            return true;
        }
    }
}
