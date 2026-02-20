using Enclave.Common.Services;
using Microsoft.Extensions.Configuration;

namespace Enclave.Common.Configuration;

/// <summary>
/// Configuration source for loading configuration from IStorageService.
/// Allows user overrides to be stored and persisted across sessions.
/// </summary>
public class StorageConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// The storage service to load configuration from.
    /// </summary>
    public IStorageService StorageService { get; set; } = default!;

    /// <summary>
    /// Prefix for storage keys (e.g., "Config:")
    /// </summary>
    public string KeyPrefix { get; set; } = "Config:";

    /// <summary>
    /// Whether the configuration source is optional.
    /// </summary>
    public bool Optional { get; set; } = true;

    /// <summary>
    /// Builds the configuration provider.
    /// </summary>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new StorageConfigurationProvider(this);
    }
}
