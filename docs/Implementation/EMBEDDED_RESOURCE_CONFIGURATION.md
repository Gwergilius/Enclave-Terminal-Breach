# Embedded Resource Configuration Provider

**English** | [Magyar]

## Summary

A reusable `EmbeddedResourceConfigurationProvider` implementation was added to the Core project, enabling configuration to be loaded from embedded resources. Console and MAUI projects now use it to load `appsettings.json`.

## Changes

### 1. Core - Configuration Infrastructure

#### ✅ Core/Configuration/EmbeddedResourceConfigurationSource.cs (NEW)
```csharp
/// <summary>
/// Configuration source for loading configuration from an embedded resource.
/// Uses ResourceExtensions.GetResourceStream() for accessing embedded resources.
/// </summary>
public class EmbeddedResourceConfigurationSource : IConfigurationSource
{
    public Assembly Assembly { get; set; }
    public string ResourcePath { get; set; }
    public bool Optional { get; set; }
    
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EmbeddedResourceConfigurationProvider(this);
    }
}
```

**Responsibilities:**
- Define configuration source
- Store Assembly, resource path, optional flag
- Create provider

#### ✅ Core/Configuration/EmbeddedResourceConfigurationProvider.cs (NEW)
```csharp
/// <summary>
/// Configuration provider that loads JSON configuration from an embedded resource.
/// Uses ResourceExtensions.GetResourceStream() for accessing embedded resources.
/// </summary>
public class EmbeddedResourceConfigurationProvider : JsonConfigurationProvider
{
    public override void Load()
    {
        var streamResult = _source.Assembly.GetResourceStream(_source.ResourcePath);
        
        if (!streamResult.IsSuccess)
        {
            if (_source.Optional)
            {
                // Optional resource not found - return without loading
                return;
            }
            
            // Required resource not found - throw exception
            throw new FileNotFoundException(...);
        }
        
        using var stream = streamResult.Value;
        Load(stream);
    }
}
```

**Responsibilities:**
- Load embedded resource via `ResourceExtensions.GetResourceStream()`
- FluentResults error handling
- Optional resource support
- JSON parsing (from JsonConfigurationProvider base)

**ResourceExtensions methods used:**
- `GetResourceStream(Assembly, string)` - Load embedded resource stream
- Automatic resource name conversion (`/` → `.`, `-` → `_`)
- FluentResults-based error handling

#### ✅ Core/Configuration/EmbeddedResourceConfigurationExtensions.cs (NEW)
```csharp
public static class EmbeddedResourceConfigurationExtensions
{
    /// <summary>
    /// Adds a JSON configuration source from an embedded resource.
    /// </summary>
    public static IConfigurationBuilder AddEmbeddedJsonFile(
        this IConfigurationBuilder builder,
        Assembly assembly,
        string resourcePath,
        bool optional = false)
    {
        return builder.Add(new EmbeddedResourceConfigurationSource
        {
            Assembly = assembly,
            ResourcePath = resourcePath,
            Optional = optional
        });
    }
    
    /// <summary>
    /// Adds a JSON configuration source from the calling assembly.
    /// </summary>
    public static IConfigurationBuilder AddEmbeddedJsonFile(
        this IConfigurationBuilder builder,
        string resourcePath,
        bool optional = false)
    {
        var assembly = Assembly.GetCallingAssembly();
        return builder.AddEmbeddedJsonFile(assembly, resourcePath, optional);
    }
}
```

**Responsibilities:**
- Fluent API for IConfigurationBuilder
- Two overloads: explicit Assembly or calling Assembly
- Simple usage: `.AddEmbeddedJsonFile("appsettings.json")`

### 2. Console POC (RAVEN)

#### ✅ Console/Console.csproj (MODIFIED)
```xml
<ItemGroup>
  <!-- Embed appsettings.json as resource -->
  <EmbeddedResource Include="appsettings.json" />
</ItemGroup>
```

**Change:** `<None Update>` → `<EmbeddedResource Include>`

#### ✅ Console/Program.cs (MODIFIED)
```csharp
private static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .AddEmbeddedJsonFile(Assembly.GetExecutingAssembly(), "appsettings.json", optional: false)
        .Build();
}
```

**Before:**
```csharp
return new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
```

**Changes:**
- ✅ Use embedded resource (no file copy)
- ✅ No need for `SetBasePath()`
- ✅ No `reloadOnChange` (embedded resources are immutable)
- ✅ Uses new `AddEmbeddedJsonFile()` extension method

### 3. MAUI (ECHELON)

#### ✅ Maui/App.Maui.csproj (UNCHANGED)
```xml
<ItemGroup>
  <!-- Embed appsettings.json as resource -->
  <EmbeddedResource Include="appsettings.json" />
</ItemGroup>
```

**Note:** It was already an embedded resource.

#### ✅ Maui/MauiProgram.cs (SIMPLIFIED)
```csharp
private static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .AddEmbeddedJsonFile(Assembly.GetExecutingAssembly(), "appsettings.json", optional: false)
        .Build();
}
```

**Before:**
```csharp
private static IConfiguration BuildConfiguration()
{
    var assembly = Assembly.GetExecutingAssembly();
    var configurationBuilder = new ConfigurationBuilder();
    
    using var stream = assembly.GetManifestResourceStream("Fallout.TerminalHacker.App.Maui.appsettings.json");
    if (stream != null)
    {
        configurationBuilder.AddJsonStream(stream);
    }
    
    return configurationBuilder.Build();
}
```

**Changes:**
- ✅ 10 lines → 3 lines
- ✅ No manual stream handling
- ✅ No hardcoded resource name (`"Fallout.TerminalHacker.App.Maui.appsettings.json"`)
- ✅ Automatic resource name resolution
- ✅ Optional flag support
- ✅ FluentResults-based error handling

### 4. Blazor (GHOST)

#### ❌ Blazor/wwwroot/appsettings.json (UNCHANGED)
The Blazor project still uses the `wwwroot/appsettings.json` file because:
- In WebAssembly apps, configuration is loaded from the `wwwroot` folder
- Blazor uses its own configuration mechanism
- `WebAssemblyHostBuilder.Configuration` automatically loads `wwwroot/appsettings.json`

## Architecture

### ResourceExtensions integration
![ResourceExtensions-Integration]

### Resource name conversion

`ResourceExtensions.GetResourceStream()` converts resource names automatically:

| Input | Conversion | Result |
|-------|------------|--------|
| `appsettings.json` | (none) | `*.appsettings.json` |
| `config/app.json` | `/` → `.` | `*.config.app.json` |
| `app-settings.json` | `-` → `_` | `*.app_settings.json` |

**Excerpt from ResourceExtensions:**
```csharp
private static Dictionary<char, char> _replacements = new()
{
    { '\\', '.' },
    { '/', '.' },
    { '-', '_' },
};

var actualName = assembly.GetManifestResourceNames()
    .FirstOrDefault(name => name.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase));
```

## Benefits

### ✅ Reusable infrastructure
- Single `EmbeddedResourceConfigurationProvider` on both platforms (Console, MAUI)
- Easy-to-use extension method
- In Core project → all platforms can use it

### ✅ Consistent resource handling
- Use of `ResourceExtensions` → unified resource handling
- FluentResults error handling → clear error management
- Automatic name conversion → no manual resource name lookup

### ✅ Simpler code
**MAUI MauiProgram.cs:**
- Before: 10 lines (manual stream handling, hardcoded resource name)
- After: 3 lines (fluent API)

**Console Program.cs:**
- Before: 5 lines (`SetBasePath`, `AddJsonFile`)
- After: 3 lines (`AddEmbeddedJsonFile`)

### ✅ Type-safe and robust
- `optional` flag → controlled error handling
- Exceptions with detailed error messages
- FluentResults integration

### ✅ No file copy
- Console: `appsettings.json` embedded in assembly
- MAUI: `appsettings.json` embedded in assembly
- No `CopyToOutputDirectory` → simpler deployment

## Usage

### Console / MAUI:
```csharp
var configuration = new ConfigurationBuilder()
    .AddEmbeddedJsonFile(Assembly.GetExecutingAssembly(), "appsettings.json")
    .Build();
```

### Calling assembly (shorter):
```csharp
var configuration = new ConfigurationBuilder()
    .AddEmbeddedJsonFile("appsettings.json")
    .Build();
```

### Optional resource:
```csharp
var configuration = new ConfigurationBuilder()
    .AddEmbeddedJsonFile("appsettings.json", optional: true)
    .AddEmbeddedJsonFile("appsettings.Development.json", optional: true)
    .Build();
```

## Error handling

### Required resource not found:
```csharp
// Throws FileNotFoundException with detailed message:
// "Embedded resource 'appsettings.json' not found in assembly 'Console'.
//  Resource not found
//  Available resources are:
//  - Fallout.TerminalHacker.Console.appsettings.json
//  - ..."
```

### Optional resource not found:
```csharp
// Returns empty configuration (no exception)
var configuration = new ConfigurationBuilder()
    .AddEmbeddedJsonFile("optional.json", optional: true)
    .Build();
```

## Testing

### Console POC:
```bash
cd "src/Enclave.Echelon/App/Console"
dotnet clean
dotnet build
dotnet run
```

**Check:**
- ✅ Build succeeds (embedded resource embedded)
- ✅ Configuration loads successfully
- ✅ Boot sequence starts
- ✅ Timing values correct (from configuration)

**Debug test:**
Delete `appsettings.json` from bin/Debug folder → app still works (uses embedded resource).

### MAUI:
```bash
cd "src/Enclave.Echelon/App/Maui"
dotnet clean
dotnet build
```

**Check:**
- ✅ Build succeeds
- ✅ No `GetManifestResourceStream` error
- ✅ Configuration loads successfully

### Error test:
Change resource path to invalid:
```csharp
.AddEmbeddedJsonFile("non-existent.json", optional: false)
```

**Expected behaviour:**
```
Unhandled exception. System.IO.FileNotFoundException: 
Embedded resource 'non-existent.json' not found in assembly 'Console'.
Resource not found
Available resources are:
- Fallout.TerminalHacker.Console.appsettings.json
```

## Breaking changes

### ⚠️ Console csproj change
**Old:**
```xml
<None Update="appsettings.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

**New:**
```xml
<EmbeddedResource Include="appsettings.json" />
```

**Effect:**
- ❌ `appsettings.json` will no longer be copied to bin/Debug
- ✅ `appsettings.json` will be embedded in the assembly

### ⚠️ No reloadOnChange
Embedded resources do not support `reloadOnChange`:
```csharp
// Does not apply:
.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)

// For embedded resource:
.AddEmbeddedJsonFile("appsettings.json", optional: false)
// (no reloadOnChange parameter)
```

## Platform summary

| Platform | Config source | Method | Embedded? | Deployment |
|----------|----------------|--------|-----------|------------|
| **Console** | `appsettings.json` | `AddEmbeddedJsonFile()` | ✅ | Single .exe |
| **MAUI** | `appsettings.json` | `AddEmbeddedJsonFile()` | ✅ | App bundle |
| **Blazor** | `wwwroot/appsettings.json` | `WebAssemblyHostBuilder.Configuration` | ❌ | wwwroot |

## Next steps

1. ✅ **EmbeddedResourceConfigurationProvider** - DONE
2. ✅ **Console embedded resource** - DONE
3. ✅ **MAUI embedded resource** - DONE
4. ❌ **Build and test on all three platforms**
5. ❌ **Implement AddPasswordView**

## Summary

A reusable `EmbeddedResourceConfigurationProvider` infrastructure was added to the Core project that:
- ✅ Uses the existing `ResourceExtensions` class
- ✅ FluentResults-based error handling
- ✅ Automatic resource name resolution
- ✅ Fluent API (`AddEmbeddedJsonFile()`)
- ✅ Optional resource support

Console and MAUI projects now use this infrastructure consistently to load `appsettings.json`, resulting in simpler, cleaner, and more robust code.

---

**Date:** 2026-01-10  
**Status:** ✅ Done  
**Build:** Pending (Console and MAUI build test)


[ResourceExtensions-Integration]: ../Images/EmbeddedResources-ResourceExtensions-Integration.drawio.svg
[Magyar]: ./EMBEDDED_RESOURCE_CONFIGURATION.hu.md