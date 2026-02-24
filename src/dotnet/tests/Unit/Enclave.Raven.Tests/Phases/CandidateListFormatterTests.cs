using Enclave.Raven.Phases;

namespace Enclave.Raven.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="CandidateListFormatter"/>. Requires InternalsVisibleTo from Enclave.Raven.
/// </summary>
[UnitTest, TestOf(nameof(CandidateListFormatter))]
public class CandidateListFormatterTests
{
    [Fact]
    public void Format_WithEmptyList_ReturnsEmptyString()
    {
        var candidates = Array.Empty<Password>();

        var result = CandidateListFormatter.Format(candidates, 5);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void Format_WithSingleWord_ReturnsWord()
    {
        var candidates = new[] { new Password("TERMS") };

        var result = CandidateListFormatter.Format(candidates, 5);

        result.ShouldBe("TERMS");
    }

    [Theory]
    [InlineData("TERMS TEXAS TANKS", 5, "TANKS  TERMS  TEXAS")]
    [InlineData("TERMS TEXAS", 5, "TERMS  TEXAS")]
    public void Format_OrdersAlphabeticallyAndUsesColumns(string words, int wordLength, string expected)
    {
        var candidates = words.Split(' ').Select(w => new Password(w)).ToList();

        var result = CandidateListFormatter.Format(candidates, wordLength);

        result.Replace("\r\n", "\n").ShouldBe(expected);
    }
}
