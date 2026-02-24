using Enclave.Shared.Phases;
using Xunit.Categories;

namespace Enclave.Shared.Tests.Phases;

/// <summary>
/// Unit tests for <see cref="ApplicationExit"/>.
/// </summary>
[UnitTest, TestOf(nameof(ApplicationExit))]
public class ApplicationExitTests
{
    [Fact]
    public void Constructor_Default_SetsMessageToDefaultMessage()
    {
        var sut = new ApplicationExit();

        sut.Message.ShouldBe(ApplicationExit.DefaultMessage);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessageToGivenValue()
    {
        const string customMessage = "Custom exit reason.";

        var sut = new ApplicationExit(customMessage);

        sut.Message.ShouldBe(customMessage);
    }

    [Fact]
    public void DefaultMessage_Constant_HasExpectedValue()
    {
        ApplicationExit.DefaultMessage.ShouldBe("Application exit requested.");
    }
}
