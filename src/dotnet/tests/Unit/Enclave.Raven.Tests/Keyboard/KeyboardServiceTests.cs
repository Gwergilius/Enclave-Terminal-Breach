using System.Collections.Generic;
using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Enclave.Raven.Keyboard;
using Enclave.Raven.Services;
using Enclave.Shared.IO;

namespace Enclave.Raven.Tests.Keyboard;

[UnitTest, TestOf(nameof(KeyboardService))]
public class KeyboardServiceTests
{
    private static ConsoleKeyInfo Key(char c, ConsoleKey k) => new(c, k, false, false, false);

    /// <summary>Console reader that yields keys from a queue; KeyAvailable = queue not empty.</summary>
    private sealed class QueueConsoleReader : IConsoleReader
    {
        private readonly Queue<ConsoleKeyInfo?> _keys = new();

        public bool KeyAvailable => _keys.Count > 0 && _keys.Peek() is not null;

        public string? ReadLine() => null;

        public ConsoleKeyInfo? ReadKey()
        {
            if (_keys.Count == 0)
                return null;
            return _keys.Dequeue();
        }

        public void Enqueue(ConsoleKeyInfo key) => _keys.Enqueue(key);
        public void EnqueueNull() => _keys.Enqueue(null); // simulates closed input
    }

    [Fact]
    public void Subscribe_WhenPriorityZeroOrNegative_ThrowsArgumentOutOfRangeException()
    {
        var reader = new QueueConsoleReader();
        reader.Enqueue(Key('x', ConsoleKey.X));
        var exitRequest = new ExitRequest();
        var sut = new KeyboardService(reader, exitRequest);

        var ex = Should.Throw<ArgumentOutOfRangeException>(() =>
            sut.Subscribe(0, _ => { }));
        ex.ParamName.ShouldBe("priority");

        var ex2 = Should.Throw<ArgumentOutOfRangeException>(() =>
            sut.Subscribe(-1, _ => { }));
        ex2.ParamName.ShouldBe("priority");
    }

    [Fact]
    public void KbHit_ReturnsReaderKeyAvailable()
    {
        var reader = new QueueConsoleReader();
        var exitRequest = new ExitRequest();
        var sut = new KeyboardService(reader, exitRequest);

        sut.KbHit().ShouldBeFalse();
        reader.Enqueue(Key('a', ConsoleKey.A));
        sut.KbHit().ShouldBeTrue();
    }

    [Fact]
    public void GetNextKey_WhenNoSubscribers_ReturnsKeyFromReader()
    {
        var reader = new QueueConsoleReader();
        reader.Enqueue(Key('a', ConsoleKey.A));
        var exitRequest = new ExitRequest();
        var sut = new KeyboardService(reader, exitRequest);

        var key = sut.GetNextKey();

        key.ShouldNotBeNull();
        key.Value.KeyChar.ShouldBe('a');
    }

    [Fact]
    public void GetNextKey_WhenReaderReturnsNull_ReturnsNull()
    {
        var reader = new QueueConsoleReader();
        reader.EnqueueNull();
        var exitRequest = new ExitRequest();
        var sut = new KeyboardService(reader, exitRequest);

        var key = sut.GetNextKey();

        key.ShouldBeNull();
    }

    [Fact]
    public void GetNextKey_WhenSubscriberDoesNotHandle_ReturnsKey()
    {
        var reader = new QueueConsoleReader();
        reader.Enqueue(Key('b', ConsoleKey.B));
        var exitRequest = new ExitRequest();
        var sut = new KeyboardService(reader, exitRequest);
        KeyPressedEventArgs? captured = null;
        sut.Subscribe(10, e => captured = e);

        var key = sut.GetNextKey();

        key.ShouldNotBeNull();
        key.Value.KeyChar.ShouldBe('b');
        captured.ShouldNotBeNull();
        captured!.Handled.ShouldBeFalse();
    }

    [Fact]
    public void GetNextKey_WhenSubscriberHandles_ConsumesKeyAndReadsNext()
    {
        var reader = new QueueConsoleReader();
        reader.Enqueue(Key('x', ConsoleKey.X));
        reader.Enqueue(Key('y', ConsoleKey.Y));
        var exitRequest = new ExitRequest();
        var sut = new KeyboardService(reader, exitRequest);
        sut.Subscribe(5, e => e.Handled = (e.Key.KeyChar == 'x')); // consume only 'x'

        var key = sut.GetNextKey();

        key.ShouldNotBeNull();
        key.Value.KeyChar.ShouldBe('y');
    }

    [Fact]
    public void GetNextKey_WhenSubscriberHandlesAndExitRequested_ReturnsNull()
    {
        var reader = new QueueConsoleReader();
        reader.Enqueue(Key('x', ConsoleKey.X));
        var exitRequest = new ExitRequest();
        var sut = new KeyboardService(reader, exitRequest);
        sut.Subscribe(5, e =>
        {
            e.Handled = true;
            exitRequest.RequestExit();
        });

        var key = sut.GetNextKey();

        key.ShouldBeNull();
    }

    [Fact]
    public void GetNextKey_SubscribersInvokedInPriorityOrder()
    {
        var reader = new QueueConsoleReader();
        reader.Enqueue(Key('a', ConsoleKey.A));
        var exitRequest = new ExitRequest();
        var sut = new KeyboardService(reader, exitRequest);
        var order = new List<int>();
        sut.Subscribe(100, _ => order.Add(100));
        sut.Subscribe(10, _ => order.Add(10));
        sut.Subscribe(50, _ => order.Add(50));

        _ = sut.GetNextKey();

        order.ShouldBe([10, 50, 100]);
    }

    [Fact]
    public void Subscribe_ReturnsDisposable_ThatUnsubscribesOnDispose()
    {
        var reader = new QueueConsoleReader();
        reader.Enqueue(Key('a', ConsoleKey.A));
        reader.Enqueue(Key('b', ConsoleKey.B));
        var exitRequest = new ExitRequest();
        var sut = new KeyboardService(reader, exitRequest);
        var highCalls = 0;
        var sub = sut.Subscribe(1, _ => highCalls++);

        _ = sut.GetNextKey();
        highCalls.ShouldBe(1);

        sub.Dispose();
        _ = sut.GetNextKey();
        highCalls.ShouldBe(1);
    }
}
