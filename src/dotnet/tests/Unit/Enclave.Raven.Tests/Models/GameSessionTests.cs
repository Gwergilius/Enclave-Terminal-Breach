using Enclave.Common.Errors;
using Enclave.Echelon.Core.Errors;
using Enclave.Raven.Errors;

namespace Enclave.Raven.Tests.Models;

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
        var session = new GameSession();

        var result = session.Add(word);

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
        var session = new GameSession();

        var result = session.Add(word);

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is InvalidPassword);
        session.Count.ShouldBe(0);
    }

    [Fact]
    public void Add_WithDifferentLengthWord_AfterFirst_ReturnsInvalidPasswordError()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var result = session.Add("TANK");

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is InvalidPassword);
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Add_WithDuplicateWord_ReturnsDuplicatedPasswordError()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var result = session.Add("terms");

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is DuplicatedPassword);
        session.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData("TERMS", "TEXAS", "TANKS")]
    public void Add_WithMultipleValidWords_AddsAll(string w1, string w2, string w3)
    {
        var session = new GameSession();

        session.Add(w1).IsSuccess.ShouldBeTrue();
        session.Add(w2).IsSuccess.ShouldBeTrue();
        session.Add(w3).IsSuccess.ShouldBeTrue();

        session.Count.ShouldBe(3);
        session.WordLength.ShouldBe(5);
    }

    #endregion

    #region Remove(string) Tests

    [Fact]
    public void Remove_WhenWordExists_ReturnsOk()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var result = session.Remove("TERMS");

        result.IsSuccess.ShouldBeTrue();
        session.Count.ShouldBe(0);
        session.WordLength.ShouldBeNull();
    }

    [Fact]
    public void Remove_WhenWordNotInList_ReturnsNotFoundError()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var result = session.Remove("TEXAS");

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Remove_WhenEmptyWord_ReturnsFailure()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var result = session.Remove("");

        result.IsFailed.ShouldBeTrue();
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Remove_IsCaseInsensitive()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        var result = session.Remove("terms");

        result.IsSuccess.ShouldBeTrue();
        session.Count.ShouldBe(0);
    }

    #endregion

    #region Add(Password) / Remove(Password) / IList Tests

    [Fact]
    public void Add_Password_WhenValid_AddsToList()
    {
        var session = new GameSession();
        var p = new Password("TERMS");

        session.Add(p);

        session.Count.ShouldBe(1);
        session[0].Word.ShouldBe("TERMS");
    }

    [Fact]
    public void Add_Password_WhenLengthMismatch_ThrowsInvalidOperationException()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TANK");

        var ex = Should.Throw<InvalidOperationException>(() => session.Add(p));
        ex.Message.ShouldContain("Word length");
    }

    [Fact]
    public void Add_Password_WhenDuplicate_ThrowsInvalidOperationException()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TERMS");

        var ex = Should.Throw<InvalidOperationException>(() => session.Add(p));
        ex.Message.ShouldContain("Already in list");
    }

    [Fact]
    public void Remove_Password_WhenExists_ReturnsTrue()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TERMS");

        var removed = session.Remove(p);

        removed.ShouldBeTrue();
        session.Count.ShouldBe(0);
    }

    [Fact]
    public void Remove_Password_WhenNotExists_ReturnsFalse()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TEXAS");

        var removed = session.Remove(p);

        removed.ShouldBeFalse();
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Clear_RemovesAllAndResetsWordLength()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();

        session.Clear();

        session.Count.ShouldBe(0);
        session.WordLength.ShouldBeNull();
    }

    [Fact]
    public void Indexer_Get_ReturnsPassword()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();

        session[0].Word.ShouldBe("TERMS");
    }

    #endregion

    #region IList Implementation Tests

    [Fact]
    public void Indexer_Set_WhenValidIndex_ReplacesPassword()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();
        var replacement = new Password("TANKS");

        session[1] = replacement;

        session[1].Word.ShouldBe("TANKS");
        session.Count.ShouldBe(2);
    }

    [Fact]
    public void Indexer_Set_WhenIndexZero_ReplacesPassword()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var replacement = new Password("TEXAS");

        session[0] = replacement;

        session[0].Word.ShouldBe("TEXAS");
        session.Count.ShouldBe(1);
    }

    [Fact]
    public void Indexer_Set_WhenIndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var replacement = new Password("TEXAS");

        var ex = Should.Throw<ArgumentOutOfRangeException>(() => session[5] = replacement);
        ex.ParamName.ShouldBe("index");
    }

    [Fact]
    public void Indexer_Set_WhenValueNull_ThrowsArgumentNullException()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();

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
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TERMS");

        session.Contains(p).ShouldBeTrue();
    }

    [Fact]
    public void Contains_WhenItemNotExists_ReturnsFalse()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TEXAS");

        session.Contains(p).ShouldBeFalse();
    }

    [Fact]
    public void Contains_IsCaseInsensitive()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("terms");

        session.Contains(p).ShouldBeTrue();
    }

    [Fact]
    public void IndexOf_WhenItemExists_ReturnsIndex()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();
        var p = new Password("TEXAS");

        var index = session.IndexOf(p);

        index.ShouldBe(1);
    }

    [Fact]
    public void IndexOf_WhenItemNotExists_ReturnsMinusOne()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("TEXAS");

        var index = session.IndexOf(p);

        index.ShouldBe(-1);
    }

    [Fact]
    public void IndexOf_IsCaseInsensitive()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var p = new Password("terms");

        var index = session.IndexOf(p);

        index.ShouldBe(0);
    }

    [Fact]
    public void RemoveAt_IsNotAllowed_ThrowsNotImplementedException()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();

        Should.Throw<NotImplementedException>(() => session.RemoveAt(0));
    }

    [Fact]
    public void Insert_IsNotAllowed_ThrowsNotImplementedException()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        var item = new Password("TEXAS");

        Should.Throw<NotImplementedException>(() => session.Insert(0, item));
    }

    [Fact]
    public void GetEnumerator_IteratesAllItems()
    {
        var session = new GameSession();
        session.Add("TERMS").IsSuccess.ShouldBeTrue();
        session.Add("TEXAS").IsSuccess.ShouldBeTrue();
        var words = new List<string>();

        foreach (var p in session)
        {
            words.Add(p.Word);
        }

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

    #endregion
}
