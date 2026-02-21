using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Services;
using Enclave.Shared.IO;
using Enclave.Shared.Models;
using Enclave.Raven.Phases;

namespace Enclave.Raven.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="HackingLoopPhase"/>. Private methods (NarrowCandidates, ReadMatchCount, WriteCandidates)
/// are exercised through Run() with mocked IConsoleIO and IPasswordSolver.
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
        var console = Mock.Of<IConsoleIO>();
        var session = new GameSession();
        var solver = Mock.Of<IPasswordSolver>();
        var phase = new HackingLoopPhase(session, console, CreateSolverFactory(solver));

        var writtenLines = new List<string>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        phase.Run();

        writtenLines.ShouldContain("No candidates. Exiting.");
    }

    [Fact]
    public void Run_WhenCorrectGuess_WritesSuccessAndExits()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var console = Mock.Of<IConsoleIO>();
        var solver = Mock.Of<IPasswordSolver>();
        var terms = new Password("TERMS");
        solver.AsMock()
            .Setup(s => s.GetBestGuess(It.IsAny<IGameSession>()))
            .Returns(terms);

        console.AsMock()
            .Setup(c => c.ReadInt(0, 5, 5, It.IsAny<string>(), null))
            .Returns(5);

        var writtenLines = new List<string>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var phase = new HackingLoopPhase(session, console, CreateSolverFactory(solver));

        phase.Run();

        writtenLines.ShouldContain("Correct. Terminal cracked.");
    }

    [Fact]
    public void Run_WhenSolverReturnsNull_WritesMessageAndExits()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var console = Mock.Of<IConsoleIO>();
        var solver = Mock.Of<IPasswordSolver>();
        solver.AsMock()
            .Setup(s => s.GetBestGuess(It.IsAny<IGameSession>()))
            .Returns((Password?)null);

        var writtenLines = new List<string>();
        console.AsMock()
            .Setup(c => c.WriteLine(It.IsAny<string?>()))
            .Callback<string?>(s => writtenLines.Add(s ?? ""));

        var phase = new HackingLoopPhase(session, console, CreateSolverFactory(solver));

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

        var console = Mock.Of<IConsoleIO>();
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

        var readIntCalls = 0;
        console.AsMock()
            .Setup(c => c.ReadInt(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null))
            .Returns(() => readIntCalls++ == 0 ? 3 : 5);

        var phase = new HackingLoopPhase(session, console, CreateSolverFactory(solver));

        phase.Run();

        console.AsMock().Verify(c => c.WriteLine("Correct. Terminal cracked."), Times.Once);
    }
}
