using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Enclave.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Enclave.Common.Configuration;

/// <summary>
/// Configuration provider that loads JSON configuration from an embedded resource.
/// Uses ResourceExtensions.GetResourceStream() for accessing embedded resources.
/// </summary>
/// <remarks>
/// Initializes a new instance of the EmbeddedJsonResourceConfigurationProvider.
/// </remarks>
/// <param name="source">The configuration source</param>
public class EmbeddedJsonResourceConfigurationProvider([NotNull] EmbeddedResourceConfigurationSource source) 
    : JsonConfigurationProvider(new JsonConfigurationSource())
{
    private readonly EmbeddedResourceConfigurationSource _source = source;

    /// <summary>
    /// Loads the configuration from the embedded resource.
    /// </summary>
    public override void Load()
    {
        var streamResult = _source.Assembly.GetResourceStream(_source.ResourcePath);

        if (streamResult.IsSuccess)
        {
            using var stream = streamResult.Value;
            Load(stream);
            return;
        } 
        else if (_source.Optional)
        {
            // Optional resource not found - just return without loading
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            return;
        }
        else
        {
            // Required resource not found - throw exception
            var errorMessages = string.Join(Environment.NewLine, streamResult.Errors.Select(e => e.Message));
            throw new FileNotFoundException(
                $"Embedded resource '{_source.ResourcePath}' not found in assembly '{_source.Assembly.GetName().Name}'.{Environment.NewLine}{errorMessages}");
        }
    }
}

/// <summary>
/// Configuration source for loading configuration from an embedded resource.
/// Uses ResourceExtensions.GetResourceStream() for accessing embedded resources.
/// </summary>
public class EmbeddedResourceConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// The assembly containing the embedded resource.
    /// </summary>
    public Assembly Assembly { get; set; } = default!;

    /// <summary>
    /// The resource path (e.g., "appsettings.json").
    /// Automatically converted to embedded resource naming convention (slashes -> dots, etc.)
    /// </summary>
    public string ResourcePath { get; set; } = string.Empty;

    /// <summary>
    /// Whether the configuration file is optional.
    /// </summary>
    public bool Optional { get; set; }

    /// <summary>
    /// Builds the configuration provider.
    /// </summary>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EmbeddedJsonResourceConfigurationProvider(this);
    }
}

