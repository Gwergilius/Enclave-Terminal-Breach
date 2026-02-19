using Enclave.Sparrow.Phases;

namespace Enclave.Sparrow.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="CandidateListFormatter"/>. Requires InternalsVisibleTo from Enclave.Sparrow.
/// </summary>
[UnitTest, TestOf(nameof(CandidateListFormatter))]
public class CandidateListFormatterTests
{
    [Fact]
    public void Format_WithEmptyList_ReturnsEmptyString()
    {
        // Arrange
        var candidates = Array.Empty<Password>();

        // Act
        var result = CandidateListFormatter.Format(candidates, 5);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void Format_WithSingleWord_ReturnsWord()
    {
        // Arrange
        var candidates = new[] { new Password("TERMS") };

        // Act
        var result = CandidateListFormatter.Format(candidates, 5);

        // Assert
        result.ShouldBe("TERMS");
    }

    [Theory]
    [InlineData("TERMS TEXAS TANKS", 5, "TANKS  TERMS  TEXAS")]
    [InlineData("TERMS TEXAS", 5, "TERMS  TEXAS")]
    public void Format_OrdersAlphabeticallyAndUsesColumns(string words, int wordLength, string expected)
    {
        // Arrange: column count = wordLength, so 5 columns for length-5 words
        var candidates = words.Split(' ').Select(w => new Password(w)).ToList();

        // Act
        var result = CandidateListFormatter.Format(candidates, wordLength);

        // Assert: row-major, sorted alphabetically
        result.Replace("\r\n", "\n").ShouldBe(expected);
    }
}
