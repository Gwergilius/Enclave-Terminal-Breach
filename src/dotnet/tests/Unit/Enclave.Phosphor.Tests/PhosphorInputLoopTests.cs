namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="PhosphorInputLoop"/> (production implementation).
/// Run() is tested with <see cref="TestableConsoleIO"/> key injection.
/// </summary>
[UnitTest, TestOf(nameof(PhosphorInputLoop))]
public sealed class PhosphorInputLoopTests
{
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
        var runTask = Task.Run(() => loop.Run());
        console.InjectKey(key);
        console.SignalReadKeyEnd();
        await runTask.WaitAsync(TimeSpan.FromSeconds(2));

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
        var runTask = Task.Run(() => loop.Run());
        console.InjectKey(key);
        console.SignalReadKeyEnd();
        await runTask.WaitAsync(TimeSpan.FromSeconds(2));

        consumingHandler.ReceivedKeys.ShouldContain(key);
        secondHandler.ReceivedKeys.ShouldBeEmpty();
    }

    [Fact]
    public async Task Stop_SignalsRunToExit()
    {
        var console = new TestableConsoleIO();
        var loop = new PhosphorInputLoop(console);
        loop.Register(new RecordingKeyboardHandler());

        var runTask = Task.Run(() => loop.Run());
        loop.Stop();
        console.SignalReadKeyEnd();
        await runTask.WaitAsync(TimeSpan.FromSeconds(2));

        var completed = runTask.IsCompletedSuccessfully || runTask.IsCanceled;
        completed.ShouldBeTrue();
    }

    private sealed class RecordingKeyboardHandler : IPhosphorReader
    {
        private readonly List<ConsoleKeyInfo> _keys = new();
        public IReadOnlyList<ConsoleKeyInfo> ReceivedKeys => _keys;

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
        private readonly List<ConsoleKeyInfo> _keys = new();
        public IReadOnlyList<ConsoleKeyInfo> ReceivedKeys => _keys;

        public string? ReadLine() => null;

        public ConsoleKeyInfo? ReadKey() => null;

        public bool OnKeyPressed(ConsoleKeyInfo key)
        {
            _keys.Add(key);
            return true;
        }
    }
}
