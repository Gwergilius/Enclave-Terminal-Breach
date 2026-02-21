using System.Reflection;

namespace Enclave.Common.Assembly;

/// <summary>
/// Extracts product and version from a .NET <see cref="Assembly"/>.
/// </summary>
public sealed class AssemblyProvider : IAssemblyProvider
{
    /// <summary>
    /// Creates a provider that reads from the given assembly.
    /// </summary>
    /// <param name="assembly">The assembly to read from.</param>
    public AssemblyProvider(System.Reflection.Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        Product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        Version = assembly.GetName().Version?.ToString(3);
    }

    /// <inheritdoc />
    public string? Product { get; }

    /// <inheritdoc />
    public string? Version { get; }
}
