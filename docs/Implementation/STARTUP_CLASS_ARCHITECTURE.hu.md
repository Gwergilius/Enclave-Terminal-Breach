# Startup Class Architecture - DI Setup Refactoring
[English] | **Magyar**

## Összefoglaló

Mindhárom platformon (Console, MAUI, Blazor) létrehoztuk a `Startup` osztályt, amely centralizálja a Dependency Injection konfigurációt. A `ConfigureServices` metódus tartalmazza az összes service regisztrációt, így a `Program.cs`/`MauiProgram.cs` fájlok sokkal tisztábbak és átláthatóbbak lettek.

## Változások

### 1. Console POC (RAVEN)

#### ✅ Console/Startup.cs (ÚJ)
```csharp
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Platform Configuration
        var platformConfig = new PlatformConfig();
        _configuration.GetSection("Platform").Bind(platformConfig);
        services.AddSingleton(platformConfig);

        // Core Services
        services.AddTransient<IPasswordSolver, PasswordSolver>();
        services.AddSingleton<IWordListService, WordListService>();

        // Console-specific Services
        services.AddSingleton<ConsoleNavigationService>();
        services.AddSingleton<INavigationService>(...);
        services.AddSingleton<IPlatformInfoService, ConsolePlatformInfoService>();

        // ViewModels, GameSession, Views
        // ...
    }

    public void RegisterViews(ConsoleNavigationService navigationService, IServiceProvider serviceProvider)
    {
        navigationService.RegisterView("BootSequence", ...);
        // ...
    }
}
```

#### ✅ Console/Program.cs (EGYSZERŰSÍTVE)
```csharp
static async Task Main(string[] args)
{
    SetupConsole();

    // Build configuration
    var configuration = BuildConfiguration();

    // Configure DI using Startup
    var services = new ServiceCollection();
    var startup = new Startup(configuration);
    startup.ConfigureServices(services);

    var serviceProvider = services.BuildServiceProvider();

    // Configure navigation
    var navigationService = serviceProvider.GetRequiredService<ConsoleNavigationService>();
    startup.RegisterViews(navigationService, serviceProvider);

    // Start application
    await navigationService.NavigateToAsync("BootSequence");
}

private static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
}
```

**Előnyök:**
- ✅ Tisztább, rövidebb `Program.cs`
- ✅ Centralizált DI konfiguráció
- ✅ Könnyen tesztelhető (Startup osztály unit tesztelhető)
- ✅ Egységes minta mindhárom platformon

### 2. MAUI (ECHELON)

#### ✅ Maui/Startup.cs (ÚJ)
```csharp
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Platform Configuration
        var platformConfig = new PlatformConfig();
        _configuration.GetSection("Platform").Bind(platformConfig);
        services.AddSingleton(platformConfig);

        // Core Services
        services.AddSingleton<IWordListService, WordListService>();
        services.AddSingleton<IPasswordSolver, PasswordSolver>();
        services.AddSingleton<IPlatformInfoService, MauiPlatformInfoService>();

        // MAUI-specific Services
        services.AddSingleton<PaletteService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<ViewModel>();

        // Pages
        services.AddTransient<MainPage>();
        services.AddTransient<BootSequencePage>();
    }
}
```

#### ✅ Maui/MauiProgram.cs (EGYSZERŰSÍTVE)
```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts => { ... });

    // Build configuration from embedded appsettings.json
    var configuration = BuildConfiguration();

    // Configure services using Startup
    var startup = new Startup(configuration);
    startup.ConfigureServices(builder.Services);

#if DEBUG
    builder.Logging.AddDebug();
#endif

    return builder.Build();
}

private static IConfiguration BuildConfiguration()
{
    var assembly = Assembly.GetExecutingAssembly();
    var configurationBuilder = new ConfigurationBuilder();

    // Read appsettings.json from embedded resource
    using var stream = assembly.GetManifestResourceStream("Fallout.TerminalHacker.App.Maui.appsettings.json");
    if (stream != null)
    {
        configurationBuilder.AddJsonStream(stream);
    }

    return configurationBuilder.Build();
}
```

#### ✅ Maui/App.Maui.csproj (FRISSÍTVE)
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Configuration" />
  <PackageReference Include="Microsoft.Extensions.Configuration.Json" />
  <PackageReference Include="Microsoft.Extensions.Configuration.Binder" />
</ItemGroup>

<ItemGroup>
  <!-- Embed appsettings.json as resource -->
  <EmbeddedResource Include="appsettings.json" />
</ItemGroup>
```

**Különbség a Console-hoz képest:**
- ❗ **Embedded Resource**: appsettings.json `<EmbeddedResource>` tag-gel van beágyazva az assembly-be
- ❗ **Resource Name**: A teljes resource name `Fallout.TerminalHacker.App.Maui.appsettings.json`
- ❗ **Configuration**: `GetManifestResourceStream()` használata `AddJsonStream()` helyett

### 3. Blazor (GHOST)

#### ✅ Web/Startup.cs (FRISSÍTVE)
```csharp
public sealed class Startup
{
    private readonly WebAssemblyHostBuilder _builder;
    private readonly IConfiguration _configuration;

    public Startup(WebAssemblyHostBuilder builder)
    {
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        _configuration = _builder.Configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Platform Configuration
        var platformConfig = new PlatformConfig();
        _configuration.GetSection("Platform").Bind(platformConfig);
        services.AddSingleton(platformConfig);

        // HTTP Client
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(_builder.HostEnvironment.BaseAddress) });

        // Core Services
        services.AddSingleton<IWordListService, WordListService>();
        services.AddSingleton<IPasswordSolver, PasswordSolver>();
        services.AddSingleton<IPlatformInfoService, BlazorPlatformInfoService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<ViewModel>();

        // Error handling, MediatR, Storage, Toast
        // ...
    }
}
```

#### ✅ Web/Program.cs (NEM VÁLTOZOTT)
```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.SetupLogger();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var startup = new Startup(builder);
startup.ConfigureServices(builder.Services);

await builder.Build().RunAsync();
```

#### ✅ Web/wwwroot/appsettings.json (FRISSÍTVE)
```json
{
  "Logging": { ... },
  "Serilog": { ... },
  "Platform": {
    "ProjectCodename": "GHOST",
    "Version": "v1.2.4",
    "PlatformName": "Web Browser (SIGNET Access)",
    "Timing": { ... }
  }
}
```

#### ✅ Web/Web.App.csproj (FRISSÍTVE)
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Configuration.Binder" />
</ItemGroup>
```

**Különbség a Console/MAUI-hoz képest:**
- ❗ **wwwroot**: appsettings.json a `wwwroot` mappában van, nem embedded resource
- ❗ **WebAssemblyHostBuilder**: A configuration már beépített a `builder.Configuration`-ben
- ❗ **Többszörös config**: Logging és Serilog beállítások is benne vannak

## Platform Configuration Összehasonlítás

| Platform | Config Hely | Betöltés Módja | Embedded? |
|----------|-------------|----------------|-----------|
| **Console** | `appsettings.json` | `AddJsonFile()` | ❌ File copy |
| **MAUI** | `appsettings.json` | `GetManifestResourceStream()` + `AddJsonStream()` | ✅ Embedded |
| **Blazor** | `wwwroot/appsettings.json` | `WebAssemblyHostBuilder.Configuration` | ❌ wwwroot |

## Startup Pattern Összehasonlítás

| Platform | Konstruktor | ConfigureServices | Egyéb Metódus |
|----------|-------------|-------------------|---------------|
| **Console** | `IConfiguration` | ✅ | `RegisterViews()` |
| **MAUI** | `IConfiguration` | ✅ | - |
| **Blazor** | `WebAssemblyHostBuilder` | ✅ | - |

## Előnyök

### ✅ Egységes minta
Mindhárom platform ugyanazt a `Startup` pattern-t követi:
1. Configuration betöltése
2. Startup osztály példányosítása
3. `ConfigureServices()` hívása
4. ServiceProvider build

### ✅ Tisztább kód
- **Program.cs/MauiProgram.cs**: Csak bootstrapping logika
- **Startup.cs**: Csak DI konfiguráció
- **Könnyebb olvashatóság**

### ✅ Tesztelhető
A `Startup` osztály könnyen unit tesztelhető:
```csharp
[Fact]
public void ConfigureServices_RegistersAllServices()
{
    // Arrange
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Platform:ProjectCodename"] = "TEST"
        })
        .Build();
    var startup = new Startup(configuration);
    var services = new ServiceCollection();

    // Act
    startup.ConfigureServices(services);
    var serviceProvider = services.BuildServiceProvider();

    // Assert
    Assert.NotNull(serviceProvider.GetService<IPlatformInfoService>());
}
```

### ✅ Bővíthető
Új service regisztrációk egyszerűen hozzáadhatók a `ConfigureServices()`-ben.

## Követelmények

### NuGet Packages
**Console & MAUI:**
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.Configuration.Json`
- `Microsoft.Extensions.Configuration.Binder`

**Blazor:**
- `Microsoft.Extensions.Configuration.Binder` (a többi már benne van)

### File Configuration
**Console:**
```xml
<None Update="appsettings.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

**MAUI:**
```xml
<EmbeddedResource Include="appsettings.json" />
```

**Blazor:**
- appsettings.json a `wwwroot` mappában

## Tesztelés

### Console POC:
```bash
cd "src/Enclave.Echelon/App/Console"
dotnet build
dotnet run
```

**Ellenőrizd:**
- ✅ Startup osztály használata
- ✅ Configuration betöltés appsettings.json-ből
- ✅ Platform Configuration binding működik
- ✅ Boot sequence timing értékek helyesek

### MAUI:
```bash
cd "src/Enclave.Echelon/App/Maui"
dotnet build
```

**Ellenőrizd:**
- ✅ Embedded resource működik
- ✅ Configuration betöltés assembly-ből
- ✅ Startup osztály használata

### Blazor:
```bash
cd "src/Enclave.Echelon/App/Web"
dotnet build
```

**Ellenőrizd:**
- ✅ wwwroot/appsettings.json használata
- ✅ Platform Configuration binding
- ✅ Startup osztály használata

## Breaking Changes

### ⚠️ Program.cs/MauiProgram.cs változások
A régi, inline DI setup kódot lecseréltük Startup osztályra.

**Régi (Console Program.cs):**
```csharp
private static void ConfigureServices(IServiceCollection services)
{
    // Hardcoded configuration
    services.AddSingleton<IPlatformInfoService, ConsolePlatformInfoService>();
    // ... minden service regisztráció inline
}
```

**Új (Console Program.cs):**
```csharp
static async Task Main(string[] args)
{
    var configuration = BuildConfiguration();
    var startup = new Startup(configuration);
    startup.ConfigureServices(services);
    // ...
}
```

**Migráció:**
- Minden platform-specifikus DI setup-ot át kell rakni a `Startup.cs`-be
- `Program.cs`/`MauiProgram.cs` csak a configuration build és Startup használatot tartalmazza

## Következő lépések

1. ✅ **Console Startup osztály** - KÉSZ
2. ✅ **MAUI Startup osztály** - KÉSZ
3. ✅ **Blazor Startup osztály** - KÉSZ
4. ❌ **Build és teszt mindhárom platformon**
5. ❌ **AddPasswordView implementálása**

## Összefoglalás

Mindhárom platformon létrehoztuk a `Startup` osztályt, amely centralizálja a Dependency Injection konfigurációt. A `ConfigureServices` metódus tartalmazza az összes service regisztrációt és a PlatformConfig binding-ot.

**Főbb változások:**
- ✅ Console: Új `Startup.cs`, egyszerűsített `Program.cs`
- ✅ MAUI: Új `Startup.cs`, egyszerűsített `MauiProgram.cs`, embedded appsettings.json
- ✅ Blazor: Frissített `Startup.cs`, `wwwroot/appsettings.json` frissítve

**Platform-specifikus különbségek:**
- Console: File copy (`appsettings.json`)
- MAUI: Embedded resource (`<EmbeddedResource Include="appsettings.json" />`)
- Blazor: wwwroot mappában (`wwwroot/appsettings.json`)

Mindhárom platform ugyanazt a tiszta, egységes Startup pattern-t követi!

---

**Dátum:** 2026-01-10  
**Státusz:** ✅ Kész  
**Build:** Függőben (mindhárom platform build tesztelése)

[English]: ./STARTUP_CLASS_ARCHITECTURE.md