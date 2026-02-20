using System.Reflection;
using Enclave.Raven;

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
    public void GetFromAssembly_WhenProductAttributeMissing_UsesFallback()
    {
        var coreLibAssembly = typeof(object).Assembly;
        var productAttr = coreLibAssembly.GetCustomAttribute<AssemblyProductAttribute>();
        if (productAttr is not null)
            return;

        var info = ProductInfo.GetFromAssembly(coreLibAssembly);
        info.Name.ShouldBe("RAVEN");
    }

    [Fact]
    public void GetFromAssembly_WhenVersionNull_UsesFallback()
    {
        var coreLibAssembly = typeof(object).Assembly;
        var version = coreLibAssembly.GetName().Version;
        if (version is not null)
            return;

        var info = ProductInfo.GetFromAssembly(coreLibAssembly);
        info.Version.ShouldBe("0.0.0");
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
