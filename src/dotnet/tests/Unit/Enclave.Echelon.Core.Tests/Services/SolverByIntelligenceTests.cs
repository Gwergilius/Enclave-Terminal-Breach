using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="SolverByIntelligence"/>.
/// </summary>
[UnitTest, TestOf(nameof(SolverByIntelligence))]
public class SolverByIntelligenceTests
{
    private const int Seed = 17;

    [Fact]
    public void GetSolver_WithLevel0_ReturnsHouseGambitPasswordSolver()
    {
        var solver = SolverByIntelligence.GetSolver(0, Seed);

        solver.ShouldBeOfType<HouseGambitPasswordSolver>();
    }

    [Fact]
    public void GetSolver_WithLevel1_ReturnsBestBucketPasswordSolver()
    {
        var solver = SolverByIntelligence.GetSolver(1, Seed);

        solver.ShouldBeOfType<BestBucketPasswordSolver>();
    }

    [Fact]
    public void GetSolver_WithLevel2_ReturnsTieBreakerPasswordSolver()
    {
        var solver = SolverByIntelligence.GetSolver(2, Seed);

        solver.ShouldBeOfType<TieBreakerPasswordSolver>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(99)]
    public void GetSolver_WithInvalidLevel_ReturnsDefaultLevelSolver(int invalidLevel)
    {
        var solver = SolverByIntelligence.GetSolver(invalidLevel, Seed);

        solver.ShouldBeOfType<BestBucketPasswordSolver>();
    }
}
