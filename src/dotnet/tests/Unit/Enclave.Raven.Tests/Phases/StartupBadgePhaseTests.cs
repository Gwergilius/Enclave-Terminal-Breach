using Enclave.Common.Test.Core;
using Enclave.Raven.Configuration;
using Enclave.Shared.IO;
using Enclave.Raven.Phases;

namespace Enclave.Raven.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="StartupBadgePhase"/>.
/// </summary>
[UnitTest, TestOf(nameof(StartupBadgePhase))]
public class StartupBadgePhaseTests
{
    [Fact]
    public void Run_WritesProductNameAndVersion()
    {
        var writtenLines = new List<string>();
        var console = Mock.Of<IConsoleIO>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var options = new RavenOptions();
        var phase = new StartupBadgePhase(console, options);

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
        var console = Mock.Of<IConsoleIO>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var options = new RavenOptions();
        var phase = new StartupBadgePhase(console, options);

        phase.Run();

        writtenLines.Count.ShouldBe(5);
        writtenLines[4].ShouldBe("");
    }

    [Fact]
    public void Run_WithIntelligenceTie_WritesOptimalAndLevel2()
    {
        var writtenLines = new List<string>();
        var console = Mock.Of<IConsoleIO>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var options = new RavenOptions { Intelligence = "tie" };
        var phase = new StartupBadgePhase(console, options);

        phase.Run();

        writtenLines.ShouldContain(l => l.StartsWith("Intelligence level: optimal (2)"));
        writtenLines.ShouldContain(l => l == "Dictionary: manual");
    }
}
