using Enclave.Echelon.Core.Validators;

namespace Enclave.Echelon.Core.Tests.Validators;

/// <summary>
/// Unit tests for <see cref="PasswordValidator"/>.
/// </summary>
[UnitTest, TestOf(nameof(PasswordValidator))]
public class PasswordValidatorTests
{
    private readonly PasswordValidator _validator = new();

    [Theory]
    [InlineData("A")]
    [InlineData("TERMS")]
    [InlineData("terms")]
    [InlineData("RELEASED")]
    public void Validate_WithValidWord_IsValid(string word)
    {
        // Act
        var result = _validator.Validate(word);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_WithNull_ThrowsInvalidOperationException()
    {
        // FluentValidation rejects null before running rules; the Password constructor
        // uses ValidationExtensions.ValidateAndThrowArgumentException which throws ArgumentNullException for null.
        // Act & Assert
        string word = null!;
        Should.Throw<InvalidOperationException>(() => _validator.Validate(word));
    }

    [Theory, InlineData("")]
    public void Validate_WithEmptyString_IsInvalid(string input)
    {
        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("empty", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_WithWhitespaceOnly_IsInvalid(string word)
    {
        // Act
        var result = _validator.Validate(word);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorMessage.Contains("whitespace", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("TERM1")]
    [InlineData("12345")]
    [InlineData("TERM-S")]
    [InlineData("TERM S")]
    [InlineData("TERM.S")]
    public void Validate_WithNonLetterCharacters_IsInvalid(string word)
    {
        // Act
        var result = _validator.Validate(word);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.ErrorMessage.Contains("letters", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validate_WithMixedInvalid_ReturnsFirstRelevantError()
    {
        // Act
        var result = _validator.Validate("");

        // Assert
        result.Errors.Count.ShouldBeGreaterThan(0);
    }
}
