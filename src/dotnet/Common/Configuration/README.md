# Embedded Resource Configuration Provider

Reusable configuration infrastructure for loading JSON configuration from embedded resources.

## Quick Start

### 1. Embed your appsettings.json

**In your .csproj:**
```xml
<ItemGroup>
  <EmbeddedResource Include="appsettings.json" />
</ItemGroup>
```

### 2. Use AddEmbeddedJsonFile()

**In your Startup/Program.cs:**
```csharp
using Enclave.Raven.Core.Configuration;

var configuration = new ConfigurationBuilder()
    .AddEmbeddedJsonFile(Assembly.GetExecutingAssembly(), "appsettings.json")
    .Build();
```

**Or shorter (uses calling assembly):**
```csharp
var configuration = new ConfigurationBuilder()
    .AddEmbeddedJsonFile("appsettings.json")
    .Build();
```

## Features

✅ **ResourceExtensions Integration** - Uses existing `ResourceExtensions.GetResourceStream()`  
✅ **Automatic Name Resolution** - Converts `/` → `.`, `-` → `_` automatically  
✅ **FluentResults Error Handling** - Clean error messages with available resources listed  
✅ **Optional Resources** - Support for optional configuration files  
✅ **Type-Safe API** - Extension methods for `IConfigurationBuilder`  

## API

### AddEmbeddedJsonFile()

```csharp
public static IConfigurationBuilder AddEmbeddedJsonFile(
    this IConfigurationBuilder builder,
    Assembly assembly,
    string resourcePath,
    bool optional = false)
```

**Parameters:**
- `assembly` - Assembly containing the embedded resource
- `resourcePath` - Resource path (e.g., "appsettings.json")
- `optional` - If true, missing resource won't throw exception

**Returns:** `IConfigurationBuilder` for chaining

### AddEmbeddedJsonFile() Overload

```csharp
public static IConfigurationBuilder AddEmbeddedJsonFile(
    this IConfigurationBuilder builder,
    string resourcePath,
    bool optional = false)
```

Uses `Assembly.GetCallingAssembly()` automatically.

## Examples

### Basic Usage
```csharp
var config = new ConfigurationBuilder()
    .AddEmbeddedJsonFile("appsettings.json")
    .Build();
```

### Optional Configuration
```csharp
var config = new ConfigurationBuilder()
    .AddEmbeddedJsonFile("appsettings.json")
    .AddEmbeddedJsonFile("appsettings.Development.json", optional: true)
    .Build();
```

### Multiple Assemblies
```csharp
var config = new ConfigurationBuilder()
    .AddEmbeddedJsonFile(typeof(CoreLib).Assembly, "core-config.json")
    .AddEmbeddedJsonFile(typeof(AppLib).Assembly, "app-config.json")
    .Build();
```

## Error Handling

### Required Resource Not Found
```csharp
// Throws FileNotFoundException:
// "Embedded resource 'missing.json' not found in assembly 'MyApp'.
//  Resource not found
//  Available resources are:
//  - MyApp.appsettings.json
//  - MyApp.Data.words.txt"
```

### Optional Resource Not Found
```csharp
// Returns empty configuration (no exception)
var config = new ConfigurationBuilder()
    .AddEmbeddedJsonFile("optional.json", optional: true)
    .Build();
```

## Resource Name Conversion

`ResourceExtensions.GetResourceStream()` automatically converts resource names:

| Input | Converted | Actual Resource Name |
|-------|-----------|---------------------|
| `appsettings.json` | (none) | `MyApp.appsettings.json` |
| `config/app.json` | `/` → `.` | `MyApp.config.app.json` |
| `app-settings.json` | `-` → `_` | `MyApp.app_settings.json` |

## Architecture

```
IConfigurationBuilder
  ↓ .AddEmbeddedJsonFile()
EmbeddedResourceConfigurationSource
  ↓ .Build()
EmbeddedResourceConfigurationProvider
  ↓ .Load()
ResourceExtensions.GetResourceStream()
  ↓
Stream → JsonConfigurationProvider
```

## Used By

- **Console POC (RAVEN)** - `appsettings.json` embedded
- **MAUI (ECHELON)** - `appsettings.json` embedded
- **Blazor (GHOST)** - Uses `wwwroot/appsettings.json` (different mechanism)

## See Also

- [EMBEDDED_RESOURCE_CONFIGURATION.md](../../../docs/EMBEDDED_RESOURCE_CONFIGURATION.md) - Full documentation
- [PLATFORM_CONFIG_FROM_APPSETTINGS.md](../../../docs/PLATFORM_CONFIG_FROM_APPSETTINGS.md) - Platform configuration
- [ResourceExtensions.cs](../Extensions/ResourceExtensions.cs) - Resource loading utilities
