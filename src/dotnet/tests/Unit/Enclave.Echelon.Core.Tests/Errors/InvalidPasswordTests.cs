using Enclave.Echelon.Core.Errors;

namespace Enclave.Echelon.Core.Tests.Errors;

/// <summary>
/// Unit tests for <see cref="InvalidPassword"/>.
/// </summary>
[UnitTest, TestOf(nameof(InvalidPassword))]
public class InvalidPasswordTests
{
    [Theory]
    [InlineData("TERM1", "Word must contain only letters.")]
    [InlineData("", "Word cannot be empty.")]
    [InlineData("TERMS", "Word length must be 5. Skipping: TEXAS")]
    public void Constructor_StoresWordAndMessage(string word, string message)
    {
        // Act
        var error = new InvalidPassword(word, message);

        // Assert
        error.Word.ShouldBe(word);
        error.Message.ShouldBe(message);
    }

    [Fact]
    public void Error_InheritsFromFluentResultsError()
    {
        // Act
        var error = new InvalidPassword("bad", "Invalid format");

        // Assert
        error.ShouldBeAssignableTo<FluentResults.IError>();
    }
}
