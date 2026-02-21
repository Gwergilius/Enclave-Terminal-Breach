using System.Reflection;

namespace Enclave.Common.Assembly;

/// <summary>
/// Provides product name and version extracted from an assembly.
/// Allows tests to mock and exercise fallback branches when attributes or version are missing.
/// </summary>
public interface IAssemblyProvider
{
    /// <summary>Product name from <see cref="AssemblyProductAttribute"/>, or null if missing.</summary>
    string? Product { get; }

    /// <summary>Version string (e.g. "1.0.0") from <see cref="AssemblyName.Version"/>, or null if missing.</summary>
    string? Version { get; }
}
