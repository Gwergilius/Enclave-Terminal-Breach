using Enclave.Common.Errors;
using Enclave.Common.Test.Core;
using Enclave.Raven.Services;
using Enclave.Shared.Phases;
using FluentResults;
using Moq;
using Xunit.Categories;

namespace Enclave.Raven.Tests.Services;

/// <summary>
/// Unit tests for <see cref="PhaseRegistry"/>.
/// </summary>
[UnitTest, TestOf(nameof(PhaseRegistry))]
public class PhaseRegistryTests
{
    private static IPhase CreatePhase(string name)
    {
        var phase = Mock.Of<IPhase>();
        phase.AsMock().Setup(p => p.Name).Returns(name);
        return phase;
    }

    [Fact]
    public void Constructor_WithNullPhases_ThrowsArgumentNullException()
    {
        var ex = Should.Throw<ArgumentNullException>(() => new PhaseRegistry(null!));

        ex.ParamName.ShouldBe("phases");
    }

    [Fact]
    public void GetPhase_WhenPhaseExists_ReturnsOkWithThatPhase()
    {
        var dataInput = CreatePhase("DataInput");
        var phases = new[] { CreatePhase("StartupBadge"), dataInput, CreatePhase("HackingLoop") };
        var sut = new PhaseRegistry(phases);

        var result = sut.GetPhase("DataInput");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(dataInput);
    }

    [Fact]
    public void GetPhase_WhenPhaseNotRegistered_ReturnsFailWithNotFoundError()
    {
        var phases = new[] { CreatePhase("DataInput") };
        var sut = new PhaseRegistry(phases);

        var result = sut.GetPhase("NonExistent");

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
        result.Errors.ShouldContain(e => e.Message.Contains("NonExistent"));
    }

    [Theory]
    [InlineData("DataInput", "DataInput")]
    [InlineData("datainput", "DataInput")]
    [InlineData("DATAINPUT", "DataInput")]
    public void GetPhase_PhaseNameLookupIsCaseInsensitive(string requestedName, string registeredName)
    {
        var phase = CreatePhase(registeredName);
        var sut = new PhaseRegistry(new[] { phase });

        var result = sut.GetPhase(requestedName);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(phase);
    }

    [Fact]
    public void GetPhase_WithEmptyPhases_ReturnsFailForAnyName()
    {
        var sut = new PhaseRegistry(Array.Empty<IPhase>());

        var result = sut.GetPhase("AnyPhase");

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
    }

    [Fact]
    public void Constructor_WithDuplicatePhaseNames_ThrowsArgumentException()
    {
        var phases = new[] { CreatePhase("DataInput"), CreatePhase("DataInput") };

        Should.Throw<ArgumentException>(() => new PhaseRegistry(phases));
    }
}
