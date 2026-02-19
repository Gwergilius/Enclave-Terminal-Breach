using Enclave.Common.Services;
using Microsoft.Extensions.Configuration;

namespace Enclave.Common.Configuration;

/// <summary>
/// Extension methods for adding storage-based configuration to IConfigurationBuilder.
/// </summary>
public static class StorageConfigurationExtensions
{
    /// <summary>
    /// Adds a configuration source that loads from IStorageService.
    /// This allows user overrides to be stored and persisted across sessions.
    /// </summary>
    /// <param name="builder">The configuration builder</param>
    /// <param name="storageService">The storage service to load configuration from</param>
    /// <param name="keyPrefix">Prefix for storage keys (default: "Config:")</param>
    /// <param name="optional">Whether the configuration source is optional (default: true)</param>
    /// <returns>The configuration builder</returns>
    /// <example>
    /// <code>
    /// var configuration = new ConfigurationBuilder()
    ///     .AddEmbeddedJsonFile("appsettings.json")
    ///     .AddStorageConfiguration(storageService)
    ///     .Build();
    /// </code>
    /// </example>
    public static IConfigurationBuilder AddStorageConfiguration(
        this IConfigurationBuilder builder,
        IStorageService storageService,
        string keyPrefix = "Config:",
        bool optional = true)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(storageService);
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return builder.Add(new StorageConfigurationSource
        {
            StorageService = storageService,
            KeyPrefix = keyPrefix,
            Optional = optional
        });
    }
}
