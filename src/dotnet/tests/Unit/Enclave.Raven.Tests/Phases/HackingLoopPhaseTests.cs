using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Services;
using Enclave.Phosphor;
using Enclave.Shared.Models;
using Enclave.Raven.Phases;

namespace Enclave.Raven.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="HackingLoopPhase"/>. Private methods (NarrowCandidates, ReadMatchCount, WriteCandidates)
/// are exercised through Run() with mocked IPhosphorWriter, IPhosphorReader and IPasswordSolver.
/// </summary>
[UnitTest, TestOf(nameof(HackingLoopPhase))]
public class HackingLoopPhaseTests
{
    private static ISolverFactory CreateSolverFactory(IPasswordSolver solver)
    {
        var factory = Mock.Of<ISolverFactory>();
        factory.AsMock().Setup(f => f.GetSolver()).Returns(solver);
        return factory;
    }

    [Fact]
    public void Run_WhenNoCandidates_WritesMessageAndExits()
    {
        var writer = Mock.Of<IPhosphorWriter>();
        var writtenLines = new List<string>();
        writer.AsMock()
            .Setup(w => w.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));
        var reader = Mock.Of<IPhosphorReader>();
        var session = new GameSession();
        var solver = Mock.Of<IPasswordSolver>();
        var phase = new HackingLoopPhase(session, writer, reader, CreateSolverFactory(solver));

        phase.Run();

        writtenLines.ShouldContain("No candidates. Exiting.");
    }

    [Fact]
    public void Run_WhenCorrectGuess_WritesSuccessAndExits()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var writer = Mock.Of<IPhosphorWriter>();
        var writtenLines = new List<string>();
        writer.AsMock()
            .Setup(w => w.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));
        writer.AsMock().Setup(w => w.Write(It.IsAny<string>()));
        var reader = Mock.Of<IPhosphorReader>();
        reader.AsMock().Setup(r => r.ReadLine()).Returns("5");

        var solver = Mock.Of<IPasswordSolver>();
        var terms = new Password("TERMS");
        solver.AsMock()
            .Setup(s => s.GetBestGuess(It.IsAny<IGameSession>()))
            .Returns(terms);

        var phase = new HackingLoopPhase(session, writer, reader, CreateSolverFactory(solver));

        phase.Run();

        writtenLines.ShouldContain("Correct. Terminal cracked.");
    }

    [Fact]
    public void Run_WhenSolverReturnsNull_WritesMessageAndExits()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var writer = Mock.Of<IPhosphorWriter>();
        var writtenLines = new List<string>();
        writer.AsMock()
            .Setup(w => w.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));
        var reader = Mock.Of<IPhosphorReader>();
        var solver = Mock.Of<IPasswordSolver>();
        solver.AsMock()
            .Setup(s => s.GetBestGuess(It.IsAny<IGameSession>()))
            .Returns((Password?)null);

        var phase = new HackingLoopPhase(session, writer, reader, CreateSolverFactory(solver));

        phase.Run();

        writtenLines.ShouldContain("No candidates left. Exiting.");
    }

    [Fact]
    public void Run_WhenPartialMatch_NarrowsCandidatesAndContinues()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();
        session.Add("TANKS").IsSuccess.ShouldBeTrue();

        var writer = Mock.Of<IPhosphorWriter>();
        writer.AsMock().Setup(w => w.Write(It.IsAny<string>()));
        var reader = Mock.Of<IPhosphorReader>();
        var readLineCalls = 0;
        reader.AsMock()
            .Setup(r => r.ReadLine())
            .Returns(() => readLineCalls++ == 0 ? "3" : "5");

        var solver = Mock.Of<IPasswordSolver>();
        var terms = new Password("TERMS");
        var texas = new Password("TEXAS");
        var callCount = 0;
        solver.AsMock()
            .Setup(s => s.GetBestGuess(It.IsAny<IGameSession>()))
            .Returns(() => callCount++ == 0 ? terms : texas);

        solver.AsMock()
            .Setup(s => s.NarrowCandidates(It.IsAny<IGameSession>(), terms, 3))
            .Returns<IGameSession, Password, int>((sess, _, _) =>
            {
                var list = sess.ToList();
                return list.Where(p => p.GetMatchCount(terms) == 3).ToList();
            });

        var phase = new HackingLoopPhase(session, writer, reader, CreateSolverFactory(solver));

        phase.Run();

        writer.AsMock().Verify(w => w.WriteLine("Correct. Terminal cracked."), Times.Once);
    }
}
