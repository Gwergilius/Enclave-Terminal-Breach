using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.Configuration;
using Enclave.Raven.Phases;
using Enclave.Shared.Models;
using Enclave.Shared.Phases;
using Moq;

namespace Enclave.Raven.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="DataInputPhase"/>. Private methods (ProcessInputLine, GetTokens, WriteCandidateCountAndList)
/// are exercised through Run() with mocked IPhosphorWriter and IPhosphorReader.
/// </summary>
[UnitTest, TestOf(nameof(DataInputPhase))]
public class DataInputPhaseTests
{
    private static INavigationService CreateNavigationReturnsOk()
    {
        var nav = Mock.Of<INavigationService>();
        nav.AsMock().Setup(n => n.NavigateTo(It.IsAny<string>(), It.IsAny<object[]>())).Returns(Result.Ok());
        return nav;
    }

    [Fact]
    public void Run_WithValidWords_AddsToSession()
    {
        var writer = Mock.Of<IPhosphorWriter>();
        var reader = Mock.Of<IPhosphorReader>();
        var readLineCalls = 0;
        reader.AsMock()
            .Setup(r => r.ReadLine())
            .Returns(() => readLineCalls++ == 0 ? "TERMS TEXAS" : "");

        var session = new GameSession();
        var options = new RavenOptions();
        var phase = new DataInputPhase(session, writer, reader, options, CreateNavigationReturnsOk());

        phase.Run();

        session.Count.ShouldBe(2);
        session.WordLength.ShouldBe(5);
        session.Any(p => p.Word == "TERMS").ShouldBeTrue();
        session.Any(p => p.Word == "TEXAS").ShouldBeTrue();
    }

    [Fact]
    public void Run_WithMinusToken_RemovesFromSession()
    {
        var writer = Mock.Of<IPhosphorWriter>();
        var reader = Mock.Of<IPhosphorReader>();
        var readLineCalls = 0;
        reader.AsMock()
            .Setup(r => r.ReadLine())
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
        var options = new RavenOptions();
        var phase = new DataInputPhase(session, writer, reader, options, CreateNavigationReturnsOk());

        phase.Run();

        session.Count.ShouldBe(1);
        session[0].Word.ShouldBe("TERMS");
    }

    [Fact]
    public void Run_WithInvalidWord_WritesErrorAndSkips()
    {
        var writer = Mock.Of<IPhosphorWriter>();
        var writtenLines = new List<string>();
        writer.AsMock()
            .Setup(w => w.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));
        var reader = Mock.Of<IPhosphorReader>();
        var readLineCalls = 0;
        reader.AsMock()
            .Setup(r => r.ReadLine())
            .Returns(() => readLineCalls++ == 0 ? "TERMS TERM1" : "");

        var session = new GameSession();
        var options = new RavenOptions();
        var phase = new DataInputPhase(session, writer, reader, options, CreateNavigationReturnsOk());

        phase.Run();

        session.Count.ShouldBe(1);
        writtenLines.ShouldContain(l => l.Contains("letters") || l.Contains("Invalid") || l.Contains("skipped"));
    }

    [Fact]
    public void Run_WithMinusToken_WhenWordNotInList_WritesErrorAndKeepsList()
    {
        var writer = Mock.Of<IPhosphorWriter>();
        var writtenLines = new List<string>();
        writer.AsMock()
            .Setup(w => w.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));
        var reader = Mock.Of<IPhosphorReader>();
        var readLineCalls = 0;
        reader.AsMock()
            .Setup(r => r.ReadLine())
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
        var options = new RavenOptions();
        var phase = new DataInputPhase(session, writer, reader, options, CreateNavigationReturnsOk());

        phase.Run();

        session.Count.ShouldBe(1);
        session[0].Word.ShouldBe("TERMS");
        writtenLines.ShouldContain(l => l.Contains("Not in list") || l.Contains("ignored"));
    }

    [Fact]
    public void Run_WithEmptyFirstLine_ExitsWithoutAdding()
    {
        var writer = Mock.Of<IPhosphorWriter>();
        var reader = Mock.Of<IPhosphorReader>();
        reader.AsMock().Setup(r => r.ReadLine()).Returns("");

        var session = new GameSession();
        var options = new RavenOptions();
        var phase = new DataInputPhase(session, writer, reader, options, CreateNavigationReturnsOk());

        phase.Run();

        session.Count.ShouldBe(0);
    }

    [Fact]
    public void Run_WithWordListPath_WhenFileExists_LoadsCandidatesAndWritesCountAndList()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "TERMS TEXAS\nTIRES\n");

            var writtenLines = new List<string>();
            var writer = Mock.Of<IPhosphorWriter>();
            writer.AsMock()
                .Setup(w => w.WriteLine(It.IsAny<string?>()))
                .Callback<string?>(s => writtenLines.Add(s ?? ""));
            var reader = Mock.Of<IPhosphorReader>();

            var session = new GameSession();
            var options = new RavenOptions { WordListPath = tempFile };
            var phase = new DataInputPhase(session, writer, reader, options, CreateNavigationReturnsOk());

            phase.Run();

            session.Count.ShouldBe(3);
            session.WordLength.ShouldBe(5);
            session.Any(p => p.Word == "TERMS").ShouldBeTrue();
            session.Any(p => p.Word == "TEXAS").ShouldBeTrue();
            session.Any(p => p.Word == "TIRES").ShouldBeTrue();
            writtenLines.ShouldContain(l => l == "3 candidate(s):");
            writtenLines.ShouldContain(l => l.Contains("TERMS") && l.Contains("TEXAS") && l.Contains("TIRES"));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void Run_WithWordListPath_WhenFileDoesNotExist_WritesErrorAndLeavesSessionEmpty()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"raven-test-{Guid.NewGuid():N}.txt");
        var writtenLines = new List<string>();
        var writer = Mock.Of<IPhosphorWriter>();
        writer.AsMock()
            .Setup(w => w.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));
        var reader = Mock.Of<IPhosphorReader>();

        var session = new GameSession();
        var options = new RavenOptions { WordListPath = nonExistentPath };
        var phase = new DataInputPhase(session, writer, reader, options, CreateNavigationReturnsOk());

        phase.Run();

        writtenLines.ShouldContain(l => l.StartsWith("Word list file not found:") && l.Contains(nonExistentPath));
        session.Count.ShouldBe(0);
    }

    [Fact]
    public void Run_WithWordListPath_WhenFileContainsMinusToken_IgnoresRemovalToken()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "TERMS TEXAS\n-TEXAS\n");

            var writer = Mock.Of<IPhosphorWriter>();
            var reader = Mock.Of<IPhosphorReader>();

            var session = new GameSession();
            var options = new RavenOptions { WordListPath = tempFile };
            var phase = new DataInputPhase(session, writer, reader, options, CreateNavigationReturnsOk());

            phase.Run();

            session.Count.ShouldBe(2);
            session.Any(p => p.Word == "TERMS").ShouldBeTrue();
            session.Any(p => p.Word == "TEXAS").ShouldBeTrue();
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
