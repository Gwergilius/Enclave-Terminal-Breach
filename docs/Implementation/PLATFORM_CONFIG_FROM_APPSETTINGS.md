# Platform Configuration from appsettings.json

**English** | [Magyar]

## Summary

All PlatformInfoService constants (`ProjectCodename`, `Version`, `PlatformName`, `Description`, `SystemModules`, `Applications`, `Timing`) now come from configuration (`appsettings.json`) in all three apps (Console, MAUI, Blazor). The `Core.Extensions.TimeSpanExtensions.ParseTimeUnit()` function is used to read TimeSpan values.

## Changes

### 1. New configuration classes (Core)

#### ✅ Core/Configuration/PlatformConfig.cs
```csharp
public class PlatformConfig
{
    public string ProjectCodename { get; set; }
    public string Version { get; set; }
    public string PlatformName { get; set; }
    public string Description { get; set; }
    public string[] SystemModules { get; set; }
    public string[] Applications { get; set; }
    public TimingConfig Timing { get; set; }
}

public class TimingConfig
{
    public string LineDelay { get; set; } = "150 ms";
    public string SlowDelay { get; set; } = "400 ms";
    public string OkStatusDelay { get; set; } = "200 ms";
    public string ProgressUpdate { get; set; } = "50 ms";
    public string ProgressDuration { get; set; } = "800 ms";
    public string WarningPause { get; set; } = "1 sec";
    public string FinalPause { get; set; } = "500 ms";
}
```

**Notes:**
- All timing values are stored as `string` (e.g. `"150 ms"`, `"1 sec"`)
- The `ParseTimeUnit()` extension method converts to `TimeSpan`
- Supported units: `ms`, `sec`/`s`, `min`/`m`, `h`

### 2. Console POC (RAVEN)

#### ✅ Console/appsettings.json
```json
{
  "Platform": {
    "ProjectCodename": "RAVEN",
    "Version": "v0.3.1",
    "PlatformName": "ROBCO TERMINAL NX-12",
    "Description": "Proof of Concept - First successful UOS breach implementation",
    "SystemModules": [
      "Detecting Enclave SIGINT module",
      "Validating cryptographic signature"
    ],
    "Applications": [
      "Loading RobCo UOS exploit database",
      "Calibrating signal intelligence modules",
      "Injecting stealth protocol handlers",
      "Activating simple cipher algorithms",
      "Synchronizing with NEST mainframe",
      "Initializing brute force attack vectors"
    ],
    "Timing": {
      "LineDelay": "200 ms",
      "SlowDelay": "500 ms",
      "OkStatusDelay": "250 ms",
      "ProgressUpdate": "0 ms",
      "ProgressDuration": "0 ms",
      "WarningPause": "1200 ms",
      "FinalPause": "600 ms"
    }
  }
}
```

**Timing characteristics:**
- Slower than ECHELON (older hardware)
- `ProgressUpdate` and `ProgressDuration` = `0 ms` (no progress bar)

#### ✅ Console/Services/ConsolePlatformInfoService.cs
```csharp
private readonly PlatformConfig _config;

public ConsolePlatformInfoService(PlatformConfig config)
{
    _config = config ?? throw new ArgumentNullException(nameof(config));
}

public TimeSpan LineDelay => _config.Timing.LineDelay.ParseTimeUnit();
public TimeSpan SlowDelay => _config.Timing.SlowDelay.ParseTimeUnit();
// ... etc.
```

#### ✅ Console/Program.cs
```csharp
// Configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Bind PlatformConfig from appsettings.json
var platformConfig = new PlatformConfig();
configuration.GetSection("Platform").Bind(platformConfig);
services.AddSingleton(platformConfig);
```

#### ✅ Console/Console.csproj
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Configuration" />
  <PackageReference Include="Microsoft.Extensions.Configuration.Json" />
  <PackageReference Include="Microsoft.Extensions.Configuration.Binder" />
</ItemGroup>

<ItemGroup>
  <None Update="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### 3. MAUI (ECHELON)

#### ✅ Maui/appsettings.json
```json
{
  "Platform": {
    "ProjectCodename": "ECHELON",
    "Version": "v2.1.7",
    "PlatformName": "Pip-Boy 3000 Mark IV",
    "Description": "Operational Deployment - Final pre-war version (October 2077)",
    "SystemModules": [...],
    "Applications": [...],
    "Timing": {
      "LineDelay": "150 ms",
      "SlowDelay": "400 ms",
      "OkStatusDelay": "200 ms",
      "ProgressUpdate": "50 ms",
      "ProgressDuration": "800 ms",
      "WarningPause": "1 sec",
      "FinalPause": "500 ms"
    }
  }
}
```

**Timing characteristics:**
- Fast, optimised (modern Pip-Boy hardware)
- Has progress bar animation

#### ✅ Maui/Services/MauiPlatformInfoService.cs
Same structure as Console, with ECHELON configuration.

### 4. Blazor (GHOST)

#### ✅ Web/appsettings.json
```json
{
  "Platform": {
    "ProjectCodename": "GHOST",
    "Version": "v1.2.4",
    "PlatformName": "Web Browser (SIGNET Access)",
    "Description": "Field Prototype - Ghost Revised (Neural pattern recognition)",
    "SystemModules": [...],
    "Applications": [...],
    "Timing": {
      "LineDelay": "175 ms",
      "SlowDelay": "450 ms",
      "OkStatusDelay": "225 ms",
      "ProgressUpdate": "60 ms",
      "ProgressDuration": "900 ms",
      "WarningPause": "1100 ms",
      "FinalPause": "550 ms"
    }
  }
}
```

**Timing characteristics:**
- Medium (web browser, field prototype)
- Has progress bar animation

#### ✅ Web/Services/BlazorPlatformInfoService.cs
Same structure as Console, with GHOST configuration.

## TimeSpan parsing examples

The `ParseTimeUnit()` extension method supports these formats:

```csharp
"150 ms"   → TimeSpan.FromMilliseconds(150)
"1 sec"    → TimeSpan.FromSeconds(1)
"1 s"      → TimeSpan.FromSeconds(1)
"5 min"    → TimeSpan.FromMinutes(5)
"5 m"      → TimeSpan.FromMinutes(5)
"2 h"      → TimeSpan.FromHours(2)
"0 ms"     → TimeSpan.Zero
```

**Regex pattern:**
```regex
^(?<ValueGroup>[0-9]*\.?[0-9]*)\s*(?<UnitGroup>ms|sec|s|min|m|h)$
```

**Example usage:**
```json
"LineDelay": "150 ms"
```

```csharp
public TimeSpan LineDelay => _config.Timing.LineDelay.ParseTimeUnit();
// Result: TimeSpan.FromMilliseconds(150)
```

## Benefits

### ✅ Easy configuration
- All platform-specific data in one place (appsettings.json)
- No hardcoded values in code
- Easy to change timing and texts

### ✅ Readable timing values
- `"150 ms"` vs. `TimeSpan.FromMilliseconds(150)`
- Units make sense: `"1 sec"`, `"500 ms"`
- Easy to edit in JSON

### ✅ Platform-specific values
- RAVEN: slow timing, simple modules
- ECHELON: fast timing, complex modules
- GHOST: medium timing, full modules

### ✅ Centralised configuration
- DI setup consistent on all platforms
- PlatformConfig binding pattern reusable
- Easy to extend with new config values

## TestPlatformInfoService

`TestPlatformInfoService` is unchanged; unit tests do not need appsettings.json. It still uses hardcoded 1ms timing values.

```csharp
public TimeSpan LineDelay { get; set; } = TimeSpan.FromMilliseconds(1);
```

## Requirements

### NuGet packages (all three apps):
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.Configuration.Json`
- `Microsoft.Extensions.Configuration.Binder`

### File copy on build:
```xml
<None Update="appsettings.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

## Testing

### Console POC:
```bash
cd "src/Enclave.Echelon/App/Console"
dotnet build
dotnet run
```

**Check:**
- Boot sequence timing slower than before (200ms vs 150ms)
- "ROBCO TERMINAL NX-12" appears
- "PROJECT RAVEN" header
- 6 application modules load

### Timing change test:
Modify `appsettings.json`:
```json
"LineDelay": "500 ms"  // Very slow
```

Run the app again → boot sequence will be noticeably slower.

## Breaking changes

### ⚠️ PlatformInfoService constructor change

**Old:**
```csharp
public ConsolePlatformInfoService()
{
    // Hardcoded values
}
```

**New:**
```csharp
public ConsolePlatformInfoService(PlatformConfig config)
{
    _config = config ?? throw new ArgumentNullException(nameof(config));
}
```

**Migration:**
- Every PlatformInfoService implementation needs a `PlatformConfig` constructor parameter
- Register `PlatformConfig` as singleton in DI
- appsettings.json file required in all three apps

## Next steps

1. ✅ **Configuration moved to appsettings.json** - DONE
2. ❌ **Update MAUI DI setup** - Pending (MauiProgram.cs)
3. ❌ **Update Blazor DI setup** - Pending (Startup.cs or Program.cs)
4. ❌ **Implement AddPasswordView**
5. ❌ **Full gameplay testing**

## Summary

All PlatformInfoService constants now come from configuration (`appsettings.json`). The `ParseTimeUnit()` extension method is used to read TimeSpan values and supports readable unit-based format (e.g. `"150 ms"`, `"1 sec"`).

Each platform (Console, MAUI, Blazor) has its own appsettings.json containing:
- Project info (codename, version, platform name)
- Boot sequence texts (system modules, applications)
- Timing constants (line delay, slow delay, etc.)

This results in a cleaner architecture and easier maintenance!

---

**Date:** 2026-01-10  
**Status:** ✅ Console done, MAUI/Blazor DI setup pending  
**Build:** Pending (Console build and test required)

[Magyar]: ./PLATFORM_CONFIG_FROM_APPSETTINGS.hu.md