# Embedded Resource Configuration Provider

[English] | **Magyar**

## Összefoglaló

Létrehoztunk egy újrafelhasználható `EmbeddedResourceConfigurationProvider` implementációt a Core projektben, amely lehetővé teszi a konfiguráció betöltését Embedded Resource-okból. A Console és MAUI projektek most is ezt használják az `appsettings.json` betöltéséhez.

## Változások

### 1. Core - Configuration Infrastructure

#### ✅ Core/Configuration/EmbeddedResourceConfigurationSource.cs (ÚJ)
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

**Felelősség:**
- Konfiguráció forrás definíciója
- Assembly, resource path, optional flag tárolása
- Provider létrehozása

#### ✅ Core/Configuration/EmbeddedResourceConfigurationProvider.cs (ÚJ)
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

**Felelősség:**
- Embedded resource betöltése `ResourceExtensions.GetResourceStream()` használatával
- FluentResults error handling
- Optional resource támogatás
- JSON parsing (JsonConfigurationProvider ősosztálytól)

**Használt ResourceExtensions metódusok:**
- `GetResourceStream(Assembly, string)` - Embedded resource stream betöltése
- Automatikus resource név konverzió (`/` → `.`, `-` → `_`)
- FluentResults alapú error handling

#### ✅ Core/Configuration/EmbeddedResourceConfigurationExtensions.cs (ÚJ)
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

**Felelősség:**
- Fluent API biztosítása az IConfigurationBuilder-hez
- Két overload: explicit Assembly vagy calling Assembly
- Egyszerű használat: `.AddEmbeddedJsonFile("appsettings.json")`

### 2. Console POC (RAVEN)

#### ✅ Console/Console.csproj (MÓDOSÍTVA)
```xml
<ItemGroup>
  <!-- Embed appsettings.json as resource -->
  <EmbeddedResource Include="appsettings.json" />
</ItemGroup>
```

**Változás:** `<None Update>` → `<EmbeddedResource Include>`

#### ✅ Console/Program.cs (MÓDOSÍTVA)
```csharp
private static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .AddEmbeddedJsonFile(Assembly.GetExecutingAssembly(), "appsettings.json", optional: false)
        .Build();
}
```

**Előtte:**
```csharp
return new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
```

**Változások:**
- ✅ Embedded resource használata (nem kell file copy)
- ✅ Nincs szükség `SetBasePath()`-ra
- ✅ Nincs `reloadOnChange` (embedded resource-ok nem változhatnak)
- ✅ Használja az új `AddEmbeddedJsonFile()` extension method-ot

### 3. MAUI (ECHELON)

#### ✅ Maui/App.Maui.csproj (NEM VÁLTOZOTT)
```xml
<ItemGroup>
  <!-- Embed appsettings.json as resource -->
  <EmbeddedResource Include="appsettings.json" />
</ItemGroup>
```

**Megjegyzés:** Már korábban is embedded resource volt.

#### ✅ Maui/MauiProgram.cs (EGYSZERŰSÍTVE)
```csharp
private static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .AddEmbeddedJsonFile(Assembly.GetExecutingAssembly(), "appsettings.json", optional: false)
        .Build();
}
```

**Előtte:**
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

**Változások:**
- ✅ 10 sor → 3 sor
- ✅ Nincs manuális stream kezelés
- ✅ Nincs hardcoded resource név (`"Fallout.TerminalHacker.App.Maui.appsettings.json"`)
- ✅ Automatikus resource név feloldás
- ✅ Optional flag támogatás
- ✅ FluentResults alapú error handling

### 4. Blazor (GHOST)

#### ❌ Blazor/wwwroot/appsettings.json (NEM VÁLTOZOTT)
A Blazor projekt továbbra is a `wwwroot/appsettings.json` fájlt használja, mert:
- WebAssembly alkalmazásokban a konfiguráció a `wwwroot` mappából töltődik be
- Blazor sajátos konfigurációs mechanizmust használ
- A `WebAssemblyHostBuilder.Configuration` automatikusan betölti a `wwwroot/appsettings.json`-t

## Architektúra

### ResourceExtensions integráció

![ResourceExtensions-Integration]

### Resource Name Conversion

`ResourceExtensions.GetResourceStream()` automatikusan konvertálja a resource neveket:

| Input | Conversion | Result |
|-------|------------|--------|
| `appsettings.json` | (none) | `*.appsettings.json` |
| `config/app.json` | `/` → `.` | `*.config.app.json` |
| `app-settings.json` | `-` → `_` | `*.app_settings.json` |

**Részlet a ResourceExtensions-ből:**
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

## Előnyök

### ✅ Újrafelhasználható infrastructure
- Egyetlen `EmbeddedResourceConfigurationProvider` mindkét platformon (Console, MAUI)
- Könnyen használható extension method
- Core projektben van → minden platform használhatja

### ✅ Konzisztens resource handling
- `ResourceExtensions` használata → egységes resource kezelés
- FluentResults error handling → tiszta error menedzsment
- Automatikus név konverzió → nem kell manuálisan resource nevet keresni

### ✅ Egyszerűbb kód
**MAUI MauiProgram.cs:**
- Előtte: 10 sor (manuális stream kezelés, hardcoded resource név)
- Utána: 3 sor (fluent API)

**Console Program.cs:**
- Előtte: 5 sor (`SetBasePath`, `AddJsonFile`)
- Utána: 3 sor (`AddEmbeddedJsonFile`)

### ✅ Type-safe és robust
- `optional` flag → kontrollált error handling
- Exception-ök részletes error message-ekkel
- FluentResults integráció

### ✅ Nincs file copy
- Console: `appsettings.json` beépül az assembly-be
- MAUI: `appsettings.json` beépül az assembly-be
- Nincs `CopyToOutputDirectory` → egyszerűbb deployment

## Használat

### Console / MAUI:
```csharp
var configuration = new ConfigurationBuilder()
    .AddEmbeddedJsonFile(Assembly.GetExecutingAssembly(), "appsettings.json")
    .Build();
```

### Calling Assembly (rövidebb):
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

## Error Handling

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

## Tesztelés

### Console POC:
```bash
cd "src/Enclave.Echelon/App/Console"
dotnet clean
dotnet build
dotnet run
```

**Ellenőrizd:**
- ✅ Build sikeres (embedded resource beágyazása)
- ✅ Configuration betöltés sikeres
- ✅ Boot sequence indul
- ✅ Timing értékek helyesek (konfigurációból)

**Debug teszt:**
Törölheted az `appsettings.json` fájlt a bin/Debug könyvtárból → az alkalmazás továbbra is működik (embedded resource-ot használ).

### MAUI:
```bash
cd "src/Enclave.Echelon/App/Maui"
dotnet clean
dotnet build
```

**Ellenőrizd:**
- ✅ Build sikeres
- ✅ Nincs `GetManifestResourceStream` error
- ✅ Configuration betöltés sikeres

### Error teszt:
Módosítsd a resource path-ot hibásra:
```csharp
.AddEmbeddedJsonFile("non-existent.json", optional: false)
```

**Elvárt viselkedés:**
```
Unhandled exception. System.IO.FileNotFoundException: 
Embedded resource 'non-existent.json' not found in assembly 'Console'.
Resource not found
Available resources are:
- Fallout.TerminalHacker.Console.appsettings.json
```

## Breaking Changes

### ⚠️ Console csproj változás
**Régi:**
```xml
<None Update="appsettings.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

**Új:**
```xml
<EmbeddedResource Include="appsettings.json" />
```

**Hatás:**
- ❌ `appsettings.json` már nem lesz kimásolva a bin/Debug mappába
- ✅ `appsettings.json` be lesz építve az assembly-be

### ⚠️ Nincs reloadOnChange
Embedded resource-ok nem támogatják a `reloadOnChange` funkciót:
```csharp
// Nem működik:
.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)

// Embedded resource esetén:
.AddEmbeddedJsonFile("appsettings.json", optional: false)
// (nincs reloadOnChange paraméter)
```

## Platform Summary

| Platform | Config Source | Method | Embedded? | Deployment |
|----------|---------------|--------|-----------|------------|
| **Console** | `appsettings.json` | `AddEmbeddedJsonFile()` | ✅ | Single .exe |
| **MAUI** | `appsettings.json` | `AddEmbeddedJsonFile()` | ✅ | App bundle |
| **Blazor** | `wwwroot/appsettings.json` | `WebAssemblyHostBuilder.Configuration` | ❌ | wwwroot |

## Következő lépések

1. ✅ **EmbeddedResourceConfigurationProvider** - KÉSZ
2. ✅ **Console embedded resource** - KÉSZ
3. ✅ **MAUI embedded resource** - KÉSZ
4. ❌ **Build és teszt mindhárom platformon**
5. ❌ **AddPasswordView implementálása**

## Összefoglalás

Létrehoztunk egy újrafelhasználható `EmbeddedResourceConfigurationProvider` infrastruktúrát a Core projektben, amely:
- ✅ Használja a meglévő `ResourceExtensions` osztályt
- ✅ FluentResults alapú error handling
- ✅ Automatikus resource név feloldás
- ✅ Fluent API (`AddEmbeddedJsonFile()`)
- ✅ Optional resource támogatás

A Console és MAUI projektek most egységesen használják ezt az infrastructure-t az `appsettings.json` betöltéséhez, ami egyszerűbb, tisztább és robusztusabb kódot eredményez.

---

**Dátum:** 2026-01-10  
**Státusz:** ✅ Kész  
**Build:** Függőben (Console és MAUI build tesztelés)

[ResourceExtensions-Integration]: ../Images/EmbeddedResources-ResourceExtensions-Integration.drawio.svg
[English]: ./EMBEDDED_RESOURCE_CONFIGURATION.md