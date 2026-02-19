using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Services;
using Enclave.Sparrow.Configuration;

namespace Enclave.Sparrow.Tests.Configuration;

/// <summary>
/// Unit tests for <see cref="SparrowIntelligence"/>.
/// </summary>
[UnitTest, TestOf(nameof(SparrowIntelligence))]
public class SparrowIntelligenceTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public void Normalize_WithValidInt_ReturnsValue(int value, int expected)
    {
        SparrowIntelligence.Normalize(value).ShouldBe(expected);
    }

    [Theory]
    [InlineData("house", 0)]
    [InlineData("bucket", 1)]
    [InlineData("tie", 2)]
    [InlineData("House", 0)]
    [InlineData("GENIUS", 2)]
    [InlineData("0", 0)]
    [InlineData("1", 1)]
    [InlineData("2", 2)]
    public void Normalize_WithAlias_ReturnsExpectedLevel(string alias, int expected)
    {
        SparrowIntelligence.Normalize(alias).ShouldBe(expected);
    }

    [Theory]
    [InlineData("baseline", 0)]
    [InlineData("tactical", 1)]
    [InlineData("optimal", 2)]
    public void Normalize_WithMilitaryAlias_ReturnsExpectedLevel(string alias, int expected)
    {
        SparrowIntelligence.Normalize(alias).ShouldBe(expected);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData(-1)]
    [InlineData(99)]
    [InlineData("null")]
    public void Normalize_WithInvalidValue_ReturnsOne(object invalid)
    {
        if(invalid.ToString() == "null") invalid = null!;
        SparrowIntelligence.Normalize(invalid).ShouldBe(SolverByIntelligence.DefaultLevel);
    }

    [Theory]
    [InlineData(0, "baseline")]
    [InlineData(1, "tactical")]
    [InlineData(2, "optimal")]
    [InlineData(99, "99")]
    public void GetDisplayName_ReturnsMilitaryAlias(int level, string expected)
    {
        SparrowIntelligence.GetDisplayName(level).ShouldBe(expected);
    }
}
