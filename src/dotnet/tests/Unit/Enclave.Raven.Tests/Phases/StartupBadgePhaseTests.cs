using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.Configuration;
using Enclave.Raven.Phases;
using Enclave.Shared.Phases;
using Moq;
using Enclave.Raven;

namespace Enclave.Raven.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="StartupBadgePhase"/>.
/// </summary>
[UnitTest, TestOf(nameof(StartupBadgePhase))]
public class StartupBadgePhaseTests
{
    private static INavigationService CreateNavigationReturnsOk()
    {
        var nav = Mock.Of<INavigationService>();
        nav.AsMock().Setup(n => n.NavigateTo(It.IsAny<string>(), It.IsAny<object[]>())).Returns(Result.Ok());
        return nav;
    }

    private static IProductInfo CreateProductInfo(string name = "RAVEN", string version = "1.0.0")
    {
        var info = Mock.Of<IProductInfo>();
        info.AsMock().Setup(p => p.Name).Returns(name);
        info.AsMock().Setup(p => p.Version).Returns(version);
        return info;
    }

    [Fact]
    public void Run_WritesProductNameAndVersion()
    {
        var writtenLines = new List<string>();
        var writer = Mock.Of<IPhosphorWriter>();
        writer.AsMock()
            .Setup(w => w.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var options = new RavenOptions();
        var phase = new StartupBadgePhase(writer, options, CreateNavigationReturnsOk(), CreateProductInfo());

        phase.Run();

        writtenLines.Count.ShouldBeGreaterThanOrEqualTo(4);
        writtenLines[0].ShouldContain("RAVEN");
        writtenLines[0].ShouldMatch(@"\d+\.\d+\.\d+");
        writtenLines[1].ShouldContain("Loading system profiles");
        writtenLines[1].ShouldContain("ms");
        writtenLines[2].ShouldContain("Intelligence level:");
        writtenLines[3].ShouldStartWith("Dictionary:");
    }

    [Fact]
    public void Run_WritesBannerLoadTimeConfigAndBlankLine()
    {
        var writtenLines = new List<string>();
        var writer = Mock.Of<IPhosphorWriter>();
        writer.AsMock()
            .Setup(w => w.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var options = new RavenOptions();
        var phase = new StartupBadgePhase(writer, options, CreateNavigationReturnsOk(), CreateProductInfo());

        phase.Run();

        writtenLines.Count.ShouldBe(5);
        writtenLines[4].ShouldBe("");
    }

    [Fact]
    public void Run_WithIntelligenceTie_WritesOptimalAndLevel2()
    {
        var writtenLines = new List<string>();
        var writer = Mock.Of<IPhosphorWriter>();
        writer.AsMock()
            .Setup(w => w.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var options = new RavenOptions { Intelligence = "tie" };
        var phase = new StartupBadgePhase(writer, options, CreateNavigationReturnsOk(), CreateProductInfo());

        phase.Run();

        writtenLines.ShouldContain(l => l.StartsWith("Intelligence level: optimal (2)"));
        writtenLines.ShouldContain(l => l == "Dictionary: manual");
    }
}
