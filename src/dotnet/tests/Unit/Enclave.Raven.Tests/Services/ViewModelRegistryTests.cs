using Enclave.Common.Errors;
using Enclave.Common.Test.Core;
using Enclave.Raven.Screens;
using Enclave.Raven.Services;
using Moq;
using Xunit.Categories;

namespace Enclave.Raven.Tests.Services;

/// <summary>
/// Unit tests for <see cref="ViewModelRegistry"/>.
/// </summary>
[UnitTest, TestOf(nameof(ViewModelRegistry))]
public class ViewModelRegistryTests
{
    private static IScreenViewModel CreateViewModel(string name)
    {
        var vm = Mock.Of<IScreenViewModel>();
        vm.AsMock().Setup(v => v.Name).Returns(name);
        return vm;
    }

    [Fact]
    public void Constructor_WithNullViewModels_ThrowsArgumentNullException()
    {
        var ex = Should.Throw<ArgumentNullException>(() => new ViewModelRegistry(null!));

        ex.ParamName.ShouldBe("viewModels");
    }

    [Fact]
    public void GetViewModel_WhenExists_ReturnsOkWithThatViewModel()
    {
        var dataInput = CreateViewModel("DataInput");
        var vms = new[] { CreateViewModel("BootScreen"), dataInput, CreateViewModel("HackingLoop") };
        var sut = new ViewModelRegistry(vms);

        var result = sut.GetViewModel("DataInput");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(dataInput);
    }

    [Fact]
    public void GetViewModel_WhenNotRegistered_ReturnsFailWithNotFoundError()
    {
        var vms = new[] { CreateViewModel("DataInput") };
        var sut = new ViewModelRegistry(vms);

        var result = sut.GetViewModel("NonExistent");

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
        result.Errors.ShouldContain(e => e.Message.Contains("NonExistent"));
    }

    [Theory]
    [InlineData("DataInput", "DataInput")]
    [InlineData("datainput", "DataInput")]
    [InlineData("DATAINPUT", "DataInput")]
    public void GetViewModel_LookupIsCaseInsensitive(string requestedName, string registeredName)
    {
        var vm = CreateViewModel(registeredName);
        var sut = new ViewModelRegistry(new[] { vm });

        var result = sut.GetViewModel(requestedName);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(vm);
    }

    [Fact]
    public void GetViewModel_WithEmptyRegistry_ReturnsFailForAnyName()
    {
        var sut = new ViewModelRegistry(Array.Empty<IScreenViewModel>());

        var result = sut.GetViewModel("AnyScreen");

        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
    }

    [Fact]
    public void Constructor_WithDuplicateNames_ThrowsArgumentException()
    {
        var vms = new[] { CreateViewModel("DataInput"), CreateViewModel("DataInput") };

        Should.Throw<ArgumentException>(() => new ViewModelRegistry(vms));
    }
}
