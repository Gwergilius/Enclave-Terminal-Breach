namespace Enclave.Raven;

/// <summary>
/// Abstraction for product name and version (e.g. for title bar and banner). Injected via DI so callers can be tested with a mock.
/// </summary>
public interface IProductInfo
{
    /// <summary>Gets the product name (e.g. "RAVEN").</summary>
    string Name { get; }

    /// <summary>Gets the version string (e.g. "1.3.2").</summary>
    string Version { get; }
}
