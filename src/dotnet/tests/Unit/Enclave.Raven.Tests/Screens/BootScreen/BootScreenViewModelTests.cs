using System.Text;
using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Xunit;
using Enclave.Raven.Configuration;
using Enclave.Raven.Screens.BootScreen;
using Enclave.Raven.Services;
using Enclave.Shared.Phases;

namespace Enclave.Raven.Tests.Screens.BootScreen;

[UnitTest, TestOf(nameof(BootScreenViewModel))]
public class BootScreenViewModelTests
{
    // --- helpers ---

    private static (IVirtualScreen screen, Layer layer) CreateScreen()
    {
        var screen = Mock.Of<IVirtualScreen>();
        var layer  = new Layer(new Rectangle(0, 0, 80, 24), zOrder: 0);
        screen.AsMock().SetupGet(s => s.Size).Returns(new Size(80, 24));
        screen.AsMock().Setup(s => s.AddLayer(It.IsAny<Rectangle>(), It.IsAny<int>())).Returns(layer);
        screen.AsMock().Setup(s => s.FlushDirtyRegions()).Returns([]);
        return (screen, layer);
    }

    private static BootScreenViewModel CreateSut(
        IVirtualScreen screen,
        INavigationService navigation,
        RavenOptions? options = null,
        IProductInfo? productInfo = null) =>
        new(screen, Mock.Of<ICompositor>(), navigation,
            options ?? new RavenOptions(),
            productInfo ?? Mock.Of<IProductInfo>());

    private static IProductInfo MockProductInfo(string name = "RAVEN", string version = "1.0.0")
    {
        var info = Mock.Of<IProductInfo>();
        info.AsMock().SetupGet(p => p.Name).Returns(name);
        info.AsMock().SetupGet(p => p.Version).Returns(version);
        return info;
    }

    private static string ReadRow(Layer layer, int row)
    {
        var sb = new StringBuilder();
        for (var col = layer.Bounds.Left; col <= layer.Bounds.Right; col++)
            sb.Append(layer.GetCell(col, row).Character);
        return sb.ToString().TrimEnd('\0');
    }

    private static string ReadAllRows(Layer layer)
    {
        var sb = new StringBuilder();
        for (var row = layer.Bounds.Top; row <= layer.Bounds.Bottom; row++)
            sb.Append(ReadRow(layer, row)).Append('\n');
        return sb.ToString();
    }

    // --- navigation ---

    [Fact]
    public async Task RunAsync_NavigatesToKeyPressWithCorrectArgs()
    {
        var (screen, _) = CreateScreen();
        var navigation  = new NavigationService();

        var sut = CreateSut(screen, navigation);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        navigation.NextPhase.ShouldBe("KeyPress");
        ((string)navigation.NextPhaseArgs[0]).ShouldBe("Press any key to begin...");
        ((string)navigation.NextPhaseArgs[1]).ShouldBe("DataInput");
    }

    // --- banner ---

    [Fact]
    public async Task RunAsync_WhenShowBanner_RendersProductInfoOnFirstRow()
    {
        var (screen, layer) = CreateScreen();
        var options = new RavenOptions { Startup = new RavenStartupOptions { ShowBanner = true, ShowLoadTime = false } };
        var sut = CreateSut(screen, Mock.Of<INavigationService>(), options, MockProductInfo("RAVEN", "2.0.0"));

        await sut.RunAsync(TestContext.Current.CancellationToken);

        ReadRow(layer, 0).ShouldContain("RAVEN");
        ReadRow(layer, 0).ShouldContain("2.0.0");
    }

    [Fact]
    public async Task RunAsync_WhenShowBannerFalse_AndLoadTimeFalse_LayerRemainsEmpty()
    {
        var (screen, layer) = CreateScreen();
        var options = new RavenOptions { Startup = new RavenStartupOptions { ShowBanner = false, ShowLoadTime = false } };

        var sut = CreateSut(screen, Mock.Of<INavigationService>(), options);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        ReadRow(layer, 0).ShouldBeEmpty();
    }

    // --- intelligence + dictionary ---

    [Fact]
    public async Task RunAsync_WhenShowBanner_RendersIntelligenceLineToLayer()
    {
        var (screen, layer) = CreateScreen();
        var options = new RavenOptions { Startup = new RavenStartupOptions { ShowBanner = true, ShowLoadTime = false } };

        var sut = CreateSut(screen, Mock.Of<INavigationService>(), options);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        ReadAllRows(layer).ShouldContain("Intelligence level:");
    }

    [Fact]
    public async Task RunAsync_WhenWordListPathIsSet_RendersDictionaryPathInLayer()
    {
        var (screen, layer) = CreateScreen();
        var options = new RavenOptions
        {
            WordListPath = "words.txt",
            Startup = new RavenStartupOptions { ShowBanner = true, ShowLoadTime = false }
        };

        var sut = CreateSut(screen, Mock.Of<INavigationService>(), options);
        await sut.RunAsync(TestContext.Current.CancellationToken);

        ReadAllRows(layer).ShouldContain("words.txt");
    }

    // --- layer lifecycle ---

    [Fact]
    public async Task RunAsync_RemovesLayerAfterRender()
    {
        var (screen, layer) = CreateScreen();

        var sut = CreateSut(screen, Mock.Of<INavigationService>());
        await sut.RunAsync(TestContext.Current.CancellationToken);

        screen.AsMock().Verify(s => s.RemoveLayer(layer), Times.Once);
    }
}
