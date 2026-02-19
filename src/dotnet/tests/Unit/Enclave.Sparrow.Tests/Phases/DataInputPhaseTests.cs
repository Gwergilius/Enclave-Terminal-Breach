using Enclave.Common.Test.Core;
using Enclave.Sparrow.Configuration;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Phases;

namespace Enclave.Sparrow.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="DataInputPhase"/>. Private methods (ProcessInputLine, GetTokens, WriteCandidateCountAndList)
/// are exercised through Run() with mocked IConsoleIO.
/// </summary>
[UnitTest, TestOf(nameof(DataInputPhase))]
public class DataInputPhaseTests
{
    [Fact]
    public void Run_WithValidWords_AddsToSession()
    {
        // Arrange: first line "TERMS TEXAS", second line empty to exit
        var console = Mock.Of<IConsoleIO>();
        var readLineCalls = 0;
        console.AsMock()
            .Setup(c => c.ReadLine())
            .Returns(() => readLineCalls++ == 0 ? "TERMS TEXAS" : "");

        var session = new GameSession();
        var options = new SparrowOptions(); // WordListPath null = manual input
        var phase = new DataInputPhase(session, console, options);

        // Act
        phase.Run();

        // Assert: session contains both words
        session.Count.ShouldBe(2);
        session.WordLength.ShouldBe(5);
        session.Any(p => p.Word == "TERMS").ShouldBeTrue();
        session.Any(p => p.Word == "TEXAS").ShouldBeTrue();
    }

    [Fact]
    public void Run_WithMinusToken_RemovesFromSession()
    {
        // Arrange: add TERMS TEXAS, then remove TEXAS with -TEXAS
        var console = Mock.Of<IConsoleIO>();
        var readLineCalls = 0;
        console.AsMock()
            .Setup(c => c.ReadLine())
            .Returns(() =>
            {
                readLineCalls++;
                return readLineCalls switch
                {
                    1 => "TERMS TEXAS",
                    2 => "-TEXAS",
                    _ => ""
                };
            });

        var session = new GameSession();
        var options = new SparrowOptions(); // WordListPath null = manual input
        var phase = new DataInputPhase(session, console, options);

        // Act
        phase.Run();

        // Assert: only TERMS remains
        session.Count.ShouldBe(1);
        session[0].Word.ShouldBe("TERMS");
    }

    [Fact]
    public void Run_WithInvalidWord_WritesErrorAndSkips()
    {
        // Arrange: TERMS (valid) then TERM1 (invalid - contains digit)
        var console = Mock.Of<IConsoleIO>();
        var writtenLines = new List<string>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));
        var readLineCalls = 0;
        console.AsMock()
            .Setup(c => c.ReadLine())
            .Returns(() => readLineCalls++ == 0 ? "TERMS TERM1" : "");

        var session = new GameSession();
        var options = new SparrowOptions(); // WordListPath null = manual input
        var phase = new DataInputPhase(session, console, options);

        // Act
        phase.Run();

        // Assert: only TERMS added; error written for TERM1
        session.Count.ShouldBe(1);
        writtenLines.ShouldContain(l => l.Contains("letters") || l.Contains("Invalid") || l.Contains("skipped"));
    }

    [Fact]
    public void Run_WithMinusToken_WhenWordNotInList_WritesErrorAndKeepsList()
    {
        // Arrange: add TERMS, then try to remove TEXAS (not in list)
        var console = Mock.Of<IConsoleIO>();
        var writtenLines = new List<string>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));
        var readLineCalls = 0;
        console.AsMock()
            .Setup(c => c.ReadLine())
            .Returns(() =>
            {
                readLineCalls++;
                return readLineCalls switch
                {
                    1 => "TERMS",
                    2 => "-TEXAS",
                    _ => ""
                };
            });

        var session = new GameSession();
        var options = new SparrowOptions(); // WordListPath null = manual input
        var phase = new DataInputPhase(session, console, options);

        // Act
        phase.Run();

        // Assert: TERMS remains; error written for non-existent -TEXAS
        session.Count.ShouldBe(1);
        session[0].Word.ShouldBe("TERMS");
        writtenLines.ShouldContain(l => l.Contains("Not in list") || l.Contains("ignored"));
    }

    [Fact]
    public void Run_WithEmptyFirstLine_ExitsWithoutAdding()
    {
        // Arrange
        var console = Mock.Of<IConsoleIO>();
        console.AsMock().Setup(c => c.ReadLine()).Returns("");

        var session = new GameSession();
        var options = new SparrowOptions(); // WordListPath null = manual input
        var phase = new DataInputPhase(session, console, options);

        // Act
        phase.Run();

        // Assert
        session.Count.ShouldBe(0);
    }
}
