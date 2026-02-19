# Startup Class Architecture - DI Setup Refactoring
**English** | [Magyar]

## Summary

On all three platforms (Console, MAUI, Blazor), a `Startup` class was introduced to centralise Dependency Injection configuration. The `ConfigureServices` method contains all service registrations, so `Program.cs`/`MauiProgram.cs` are much cleaner and easier to follow.

## Changes

### 1. Console POC (RAVEN)

#### ✅ Console/Startup.cs (NEW)
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

#### ✅ Console/Program.cs (SIMPLIFIED)
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

**Benefits:**
- ✅ Cleaner, shorter `Program.cs`
- ✅ Centralised DI configuration
- ✅ Easy to test (Startup class can be unit tested)
- ✅ Consistent pattern on all platforms

### 2. MAUI (ECHELON)

#### ✅ Maui/Startup.cs (NEW)
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

#### ✅ Maui/MauiProgram.cs (SIMPLIFIED)
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

#### ✅ Maui/App.Maui.csproj (UPDATED)
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

**Difference from Console:**
- ❗ **Embedded resource**: appsettings.json is embedded with `<EmbeddedResource>` tag
- ❗ **Resource name**: Full resource name is `Fallout.TerminalHacker.App.Maui.appsettings.json`
- ❗ **Configuration**: Uses `GetManifestResourceStream()` with `AddJsonStream()`

### 3. Blazor (GHOST)

#### ✅ Web/Startup.cs (UPDATED)
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

#### ✅ Web/Program.cs (UNCHANGED)
```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.SetupLogger();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var startup = new Startup(builder);
startup.ConfigureServices(builder.Services);

await builder.Build().RunAsync();
```

#### ✅ Web/wwwroot/appsettings.json (UPDATED)
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

#### ✅ Web/Web.App.csproj (UPDATED)
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Configuration.Binder" />
</ItemGroup>
```

**Difference from Console/MAUI:**
- ❗ **wwwroot**: appsettings.json is in the `wwwroot` folder, not embedded
- ❗ **WebAssemblyHostBuilder**: Configuration is already built into `builder.Configuration`
- ❗ **Multiple config**: Logging and Serilog settings are also included

## Platform configuration comparison

| Platform | Config location | Load method | Embedded? |
|----------|-----------------|-------------|-----------|
| **Console** | `appsettings.json` | `AddJsonFile()` | ❌ File copy |
| **MAUI** | `appsettings.json` | `GetManifestResourceStream()` + `AddJsonStream()` | ✅ Embedded |
| **Blazor** | `wwwroot/appsettings.json` | `WebAssemblyHostBuilder.Configuration` | ❌ wwwroot |

## Startup pattern comparison

| Platform | Constructor | ConfigureServices | Other method |
|----------|-------------|-------------------|--------------|
| **Console** | `IConfiguration` | ✅ | `RegisterViews()` |
| **MAUI** | `IConfiguration` | ✅ | - |
| **Blazor** | `WebAssemblyHostBuilder` | ✅ | - |

## Benefits

### ✅ Consistent pattern
All three platforms follow the same `Startup` pattern:
1. Load configuration
2. Instantiate Startup class
3. Call `ConfigureServices()`
4. Build ServiceProvider

### ✅ Cleaner code
- **Program.cs/MauiProgram.cs**: Only bootstrapping logic
- **Startup.cs**: Only DI configuration
- **Easier to read**

### ✅ Testable
The `Startup` class can be unit tested:
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

### ✅ Extensible
New service registrations are easy to add in `ConfigureServices()`.

## Requirements

### NuGet packages
**Console & MAUI:**
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.Configuration.Json`
- `Microsoft.Extensions.Configuration.Binder`

**Blazor:**
- `Microsoft.Extensions.Configuration.Binder` (others already included)

### File configuration
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
- appsettings.json in `wwwroot` folder

## Testing

### Console POC:
```bash
cd "src/Enclave.Echelon/App/Console"
dotnet build
dotnet run
```

**Check:**
- ✅ Startup class used
- ✅ Configuration loaded from appsettings.json
- ✅ Platform configuration binding works
- ✅ Boot sequence timing values correct

### MAUI:
```bash
cd "src/Enclave.Echelon/App/Maui"
dotnet build
```

**Check:**
- ✅ Embedded resource works
- ✅ Configuration loaded from assembly
- ✅ Startup class used

### Blazor:
```bash
cd "src/Enclave.Echelon/App/Web"
dotnet build
```

**Check:**
- ✅ wwwroot/appsettings.json used
- ✅ Platform configuration binding
- ✅ Startup class used

## Breaking changes

### ⚠️ Program.cs/MauiProgram.cs changes
The old inline DI setup code was replaced with the Startup class.

**Old (Console Program.cs):**
```csharp
private static void ConfigureServices(IServiceCollection services)
{
    // Hardcoded configuration
    services.AddSingleton<IPlatformInfoService, ConsolePlatformInfoService>();
    // ... all service registrations inline
}
```

**New (Console Program.cs):**
```csharp
static async Task Main(string[] args)
{
    var configuration = BuildConfiguration();
    var startup = new Startup(configuration);
    startup.ConfigureServices(services);
    // ...
}
```

**Migration:**
- All platform-specific DI setup must move to `Startup.cs`
- `Program.cs`/`MauiProgram.cs` only contain configuration build and Startup usage

## Next steps

1. ✅ **Console Startup class** - DONE
2. ✅ **MAUI Startup class** - DONE
3. ✅ **Blazor Startup class** - DONE
4. ❌ **Build and test on all three platforms**
5. ❌ **Implement AddPasswordView**

## Summary

A `Startup` class was added on all three platforms to centralise Dependency Injection configuration. The `ConfigureServices` method contains all service registrations and PlatformConfig binding.

**Main changes:**
- ✅ Console: New `Startup.cs`, simplified `Program.cs`
- ✅ MAUI: New `Startup.cs`, simplified `MauiProgram.cs`, embedded appsettings.json
- ✅ Blazor: Updated `Startup.cs`, updated `wwwroot/appsettings.json`

**Platform-specific differences:**
- Console: File copy (`appsettings.json`)
- MAUI: Embedded resource (`<EmbeddedResource Include="appsettings.json" />`)
- Blazor: In wwwroot folder (`wwwroot/appsettings.json`)

All three platforms follow the same clean, consistent Startup pattern!

---

**Date:** 2026-01-10  
**Status:** ✅ Done  
**Build:** Pending (build test on all three platforms)

[Magyar]: ./STARTUP_CLASS_ARCHITECTURE.hu.md