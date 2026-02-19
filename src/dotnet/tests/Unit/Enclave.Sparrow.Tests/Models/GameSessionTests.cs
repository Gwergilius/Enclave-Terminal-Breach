using Enclave.Common.Errors;
using Enclave.Echelon.Core.Errors;
using Enclave.Sparrow.Errors;

namespace Enclave.Sparrow.Tests.Models;

/// <summary>
/// Unit tests for <see cref="GameSession"/>. Tests public API; private logic (AddCandidate, RemoveCandidate) is exercised through Add/Remove.
/// </summary>
[UnitTest, TestOf(nameof(GameSession))]
public class GameSessionTests
{
    #region Add(string) Tests

    [Theory]
    [InlineData("TERMS")]
    [InlineData("TEXAS")]
    [InlineData("tanks")]
    public void Add_WithValidWord_AddsAndSetsWordLength(string word)
    {
        // Arrange
        var session = new GameSession();

        // Act
        var result = session.Add(word);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        session.Count.ShouldBe(1);
        session.WordLength.ShouldBe(word.Length);
        session[0].Word.ShouldBe(word.ToUpperInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TERM1")]
    public void Add_WithInvalidWord_ReturnsInvalidPasswordError(string word)
    {
        // Arrange
        var session = new GameSession();

        // Act
        var result = session.Add(word);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is InvalidPassword);
        session.Count.ShouldBe(0);
    }

    [Fact]
    public void Add_WithDifferentLengthWord_AfterFirst_ReturnsInvalidPasswordError()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        // Act
        var result = session.Add("TANK");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is InvalidPassword);
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Add_WithDuplicateWord_ReturnsDuplicatedPasswordError()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        // Act
        var result = session.Add("terms");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is DuplicatedPassword);
        session.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData("TERMS", "TEXAS", "TANKS")]
    public void Add_WithMultipleValidWords_AddsAll(string w1, string w2, string w3)
    {
        // Arrange
        var session = new GameSession();

        // Act
        session.Add(w1).IsSuccess.ShouldBeTrue();
        session.Add(w2).IsSuccess.ShouldBeTrue();
        session.Add(w3).IsSuccess.ShouldBeTrue();

        // Assert
        session.Count.ShouldBe(3);
        session.WordLength.ShouldBe(5);
    }

    #endregion Add(string) Tests

    #region Remove(string) Tests

    [Fact]
    public void Remove_WhenWordExists_ReturnsOk()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        // Act
        var result = session.Remove("TERMS");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        session.Count.ShouldBe(0);
        session.WordLength.ShouldBeNull();
    }

    [Fact]
    public void Remove_WhenWordNotInList_ReturnsNotFoundError()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        // Act
        var result = session.Remove("TEXAS");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Remove_WhenEmptyWord_ReturnsFailure()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        // Act
        var result = session.Remove("");

        // Assert
        result.IsFailed.ShouldBeTrue();
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Remove_IsCaseInsensitive()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        // Act
        var result = session.Remove("terms");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        session.Count.ShouldBe(0);
    }

    #endregion Remove(string) Tests

    #region Add(Password) / Remove(Password) / IList Tests

    [Fact]
    public void Add_Password_WhenValid_AddsToList()
    {
        // Arrange
        var session = new GameSession();
        var p = new Password("TERMS");

        // Act
        session.Add(p);

        // Assert
        session.Count.ShouldBe(1);
        session[0].Word.ShouldBe("TERMS");
    }

    [Fact]
    public void Add_Password_WhenLengthMismatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TANK");

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => session.Add(p));
        ex.Message.ShouldContain("Word length");
    }

    [Fact]
    public void Add_Password_WhenDuplicate_ThrowsInvalidOperationException()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TERMS");

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => session.Add(p));
        ex.Message.ShouldContain("Already in list");
    }

    [Fact]
    public void Remove_Password_WhenExists_ReturnsTrue()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TERMS");

        // Act
        var removed = session.Remove(p);

        // Assert
        removed.ShouldBeTrue();
        session.Count.ShouldBe(0);
    }

    [Fact]
    public void Remove_Password_WhenNotExists_ReturnsFalse()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TEXAS");

        // Act
        var removed = session.Remove(p);

        // Assert
        removed.ShouldBeFalse();
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Clear_RemovesAllAndResetsWordLength()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();

        // Act
        session.Clear();

        // Assert
        session.Count.ShouldBe(0);
        session.WordLength.ShouldBeNull();
    }

    [Fact]
    public void Indexer_Get_ReturnsPassword()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        // Act & Assert
        session[0].Word.ShouldBe("TERMS");
    }

    #endregion Add(Password) / Remove(Password) / IList Tests

    #region IList Implementation Tests

    [Fact]
    public void Indexer_Set_WhenValidIndex_ReplacesPassword()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();
        var replacement = new Password("TANKS");

        // Act
        session[1] = replacement;

        // Assert
        session[1].Word.ShouldBe("TANKS");
        session.Count.ShouldBe(2);
    }

    [Fact]
    public void Indexer_Set_WhenIndexZero_ReplacesPassword()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var replacement = new Password("TEXAS");

        // Act
        session[0] = replacement;

        // Assert
        session[0].Word.ShouldBe("TEXAS");
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Indexer_Set_WhenIndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var replacement = new Password("TEXAS");

        // Act & Assert
        var ex = Should.Throw<ArgumentOutOfRangeException>(() => session[5] = replacement);
        ex.ParamName.ShouldBe("index");
    }

    [Fact]
    public void Indexer_Set_WhenValueNull_ThrowsArgumentNullException()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();

        // Act & Assert
        var ex = Should.Throw<ArgumentNullException>(() => session[1] = null!);
        ex.ParamName.ShouldBe("value");
    }

    [Fact]
    public void IsReadOnly_ReturnsFalse()
    {
        var session = new GameSession();
        session.IsReadOnly.ShouldBeFalse();
    }

    [Fact]
    public void Contains_WhenItemExists_ReturnsTrue()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TERMS");

        // Act & Assert
        session.Contains(p).ShouldBeTrue();
    }

    [Fact]
    public void Contains_WhenItemNotExists_ReturnsFalse()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TEXAS");

        // Act & Assert
        session.Contains(p).ShouldBeFalse();
    }

    [Fact]
    public void Contains_IsCaseInsensitive()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("terms");

        // Act & Assert
        session.Contains(p).ShouldBeTrue();
    }

    [Fact]
    public void IndexOf_WhenItemExists_ReturnsIndex()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();
        var p = new Password("TEXAS");

        // Act
        var index = session.IndexOf(p);

        // Assert
        index.ShouldBe(1);
    }

    [Fact]
    public void IndexOf_WhenItemNotExists_ReturnsMinusOne()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TEXAS");

        // Act
        var index = session.IndexOf(p);

        // Assert
        index.ShouldBe(-1);
    }

    [Fact]
    public void IndexOf_IsCaseInsensitive()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("terms");

        // Act
        var index = session.IndexOf(p);

        // Assert
        index.ShouldBe(0);
    }

    [Fact]
    public void RemoveAt_IsNotAllowed_ThrowsNotImplementedException()
    {
        // Arrange: GameSession implements IList<Password> but RemoveAt is not supported; use Remove(string) to remove by word.
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();

        // Act & Assert
        Should.Throw<NotImplementedException>(() => session.RemoveAt(0));
    }

    [Fact]
    public void Insert_IsNotAllowed_ThrowsNotImplementedException()
    {
        // Arrange: GameSession implements IList<Password> but Insert is not supported (candidates are added only via Add).
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var item = new Password("TEXAS");

        // Act & Assert
        Should.Throw<NotImplementedException>(() => session.Insert(0, item));
    }

    [Fact]
    public void GetEnumerator_IteratesAllItems()
    {
        // Arrange
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();
        var words = new List<string>();

        // Act
        foreach (var p in session)
        {
            words.Add(p.Word);
        }

        // Assert
        words.ShouldBe(["TERMS", "TEXAS"]);
    }

    [Fact]
    public void GetEnumerator_WhenEmpty_ReturnsNoItems()
    {
        var session = new GameSession();
        var count = 0;
        foreach (var _ in session)
        {
            count++;
        }
        count.ShouldBe(0);
    }

    #endregion IList Implementation Tests
}
