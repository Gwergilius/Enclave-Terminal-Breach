using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.Input;

namespace Enclave.Raven.Tests.Input;

[UnitTest, TestOf(nameof(MatchCountKeyFilter))]
public class MatchCountKeyFilterTests
{
    private static ConsoleKeyInfo Key(char c, ConsoleKey k) => new(c, k, false, false, false);

    [Fact]
    public void Handle_Enter_ReturnsEnter()
    {
        var sut = new MatchCountKeyFilter();
        sut.Handle(Key('\r', ConsoleKey.Enter), "").Kind.ShouldBe(KeyFilterKind.Enter);
    }

    [Fact]
    public void Handle_Backspace_WhenLineNotEmpty_ReturnsBackspace()
    {
        var sut = new MatchCountKeyFilter();
        sut.Handle(Key('\b', ConsoleKey.Backspace), "1").Kind.ShouldBe(KeyFilterKind.Backspace);
    }

    [Fact]
    public void Handle_Backspace_WhenLineEmpty_ReturnsIgnore()
    {
        var sut = new MatchCountKeyFilter();
        sut.Handle(Key('\b', ConsoleKey.Backspace), "").Kind.ShouldBe(KeyFilterKind.Ignore);
    }

    [Fact]
    public void Handle_Digit_WhenUnderMaxLength_ReturnsAppend()
    {
        var sut = new MatchCountKeyFilter(maxLength: 2);
        var result = sut.Handle(Key('5', ConsoleKey.D5), "");
        result.Kind.ShouldBe(KeyFilterKind.Append);
        result.Char.ShouldBe('5');
    }

    [Fact]
    public void Handle_Digit_WhenAtMaxLength_ReturnsIgnore()
    {
        var sut = new MatchCountKeyFilter(maxLength: 2);
        sut.Handle(Key('9', ConsoleKey.D9), "12").Kind.ShouldBe(KeyFilterKind.Ignore);
    }

    [Fact]
    public void Handle_Letter_ReturnsIgnore()
    {
        var sut = new MatchCountKeyFilter();
        sut.Handle(Key('a', ConsoleKey.A), "").Kind.ShouldBe(KeyFilterKind.Ignore);
    }

    [Fact]
    public void Handle_UpArrow_ReturnsIgnore()
    {
        var sut = new MatchCountKeyFilter();
        sut.Handle(Key('\0', ConsoleKey.UpArrow), "").Kind.ShouldBe(KeyFilterKind.Ignore);
    }

    [Fact]
    public void Constructor_DefaultMaxLength_Is2()
    {
        var sut = new MatchCountKeyFilter(); // default maxLength = 2
        sut.Handle(Key('1', ConsoleKey.D1), "").Kind.ShouldBe(KeyFilterKind.Append);
        sut.Handle(Key('2', ConsoleKey.D2), "1").Kind.ShouldBe(KeyFilterKind.Append);
        sut.Handle(Key('0', ConsoleKey.D0), "12").Kind.ShouldBe(KeyFilterKind.Ignore);
    }
}
