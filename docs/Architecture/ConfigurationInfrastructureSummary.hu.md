# Konfigurációs infrastruktúra – összefoglaló

**[English]** | Magyar

Az ECHELON projekt konfigurációval kapcsolatos fejlesztései: egységes, karbantartható, típusbiztos konfigurációs infrastruktúra.

## Változások

1. Platform-specifikus boot időzítés → IPlatformInfoService  
2. Konfiguráció appsettings.json-ból  
3. Startup osztály architektúra  
4. Beágyazott erőforrás konfiguráció  
5. MAUI betűtípus konfiguráció  

## Architektúra

appsettings.json (beágyazott) → EmbeddedResourceConfigurationProvider → IConfiguration → Startup.cs (PlatformConfig binding, szolgáltatás regisztráció) → PlatformInfoService (Console/MAUI/Blazor) → Boot szekvencia fázisok.

## Core

**Core/Configuration/:** PlatformConfig.cs, TimingConfig.cs, EmbeddedResourceConfigurationSource/Provider/Extensions.  
**Core/Services:** IPlatformInfoService – ProjectCodename, Version, PlatformName, Description, SystemModules, Applications, LineDelay, SlowDelay, OkStatusDelay, ProgressUpdate, ProgressDuration, WarningPause, FinalPause.

## Platformok

**Console (RAVEN):** beágyazott appsettings.json, AddEmbeddedJsonFile, lassabb időzítés, ProgressUpdate 0 (nincs progress bar).  
**MAUI (ECHELON):** beágyazott appsettings, gyors időzítés, Fonts szekció, 50 ms progress.  
**Blazor (GHOST):** wwwroot/appsettings.json, közepes időzítés, 60 ms progress.

## Előnyök

Típusbiztos konfig, platform-specifikus időzítés, lore-konzisztens különbségek, központosított DI, nincs beégetett érték. Részletes táblázatok, JSON/C# példák és migráció: [angol verzió][English].

[English]: ./ConfigurationInfrastructureSummary.md
