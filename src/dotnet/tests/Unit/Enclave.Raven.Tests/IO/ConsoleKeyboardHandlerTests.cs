using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.IO;
using Enclave.Shared.IO;

namespace Enclave.Raven.Tests.IO;

/// <summary>
/// Unit tests for <see cref="ConsoleKeyboardHandler"/>.
/// </summary>
[UnitTest, TestOf(nameof(ConsoleKeyboardHandler))]
public sealed class ConsoleKeyboardHandlerTests
{
    [Fact]
    public void ReadLine_DelegatesToConsole()
    {
        var console = Mock.Of<IConsoleIO>();
        console.AsMock()
            .Setup(c => c.ReadLine())
            .Returns("hello");

        var handler = new ConsoleKeyboardHandler(console);

        handler.ReadLine().ShouldBe("hello");
    }

    [Fact]
    public void ReadLine_WhenConsoleReturnsNull_ReturnsNull()
    {
        var console = Mock.Of<IConsoleIO>();
        console.AsMock().Setup(c => c.ReadLine()).Returns((string?)null);

        var handler = new ConsoleKeyboardHandler(console);

        handler.ReadLine().ShouldBeNull();
    }

    [Fact]
    public void ReadKey_DelegatesToConsole()
    {
        var key = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false);
        var console = Mock.Of<IConsoleIO>();
        console.AsMock()
            .Setup(c => c.ReadKey())
            .Returns(key);

        var handler = new ConsoleKeyboardHandler(console);

        handler.ReadKey().ShouldBe(key);
    }

    [Fact]
    public void ReadKey_WhenConsoleReturnsNull_ReturnsNull()
    {
        var console = Mock.Of<IConsoleIO>();
        console.AsMock().Setup(c => c.ReadKey()).Returns((ConsoleKeyInfo?)null);

        var handler = new ConsoleKeyboardHandler(console);

        handler.ReadKey().ShouldBeNull();
    }

    [Fact]
    public void OnKeyPressed_ReturnsFalse()
    {
        var console = Mock.Of<IConsoleIO>();
        var handler = new ConsoleKeyboardHandler(console);
        var key = new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false);

        handler.OnKeyPressed(key).ShouldBeFalse();
    }
}
