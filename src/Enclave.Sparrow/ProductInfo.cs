using System.Reflection;

namespace Enclave.Sparrow;

/// <summary>
/// Product name and version read from an assembly.
/// </summary>
public sealed class ProductInfo
{
    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the version string (e.g. "1.1.0").
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ProductInfo"/>.
    /// </summary>
    public ProductInfo(string name, string version)
    {
        Name = name;
        Version = version;
    }

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
    /// <returns>Product info with Name and Version; uses "SPARROW" and "0.0.0" when attributes are missing.</returns>
    public static ProductInfo GetFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var version = assembly.GetName().Version?.ToString(3) ?? "0.0.0";
        var product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "SPARROW";
        return new ProductInfo(product, version);
    }
}
