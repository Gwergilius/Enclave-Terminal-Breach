using System.Reflection;
using Enclave.Raven;
using Enclave.Common.Assembly;
using Moq;

namespace Enclave.Raven.Tests;

/// <summary>
/// Unit tests for <see cref="ProductInfo"/>.
/// </summary>
[UnitTest, TestOf(nameof(ProductInfo))]
public class ProductInfoTests
{
    [Fact]
    public void GetCurrent_ReturnsProductAndVersion()
    {
        var info = ProductInfo.GetCurrent();

        info.Name.ShouldNotBeNullOrEmpty();
        info.Version.ShouldNotBeNullOrEmpty();
        info.Version.ShouldMatch(@"^\d+\.\d+\.\d+$");
    }

    [Fact]
    public void GetFromAssembly_WithExecutingAssembly_ReturnsProductAndVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var info = ProductInfo.GetFromAssembly(assembly);

        info.Name.ShouldNotBeNullOrEmpty();
        info.Version.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void GetFromProvider_WhenProductNull_UsesFallback()
    {
        var provider = Mock.Of<IAssemblyProvider>(p =>
            p.Product == null &&
            p.Version == "2.0.0");

        var info = ProductInfo.GetFromProvider(provider);

        info.Name.ShouldBe("RAVEN");
        info.Version.ShouldBe("2.0.0");
    }

    [Fact]
    public void GetFromProvider_WhenVersionNull_UsesFallback()
    {
        var provider = Mock.Of<IAssemblyProvider>(p =>
            p.Product == "MyProduct" &&
            p.Version == null);

        var info = ProductInfo.GetFromProvider(provider);

        info.Name.ShouldBe("MyProduct");
        info.Version.ShouldBe("0.0.0");
    }

    [Fact]
    public void GetFromProvider_WhenBothNull_UsesBothFallbacks()
    {
        var provider = Mock.Of<IAssemblyProvider>(p =>
            p.Product == null &&
            p.Version == null);

        var info = ProductInfo.GetFromProvider(provider);

        info.Name.ShouldBe("RAVEN");
        info.Version.ShouldBe("0.0.0");
    }

    [Fact]
    public void GetFromProvider_WithNonNullProvider_ReturnsProviderValues()
    {
        var provider = Mock.Of<IAssemblyProvider>(p =>
            p.Product == "CustomProduct" &&
            p.Version == "9.9.9");

        var info = ProductInfo.GetFromProvider(provider);

        info.Name.ShouldBe("CustomProduct");
        info.Version.ShouldBe("9.9.9");
    }

    [Fact]
    public void GetFromProvider_WithNullProvider_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ProductInfo.GetFromProvider(null!))
            .ParamName.ShouldBe("provider");
    }

    [Fact]
    public void GetFromAssembly_WithNullAssembly_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ProductInfo.GetFromAssembly(null!))
            .ParamName.ShouldBe("assembly");
    }

    [Fact]
    public void Constructor_StoresNameAndVersion()
    {
        var info = new ProductInfo("Test", "1.2.3");

        info.Name.ShouldBe("Test");
        info.Version.ShouldBe("1.2.3");
    }
}
