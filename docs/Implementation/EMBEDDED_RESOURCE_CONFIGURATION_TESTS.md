# Unit Test Example - EmbeddedResourceConfigurationProvider
**English** | [Magyar]

## Example Test Class

```csharp
using Fallout.TerminalHacker.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Fallout.TerminalHacker.Core.Tests.Configuration;

public class EmbeddedResourceConfigurationProviderTests
{
    [Fact]
    public void AddEmbeddedJsonFile_LoadsConfigurationFromEmbeddedResource()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        
        // Act
        var configuration = new ConfigurationBuilder()
            .AddEmbeddedJsonFile(assembly, "test-appsettings.json", optional: false)
            .Build();
        
        // Assert
        configuration.ShouldNotBeNull();
        configuration["TestKey"].ShouldBe("TestValue");
    }
    
    [Fact]
    public void AddEmbeddedJsonFile_OptionalResourceNotFound_ReturnsEmptyConfiguration()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        
        // Act
        var configuration = new ConfigurationBuilder()
            .AddEmbeddedJsonFile(assembly, "non-existent.json", optional: true)
            .Build();
        
        // Assert
        configuration.ShouldNotBeNull();
        configuration["AnyKey"].ShouldBeNull();
    }
    
    [Fact]
    public void AddEmbeddedJsonFile_RequiredResourceNotFound_ThrowsException()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        
        // Act & Assert
        Should.Throw<FileNotFoundException>(() =>
        {
            var configuration = new ConfigurationBuilder()
                .AddEmbeddedJsonFile(assembly, "non-existent.json", optional: false)
                .Build();
            
            // Trigger configuration load
            _ = configuration["AnyKey"];
        });
    }
    
    [Fact]
    public void AddEmbeddedJsonFile_LoadsPlatformConfiguration()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        
        // Act
        var configuration = new ConfigurationBuilder()
            .AddEmbeddedJsonFile(assembly, "test-appsettings.json", optional: false)
            .Build();
        
        var platformConfig = new PlatformConfig();
        configuration.GetSection("Platform").Bind(platformConfig);
        
        // Assert
        platformConfig.ProjectCodename.ShouldBe("TEST");
        platformConfig.Version.ShouldBe("v0.0.0");
        platformConfig.PlatformName.ShouldBe("Test Platform");
        platformConfig.Timing.LineDelay.ShouldBe("150 ms");
    }
    
    [Fact]
    public void AddEmbeddedJsonFile_CallingAssemblyOverload_Works()
    {
        // Act
        var configuration = new ConfigurationBuilder()
            .AddEmbeddedJsonFile("test-appsettings.json")
            .Build();
        
        // Assert
        configuration["TestKey"].ShouldBe("TestValue");
    }
}
```

## Test appsettings.json

**File:** `Core.Tests/test-appsettings.json`

```json
{
  "TestKey": "TestValue",
  "Platform": {
    "ProjectCodename": "TEST",
    "Version": "v0.0.0",
    "PlatformName": "Test Platform",
    "Description": "Test configuration",
    "SystemModules": [
      "Test Module 1",
      "Test Module 2"
    ],
    "Applications": [
      "Test App 1",
      "Test App 2"
    ],
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

## Core.Tests.csproj Setup

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xUnit" />
    <PackageReference Include="Shouldly" />
    <PackageReference Include="Microsoft.Extensions.Configuration" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Binder" />
  </ItemGroup>

  <ItemGroup>
    <!-- Embed test configuration -->
    <EmbeddedResource Include="test-appsettings.json" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Core\Core.csproj" />
  </ItemGroup>
</Project>
```

## Running Tests

```bash
cd tests/Core.Tests
dotnet test
```

**Expected Output:**
```
Test Run Successful.
Total tests: 5
     Passed: 5
```

## Integration Test Example

```csharp
[Fact]
public void Console_Startup_LoadsConfigurationCorrectly()
{
    // Arrange
    var assembly = typeof(ConsolePlatformInfoService).Assembly;
    
    // Act
    var configuration = new ConfigurationBuilder()
        .AddEmbeddedJsonFile(assembly, "appsettings.json")
        .Build();
    
    var startup = new Startup(configuration);
    var services = new ServiceCollection();
    startup.ConfigureServices(services);
    var serviceProvider = services.BuildServiceProvider();
    
    // Assert
    var platformInfo = serviceProvider.GetRequiredService<IPlatformInfoService>();
    platformInfo.ProjectCodename.ShouldBe("RAVEN");
    platformInfo.Version.ShouldBe("v0.3.1");
    platformInfo.PlatformName.ShouldBe("ROBCO TERMINAL NX-12");
}
```

## Mocking for Unit Tests

```csharp
[Fact]
public void Service_UsesConfiguration_Correctly()
{
    // Arrange
    var configData = new Dictionary<string, string>
    {
        ["Platform:ProjectCodename"] = "MOCK",
        ["Platform:Version"] = "v1.0.0"
    };
    
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(configData)
        .Build();
    
    var platformConfig = new PlatformConfig();
    configuration.GetSection("Platform").Bind(platformConfig);
    
    var service = new ConsolePlatformInfoService(platformConfig);
    
    // Assert
    service.ProjectCodename.ShouldBe("MOCK");
    service.Version.ShouldBe("v1.0.0");
}
```

## Notes

- ✅ Use `<EmbeddedResource Include="test-appsettings.json" />` in test project
- ✅ Mock with `AddInMemoryCollection()` for fast tests
- ✅ Integration tests can use real embedded resources
- ✅ Use `Shouldly` for readable assertions
- ✅ Test both optional and required resource scenarios

[Magyar]: ./EMBEDDED_RESOURCE_CONFIGURATION_TESTS.hu.md