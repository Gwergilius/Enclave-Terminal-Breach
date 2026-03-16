using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.Input;

namespace Enclave.Raven.Tests.Input;

[UnitTest, TestOf(nameof(DataInputKeyFilter))]
public class DataInputKeyFilterTests
{
    private static ConsoleKeyInfo Key(char c, ConsoleKey k, bool control = false, bool alt = false) =>
        new(c, k, shift: false, alt, control);

    private readonly DataInputKeyFilter _sut = new();

    [Fact]
    public void Handle_Enter_ReturnsEnter()
    {
        _sut.Handle(Key('\r', ConsoleKey.Enter), "").Kind.ShouldBe(KeyFilterKind.Enter);
    }

    [Fact]
    public void Handle_UpArrow_ReturnsSpecial()
    {
        _sut.Handle(Key('\0', ConsoleKey.UpArrow), "").Kind.ShouldBe(KeyFilterKind.Special);
    }

    [Fact]
    public void Handle_DownArrow_ReturnsSpecial()
    {
        _sut.Handle(Key('\0', ConsoleKey.DownArrow), "").Kind.ShouldBe(KeyFilterKind.Special);
    }

    [Fact]
    public void Handle_Backspace_WhenLineNotEmpty_ReturnsBackspace()
    {
        _sut.Handle(Key('\b', ConsoleKey.Backspace), "x").Kind.ShouldBe(KeyFilterKind.Backspace);
    }

    [Fact]
    public void Handle_Backspace_WhenLineEmpty_ReturnsIgnore()
    {
        _sut.Handle(Key('\b', ConsoleKey.Backspace), "").Kind.ShouldBe(KeyFilterKind.Ignore);
    }

    [Fact]
    public void Handle_Letter_ReturnsAppendWithChar()
    {
        var result = _sut.Handle(Key('a', ConsoleKey.A), "");
        result.Kind.ShouldBe(KeyFilterKind.Append);
        result.Char.ShouldBe('a');
    }

    [Fact]
    public void Handle_Digit_ReturnsAppendWithChar()
    {
        var result = _sut.Handle(Key('7', ConsoleKey.D7), "");
        result.Kind.ShouldBe(KeyFilterKind.Append);
        result.Char.ShouldBe('7');
    }

    [Fact]
    public void Handle_Space_ReturnsAppendWithChar()
    {
        var result = _sut.Handle(Key(' ', ConsoleKey.Spacebar), "");
        result.Kind.ShouldBe(KeyFilterKind.Append);
        result.Char.ShouldBe(' ');
    }

    [Fact]
    public void Handle_Minus_ReturnsAppendWithChar()
    {
        var result = _sut.Handle(Key('-', ConsoleKey.OemMinus), "");
        result.Kind.ShouldBe(KeyFilterKind.Append);
        result.Char.ShouldBe('-');
    }

    [Fact]
    public void Handle_Punctuation_ReturnsAppendWithChar()
    {
        var result = _sut.Handle(Key('.', ConsoleKey.OemPeriod), "");
        result.Kind.ShouldBe(KeyFilterKind.Append);
        result.Char.ShouldBe('.');
    }

    [Fact]
    public void Handle_NonPrintableKey_ReturnsIgnore()
    {
        // Tab is not letter/digit/punct/space/minus
        var result = _sut.Handle(new ConsoleKeyInfo('\t', ConsoleKey.Tab, shift: false, alt: false, control: false), "");
        result.Kind.ShouldBe(KeyFilterKind.Ignore);
    }
}
