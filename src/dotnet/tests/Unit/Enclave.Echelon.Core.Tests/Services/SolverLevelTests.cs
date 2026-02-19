using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="SolverLevel"/>.
/// </summary>
[UnitTest, TestOf(nameof(SolverLevel))]
public class SolverLevelTests
{
    public static IEnumerable<object?[]> FromIntValidTestData =>
        new (int, SolverLevel)[] { (0, SolverLevel.HouseGambit), (1, SolverLevel.BestBucket), (2, SolverLevel.TieBreaker) }.ToTestData();

    [Theory]
    [MemberData(nameof(FromIntValidTestData))]
    public void FromInt_WithValidValue_ReturnsExpectedLevel(int value, SolverLevel expected)
    {
        SolverLevel.FromInt(value).ShouldBe(expected);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(99)]
    public void FromInt_WithInvalidValue_ReturnsDefault(int value)
    {
        SolverLevel.FromInt(value).ShouldBe(SolverLevel.Default);
    }

    [Theory]
    [MemberData(nameof(FromIntValidTestData))]
    public void FromValue_WithValidValue_ReturnsCanonicalLevel(int value, SolverLevel expected)
    {
        SolverLevel.FromValue(value).ShouldBe(expected);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(99)]
    public void FromValue_WithInvalidValue_ReturnsDefault(int value)
    {
        SolverLevel.FromValue(value).ShouldBe(SolverLevel.Default);
    }

    public static IEnumerable<object?[]> TryParseValidTestData =>
        new (string, SolverLevel)[]
        {
            ("0", SolverLevel.HouseGambit),
            ("1", SolverLevel.BestBucket),
            ("2", SolverLevel.TieBreaker),
            ("HouseGambit", SolverLevel.HouseGambit),
            ("housegambit", SolverLevel.HouseGambit),
            ("BestBucket", SolverLevel.BestBucket),
            ("TieBreaker", SolverLevel.TieBreaker),
            ("house", SolverLevel.HouseGambit),
            ("dumb", SolverLevel.HouseGambit),
            ("baseline", SolverLevel.HouseGambit),
            ("random", SolverLevel.HouseGambit),
            ("smart", SolverLevel.BestBucket),
            ("bucket", SolverLevel.BestBucket),
            ("tactical", SolverLevel.BestBucket),
            ("genius", SolverLevel.TieBreaker),
            ("tie", SolverLevel.TieBreaker),
            ("optimal", SolverLevel.TieBreaker),
        }.ToTestData();

    [Theory]
    [MemberData(nameof(TryParseValidTestData))]
    public void TryParse_WithValidString_ReturnsTrueAndExpectedLevel(string input, SolverLevel expected)
    {
        SolverLevel.TryParse(input, out var level).ShouldBeTrue();
        level.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("invalid")]
    [InlineData("99")]
    public void TryParse_WithInvalidString_ReturnsFalse(string? input)
    {
        SolverLevel.TryParse(input, out _).ShouldBeFalse();
    }

    [Fact]
    public void ToString_Default_ReturnsMilitaryAlias()
    {
        SolverLevel.HouseGambit.ToString().ShouldBe("baseline");
        SolverLevel.BestBucket.ToString().ShouldBe("tactical");
        SolverLevel.TieBreaker.ToString().ShouldBe("optimal");
    }

    [Fact]
    public void ToString_EmptyPrefix_ReturnsNoPrefixAlias()
    {
        SolverLevel.HouseGambit.ToString("").ShouldBe("house");
        SolverLevel.BestBucket.ToString("").ShouldBe("smart");
        SolverLevel.TieBreaker.ToString("").ShouldBe("genius");
    }

    [Fact]
    public void ToString_MilitaryPrefix_ReturnsBaselineTacticalOptimal()
    {
        SolverLevel.HouseGambit.ToString("military").ShouldBe("baseline");
        SolverLevel.BestBucket.ToString("military").ShouldBe("tactical");
        SolverLevel.TieBreaker.ToString("military").ShouldBe("optimal");
    }

    [Fact]
    public void ToString_AlgorithmPrefix_ReturnsRandomBucketTie()
    {
        SolverLevel.HouseGambit.ToString("algorithm").ShouldBe("random");
        SolverLevel.BestBucket.ToString("algorithm").ShouldBe("bucket");
        SolverLevel.TieBreaker.ToString("algorithm").ShouldBe("tie");
    }

    [Fact]
    public void ToString_UnknownPrefix_ReturnsLevelName()
    {
        SolverLevel.HouseGambit.ToString("unknown").ShouldBe("HouseGambit");
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        SolverLevel.HouseGambit.Equals(SolverLevel.HouseGambit).ShouldBeTrue();
        SolverLevel.HouseGambit.Equals(SolverLevel.FromInt(0)).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        SolverLevel.HouseGambit.Equals(SolverLevel.BestBucket).ShouldBeFalse();
        SolverLevel.BestBucket.Equals(SolverLevel.TieBreaker).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        SolverLevel.HouseGambit.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void Equals_NonSolverLevel_ReturnsFalse()
    {
        SolverLevel.HouseGambit.Equals(0).ShouldBeFalse();
        SolverLevel.HouseGambit.Equals("HouseGambit").ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHashCode()
    {
        SolverLevel.HouseGambit.GetHashCode().ShouldBe(SolverLevel.FromInt(0).GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ReturnDifferentHashCodes()
    {
        var hashes = new[] { SolverLevel.HouseGambit, SolverLevel.BestBucket, SolverLevel.TieBreaker }
            .Select(l => l.GetHashCode())
            .ToHashSet();
        hashes.Count.ShouldBe(3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void ImplicitConversion_ToInt_ReturnsValue(int value)
    {
        SolverLevel level = SolverLevel.FromInt(value);
        int n = level;
        n.ShouldBe(value);
    }

    [Fact]
    public void ImplicitConversion_FromInt_InvalidReturnsDefault()
    {
        SolverLevel level = 99;
        level.ShouldBe(SolverLevel.Default);
    }
}
