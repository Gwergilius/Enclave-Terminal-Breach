# Konfigurációs infrastruktúra – összefoglaló

**[English]** | Magyar

## Áttekintés

Az ECHELON projekt konfigurációval kapcsolatos fejlesztései: egységes, karbantartható, típusbiztos konfigurációs infrastruktúra.

## Változások idővonala

1. ✅ **Platform-specifikus boot időzítés** – időzítési konstansok áthelyezve az `IPlatformInfoService`-be
2. ✅ **Konfiguráció appsettings.json-ból** – platform konfigurációk JSON fájlból
3. ✅ **Startup osztály architektúra** – központosított DI a `Startup` osztályokban
4. ✅ **Beágyazott erőforrás konfiguráció** – újrafelhasználható konfigurációs provider infrastruktúra
5. ✅ **MAUI betűtípus konfiguráció** – betűk betöltése appsettings.json-ból

## Végső architektúra

![Végső architektúra][Final Architecture]

## Core infrastruktúra

### 1. Konfigurációs osztályok

**Hely:** `Core/Configuration/`

| Fájl | Cél |
|------|-----|
| `PlatformConfig.cs` | Platform beállítások konfigurációs modellje |
| `TimingConfig.cs` | Időzítés (PlatformConfig alatt) |
| `EmbeddedResourceConfigurationSource.cs` | Beágyazott erőforrások forrása |
| `EmbeddedResourceConfigurationProvider.cs` | Provider beágyazott erőforrásból |
| `EmbeddedResourceConfigurationExtensions.cs` | Fluent API: `AddEmbeddedJsonFile()` |

### 2. Platform szolgáltatások

**Hely:** `Core/Services/IPlatformInfoService.cs`

```csharp
public interface IPlatformInfoService
{
    string ProjectCodename { get; }
    string Version { get; }
    string PlatformName { get; }
    string Description { get; }
    string[] SystemModules { get; }
    string[] Applications { get; }
    TimeSpan LineDelay { get; }
    TimeSpan SlowDelay { get; }
    TimeSpan OkStatusDelay { get; }
    TimeSpan ProgressUpdate { get; }
    TimeSpan ProgressDuration { get; }
    TimeSpan WarningPause { get; }
    TimeSpan FinalPause { get; }
}
```

## Platform implementációk

### Console POC (RAVEN)

Beágyazott `appsettings.json`, `AddEmbeddedJsonFile()`, lassabb időzítés (régebbi hardver), nincs progress bar (`ProgressUpdate` = 0 ms).

### MAUI (ECHELON)

Beágyazott `appsettings.json`, gyors időzítés, Fonts szekció (OpenSans, FixedsysExcelsior), 50 ms progress.

### Blazor (GHOST)

`wwwroot/appsettings.json`, közepes időzítés, 60 ms progress, hálózati megfontolásokkal.

## Platform összehasonlítás

| Jellemző | Console (RAVEN) | MAUI (ECHELON) | Blazor (GHOST) |
|----------|-----------------|----------------|----------------|
| **Konfig helye** | Beágyazott | Beágyazott | wwwroot |
| **LineDelay** | 200 ms | 150 ms | 175 ms |
| **Progress** | ❌ (0 ms) | ✅ (50 ms) | ✅ (60 ms) |
| **Fonts config** | ❌ | ✅ | ❌ (CSS) |

## Előnyök

- Típusbiztos konfiguráció, platform-specifikus időzítés, lore-konzisztens különbségek (RAVEN lassabb, ECHELON gyors, GHOST közepes)
- Központosított DI (Startup minta), újrafelhasználható infrastruktúra, nincs beégetett érték

Részletes táblázatok, JSON/C# példák és migráció: [angol verzió][English].

[English]: ./ConfigurationInfrastructureSummary.md
[Final Architecture]: ../Images/ConfigurationInfrastructureSummary-FinalArchitecture.drawio.svg
