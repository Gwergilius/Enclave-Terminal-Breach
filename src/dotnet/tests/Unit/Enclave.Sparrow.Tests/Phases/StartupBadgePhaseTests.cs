using Enclave.Common.Test.Core;
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

        var phase = new StartupBadgePhase(console);

        // Act
        phase.Run();

        // Assert
        writtenLines.Count.ShouldBeGreaterThanOrEqualTo(2);
        writtenLines[0].ShouldContain("SPARROW");
        writtenLines[0].ShouldMatch(@"\d+\.\d+\.\d+"); // version format X.Y.Z
        writtenLines[1].ShouldContain("Loading system profiles");
        writtenLines[1].ShouldContain("ms");
    }

    [Fact]
    public void Run_WritesThreeLines()
    {
        // Arrange
        var writtenLines = new List<string>();
        var console = Mock.Of<IConsoleIO>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var phase = new StartupBadgePhase(console);

        // Act
        phase.Run();

        // Assert
        writtenLines.Count.ShouldBe(3);
    }

}
