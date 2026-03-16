using System.Collections.Generic;
using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.Input;
using Enclave.Raven.Keyboard;

namespace Enclave.Raven.Tests.Input;

[UnitTest, TestOf(nameof(FixedRegionReadLine))]
public class FixedRegionReadLineTests
{
    private static ConsoleKeyInfo Key(char c, ConsoleKey k) => new(c, k, false, false, false);

    private sealed class QueueKeyboardService : IKeyboardService
    {
        private readonly Queue<ConsoleKeyInfo?> _keys = new();

        public bool KeyAvailable => _keys.Count > 0 && _keys.Peek() is not null;

        public void Enqueue(ConsoleKeyInfo key) => _keys.Enqueue(key);
        public void EnqueueNull() => _keys.Enqueue(null);

        public IDisposable Subscribe(int priority, Action<KeyPressedEventArgs> handler) =>
            throw new NotSupportedException("Tests use queue only.");

        public bool KbHit() => KeyAvailable;

        public ConsoleKeyInfo? GetNextKey() => _keys.Count == 0 ? null : _keys.Dequeue();
    }

    private sealed class TestInputView : IReadLineInputView
    {
        public string Prompt { get; set; } = "";
        public string CurrentLineContent { get; set; } = "";
        public int NextRow { get; } = 1;

        public void Render(LayerWriter writer)
        {
            writer.Write(Prompt);
            writer.Write(CurrentLineContent);
        }
    }

    private static (IVirtualScreen screen, Layer inputLayer, ICompositor compositor, IPhosphorCursor cursor)
        CreateMocks()
    {
        var inputLayer = new Layer(new Rectangle(0, 0, 80, 2), zOrder: 0);
        var screen = Mock.Of<IVirtualScreen>();
        screen.AsMock().SetupGet(s => s.Size).Returns(new Size(80, 24));
        screen.AsMock().Setup(s => s.FlushDirtyRegions()).Returns(new List<Rectangle>());
        var compositor = Mock.Of<ICompositor>();
        var cursor = Mock.Of<IPhosphorCursor>();
        return (screen, inputLayer, compositor, cursor);
    }

    private static FixedRegionReadLine CreateSut(IKeyboardService keyboard)
    {
        var (screen, _, compositor, cursor) = CreateMocks();
        return new FixedRegionReadLine(keyboard, screen, compositor, cursor);
    }

    [Fact]
    public void ReadLine_WhenKeysThenEnter_ReturnsLine()
    {
        var keyboard = new QueueKeyboardService();
        keyboard.Enqueue(Key('H', ConsoleKey.H));
        keyboard.Enqueue(Key('i', ConsoleKey.I));
        keyboard.Enqueue(Key('\r', ConsoleKey.Enter));
        var (screen, inputLayer, compositor, cursor) = CreateMocks();
        var view = new TestInputView { Prompt = "> " };
        var sut = new FixedRegionReadLine(keyboard, screen, compositor, cursor);

        var result = sut.ReadLine(new ReadLineParams
        {
            InputLayer = inputLayer,
            InputView = view,
            KeyFilter = new DataInputKeyFilter()
        });

        result.ShouldBe("Hi");
    }

    [Fact]
    public void ReadLine_WhenGetNextKeyReturnsNull_ReturnsNull()
    {
        var keyboard = new QueueKeyboardService();
        keyboard.EnqueueNull();
        var (screen, inputLayer, compositor, cursor) = CreateMocks();
        var view = new TestInputView();
        var sut = new FixedRegionReadLine(keyboard, screen, compositor, cursor);

        var result = sut.ReadLine(new ReadLineParams
        {
            InputLayer = inputLayer,
            InputView = view,
            KeyFilter = new DataInputKeyFilter()
        });

        result.ShouldBeNull();
    }

    [Fact]
    public void ReadLine_EmptyLineWithEnter_ReturnsEmptyString()
    {
        var keyboard = new QueueKeyboardService();
        keyboard.Enqueue(Key('\r', ConsoleKey.Enter));
        var sut = CreateSut(keyboard);
        var (_, inputLayer, _, _) = CreateMocks();
        var view = new TestInputView();

        var result = sut.ReadLine(new ReadLineParams
        {
            InputLayer = inputLayer,
            InputView = view,
            KeyFilter = new DataInputKeyFilter()
        });

        result.ShouldBe("");
    }

    [Fact]
    public void PushBack_ThenReadChar_ReturnsPushedKey()
    {
        var keyboard = new QueueKeyboardService();
        keyboard.EnqueueNull();
        var sut = CreateSut(keyboard);

        sut.PushBack(Key('z', ConsoleKey.Z));
        var c = sut.ReadChar();

        c.ShouldNotBeNull();
        c.Value.KeyChar.ShouldBe('z');
    }

    [Fact]
    public void KbHit_WhenPushBackNotEmpty_ReturnsTrue()
    {
        var keyboard = new QueueKeyboardService();
        keyboard.EnqueueNull();
        var sut = CreateSut(keyboard);

        sut.KbHit().ShouldBeFalse();
        sut.PushBack(Key('a', ConsoleKey.A));
        sut.KbHit().ShouldBeTrue();
    }

    [Fact]
    public void ReadLine_WhenValidatorRejectsThenAccepts_ReturnsAcceptedLine()
    {
        var keyboard = new QueueKeyboardService();
        keyboard.Enqueue(Key('9', ConsoleKey.D9)); // invalid for wordLength 5, allowed [0,1,4,5]
        keyboard.Enqueue(Key('\r', ConsoleKey.Enter));
        keyboard.Enqueue(Key('4', ConsoleKey.D4));
        keyboard.Enqueue(Key('\r', ConsoleKey.Enter));
        var (screen, inputLayer, compositor, cursor) = CreateMocks();
        var view = new TestInputView();
        var sut = new FixedRegionReadLine(keyboard, screen, compositor, cursor);
        var invalidCalls = new List<string>();
        var validator = new MatchCountValidator(5, [0, 1, 4, 5]);

        var result = sut.ReadLine(new ReadLineParams
        {
            InputLayer = inputLayer,
            InputView = view,
            KeyFilter = new MatchCountKeyFilter(),
            Validator = validator,
            OnInvalidInput = invalidCalls.Add
        });

        result.ShouldBe("4");
        invalidCalls.Count.ShouldBe(1);
        invalidCalls[0].ShouldContain("0, 1, 4, 5");
    }

    [Fact]
    public void ReadLine_Backspace_RemovesLastCharacter()
    {
        var keyboard = new QueueKeyboardService();
        keyboard.Enqueue(Key('A', ConsoleKey.A));
        keyboard.Enqueue(Key('B', ConsoleKey.B));
        keyboard.Enqueue(Key('\b', ConsoleKey.Backspace));
        keyboard.Enqueue(Key('\r', ConsoleKey.Enter));
        var sut = CreateSut(keyboard);
        var (_, inputLayer, _, _) = CreateMocks();
        var view = new TestInputView();

        var result = sut.ReadLine(new ReadLineParams
        {
            InputLayer = inputLayer,
            InputView = view,
            KeyFilter = new DataInputKeyFilter()
        });

        result.ShouldBe("A");
    }

    [Fact]
    public void ReadLine_WhenSpecialKey_InvokesOnSpecialKeyAndRenderExtra()
    {
        var keyboard = new QueueKeyboardService();
        keyboard.Enqueue(Key('\0', ConsoleKey.UpArrow));
        keyboard.Enqueue(Key('\r', ConsoleKey.Enter));
        var (screen, inputLayer, compositor, cursor) = CreateMocks();
        var view = new TestInputView();
        var sut = new FixedRegionReadLine(keyboard, screen, compositor, cursor);
        ConsoleKeyInfo? specialKey = null;
        var renderExtraCalled = false;

        _ = sut.ReadLine(new ReadLineParams
        {
            InputLayer = inputLayer,
            InputView = view,
            KeyFilter = new DataInputKeyFilter(),
            OnSpecialKey = k => specialKey = k,
            RenderExtra = () => renderExtraCalled = true
        });

        specialKey.ShouldNotBeNull();
        specialKey!.Value.Key.ShouldBe(ConsoleKey.UpArrow);
        renderExtraCalled.ShouldBeTrue();
    }
}
