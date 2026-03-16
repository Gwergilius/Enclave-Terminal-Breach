using Enclave.Common.Test.Core;
using Enclave.Raven.Screens.DataInput;

namespace Enclave.Raven.Tests.Screens.DataInput;

[UnitTest, TestOf(nameof(DataInputLogic))]
public class DataInputLogicTests
{
    // --- GetTokens ---

    [Fact]
    public void GetTokens_WithSingleWord_ReturnsThatWord()
    {
        DataInputLogic.GetTokens("HELLO").ShouldBe(["HELLO"]);
    }

    [Fact]
    public void GetTokens_SplitsOnWhitespace()
    {
        DataInputLogic.GetTokens("HELLO WORLD").ShouldBe(["HELLO", "WORLD"]);
    }

    [Fact]
    public void GetTokens_TrimsLeadingAndTrailingSpaces()
    {
        DataInputLogic.GetTokens("  HELLO  WORLD  ").ShouldBe(["HELLO", "WORLD"]);
    }

    [Fact]
    public void GetTokens_WithEmptyString_ReturnsEmpty()
    {
        DataInputLogic.GetTokens(string.Empty).ShouldBeEmpty();
    }

    [Fact]
    public void GetTokens_WithWhitespaceOnly_ReturnsEmpty()
    {
        DataInputLogic.GetTokens("   ").ShouldBeEmpty();
    }

    // --- ApplyLine: add ---

    [Fact]
    public void ApplyLine_WithSingleWord_AddsToSession()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().Setup(s => s.Add("HELLO")).Returns(Result.Ok());

        var error = DataInputLogic.ApplyLine("HELLO", session);

        session.AsMock().Verify(s => s.Add("HELLO"), Times.Once);
        error.ShouldBeNull();
    }

    [Fact]
    public void ApplyLine_WithMultipleWords_AddsAll()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().Setup(s => s.Add(It.IsAny<string>())).Returns(Result.Ok());

        DataInputLogic.ApplyLine("ALPHA BETA GAMMA", session);

        session.AsMock().Verify(s => s.Add("ALPHA"), Times.Once);
        session.AsMock().Verify(s => s.Add("BETA"),  Times.Once);
        session.AsMock().Verify(s => s.Add("GAMMA"), Times.Once);
    }

    [Fact]
    public void ApplyLine_WhenAddFails_ReturnsErrorMessage()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().Setup(s => s.Add(It.IsAny<string>())).Returns(Result.Fail("length mismatch"));

        var error = DataInputLogic.ApplyLine("BAD", session);

        error.ShouldBe("length mismatch");
    }

    // --- ApplyLine: remove ---

    [Fact]
    public void ApplyLine_WithMinusPrefix_RemovesWordWithoutPrefix()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().Setup(s => s.Remove("HELLO")).Returns(Result.Ok());

        var error = DataInputLogic.ApplyLine("-HELLO", session);

        session.AsMock().Verify(s => s.Remove("HELLO"), Times.Once);
        session.AsMock().Verify(s => s.Add(It.IsAny<string>()), Times.Never);
        error.ShouldBeNull();
    }

    [Fact]
    public void ApplyLine_WhenRemoveFails_ReturnsErrorMessage()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().Setup(s => s.Remove(It.IsAny<string>())).Returns(Result.Fail("not found"));

        var error = DataInputLogic.ApplyLine("-MISSING", session);

        error.ShouldBe("not found");
    }

    // --- ApplyLine: mixed ---

    [Fact]
    public void ApplyLine_WithMixedAddAndRemove_ProcessesAll()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().Setup(s => s.Add(It.IsAny<string>())).Returns(Result.Ok());
        session.AsMock().Setup(s => s.Remove(It.IsAny<string>())).Returns(Result.Ok());

        DataInputLogic.ApplyLine("HELLO -WORLD FOO", session);

        session.AsMock().Verify(s => s.Add("HELLO"),    Times.Once);
        session.AsMock().Verify(s => s.Remove("WORLD"), Times.Once);
        session.AsMock().Verify(s => s.Add("FOO"),      Times.Once);
    }

    [Fact]
    public void ApplyLine_WithMultipleFailures_ReturnsLastErrorMessage()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().Setup(s => s.Add("FIRST")).Returns(Result.Fail("first error"));
        session.AsMock().Setup(s => s.Add("SECOND")).Returns(Result.Fail("second error"));

        var error = DataInputLogic.ApplyLine("FIRST SECOND", session);

        error.ShouldBe("second error");
    }

    [Fact]
    public void ApplyLineCollectErrors_WithMultipleFailures_ReturnsAllErrorMessages()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().Setup(s => s.Add("FIRST")).Returns(Result.Fail("first error"));
        session.AsMock().Setup(s => s.Add("SECOND")).Returns(Result.Fail("second error"));

        var errors = DataInputLogic.ApplyLineCollectErrors("FIRST SECOND", session);

        errors.ShouldBe(["first error", "second error"]);
    }

    [Fact]
    public void ApplyLineCollectErrors_WithNoFailures_ReturnsEmptyList()
    {
        var session = Mock.Of<IGameSession>();
        session.AsMock().Setup(s => s.Add(It.IsAny<string>())).Returns(Result.Ok());

        var errors = DataInputLogic.ApplyLineCollectErrors("HELLO WORLD", session);

        errors.ShouldBeEmpty();
    }
}
