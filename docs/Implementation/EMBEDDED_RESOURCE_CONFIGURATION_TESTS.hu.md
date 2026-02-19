# Unit teszt példa – EmbeddedResourceConfigurationProvider

[English] | **Magyar**

## Példa teszt osztály

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
        // ... lásd angol EMBEDDED_RESOURCE_CONFIGURATION_TESTS.md
    }
    
    [Fact]
    public void AddEmbeddedJsonFile_RequiredResourceNotFound_ThrowsException()
    {
        // ... lásd angol EMBEDDED_RESOURCE_CONFIGURATION_TESTS.md
    }
    
    [Fact]
    public void AddEmbeddedJsonFile_LoadsPlatformConfiguration()
    {
        // ... lásd angol EMBEDDED_RESOURCE_CONFIGURATION_TESTS.md
    }
}
```

A teljes teszt kód és további tesztek: angol [EMBEDDED_RESOURCE_CONFIGURATION_TESTS.md][English].

[English]: ./EMBEDDED_RESOURCE_CONFIGURATION_TESTS.md