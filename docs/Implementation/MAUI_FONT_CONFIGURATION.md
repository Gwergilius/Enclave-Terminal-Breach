# MAUI Font Configuration from appsettings.json

**English** | [Magyar]

## Summary

In the MAUI application, font configuration was moved to the `appsettings.json` file, so there is no need for hardcoded font registration in `MauiProgram.cs`.

## Changes

### 1. PlatformConfig extension

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

**Properties:**
- **Key (string)**: Font alias name (e.g. `"OpenSansRegular"`)
- **Value (string)**: Font file path (e.g. `"OpenSans-Regular.ttf"`)

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

**Font entries:**
- **OpenSansRegular**: Standard font for MAUI controls
- **OpenSansSemibold**: Bold variant
- **FixedsysExcelsior**: Fallout-style monospace font for terminal display

### 3. MauiProgram.cs update

#### ✅ BEFORE (Hardcoded):
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

#### ✅ AFTER (Configuration-based):
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

**Changes:**
1. ✅ Configuration loaded **before** fonts
2. ✅ PlatformConfig binding for font configuration
3. ✅ Dynamic font registration with `foreach`
4. ✅ `font.Value` = file path (e.g. `"OpenSans-Regular.ttf"`)
5. ✅ `font.Key` = font alias (e.g. `"OpenSansRegular"`)

## Benefits

### ✅ No hardcoded values
```csharp
// No need for:
fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");

// Instead:
// All fonts from configuration
```

### ✅ Easy to add new fonts
```json
// Just add a new entry:
"Fonts": {
  "OpenSansRegular": "OpenSans-Regular.ttf",
  "MyCustomFont": "MyFont.ttf"  // <- New font
}
```

### ✅ Centralised configuration
All platform-specific data in one place (`appsettings.json`):
- Project information
- Boot sequence texts
- Timing values
- **Fonts** (MAUI only)

### ✅ Platform-dependent configuration
- Console/Blazor: No Fonts section (not used)
- MAUI: Has Fonts section (specific font handling)

## Platform-specific notes

### MAUI (ECHELON)
✅ **Uses Fonts configuration**
- Fonts section required in appsettings.json
- Font files in `Resources/Fonts/` folder
- In `.csproj`: `<MauiFont Include="Resources\Fonts\*" />`

### Console (RAVEN)
❌ **Does not use Fonts configuration**
- No Fonts section in appsettings.json
- Uses terminal default font
- Font configuration not relevant

### Blazor (GHOST)
❌ **Does not use Fonts configuration**
- No Fonts section in appsettings.json
- Uses CSS `@font-face` rules
- Font configuration not relevant

## Font file handling

### MAUI .csproj
```xml
<ItemGroup>
  <!-- Custom Fonts -->
  <MauiFont Include="Resources\Fonts\*" />
</ItemGroup>
```

**Font file location:**
```
App/Maui/
  └── Resources/
      └── Fonts/
          ├── OpenSans-Regular.ttf
          ├── OpenSans-Semibold.ttf
          └── FixedsysExcelsior.ttf
```

### Using fonts in XAML
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

## Testing

### Build test:
```bash
cd "src/Enclave.Echelon/App/Maui"
dotnet build
```

**Check:**
- ✅ Build succeeds
- ✅ No font-related errors
- ✅ Configuration binding works

### Runtime test:
```bash
# On Android emulator or device
dotnet build -t:Run -f net10.0-android
```

**Check:**
- ✅ App starts
- ✅ Fonts render correctly
- ✅ OpenSansRegular used in UI
- ✅ FixedsysExcelsior used in terminal view

### Configuration test:
Modify appsettings.json:
```json
"Fonts": {
  "TestFont": "OpenSans-Regular.ttf"
}
```

**Expected behaviour:**
- ✅ Build succeeds
- ✅ Only "TestFont" alias registered
- ⚠️ "OpenSansRegular" not found (if used in XAML)

## Troubleshooting

### Problem: "Font not found"
**Cause:** Font alias does not match the key in configuration.

**Solution:**
```json
// appsettings.json:
"Fonts": {
  "MyFont": "MyFont.ttf"  // <- Key = "MyFont"
}
```

```xaml
<!-- XAML: -->
<Label FontFamily="MyFont" />  <!-- <- Same -->
```

### Problem: "Font file not found"
**Cause:** Wrong font file path or file missing.

**Solution:**
1. Check file exists: `Resources/Fonts/MyFont.ttf`
2. Check in .csproj: `<MauiFont Include="Resources\Fonts\*" />`
3. Check path in appsettings.json

### Problem: Configuration binding fails
**Cause:** appsettings.json format error or embedded resource issue.

**Solution:**
```csharp
// Add debug log:
var platformConfig = new PlatformConfig();
configuration.GetSection("Platform").Bind(platformConfig);

Console.WriteLine($"Fonts loaded: {platformConfig.Fonts.Count}");
foreach (var font in platformConfig.Fonts)
{
    Console.WriteLine($"  {font.Key} => {font.Value}");
}
```

## Breaking changes

### ⚠️ MauiProgram.cs change
**Old:**
```csharp
builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts => { ... });

var configuration = BuildConfiguration();
```

**New:**
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

**Migration:**
1. Configuration must be loaded before fonts
2. PlatformConfig binding required
3. Font aliases come from configuration
4. To add a new font: only change appsettings.json

## Example: Adding a new font

### 1. Add font file
```
Resources/Fonts/
  └── Courier-New.ttf  (new)
```

### 2. Update appsettings.json
```json
"Fonts": {
  "OpenSansRegular": "OpenSans-Regular.ttf",
  "OpenSansSemibold": "OpenSans-Semibold.ttf",
  "FixedsysExcelsior": "FixedsysExcelsior.ttf",
  "CourierNew": "Courier-New.ttf"  // <- New font
}
```

### 3. Use in XAML
```xaml
<Label 
    Text="Monospace text"
    FontFamily="CourierNew" />
```

### 4. Build and test
```bash
dotnet build
dotnet build -t:Run -f net10.0-android
```

**Done!** No code changes, only configuration.

## Next steps

1. ✅ **Font configuration** - DONE
2. ❌ **MAUI build and test**
3. ❌ **Font rendering test on different platforms**
4. ❌ **Implement AddPasswordView**

## Summary

MAUI application fonts were moved to `appsettings.json` configuration:
- ✅ **No hardcoded font registration** in code
- ✅ **Easy to add new fonts** (configuration only)
- ✅ **Centralised configuration** (all in one place)
- ✅ **Platform-specific** (MAUI only)

Font configuration is stored as `Dictionary<string, string>` where:
- **Key**: Font alias (usable in XAML)
- **Value**: Font file path (in Resources/Fonts/ folder)

---

**Date:** 2026-01-10  
**Status:** ✅ Done  
**Build:** Pending (MAUI build test)

[Magyar]: ./MAUI_FONT_CONFIGURATION.hu.md