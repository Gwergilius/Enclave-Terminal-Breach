# MAUI Font Configuration from appsettings.json

[English] | **Magyar**

## Összefoglaló

A MAUI alkalmazásban a fontok konfigurációját áthelyeztük az `appsettings.json` fájlba, így nincs szükség hardcoded font regisztrációra a `MauiProgram.cs`-ben.

## Változások

### 1. PlatformConfig bővítése

#### ✅ Core/Configuration/PlatformConfig.cs
```csharp
public class PlatformConfig
{
    // ... existing properties
    
    /// <summary>
    /// Font configuration (font name => file path)
    /// Only used by MAUI platform
    /// </summary>
    public Dictionary<string, string> Fonts { get; set; } = new();
}
```

**Tulajdonságok:**
- **Key (string)**: Font alias name (pl. `"OpenSansRegular"`)
- **Value (string)**: Font file path (pl. `"OpenSans-Regular.ttf"`)

### 2. MAUI appsettings.json

#### ✅ Maui/appsettings.json
```json
{
  "Platform": {
    "ProjectCodename": "ECHELON",
    "Version": "v2.1.7",
    "Timing": { ... },
    "Fonts": {
      "OpenSansRegular": "OpenSans-Regular.ttf",
      "OpenSansSemibold": "OpenSans-Semibold.ttf",
      "FixedsysExcelsior": "FixedsysExcelsior.ttf"
    },
    "DefaultFont": "FixedsysExcelsior"
  }
}
```

**Font bejegyzések:**
- **OpenSansRegular**: Standard font a MAUI Controls számára
- **OpenSansSemibold**: Félkövér változat
- **FixedsysExcelsior**: Fallout-stílusú monospace font a terminal megjelenítéshez

### 3. MauiProgram.cs frissítése

#### ✅ ELŐTTE (Hardcoded):
```csharp
builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        fonts.AddFont("FixedsysExcelsior.ttf", "FixedsysExcelsior");
    });
```

#### ✅ UTÁNA (Configuration-based):
```csharp
// Build configuration first (needed for fonts)
var configuration = BuildConfiguration();

// Extract platform configuration for fonts
var platformConfig = new PlatformConfig();
configuration.GetSection("Platform").Bind(platformConfig);

builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts =>
    {
        // Load fonts from configuration
        foreach (var font in platformConfig.Fonts)
        {
            fonts.AddFont(font.Value, font.Key);
        }
    });
```

**Változások:**
1. ✅ Configuration betöltés **előbb** történik (fonts előtt)
2. ✅ PlatformConfig binding a font konfigurációhoz
3. ✅ Dinamikus font regisztráció `foreach` ciklussal
4. ✅ `font.Value` = file path (pl. `"OpenSans-Regular.ttf"`)
5. ✅ `font.Key` = font alias (pl. `"OpenSansRegular"`)

## Előnyök

### ✅ Nincs hardcoded érték
```csharp
// Nem kell:
fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");

// Helyette:
// Minden font a konfigurációból
```

### ✅ Könnyű új font hozzáadása
```json
// Csak adj hozzá egy új bejegyzést:
"Fonts": {
  "OpenSansRegular": "OpenSans-Regular.ttf",
  "MyCustomFont": "MyFont.ttf"  // <- Új font
}
```

### ✅ Centralizált konfiguráció
Minden platform-specifikus adat egy helyen (`appsettings.json`):
- Project information
- Boot sequence texts
- Timing values
- **Fonts** (MAUI only)

### ✅ Platformfüggő konfiguráció
- Console/Blazor: Nincs Fonts section (nem használják)
- MAUI: Van Fonts section (egyedi font kezelés)

## Platform-specifikus megjegyzések

### MAUI (ECHELON)
✅ **Használja a Fonts konfigurációt**
- Fonts section kötelező az appsettings.json-ben
- Font fájlok a `Resources/Fonts/` mappában
- `.csproj`-ben: `<MauiFont Include="Resources\Fonts\*" />`

### Console (RAVEN)
❌ **Nem használja a Fonts konfigurációt**
- Nincs Fonts section az appsettings.json-ben
- Terminál alapértelmezett fontját használja
- Font konfiguráció nem releváns

### Blazor (GHOST)
❌ **Nem használja a Fonts konfigurációt**
- Nincs Fonts section az appsettings.json-ben
- CSS-ben definiált `@font-face` szabályokat használ
- Font konfiguráció nem releváns

## Font fájlok kezelése

### MAUI .csproj
```xml
<ItemGroup>
  <!-- Custom Fonts -->
  <MauiFont Include="Resources\Fonts\*" />
</ItemGroup>
```

**Font fájlok helye:**
```
App/Maui/
  └── Resources/
      └── Fonts/
          ├── OpenSans-Regular.ttf
          ├── OpenSans-Semibold.ttf
          └── FixedsysExcelsior.ttf
```

### Font használata XAML-ben
```xaml
<Label 
    Text="ECHELON Terminal"
    FontFamily="OpenSansRegular"
    FontSize="14" />

<Label 
    Text="SYSTEM READY"
    FontFamily="FixedsysExcelsior"
    FontSize="12" />
```

## Tesztelés

### Build teszt:
```bash
cd "src/Enclave.Echelon/App/Maui"
dotnet build
```

**Ellenőrizd:**
- ✅ Build sikeres
- ✅ Nincs font-related error
- ✅ Configuration binding működik

### Runtime teszt:
```bash
# Android emulator vagy device-on
dotnet build -t:Run -f net10.0-android
```

**Ellenőrizd:**
- ✅ Alkalmazás elindul
- ✅ Fontok megfelelően renderelnek
- ✅ OpenSansRegular használva a UI-ban
- ✅ FixedsysExcelsior használva a terminal nézetben

### Configuration teszt:
Módosítsd az appsettings.json-t:
```json
"Fonts": {
  "TestFont": "OpenSans-Regular.ttf"
}
```

**Elvárt viselkedés:**
- ✅ Build sikeres
- ✅ Csak a "TestFont" alias regisztrálva
- ⚠️ "OpenSansRegular" nem található (ha a XAML-ben használod)

## Hibaelhárítás

### Probléma: "Font not found"
**Ok:** Font alias nem egyezik a konfigurációban szereplő key-vel.

**Megoldás:**
```json
// appsettings.json:
"Fonts": {
  "MyFont": "MyFont.ttf"  // <- Key = "MyFont"
}
```

```xaml
<!-- XAML: -->
<Label FontFamily="MyFont" />  <!-- <- Ugyanaz -->
```

### Probléma: "Font file not found"
**Ok:** Font fájl path hibás vagy a fájl hiányzik.

**Megoldás:**
1. Ellenőrizd a fájl létezését: `Resources/Fonts/MyFont.ttf`
2. Ellenőrizd a .csproj-ben: `<MauiFont Include="Resources\Fonts\*" />`
3. Ellenőrizd a path-t az appsettings.json-ben

### Probléma: Configuration binding sikertelen
**Ok:** appsettings.json formátum hiba vagy embedded resource probléma.

**Megoldás:**
```csharp
// Debug log hozzáadása:
var platformConfig = new PlatformConfig();
configuration.GetSection("Platform").Bind(platformConfig);

Console.WriteLine($"Fonts loaded: {platformConfig.Fonts.Count}");
foreach (var font in platformConfig.Fonts)
{
    Console.WriteLine($"  {font.Key} => {font.Value}");
}
```

## Breaking Changes

### ⚠️ MauiProgram.cs változás
**Régi:**
```csharp
builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts => { ... });

var configuration = BuildConfiguration();
```

**Új:**
```csharp
var configuration = BuildConfiguration();
var platformConfig = new PlatformConfig();
configuration.GetSection("Platform").Bind(platformConfig);

builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts =>
    {
        foreach (var font in platformConfig.Fonts)
        {
            fonts.AddFont(font.Value, font.Key);
        }
    });
```

**Migráció:**
1. Configuration betöltés előre kell (fonts előtt)
2. PlatformConfig binding szükséges
3. Font aliasok a konfigurációból jönnek
4. Új font hozzáadása: csak appsettings.json módosítás

## Példa: Új font hozzáadása

### 1. Font fájl hozzáadása
```
Resources/Fonts/
  └── Courier-New.ttf  (új)
```

### 2. appsettings.json frissítése
```json
"Fonts": {
  "OpenSansRegular": "OpenSans-Regular.ttf",
  "OpenSansSemibold": "OpenSans-Semibold.ttf",
  "FixedsysExcelsior": "FixedsysExcelsior.ttf",
  "CourierNew": "Courier-New.ttf"  // <- Új font
}
```

### 3. Használat XAML-ben
```xaml
<Label 
    Text="Monospace text"
    FontFamily="CourierNew" />
```

### 4. Build és teszt
```bash
dotnet build
dotnet build -t:Run -f net10.0-android
```

**Kész!** Nincs szükség kód módosításra, csak konfiguráció.

## Következő lépések

1. ✅ **Font konfiguráció** - KÉSZ
2. ❌ **MAUI build és teszt**
3. ❌ **Font rendering teszt különböző platformokon**
4. ❌ **AddPasswordView implementálása**

## Összefoglalás

A MAUI alkalmazás fontjait áthelyeztük az `appsettings.json` konfigurációba:
- ✅ **Nincs hardcoded font regisztráció** a kódban
- ✅ **Könnyű új font hozzáadása** (csak konfiguráció)
- ✅ **Centralizált konfiguráció** (minden egy helyen)
- ✅ **Platform-specifikus** (csak MAUI használja)

A font konfigurációt a `Dictionary<string, string>` formátumban tároljuk, ahol:
- **Key**: Font alias (használható XAML-ben)
- **Value**: Font fájl path (Resources/Fonts/ mappában)

---

**Dátum:** 2026-01-10  
**Státusz:** ✅ Kész  
**Build:** Függőben (MAUI build teszt)

[English]: ./MAUI_FONT_CONFIGURATION.md