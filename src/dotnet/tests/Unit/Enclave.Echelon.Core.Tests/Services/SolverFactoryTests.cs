using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="SolverFactory"/>.
/// </summary>
[UnitTest, TestOf(nameof(SolverFactory))]
public class SolverFactoryTests
{
    private static IRandom Random { get; } = new GameRandom(17);

    private static IEnumerable<IPasswordSolver> CreateSolvers() =>
    [
        new HouseGambitPasswordSolver(Random),
        new BestBucketPasswordSolver(Random),
        new TieBreakerPasswordSolver(),
    ];

    private static SolverFactory CreateFactory(SolverLevel level)
    {
        var config = new StubSolverConfiguration(level);
        return new SolverFactory(CreateSolvers(), config);
    }

    private sealed class StubSolverConfiguration(SolverLevel level) : ISolverConfiguration
    {
        public SolverLevel Level => level;
    }

    [Fact]
    public void GetSolver_WithHouseGambit_ReturnsHouseGambitPasswordSolver()
    {
        var factory = CreateFactory(SolverLevel.HouseGambit);

        var solver = factory.GetSolver();

        solver.ShouldBeOfType<HouseGambitPasswordSolver>();
        solver.Level.ShouldBe(SolverLevel.HouseGambit);
    }

    [Fact]
    public void GetSolver_WithBestBucket_ReturnsBestBucketPasswordSolver()
    {
        var factory = CreateFactory(SolverLevel.BestBucket);

        var solver = factory.GetSolver();

        solver.ShouldBeOfType<BestBucketPasswordSolver>();
        solver.Level.ShouldBe(SolverLevel.BestBucket);
    }

    [Fact]
    public void GetSolver_WithTieBreaker_ReturnsTieBreakerPasswordSolver()
    {
        var factory = CreateFactory(SolverLevel.TieBreaker);

        var solver = factory.GetSolver();

        solver.ShouldBeOfType<TieBreakerPasswordSolver>();
        solver.Level.ShouldBe(SolverLevel.TieBreaker);
    }

    [Fact]
    public void GetSolver_WithInvalidLevel_ReturnsDefaultLevelSolver()
    {
        var config = new StubSolverConfiguration(SolverLevel.FromValue(99));
        var factory = new SolverFactory(CreateSolvers(), config);

        var solver = factory.GetSolver();

        solver.ShouldBeOfType<BestBucketPasswordSolver>();
    }

    [Fact]
    public void GetSolver_ReturnsSameInstance_WhenCalledMultipleTimes()
    {
        var factory = CreateFactory(SolverLevel.BestBucket);

        var a = factory.GetSolver();
        var b = factory.GetSolver();

        a.ShouldBeSameAs(b);
    }
}
