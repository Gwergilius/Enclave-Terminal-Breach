# Platform Configuration from appsettings.json

[English] | **Magyar**

## Összefoglaló

A PlatformInfoService összes konstansa (`ProjectCodename`, `Version`, `PlatformName`, `Description`, `SystemModules`, `Applications`, `Timing`) most már konfigurációból (`appsettings.json`) érkezik mindhárom App-ban (Console, MAUI, Blazor). A TimeSpan értékek beolvasására a `Core.Extensions.TimeSpanExtensions.ParseTimeUnit()` függvényt használjuk.

## Változások

### 1. Új Configuration osztályok (Core)

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

**Megjegyzések:**
- Minden timing érték `string` formátumban van tárolva (pl. `"150 ms"`, `"1 sec"`)
- A `ParseTimeUnit()` extension method konvertálja `TimeSpan`-né
- Támogatott mértékegységek: `ms`, `sec`/`s`, `min`/`m`, `h`

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

**Timing jellemzők:**
- Lassabb, mint ECHELON (régebbi hardware)
- `ProgressUpdate` és `ProgressDuration` = `0 ms` (nincs progress bar)

#### ✅ Console/Services/ConsolePlatformInfoService.cs
```csharp
private readonly PlatformConfig _config;

public ConsolePlatformInfoService(PlatformConfig config)
{
    _config = config ?? throw new ArgumentNullException(nameof(config));
}

public TimeSpan LineDelay => _config.Timing.LineDelay.ParseTimeUnit();
public TimeSpan SlowDelay => _config.Timing.SlowDelay.ParseTimeUnit();
// ... stb.
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

**Timing jellemzők:**
- Gyors, optimalizált (modern Pip-Boy hardware)
- Van progress bar animáció

#### ✅ Maui/Services/MauiPlatformInfoService.cs
Hasonló szerkezet, mint a Console, de ECHELON konfigurációval.

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

**Timing jellemzők:**
- Közepes (web browser, field prototype)
- Van progress bar animáció

#### ✅ Web/Services/BlazorPlatformInfoService.cs
Hasonló szerkezet, mint a Console, de GHOST konfigurációval.

## TimeSpan parsing példák

A `ParseTimeUnit()` extension method a következő formátumokat támogatja:

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

**Példa használat:**
```json
"LineDelay": "150 ms"
```

```csharp
public TimeSpan LineDelay => _config.Timing.LineDelay.ParseTimeUnit();
// Result: TimeSpan.FromMilliseconds(150)
```

## Előnyök

### ✅ Könnyű konfiguráció
- Minden platform-specifikus adat egy helyen (appsettings.json)
- Nincs hardcoded érték a kódban
- Könnyen módosítható timing és szövegek

### ✅ Olvasható timing értékek
- `"150 ms"` vs. `TimeSpan.FromMilliseconds(150)`
- Mértékegységgel érthető: `"1 sec"`, `"500 ms"`
- JSON-ban egyszerűen szerkeszthető

### ✅ Platform-specifikus értékek
- RAVEN: lassú timing, egyszerű modulok
- ECHELON: gyors timing, komplex modulok
- GHOST: közepes timing, teljes modulok

### ✅ Centralizált konfiguráció
- DI setup egységes minden platformon
- PlatformConfig binding pattern újrafelhasználható
- Könnyen bővíthető új konfigurációs értékekkel

## TestPlatformInfoService

A `TestPlatformInfoService` nem változott, mert unit tesztekben nem szükséges appsettings.json. Továbbra is hardcoded 1ms timing értékeket használ.

```csharp
public TimeSpan LineDelay { get; set; } = TimeSpan.FromMilliseconds(1);
```

## Követelmények

### NuGet Packages (mindhárom App):
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.Configuration.Json`
- `Microsoft.Extensions.Configuration.Binder`

### File másolás build során:
```xml
<None Update="appsettings.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

## Tesztelés

### Console POC:
```bash
cd "src/Enclave.Echelon/App/Console"
dotnet build
dotnet run
```

**Ellenőrizd:**
- Boot sequence timing lassabb, mint korábban (200ms vs 150ms)
- "ROBCO TERMINAL NX-12" jelenik meg
- "PROJECT RAVEN" header
- 6 alkalmazás modul töltődik be

### Timing módosítás teszt:
Módosítsd az `appsettings.json`-t:
```json
"LineDelay": "500 ms"  // Nagyon lassú
```

Futtasd újra az alkalmazást → Boot sequence észrevehetően lassabb lesz.

## Breaking Changes

### ⚠️ PlatformInfoService konstruktor változás

**Régi:**
```csharp
public ConsolePlatformInfoService()
{
    // Hardcoded értékek
}
```

**Új:**
```csharp
public ConsolePlatformInfoService(PlatformConfig config)
{
    _config = config ?? throw new ArgumentNullException(nameof(config));
}
```

**Migráció:**
- Minden PlatformInfoService implementációnak szüksége van `PlatformConfig` konstruktor paraméterre
- DI-ban a `PlatformConfig`-ot regisztrálni kell singleton-ként
- appsettings.json fájl kötelező mindhárom App-ban

## Következő lépések

1. ✅ **Konfiguráció átrakva appsettings.json-be** - KÉSZ
2. ❌ **MAUI DI setup frissítése** - Függőben (MauiProgram.cs)
3. ❌ **Blazor DI setup frissítése** - Függőben (Startup.cs vagy Program.cs)
4. ❌ **AddPasswordView implementálása**
5. ❌ **Teljes játékmenet tesztelés**

## Összefoglalás

A PlatformInfoService összes konstansa most már konfigurációból érkezik (`appsettings.json`). A TimeSpan értékek beolvasására a `ParseTimeUnit()` extension method-ot használjuk, amely lehetővé teszi olvasható mértékegységes formátumot (pl. `"150 ms"`, `"1 sec"`).

Minden platform (Console, MAUI, Blazor) saját appsettings.json fájllal rendelkezik, ami tartalmazza:
- Projekt információkat (codename, version, platform name)
- Boot sequence szövegeket (system modules, applications)
- Timing konstansokat (line delay, slow delay, stb.)

Ez tisztább architektúrát és könnyebb karbantarthatóságot eredményez!

---

**Dátum:** 2026-01-10  
**Státusz:** ✅ Console kész, MAUI/Blazor DI setup függőben  
**Build:** Függőben (Console build és teszt szükséges)

[English]: ./PLATFORM_CONFIG_FROM_APPSETTINGS.md