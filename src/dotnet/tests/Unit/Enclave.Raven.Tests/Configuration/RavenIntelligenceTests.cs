using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Services;
using Enclave.Raven.Configuration;

namespace Enclave.Raven.Tests.Configuration;

/// <summary>
/// Unit tests for <see cref="RavenIntelligence"/>.
/// </summary>
[UnitTest, TestOf(nameof(RavenIntelligence))]
public class RavenIntelligenceTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public void Normalize_WithValidInt_ReturnsValue(int value, int expected)
    {
        RavenIntelligence.Normalize(value).ShouldBe(expected);
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
        RavenIntelligence.Normalize(alias).ShouldBe(expected);
    }

    [Theory]
    [InlineData("baseline", 0)]
    [InlineData("tactical", 1)]
    [InlineData("optimal", 2)]
    public void Normalize_WithMilitaryAlias_ReturnsExpectedLevel(string alias, int expected)
    {
        RavenIntelligence.Normalize(alias).ShouldBe(expected);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData(-1)]
    [InlineData(99)]
    [InlineData("null")]
    public void Normalize_WithInvalidValue_ReturnsOne(object invalid)
    {
        if (invalid.ToString() == "null") invalid = null!;
        RavenIntelligence.Normalize(invalid).ShouldBe(SolverLevel.Default.Value);
    }

    [Theory]
    [InlineData(0, "baseline")]
    [InlineData(1, "tactical")]
    [InlineData(2, "optimal")]
    [InlineData(99, "99")]
    public void GetDisplayName_ReturnsMilitaryAlias(int level, string expected)
    {
        RavenIntelligence.GetDisplayName(level).ShouldBe(expected);
    }
}
