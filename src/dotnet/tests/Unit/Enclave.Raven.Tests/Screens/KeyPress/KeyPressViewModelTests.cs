using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Xunit;
using Enclave.Raven.Keyboard;
using Enclave.Raven.Screens.KeyPress;
using Enclave.Raven.Services;
using Enclave.Shared.Phases;

namespace Enclave.Raven.Tests.Screens.KeyPress;

[UnitTest, TestOf(nameof(KeyPressViewModel))]
public class KeyPressViewModelTests
{
    // --- helpers ---

    private static (IVirtualScreen screen, Layer layer) CreateScreen()
    {
        var screen = Mock.Of<IVirtualScreen>();
        var layer  = new Layer(new Rectangle(0, 0, 80, 24), zOrder: 0);
        screen.AsMock().SetupGet(s => s.Size).Returns(new Size(80, 24));
        screen.AsMock().Setup(s => s.AddLayer(It.IsAny<Rectangle>(), It.IsAny<int>())).Returns(layer);
        screen.AsMock().Setup(s => s.FlushDirtyRegions()).Returns(new List<Rectangle>());
        return (screen, layer);
    }

    private static KeyPressViewModel CreateSut(
        IKeyboardService keyboard,
        IVirtualScreen screen,
        INavigationService navigation) =>
        new(keyboard, screen, Mock.Of<ICompositor>(), navigation);

    // --- navigation ---

    [Fact]
    public async Task RunAsync_WhenArgsProvide_NavigatesToSpecifiedNext()
    {
        var (screen, _) = CreateScreen();
        var navigation = new NavigationService();
        navigation.NavigateTo("KeyPress", "Test prompt", "DataInput");

        var sut = CreateSut(Mock.Of<IKeyboardService>(), screen, navigation);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        navigation.NextPhase.ShouldBe("DataInput");
    }

    [Fact]
    public async Task RunAsync_WhenNoArgs_NavigatesToExitByDefault()
    {
        var (screen, _) = CreateScreen();
        var navigation = new NavigationService();
        // No NavigateTo("KeyPress", ...) call — NextPhaseArgs is empty

        var sut = CreateSut(Mock.Of<IKeyboardService>(), screen, navigation);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        navigation.NextPhase.ShouldBe("Exit");
    }

    // --- input ---

    [Fact]
    public async Task RunAsync_AlwaysCallsGetNextKey()
    {
        var (screen, _) = CreateScreen();
        var navigation = new NavigationService();
        navigation.NavigateTo("KeyPress", "Prompt", "DataInput");
        var keyboard = Mock.Of<IKeyboardService>();
        keyboard.AsMock().Setup(k => k.GetNextKey()).Returns(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));

        var sut = CreateSut(keyboard, screen, navigation);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        keyboard.AsMock().Verify(k => k.GetNextKey(), Times.Once);
    }
}
