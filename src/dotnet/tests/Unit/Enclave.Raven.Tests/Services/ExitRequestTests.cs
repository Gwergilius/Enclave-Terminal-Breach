using Enclave.Raven.Services;
using Xunit.Categories;

namespace Enclave.Raven.Tests.Services;

/// <summary>
/// Unit tests for <see cref="ExitRequest"/>.
/// </summary>
[UnitTest, TestOf(nameof(ExitRequest))]
public class ExitRequestTests
{
    [Fact]
    public void IsExitRequested_Initially_ReturnsFalse()
    {
        var sut = new ExitRequest();

        sut.IsExitRequested.ShouldBeFalse();
    }

    [Fact]
    public void IsExitRequested_AfterRequestExit_ReturnsTrue()
    {
        var sut = new ExitRequest();

        sut.RequestExit();

        sut.IsExitRequested.ShouldBeTrue();
    }

    [Fact]
    public void RequestExit_CanBeCalledMultipleTimes_IsExitRequestedStaysTrue()
    {
        var sut = new ExitRequest();

        sut.RequestExit();
        sut.RequestExit();

        sut.IsExitRequested.ShouldBeTrue();
    }
}
