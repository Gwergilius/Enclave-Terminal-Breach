using Enclave.Common.Test.Core;
using Enclave.Raven.Input;

namespace Enclave.Raven.Tests.Input;

[UnitTest, TestOf(nameof(MatchCountValidator))]
public class MatchCountValidatorTests
{
    private const int WordLength = 5;
    private static readonly IReadOnlyList<int> Allowed = [0, 1, 4, 5];

    [Fact]
    public void Validate_EmptyLine_IsValid()
    {
        var sut = new MatchCountValidator(WordLength, Allowed);
        var (isValid, errorMessage) = sut.Validate("");
        isValid.ShouldBeTrue();
        errorMessage.ShouldBeNull();
    }

    [Fact]
    public void Validate_WhitespaceOnly_IsValid()
    {
        var sut = new MatchCountValidator(WordLength, Allowed);
        var (isValid, _) = sut.Validate("  ");
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_AllowedValue_IsValid()
    {
        var sut = new MatchCountValidator(WordLength, Allowed);
        var (isValid, _) = sut.Validate("4");
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ValueNotInAllowedList_IsInvalid_WithFormattedMessage()
    {
        var sut = new MatchCountValidator(WordLength, Allowed);
        var (isValid, errorMessage) = sut.Validate("3");
        isValid.ShouldBeFalse();
        errorMessage!.ShouldContain("0, 1, 4, 5");
        errorMessage!.ShouldContain("Enter for 5");
    }

    [Fact]
    public void Validate_OutOfRange_IsInvalid()
    {
        var sut = new MatchCountValidator(WordLength, Allowed);
        var (isValid, _) = sut.Validate("6");
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void Validate_NonNumeric_IsInvalid()
    {
        var sut = new MatchCountValidator(WordLength, Allowed);
        var (isValid, _) = sut.Validate("x");
        isValid.ShouldBeFalse();
    }
}
