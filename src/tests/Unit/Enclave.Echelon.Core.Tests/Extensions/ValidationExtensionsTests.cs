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

    [Theory,
        InlineData("", "word"),
        InlineData("null", "word")]
    public void ValidateAndThrowArgumentException_WithInvalidInstance_ThrowsArgumentException(string input, string paramName)
    {
        // Arrange
        var validator = new PasswordValidator();
        string expectedMessagePart = input == "null" ? "null" : "empty";
        input = input == "null" ? null! : input; // Simulate null input for "null" case 

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() =>
            validator.ValidateAndThrowArgumentException(input, paramName));
        ex.ParamName.ShouldBe(paramName);
        ex.Message.Contains(expectedMessagePart, StringComparison.OrdinalIgnoreCase).ShouldBeTrue();
    }


    [Theory,
        InlineData(""),
        InlineData("null")]
    public void ValidateAndThrowArgumentException_WithDefaultParameterName_UsesValue(string input)
    {
        // Arrange
        var validator = new PasswordValidator();
        input = input == "null" ? null! : input; // Simulate null input for "null" case 

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() =>
            validator.ValidateAndThrowArgumentException(input));
        ex.ParamName.ShouldBe("value");
    }

    [Theory, InlineData("bad", "param")]
    public void ValidateAndThrowArgumentException_WhenValidatorReturnsMultipleErrors_ThrowsWithFirstError(string input, string paramName)
    {
        // Arrange
        var validator = Mock.Of<IValidator<string>>();
        validator.AsMock()
            .Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("x", "First error."),
                new FluentValidation.Results.ValidationFailure("y", "Second error.")
            }));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() =>
            validator.ValidateAndThrowArgumentException(input, paramName));
        ex.Message.Contains("First error.", StringComparison.Ordinal).ShouldBeTrue();
        ex.Message.Contains("Second error.", StringComparison.Ordinal).ShouldBeFalse();
        ex.ParamName.ShouldBe(paramName);
    }

    [Theory, InlineData("null", "word")]
    public void ValidateAndThrowArgumentException_WhenFirstErrorContainsNull_ThrowsArgumentNullException(string input, string paramName)
    {
        // Arrange - FluentValidation's null rule message typically contains "null"
        var validator = new PasswordValidator();
        input = input == "null" ? null! : input; // Simulate null input for "null" case 

        // Act & Assert
        var ex = Should.Throw<ArgumentNullException>(() =>
            validator.ValidateAndThrowArgumentException(input, paramName));
        ex.ParamName.ShouldBe(paramName);
    }
}
