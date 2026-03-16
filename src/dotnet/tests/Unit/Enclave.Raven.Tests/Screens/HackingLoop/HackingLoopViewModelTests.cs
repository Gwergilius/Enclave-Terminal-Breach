using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Models;
using Xunit;
using Enclave.Echelon.Core.Services;
using Enclave.Phosphor;
using Enclave.Raven.Input;
using Enclave.Raven.Screens.HackingLoop;
using Enclave.Raven.Services;
using Enclave.Shared.Phases;

namespace Enclave.Raven.Tests.Screens.HackingLoop;

[UnitTest, TestOf(nameof(HackingLoopViewModel))]
public class HackingLoopViewModelTests
{
    // --- helpers ---

    private static (IVirtualScreen screen, Layer layer) CreateScreen()
    {
        var screen = Mock.Of<IVirtualScreen>();
        var layer = new Layer(new Rectangle(0, 0, 80, 24), zOrder: 0);
        screen.AsMock().SetupGet(s => s.Size).Returns(new Size(80, 24));
        screen.AsMock().Setup(s => s.AddLayer(It.IsAny<Rectangle>(), It.IsAny<int>())).Returns(layer);
        screen.AsMock().Setup(s => s.FlushDirtyRegions()).Returns(new List<Rectangle>());
        return (screen, layer);
    }

    private static HackingLoopViewModel CreateSut(
        IGameSession session,
        IVirtualScreen screen,
        INavigationService navigation,
        IExitRequest exitRequest,
        ISolverFactory solverFactory,
        IFixedRegionReadLine fixedRegionReadLine) =>
        new(session, screen, Mock.Of<ICompositor>(), Mock.Of<IPhosphorCursor>(), navigation, exitRequest, solverFactory, fixedRegionReadLine);

    // --- no-candidates path ---

    [Fact]
    public async Task RunAsync_WhenNoWordLength_WhenExitRequested_NavigatesToExit()
    {
        var (screen, _) = CreateScreen();
        var session = Mock.Of<IGameSession>();
        session.AsMock().SetupGet(s => s.WordLength).Returns((int?)null);
        session.AsMock().SetupGet(s => s.Count).Returns(0);
        var navigation = Mock.Of<INavigationService>();
        var exitRequest = Mock.Of<IExitRequest>();
        exitRequest.AsMock().SetupGet(e => e.IsExitRequested).Returns(true);

        var sut = CreateSut(session, screen, navigation, exitRequest, Mock.Of<ISolverFactory>(), Mock.Of<IFixedRegionReadLine>());
        await sut.RunAsync(TestContext.Current.CancellationToken);

        navigation.AsMock().Verify(n => n.NavigateTo("Exit"), Times.Once);
        navigation.AsMock().Verify(n => n.NavigateTo("DataInput"), Times.Never);
    }

    [Fact]
    public async Task RunAsync_WhenNoWordLength_WhenNotExiting_NavigatesToKeyPress()
    {
        var (screen, _) = CreateScreen();
        var session = Mock.Of<IGameSession>();
        session.AsMock().SetupGet(s => s.WordLength).Returns((int?)null);
        session.AsMock().SetupGet(s => s.Count).Returns(0);
        var navigation = new NavigationService();
        var exitRequest = Mock.Of<IExitRequest>();
        exitRequest.AsMock().SetupGet(e => e.IsExitRequested).Returns(false);

        var sut = CreateSut(session, screen, navigation, exitRequest, Mock.Of<ISolverFactory>(), Mock.Of<IFixedRegionReadLine>());
        await sut.RunAsync(TestContext.Current.CancellationToken);

        navigation.NextPhase.ShouldBe("KeyPress");
        ((string)navigation.NextPhaseArgs[1]).ShouldBe("DataInput");
    }

    // --- solver-returns-null path ---

    [Fact]
    public async Task RunAsync_WhenSolverHasNoGuess_WhenExitRequested_NavigatesToExit()
    {
        var (screen, _) = CreateScreen();
        var session = Mock.Of<IGameSession>();
        session.AsMock().SetupGet(s => s.WordLength).Returns(5);
        session.AsMock().SetupGet(s => s.Count).Returns(3);
        var solver = Mock.Of<IPasswordSolver>();
        solver.AsMock().Setup(s => s.GetBestGuess(It.IsAny<IEnumerable<Password>>())).Returns((Password?)null);
        var solverFactory = Mock.Of<ISolverFactory>();
        solverFactory.AsMock().Setup(f => f.GetSolver()).Returns(solver);
        var navigation = Mock.Of<INavigationService>();
        var exitRequest = Mock.Of<IExitRequest>();
        exitRequest.AsMock().SetupGet(e => e.IsExitRequested).Returns(true);

        var sut = CreateSut(session, screen, navigation, exitRequest, solverFactory, Mock.Of<IFixedRegionReadLine>());
        await sut.RunAsync(TestContext.Current.CancellationToken);

        navigation.AsMock().Verify(n => n.NavigateTo("Exit"), Times.Once);
    }

    // --- immediate-solve path ---

    [Fact]
    public async Task RunAsync_WhenImmediatelySolved_NavigatesToKeyPress()
    {
        var (screen, _) = CreateScreen();
        var session = Mock.Of<IGameSession>();
        var candidates = new List<Password> { new("HELLO"), new("WORLD"), new("CELLO") };
        session.AsMock().SetupGet(s => s.WordLength).Returns(5);
        session.AsMock().SetupGet(s => s.Count).Returns(3);
        session.AsMock().Setup(s => s.GetEnumerator()).Returns(() => candidates.GetEnumerator());
        var guess = new Password("HELLO");
        var solver = Mock.Of<IPasswordSolver>();
        solver.AsMock().Setup(s => s.GetBestGuess(It.IsAny<IEnumerable<Password>>())).Returns(guess);
        var solverFactory = Mock.Of<ISolverFactory>();
        solverFactory.AsMock().Setup(f => f.GetSolver()).Returns(solver);
        var readLine = Mock.Of<IFixedRegionReadLine>();
        readLine.AsMock().Setup(r => r.ReadLine(It.IsAny<ReadLineParams>())).Returns("5");
        var navigation = new NavigationService();
        var exitRequest = Mock.Of<IExitRequest>();
        exitRequest.AsMock().SetupGet(e => e.IsExitRequested).Returns(false);

        var sut = CreateSut(session, screen, navigation, exitRequest, solverFactory, readLine);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        navigation.NextPhase.ShouldBe("KeyPress");
        ((string)navigation.NextPhaseArgs[1]).ShouldBe("DataInput");
    }
}
