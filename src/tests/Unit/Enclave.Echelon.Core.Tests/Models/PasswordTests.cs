using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Tests.Models;

/// <summary>
/// Unit tests for <see cref="Password"/>.
/// </summary>
[UnitTest, TestOf(nameof(Password))]
public class PasswordTests
{
    #region Constructor Tests
    [Theory]
    [InlineData("terms")]
    [InlineData("texas")]
    [InlineData("A")]
    [InlineData("TERMS")]
    [InlineData("RELEASED")]
    public void Constructor_WithValidWord_StoresUppercaseWord(string word)
    {
        // Arrange
        var expected = word.ToUpperInvariant();

        // Act
        var password = new Password(word);

        // Assert
        password.Word.ShouldBe(expected);
        password.IsEliminated.ShouldBeFalse();
    }

    [Fact]
    public void Constructor_WithNullWord_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentNullException>(() => new Password(null!));
        ex.ParamName.ShouldBe("word");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_WithEmptyWord_ThrowsArgumentException(string empty)
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => new Password(empty));
        ex.ParamName.ShouldBe("word");
    }

    [Theory]
    [InlineData("TERM1")]
    [InlineData("TERM-S")]
    [InlineData("TERM S")]
    [InlineData("TERM.S")]
    public void Constructor_WithNonLetterCharacters_ThrowsArgumentException(string word)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new Password(word)).ParamName.ShouldBe("word");
    }
    #endregion Constructor Tests

    #region GetMatchCount Tests
    [Theory]
    [InlineData("TERMS")]
    [InlineData("TEXAS")]
    public void GetMatchCount_WithSameWord_ReturnsFullLength(string word)
    {
        // Arrange
        var p = new Password(word);

        // Act
        var count = p.GetMatchCount(p);

        // Assert
        count.ShouldBe(word.Length);
    }

    [Theory]
    [InlineData("TERMS", "TEXAS", 3)]
    [InlineData("ABCD", "WXYZ", 0)]
    public void GetMatchCount_WithDifferentWord_ReturnsCorrectCount(string word1, string word2, int expectedCount)
    {
        // Arrange
        var p1 = new Password(word1);
        var p2 = new Password(word2);
        // Act
        var count = p1.GetMatchCount(p2);

        // Assert
        count.ShouldBe(expectedCount); // T(0), E(1), S(4)
    }

    [Theory]
    [InlineData("TERMS")]
    [InlineData("TEXAS")]
    public void GetMatchCount_WithNullOther_ThrowsArgumentNullException(string word)
    {
        // Arrange
        var p = new Password(word);

        // Act & Assert
        var ex = Should.Throw<ArgumentNullException>(() => p.GetMatchCount(null!));
        ex.ParamName.ShouldBe("other");
    }

    [Theory]
    [InlineData("TERMS", "TEXAS")]
    [InlineData("RELEASED", "DIRECTOR")]
    public void GetMatchCount_IsSymmetricAndCached(string word1, string word2)
    {
        // Arrange
        var p1 = new Password(word1);
        var p2 = new Password(word2);

        // Act
        var count1 = p1.GetMatchCount(p2);
        var count2 = p2.GetMatchCount(p1);

        // Assert
        count1.ShouldBe(count2);
        p1.HasCachedMatchCount(p2).ShouldBeTrue();
        p2.HasCachedMatchCount(p1).ShouldBeTrue();
    }

    [Theory]
    [InlineData("TERMS", "TEXAS")]
    [InlineData("RELEASED", "DIRECTOR")]
    public void HasCachedMatchCount_BeforeGetMatchCount_ReturnsFalse(string word1, string word2)
    {
        // Arrange
        var p1 = new Password(word1);
        var p2 = new Password(word2);

        // Assert
        p1.HasCachedMatchCount(p2).ShouldBeFalse();
        p2.HasCachedMatchCount(p1).ShouldBeFalse();
    }

    [Theory]
    [InlineData("TERMS", "TEXAS")]
    [InlineData("RELEASED", "DIRECTOR")]
    public void HasCachedMatchCount_AfterGetMatchCount_ReturnsTrue(string word1, string word2)
    {
        // Arrange
        var p1 = new Password(word1);
        var p2 = new Password(word2);
        p1.GetMatchCount(p2);

        // Assert
        p1.HasCachedMatchCount(p2).ShouldBeTrue();
        p2.HasCachedMatchCount(p1).ShouldBeTrue();
    }

    [Fact]
    public void CacheSize_IncreasesAfterGetMatchCount()
    {
        // Arrange
        var p1 = new Password("TERMS");
        var p2 = new Password("TEXAS");
        var p3 = new Password("TANKS");

        // Act
        p1.GetMatchCount(p2);
        p1.GetMatchCount(p3);

        // Assert
        p1.CacheSize.ShouldBe(2);
    }
    #endregion GetMatchCount Tests

    #region Equality & GetHascode Tests
    [Theory]
    [InlineData("TERMS")]
    [InlineData("TEXAS")]
    [InlineData("tanks")]
    public void ToString_ReturnsWord(string word)
    {
        // Arrange
        var expected = word.ToUpperInvariant();
        var p = new Password(word);

        // Assert
        p.ToString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("TERMS")]
    [InlineData("TEXAS")]
    public void Equals_WithSameWord_ReturnsTrue(string word)
    {
        // Arrange
        var p1 = new Password(word.ToUpperInvariant());
        var p2 = new Password(word.ToLowerInvariant());

        // Assert
        p1.Equals(p2).ShouldBeTrue();
        p1.Equals((object)p2).ShouldBeTrue();
    }

    [Theory]
    [InlineData("TERMS", "TEXAS")]
    [InlineData("RELEASED", "DIRECTOR")]
    public void Equals_WithDifferentWord_ReturnsFalse(string word1, string word2)
    {
        // Arrange
        var p1 = new Password(word1);
        var p2 = new Password(word2);

        // Assert
        p1.Equals(p2).ShouldBeFalse();
        p1.Equals((object?)null).ShouldBeFalse();
        p1.Equals(word1).ShouldBeFalse();
    }

    [Theory]
    [InlineData("TERMS")]
    [InlineData("TEXAS")]
    public void GetHashCode_IsConsistentForSameWord(string word)
    {
        // Arrange
        var p1 = new Password(word.ToUpperInvariant());
        var p2 = new Password(word.ToLowerInvariant());

        // Assert
        p1.GetHashCode().ShouldBe(p2.GetHashCode());
    }
    #endregion Equality & GetHascode Tests

    #region Subtraction Tests
    [Theory]
    [InlineData("TERMS", "TEXAS", 2)]
    [InlineData("ABCD", "WXYZ", 4)]
    [InlineData("null", "WXYZ", 4)]
    [InlineData("null", "TEXAS", 5)]
    [InlineData("ABCD", "null", 4)]
    [InlineData("TEXAS", "null", 5)]
    public void SubtractionOperator_ReturnsNonMatchingCharacterCount(string word1, string word2, int expected)
    {
        // Arrange
        var p1 = word1 == "null" ? null! : new Password(word1);
        var p2 = word2 == "null" ? null! : new Password(word2);
        // Act
        var diff = p1 - p2;

        // Assert
        diff.ShouldBe(expected); // max 5 - match 3 (T, E, S)
    }
    #endregion Subtraction Tests

    #region Implicit Operator Tests
    [Theory]
    [InlineData("TERMS")]
    [InlineData("TEXAS")]
    [InlineData("tanks")]
    public void ImplicitOperator_ToString_ReturnsWord(string word)
    {
        // Arrange
        var p = new Password(word);

        // Act
        string s = p;

        // Assert
        s.ShouldBe(word.ToUpperInvariant());
    }

    [Theory]
    [InlineData("TERMS")]
    [InlineData("TEXAS")]
    [InlineData("tanks")]
    public void ImplicitOperator_FromString_CreatesPassword(string word)
    {
        // Act
        Password p = word;

        // Assert
        p.Word.ShouldBe(word.ToUpperInvariant());
    }
    #endregion Implicit Operator Tests
}
