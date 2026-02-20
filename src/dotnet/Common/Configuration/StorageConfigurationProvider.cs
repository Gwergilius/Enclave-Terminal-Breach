using Microsoft.Extensions.Configuration;

namespace Enclave.Common.Configuration;

/// <summary>
/// Configuration provider that loads configuration from IStorageService.
/// Supports hierarchical configuration keys (e.g., "Platform:DefaultFont").
/// </summary>
public class StorageConfigurationProvider : ConfigurationProvider
{
    private readonly StorageConfigurationSource _source;

    /// <summary>
    /// Initializes a new instance of the StorageConfigurationProvider.
    /// </summary>
    /// <param name="source">The configuration source</param>
    public StorageConfigurationProvider(StorageConfigurationSource source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>
    /// Loads configuration from IStorageService.
    /// Reads all keys with the specified prefix and converts them to configuration format.
    /// </summary>
    public override void Load()
    {
        try
        {
            // Load synchronously by blocking on the async operation
            // This is acceptable during application startup
            LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            if (!_source.Optional)
            {
                throw new InvalidOperationException(
                    $"Failed to load configuration from storage service. Set Optional=true to ignore this error.", ex);
            }

            // Optional source - just initialize with empty data
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private async Task LoadAsync()
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        // In a real implementation, you would iterate over known keys or
        // have a manifest of stored configuration keys.
        // For now, we'll load well-known configuration keys.

        var knownKeys = new[]
        {
            "Platform:DefaultFont",
            "Platform:DefaultPalette",
            // Add more known configuration keys here
        };

        foreach (var configKey in knownKeys)
        {
            var storageKey = _source.KeyPrefix + configKey;
            var value = await _source.StorageService.GetStringAsync(storageKey);

            if (!string.IsNullOrEmpty(value))
            {
                data[configKey] = value;
            }
        }

        Data = data;
    }

    /// <summary>
    /// Sets a configuration value and persists it to storage.
    /// </summary>
    /// <param name="key">Configuration key</param>
    /// <param name="value">Configuration value</param>
    public override void Set(string key, string? value)
    {
        base.Set(key, value);

        // Persist to storage asynchronously (fire and forget for now)
        // In production, you might want to handle errors here
        _ = Task.Run(async () =>
        {
            var storageKey = _source.KeyPrefix + key;
            await _source.StorageService.SetStringAsync(storageKey, value);
        });
    }
}
