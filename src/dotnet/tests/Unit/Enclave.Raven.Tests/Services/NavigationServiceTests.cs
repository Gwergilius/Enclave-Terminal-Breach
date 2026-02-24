using Enclave.Raven.Services;
using Enclave.Shared.Phases;
using FluentResults;
using Xunit.Categories;

namespace Enclave.Raven.Tests.Services;

/// <summary>
/// Unit tests for <see cref="NavigationService"/>.
/// </summary>
[UnitTest, TestOf(nameof(NavigationService))]
public class NavigationServiceTests
{
    [Fact]
    public void NextPhase_Initially_IsNull()
    {
        var sut = new NavigationService();

        sut.NextPhase.ShouldBeNull();
    }

    [Fact]
    public void NextPhaseArgs_Initially_IsEmpty()
    {
        var sut = new NavigationService();

        sut.NextPhaseArgs.ShouldBeEmpty();
    }

    [Fact]
    public void NavigateTo_WithNullPhaseName_ThrowsArgumentNullException()
    {
        var sut = new NavigationService();

        var ex = Should.Throw<ArgumentNullException>(() => sut.NavigateTo(null!));

        ex.ParamName.ShouldBe("phaseName");
    }

    [Fact]
    public void NavigateTo_WithNormalPhase_SetsNextPhaseAndNextPhaseArgs_ReturnsOk()
    {
        var sut = new NavigationService();
        var args = new object[] { 42, "test" };

        var result = sut.NavigateTo("DataInput", args);

        result.IsSuccess.ShouldBeTrue();
        sut.NextPhase.ShouldBe("DataInput");
        sut.NextPhaseArgs.Count.ShouldBe(2);
        sut.NextPhaseArgs[0].ShouldBe(42);
        sut.NextPhaseArgs[1].ShouldBe("test");
    }

    [Fact]
    public void NavigateTo_WithNormalPhase_NoArgs_SetsNextPhaseArgsToEmpty()
    {
        var sut = new NavigationService();

        sut.NavigateTo("StartupBadge");

        sut.NextPhase.ShouldBe("StartupBadge");
        sut.NextPhaseArgs.ShouldBeEmpty();
    }

    [Fact]
    public void NavigateTo_WithNullArgsArray_SetsNextPhaseArgsToEmpty()
    {
        // params object[] args can be null when caller passes (object[])null or a null object[] variable
        var sut = new NavigationService();

        var result = sut.NavigateTo("DataInput", (object[])null!);

        result.IsSuccess.ShouldBeTrue();
        sut.NextPhase.ShouldBe("DataInput");
        sut.NextPhaseArgs.ShouldBeEmpty();
    }

    [Fact]
    public void NavigateTo_WithExit_NoArgs_ReturnsFailWithApplicationExit()
    {
        var sut = new NavigationService();

        var result = sut.NavigateTo("Exit");

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is ApplicationExit);
    }

    [Fact]
    public void NavigateTo_WithExit_FirstArgIsResult_ReturnsThatResult()
    {
        var sut = new NavigationService();
        var exitResult = Result.Fail("Custom exit reason");

        var result = sut.NavigateTo("Exit", exitResult);

        result.ShouldBe(exitResult);
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e.Message == "Custom exit reason");
    }

    [Fact]
    public void NavigateTo_WithExit_FirstArgNotResult_ReturnsFailWithApplicationExit()
    {
        var sut = new NavigationService();

        var result = sut.NavigateTo("Exit", "not a Result");

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is ApplicationExit);
    }

    [Theory]
    [InlineData("Exit")]
    [InlineData("EXIT")]
    [InlineData("exit")]
    public void NavigateTo_ExitPhaseName_IsCaseInsensitive(string phaseName)
    {
        var sut = new NavigationService();

        var result = sut.NavigateTo(phaseName);

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is ApplicationExit);
    }

    [Fact]
    public void NavigateTo_OverwritesPreviousNextPhase()
    {
        var sut = new NavigationService();
        sut.NavigateTo("DataInput");

        sut.NavigateTo("HackingLoop");

        sut.NextPhase.ShouldBe("HackingLoop");
    }
}
