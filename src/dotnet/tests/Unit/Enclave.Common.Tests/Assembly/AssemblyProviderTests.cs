using System.Reflection;
using Enclave.Common.Assembly;
using Shouldly;
using Xunit;

namespace Enclave.Common.Tests.Assembly;

/// <summary>
/// Unit tests for <see cref="AssemblyProvider"/>.
/// </summary>
[UnitTest, TestOf(nameof(AssemblyProvider))]
public sealed class AssemblyProviderTests
{
    [Fact]
    public void Constructor_WithExecutingAssembly_ReturnsProductAndVersion()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var provider = new AssemblyProvider(assembly);

        provider.Product.ShouldNotBeNullOrEmpty();
        provider.Version.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_WithNullAssembly_ThrowsArgumentNullException()
    {
        var ex = Should.Throw<ArgumentNullException>(() => new AssemblyProvider(null!));
        ex.ParamName.ShouldBe("assembly");
    }
}
