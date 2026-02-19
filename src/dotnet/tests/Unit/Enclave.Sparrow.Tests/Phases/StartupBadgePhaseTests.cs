using Enclave.Common.Test.Core;
using Enclave.Sparrow.Configuration;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Phases;

namespace Enclave.Sparrow.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="StartupBadgePhase"/>.
/// </summary>
[UnitTest, TestOf(nameof(StartupBadgePhase))]
public class StartupBadgePhaseTests
{
    [Fact]
    public void Run_WritesProductNameAndVersion()
    {
        // Arrange
        var writtenLines = new List<string>();
        var console = Mock.Of<IConsoleIO>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var options = new SparrowOptions(); // ShowBanner and ShowLoadTime default true
        var phase = new StartupBadgePhase(console, options);

        // Act
        phase.Run();

        // Assert
        writtenLines.Count.ShouldBeGreaterThanOrEqualTo(4);
        writtenLines[0].ShouldContain("SPARROW");
        writtenLines[0].ShouldMatch(@"\d+\.\d+\.\d+"); // version format X.Y.Z
        writtenLines[1].ShouldContain("Loading system profiles");
        writtenLines[1].ShouldContain("ms");
        writtenLines[2].ShouldContain("Intelligence level:");
        writtenLines[3].ShouldStartWith("Dictionary:");
    }

    [Fact]
    public void Run_WritesBannerLoadTimeConfigAndBlankLine()
    {
        // Arrange
        var writtenLines = new List<string>();
        var console = Mock.Of<IConsoleIO>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var options = new SparrowOptions(); // ShowBanner and ShowLoadTime default true
        var phase = new StartupBadgePhase(console, options);

        // Act
        phase.Run();

        // Assert: banner, load time, Intelligence level, Dictionary, blank line
        writtenLines.Count.ShouldBe(5);
        writtenLines[4].ShouldBe("");
    }

    [Fact]
    public void Run_WithIntelligenceTie_WritesOptimalAndLevel2()
    {
        // Arrange: config "tie" normalizes to level 2; display uses military alias "optimal"
        var writtenLines = new List<string>();
        var console = Mock.Of<IConsoleIO>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var options = new SparrowOptions { Intelligence = "tie" };
        var phase = new StartupBadgePhase(console, options);

        // Act
        phase.Run();

        // Assert: badge always shows military display name (optimal, tactical, baseline)
        writtenLines.ShouldContain(l => l.StartsWith("Intelligence level: optimal (2)"));
        writtenLines.ShouldContain(l => l == "Dictionary: internal");
    }

}
