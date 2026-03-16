using Enclave.Common.Test.Core;
using Enclave.Raven.Keyboard;
using Enclave.Raven.Services;

namespace Enclave.Raven.Tests.Keyboard;

[UnitTest, TestOf(nameof(ExitService))]
public class ExitServiceTests
{
    private static ConsoleKeyInfo Key(char c, ConsoleKey k, bool control = false, bool alt = false) =>
        new(c, k, shift: false, alt, control);

    [Fact]
    public void Start_SubscribesToKeyboard()
    {
        var keyboard = Mock.Of<IKeyboardService>();
        var exitRequest = new ExitRequest();
        var sut = new ExitService(keyboard, exitRequest);

        sut.Start();

        keyboard.AsMock().Verify(
            k => k.Subscribe(It.IsAny<int>(), It.IsAny<Action<KeyPressedEventArgs>>()),
            Times.Once);
    }

    [Fact]
    public void OnKeyPressed_CtrlC_RequestsExitAndSetsHandled()
    {
        Action<KeyPressedEventArgs>? capturedHandler = null;
        var keyboard = Mock.Of<IKeyboardService>();
        keyboard.AsMock()
            .Setup(k => k.Subscribe(It.IsAny<int>(), It.IsAny<Action<KeyPressedEventArgs>>()))
            .Callback<int, Action<KeyPressedEventArgs>>((_, h) => capturedHandler = h)
            .Returns(Mock.Of<IDisposable>());
        var exitRequest = new ExitRequest();
        var sut = new ExitService(keyboard, exitRequest);
        sut.Start();
        capturedHandler.ShouldNotBeNull();

        var e = new KeyPressedEventArgs(Key('\x03', ConsoleKey.C)); // Ctrl+C (ETX)
        capturedHandler(e);

        exitRequest.IsExitRequested.ShouldBeTrue();
        e.Handled.ShouldBeTrue();
    }

    [Fact]
    public void OnKeyPressed_CtrlC_Modifiers_RequestsExitAndSetsHandled()
    {
        Action<KeyPressedEventArgs>? capturedHandler = null;
        var keyboard = Mock.Of<IKeyboardService>();
        keyboard.AsMock()
            .Setup(k => k.Subscribe(It.IsAny<int>(), It.IsAny<Action<KeyPressedEventArgs>>()))
            .Callback<int, Action<KeyPressedEventArgs>>((_, h) => capturedHandler = h)
            .Returns(Mock.Of<IDisposable>());
        var exitRequest = new ExitRequest();
        var sut = new ExitService(keyboard, exitRequest);
        sut.Start();
        capturedHandler.ShouldNotBeNull();

        var e = new KeyPressedEventArgs(Key('c', ConsoleKey.C, control: true));
        capturedHandler(e);

        exitRequest.IsExitRequested.ShouldBeTrue();
        e.Handled.ShouldBeTrue();
    }

    [Fact]
    public void OnKeyPressed_AltF4_RequestsExitAndSetsHandled()
    {
        Action<KeyPressedEventArgs>? capturedHandler = null;
        var keyboard = Mock.Of<IKeyboardService>();
        keyboard.AsMock()
            .Setup(k => k.Subscribe(It.IsAny<int>(), It.IsAny<Action<KeyPressedEventArgs>>()))
            .Callback<int, Action<KeyPressedEventArgs>>((_, h) => capturedHandler = h)
            .Returns(Mock.Of<IDisposable>());
        var exitRequest = new ExitRequest();
        var sut = new ExitService(keyboard, exitRequest);
        sut.Start();
        capturedHandler.ShouldNotBeNull();

        var e = new KeyPressedEventArgs(Key('\0', ConsoleKey.F4, alt: true));
        capturedHandler(e);

        exitRequest.IsExitRequested.ShouldBeTrue();
        e.Handled.ShouldBeTrue();
    }

    [Fact]
    public void OnKeyPressed_OrdinaryKey_DoesNotRequestExit_DoesNotSetHandled()
    {
        Action<KeyPressedEventArgs>? capturedHandler = null;
        var keyboard = Mock.Of<IKeyboardService>();
        keyboard.AsMock()
            .Setup(k => k.Subscribe(It.IsAny<int>(), It.IsAny<Action<KeyPressedEventArgs>>()))
            .Callback<int, Action<KeyPressedEventArgs>>((_, h) => capturedHandler = h)
            .Returns(Mock.Of<IDisposable>());
        var exitRequest = new ExitRequest();
        var sut = new ExitService(keyboard, exitRequest);
        sut.Start();
        capturedHandler.ShouldNotBeNull();

        var e = new KeyPressedEventArgs(Key('x', ConsoleKey.X));
        capturedHandler(e);

        exitRequest.IsExitRequested.ShouldBeFalse();
        e.Handled.ShouldBeFalse();
    }

    [Fact]
    public void Start_CalledTwice_SubscribesOnlyOnce()
    {
        var keyboard = Mock.Of<IKeyboardService>();
        keyboard.AsMock()
            .Setup(k => k.Subscribe(It.IsAny<int>(), It.IsAny<Action<KeyPressedEventArgs>>()))
            .Returns(Mock.Of<IDisposable>());
        var exitRequest = new ExitRequest();
        var sut = new ExitService(keyboard, exitRequest);

        sut.Start();
        sut.Start();

        keyboard.AsMock().Verify(
            k => k.Subscribe(It.IsAny<int>(), It.IsAny<Action<KeyPressedEventArgs>>()),
            Times.Once);
    }
}
