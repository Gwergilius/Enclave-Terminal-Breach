using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="GameRandom"/>.
/// </summary>
[UnitTest, TestOf(nameof(GameRandom))]
public class GameRandomTests
{
    private const int Seed = 42;

    [Fact]
    public void DefaultConstructor_InstantiatesAndProducesValuesInExpectedRanges()
    {
        var r = new GameRandom();

        double d = r.Next();
        d.ShouldBeInRange(0.0, 1.0);

        int n = r.Next(10, 20);
        n.ShouldBeInRange(10, 19);
    }

    [Fact]
    public void Next_WithSeed_ReturnsDeterministicDoubleInZeroToOne()
    {
        var r = new GameRandom(Seed);

        double a = r.Next();
        double b = r.Next();

        a.ShouldBeInRange(0.0, 1.0);
        b.ShouldBeInRange(0.0, 1.0);
        new GameRandom(Seed).Next().ShouldBe(a);
    }

    [Fact]
    public void Next_WithSeedAndMinMax_ReturnsDeterministicIntInRange()
    {
        var r = new GameRandom(Seed);

        int value = r.Next(5, 15);

        value.ShouldBeInRange(5, 14);
        new GameRandom(Seed).Next(5, 15).ShouldBe(value);
    }
}
