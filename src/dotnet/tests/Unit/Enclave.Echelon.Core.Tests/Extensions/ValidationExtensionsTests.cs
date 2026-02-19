using Enclave.Common.Test.Core;
using Enclave.Echelon.Core.Extensions;
using Enclave.Echelon.Core.Validators;



namespace Enclave.Echelon.Core.Tests.Extensions;

/// <summary>
/// Unit tests for <see cref="ValidationExtensions"/>.
/// </summary>
[UnitTest, TestOf(nameof(ValidationExtensions))]
public class ValidationExtensionsTests
{
    [Theory, 
        InlineData("TERMS"),
        InlineData("TEXAS")]
    public void ValidateAndThrowArgumentException_WithValidInstance_DoesNotThrow(string input)
    {
        // Arrange
        var validator = new PasswordValidator();

        // Act & Assert
        Should.NotThrow(() => validator.ValidateAndThrowArgumentException(input, nameof(validator)));
    }

    [Theory]
    [InlineData("", "word")]
    [InlineData("null", "word")]
    public void ValidateAndThrowArgumentException_WithInvalidInstance_ThrowsArgumentException(string input, string paramName)
    {
        // Arrange
        var validator = new PasswordValidator();
        var expectedMessagePart = string.IsNullOrEmpty(input) ? "empty" : "null";
        if (input == "null")
        {
            input = null!;
        }

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() =>
            validator.ValidateAndThrowArgumentException(input, paramName));
        ex.ParamName.ShouldBe(paramName);
        ex.Message.Contains(expectedMessagePart, StringComparison.OrdinalIgnoreCase).ShouldBeTrue();
    }

    [Fact]
    public void ValidateAndThrowArgumentException_WhenParameterNameOmitted_UsesCallerArgumentExpression()
    {
        // Arrange - when second argument is omitted, [CallerArgumentExpression(nameof(instance))] yields "parameter"
        var validator = new PasswordValidator();
        var parameter = string.Empty;
        // Act & Assert (empty string -> ArgumentException; param name from caller)
        var ex = Should.Throw<ArgumentException>(() =>
            validator.ValidateAndThrowArgumentException(parameter));
        ex.ParamName.ShouldBe(nameof(parameter));
    }

    [Theory, InlineData("bad", "param")]
    public void ValidateAndThrowArgumentException_WhenValidatorReturnsMultipleErrors_ThrowsWithFirstError(string input, string paramName)
    {
        // Arrange
        var validator = Mock.Of<IValidator<string>>();
        validator.AsMock()
            .Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new FluentValidation.Results.ValidationResult(
            [
                new FluentValidation.Results.ValidationFailure("x", "First error."),
                new FluentValidation.Results.ValidationFailure("y", "Second error.")
            ]));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() =>
            validator.ValidateAndThrowArgumentException(input, paramName));
        ex.Message.Contains("First error.", StringComparison.Ordinal).ShouldBeTrue();
        ex.Message.Contains("Second error.", StringComparison.Ordinal).ShouldBeFalse();
        ex.ParamName.ShouldBe(paramName);
    }

    [Theory]
    [InlineData("word")]
    public void ValidateAndThrowArgumentException_WhenRequiredFieldMissing_ValidationErrorContainsNull_ThrowsArgumentNullException(string paramName)
    {
        // Arrange: instance is non-null but a required field is missing (null); validator returns error message containing "null" -> line 48
        var validator = Mock.Of<IValidator<string>>();
        validator.AsMock()
            .Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new FluentValidation.Results.ValidationResult(
            [
                new FluentValidation.Results.ValidationFailure("RequiredField", "Required field is null.")
            ]));

        // Act & Assert
        var ex = Should.Throw<ArgumentNullException>(() =>
            validator.ValidateAndThrowArgumentException("something", paramName));
        ex.ParamName.ShouldBe(paramName);
        ex.Message.ShouldContain("Required field is null.");
    }

    [Fact]
    public void ValidateAndThrowArgumentException_WhenValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IValidator<string>? validator = null;

        // Act & Assert
        var ex = Should.Throw<ArgumentNullException>(() =>
            validator!.ValidateAndThrowArgumentException("x", "param"));
        ex.ParamName.ShouldBe("validator");
    }
}
