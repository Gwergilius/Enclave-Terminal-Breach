using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Xunit;
using Enclave.Raven.Configuration;
using Enclave.Raven.Input;
using Enclave.Raven.Screens.DataInput;
using Enclave.Shared.Phases;

namespace Enclave.Raven.Tests.Screens.DataInput;

[UnitTest, TestOf(nameof(DataInputViewModel))]
public class DataInputViewModelTests
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

    private static DataInputViewModel CreateSut(
        IGameSession session,
        IVirtualScreen screen,
        INavigationService navigation,
        RavenOptions options,
        IFixedRegionReadLine fixedRegionReadLine) =>
        new(session, screen, Mock.Of<ICompositor>(), Mock.Of<IPhosphorCursor>(), navigation, options, fixedRegionReadLine);

    private static IGameSession EmptySession()
    {
        var s = Mock.Of<IGameSession>();
        s.AsMock().SetupGet(x => x.Count).Returns(0);
        s.AsMock().SetupGet(x => x.WordLength).Returns((int?)null);
        return s;
    }

    // --- interactive mode ---

    private static ConsoleKeyInfo Key(char c, ConsoleKey k) => new(c, k, false, false, false);

    [Fact]
    public async Task RunAsync_Interactive_WhenInputIsEmptyImmediately_NavigatesToHackingLoop()
    {
        var (screen, _) = CreateScreen();
        var readLine = Mock.Of<IFixedRegionReadLine>();
        readLine.AsMock().Setup(r => r.ReadLine(It.IsAny<ReadLineParams>())).Returns("");
        var navigation = Mock.Of<INavigationService>();

        var sut = CreateSut(EmptySession(), screen, navigation, new RavenOptions(), readLine);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        navigation.AsMock().Verify(n => n.NavigateTo("HackingLoop"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Interactive_ProcessesWordsBeforeEmptyLine()
    {
        var (screen, _) = CreateScreen();
        var session = Mock.Of<IGameSession>();
        session.AsMock().SetupGet(s => s.Count).Returns(0);
        session.AsMock().SetupGet(s => s.WordLength).Returns((int?)null);
        session.AsMock().Setup(s => s.Add(It.IsAny<string>())).Returns(Result.Ok());
        var lines = new Queue<string?>(["HELLO", "WORLD", ""]);
        var readLine = Mock.Of<IFixedRegionReadLine>();
        readLine.AsMock().Setup(r => r.ReadLine(It.IsAny<ReadLineParams>())).Returns(() => lines.Dequeue());
        var navigation = Mock.Of<INavigationService>();

        var sut = CreateSut(session, screen, navigation, new RavenOptions(), readLine);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        session.AsMock().Verify(s => s.Add("HELLO"), Times.Once);
        session.AsMock().Verify(s => s.Add("WORLD"), Times.Once);
        navigation.AsMock().Verify(n => n.NavigateTo("HackingLoop"), Times.Once);
    }

    // --- file mode ---

    [Fact]
    public async Task RunAsync_FileMode_WithNonExistentPath_NavigatesToHackingLoopWithError()
    {
        var (screen, _) = CreateScreen();
        var navigation = Mock.Of<INavigationService>();
        var options = new RavenOptions { WordListPath = "definitely-does-not-exist.txt" };

        var sut = CreateSut(EmptySession(), screen, navigation, options, Mock.Of<IFixedRegionReadLine>());
        await sut.RunAsync(TestContext.Current.CancellationToken);

        // Even on missing file the flow must complete and navigate forward
        navigation.AsMock().Verify(n => n.NavigateTo("HackingLoop"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_FileMode_WithExistingFile_LoadsWordsAndNavigates()
    {
        var (screen, _) = CreateScreen();
        var session = Mock.Of<IGameSession>();
        session.AsMock().SetupGet(s => s.Count).Returns(0);
        session.AsMock().SetupGet(s => s.WordLength).Returns((int?)null);
        session.AsMock().Setup(s => s.Add(It.IsAny<string>())).Returns(Result.Ok());

        var tmp = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tmp, "ALPHA BETA", TestContext.Current.CancellationToken);
            var navigation = Mock.Of<INavigationService>();
            var options    = new RavenOptions { WordListPath = tmp };

            var sut = CreateSut(session, screen, navigation, options, Mock.Of<IFixedRegionReadLine>());
            await sut.RunAsync(TestContext.Current.CancellationToken);

            session.AsMock().Verify(s => s.Add("ALPHA"), Times.Once);
            session.AsMock().Verify(s => s.Add("BETA"),  Times.Once);
            navigation.AsMock().Verify(n => n.NavigateTo("HackingLoop"), Times.Once);
        }
        finally
        {
            File.Delete(tmp);
        }
    }
}
