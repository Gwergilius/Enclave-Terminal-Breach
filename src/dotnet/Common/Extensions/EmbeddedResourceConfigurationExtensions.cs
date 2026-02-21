using Enclave.Common.Configuration;
using Microsoft.Extensions.Configuration;

namespace Enclave.Common.Extensions;

/// <summary>
/// Extension methods for adding embedded resource configuration to IConfigurationBuilder.
/// </summary>
public static class EmbeddedResourceConfigurationExtensions
{
    /// <summary>
    /// Adds a JSON configuration source from an embedded resource.
    /// </summary>
    /// <param name="builder">The configuration builder</param>
    /// <param name="assembly">The assembly containing the embedded resource</param>
    /// <param name="resourcePath">The resource path (e.g., "appsettings.json")</param>
    /// <param name="optional">Whether the configuration file is optional</param>
    /// <returns>The configuration builder</returns>
    /// <example>
    /// <code>
    /// var configuration = new ConfigurationBuilder()
    ///     .AddEmbeddedJsonFile(Assembly.GetExecutingAssembly(), "appsettings.json")
    ///     .Build();
    /// </code>
    /// </example>
    public static IConfigurationBuilder AddEmbeddedJsonFile(
        this IConfigurationBuilder builder,
        System.Reflection.Assembly assembly,
        string resourcePath,
        bool optional = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourcePath);

        return builder.Add(new EmbeddedResourceConfigurationSource
        {
            Assembly = assembly,
            ResourcePath = resourcePath,
            Optional = optional
        });
    }

    /// <summary>
    /// Adds a JSON configuration source from an embedded resource in the calling assembly.
    /// </summary>
    /// <param name="builder">The configuration builder</param>
    /// <param name="resourcePath">The resource path (e.g., "appsettings.json")</param>
    /// <param name="optional">Whether the configuration file is optional</param>
    /// <returns>The configuration builder</returns>
    /// <example>
    /// <code>
    /// var configuration = new ConfigurationBuilder()
    ///     .AddEmbeddedJsonFile("appsettings.json")
    ///     .Build();
    /// </code>
    /// </example>
    public static IConfigurationBuilder AddEmbeddedJsonFile(
        this IConfigurationBuilder builder,
        string resourcePath,
        bool optional = false)
    {
        // Get calling assembly (the assembly that called this method)
        var assembly = System.Reflection.Assembly.GetCallingAssembly();
        return builder.AddEmbeddedJsonFile(assembly, resourcePath, optional);
    }
}
