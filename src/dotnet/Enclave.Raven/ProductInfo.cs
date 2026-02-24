using System.Reflection;
using Enclave.Common.Assembly;

namespace Enclave.Raven;

/// <summary>
/// Product name and version read from an assembly.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="ProductInfo"/>.
/// </remarks>
public sealed class ProductInfo(string name, string version) : IProductInfo
{
    private const string DefaultProduct = "RAVEN";
    private const string DefaultVersion = "0.0.0";

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the version string (e.g. "1.3.0").
    /// </summary>
    public string Version { get; } = version;

    /// <summary>
    /// Gets product info for the executing assembly.
    /// </summary>
    public static ProductInfo GetCurrent()
    {
        return GetFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Gets product info from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to read from.</param>
    /// <returns>Product info with Name and Version; uses <see cref="DefaultProduct"/> and <see cref="DefaultVersion"/> when attributes are missing.</returns>
    public static ProductInfo GetFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return GetFromProvider(new AssemblyProvider(assembly));
    }

    /// <summary>
    /// Gets product info from the given provider. Applies fallbacks when <see cref="IAssemblyProvider.Product"/> or <see cref="IAssemblyProvider.Version"/> is null.
    /// </summary>
    /// <param name="provider">The assembly provider (e.g. from assembly or mock).</param>
    /// <returns>Product info with Name and Version.</returns>
    public static ProductInfo GetFromProvider(IAssemblyProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        var version = provider.Version ?? DefaultVersion;
        var product = provider.Product ?? DefaultProduct;
        return new ProductInfo(product, version);
    }
}
