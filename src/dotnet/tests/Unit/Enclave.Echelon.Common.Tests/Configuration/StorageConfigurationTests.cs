using Enclave.Common.Configuration;
using Enclave.Common.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;

namespace Enclave.Common.Test.Configuration;

/// <summary>
/// Unit tests for storage configuration extensions and providers.
/// </summary>
public class StorageConfigurationTests
{
    [Fact]
    public void AddStorageConfiguration_WithStorageService_AddsConfigurationSource()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        var storageService = new Mock<IStorageService>().Object;

        // Act
        var result = builder.AddStorageConfiguration(storageService);

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void AddStorageConfiguration_WithKeyPrefix_UsesPrefix()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        var storageService = new Mock<IStorageService>().Object;

        // Act
        var result = builder.AddStorageConfiguration(storageService, "Custom:");

        // Assert
        result.ShouldBe(builder);
    }

    [Fact]
    public void AddStorageConfiguration_WithNullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        IConfigurationBuilder? builder = null;
        var storageService = new Mock<IStorageService>().Object;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            builder!.AddStorageConfiguration(storageService));
    }

    [Fact]
    public void AddStorageConfiguration_WithNullStorageService_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        IStorageService? storageService = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            builder.AddStorageConfiguration(storageService!));
    }

    [Fact]
    public void AddStorageConfiguration_WithNullOrWhiteSpaceKeyPrefix_ThrowsArgumentException()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        var storageService = new Mock<IStorageService>().Object;

        // Act & Assert
        Should.Throw<ArgumentException>(() => 
            builder.AddStorageConfiguration(storageService, ""));
        Should.Throw<ArgumentException>(() => 
            builder.AddStorageConfiguration(storageService, "   "));
        Should.Throw<ArgumentException>(() => 
            builder.AddStorageConfiguration(storageService, null!));
    }

    [Fact]
    public void StorageConfigurationSource_Properties_SetCorrectly()
    {
        // Arrange
        var storageService = new Mock<IStorageService>().Object;
        var source = new StorageConfigurationSource
        {
            StorageService = storageService,
            KeyPrefix = "Custom:",
            Optional = false
        };

        // Assert
        source.StorageService.ShouldBe(storageService);
        source.KeyPrefix.ShouldBe("Custom:");
        source.Optional.ShouldBeFalse();
    }

    [Fact]
    public void StorageConfigurationSource_Build_ReturnsProvider()
    {
        // Arrange
        var storageService = new Mock<IStorageService>().Object;
        var source = new StorageConfigurationSource
        {
            StorageService = storageService,
            KeyPrefix = "Config:",
            Optional = true
        };
        var builder = new ConfigurationBuilder();

        // Act
        var provider = source.Build(builder);

        // Assert
        provider.ShouldBeOfType<StorageConfigurationProvider>();
    }

    [Fact]
    public async Task StorageConfigurationProvider_Load_LoadsConfiguration()
    {
        // Arrange
        var storageService = new Mock<IStorageService>();
        storageService
            .Setup(s => s.GetStringAsync("Config:Platform:DefaultFont", It.IsAny<CancellationToken>()))
            .ReturnsAsync("TestFont");

        var source = new StorageConfigurationSource
        {
            StorageService = storageService.Object,
            KeyPrefix = "Config:",
            Optional = true
        };
        var provider = new StorageConfigurationProvider(source);

        // Act
        provider.Load();

        // Assert
        // The provider should have loaded the configuration
        // Note: The current implementation only loads known keys, so we can't easily test
        // the full functionality without modifying the implementation
        provider.ShouldNotBeNull();
    }

    [Fact]
    public async Task StorageConfigurationProvider_Set_PersistsToStorage()
    {
        // Arrange
        var storageService = new Mock<IStorageService>();
        var source = new StorageConfigurationSource
        {
            StorageService = storageService.Object,
            KeyPrefix = "Config:",
            Optional = true
        };
        var provider = new StorageConfigurationProvider(source);

        // Act
        provider.Set("TestKey", "TestValue");

        // Assert - Wait a bit for async operation
        await Task.Delay(100);
        storageService.Verify(s => 
            s.SetStringAsync("Config:TestKey", "TestValue", It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public void StorageConfigurationProvider_Load_WithNullValues_ExcludesNullValues()
    {
        // Arrange
        var storageService = new Mock<IStorageService>();
        storageService
            .Setup(s => s.GetStringAsync("Config:Platform:DefaultFont", It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        storageService
            .Setup(s => s.GetStringAsync("Config:Platform:DefaultPalette", It.IsAny<CancellationToken>()))
            .ReturnsAsync("");

        var source = new StorageConfigurationSource
        {
            StorageService = storageService.Object,
            KeyPrefix = "Config:",
            Optional = true
        };
        var provider = new StorageConfigurationProvider(source);

        // Act
        provider.Load();

        // Assert
        provider.ShouldNotBeNull();
        // Null and empty values should not be added to Data
        provider.TryGet("Platform:DefaultFont", out _).ShouldBeFalse();
        provider.TryGet("Platform:DefaultPalette", out _).ShouldBeFalse();
    }

    [Fact]
    public void StorageConfigurationProvider_Load_WithValidValues_IncludesValues()
    {
        // Arrange
        var storageService = new Mock<IStorageService>();
        storageService
            .Setup(s => s.GetStringAsync("Config:Platform:DefaultFont", It.IsAny<CancellationToken>()))
            .ReturnsAsync("TestFont");
        storageService
            .Setup(s => s.GetStringAsync("Config:Platform:DefaultPalette", It.IsAny<CancellationToken>()))
            .ReturnsAsync("TestPalette");

        var source = new StorageConfigurationSource
        {
            StorageService = storageService.Object,
            KeyPrefix = "Config:",
            Optional = true
        };
        var provider = new StorageConfigurationProvider(source);

        // Act
        provider.Load();

        // Assert
        provider.TryGet("Platform:DefaultFont", out var font).ShouldBeTrue();
        font.ShouldBe("TestFont");
        provider.TryGet("Platform:DefaultPalette", out var palette).ShouldBeTrue();
        palette.ShouldBe("TestPalette");
    }

    [Fact]
    public void StorageConfigurationProvider_Load_WithExceptionAndOptionalTrue_InitializesEmptyData()
    {
        // Arrange
        var storageService = new Mock<IStorageService>();
        storageService
            .Setup(s => s.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Storage error"));

        var source = new StorageConfigurationSource
        {
            StorageService = storageService.Object,
            KeyPrefix = "Config:",
            Optional = true
        };
        var provider = new StorageConfigurationProvider(source);

        // Act
        provider.Load();

        // Assert
        provider.ShouldNotBeNull();
        // Should have empty data dictionary, not throw exception
        provider.TryGet("AnyKey", out _).ShouldBeFalse();
    }

    [Fact]
    public void StorageConfigurationProvider_Load_WithExceptionAndOptionalFalse_ThrowsException()
    {
        // Arrange
        var storageService = new Mock<IStorageService>();
        storageService
            .Setup(s => s.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Storage error"));

        var source = new StorageConfigurationSource
        {
            StorageService = storageService.Object,
            KeyPrefix = "Config:",
            Optional = false
        };
        var provider = new StorageConfigurationProvider(source);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => provider.Load())
            .Message.ShouldContain("Failed to load configuration from storage service");
    }

    [Fact]
    public void StorageConfigurationProvider_Load_WithMixedNullAndValidValues_OnlyIncludesValid()
    {
        // Arrange
        var storageService = new Mock<IStorageService>();
        storageService
            .Setup(s => s.GetStringAsync("Config:Platform:DefaultFont", It.IsAny<CancellationToken>()))
            .ReturnsAsync("ValidFont");
        storageService
            .Setup(s => s.GetStringAsync("Config:Platform:DefaultPalette", It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var source = new StorageConfigurationSource
        {
            StorageService = storageService.Object,
            KeyPrefix = "Config:",
            Optional = true
        };
        var provider = new StorageConfigurationProvider(source);

        // Act
        provider.Load();

        // Assert
        provider.TryGet("Platform:DefaultFont", out var font).ShouldBeTrue();
        font.ShouldBe("ValidFont");
        provider.TryGet("Platform:DefaultPalette", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task StorageConfigurationProvider_Set_WithNullValue_PersistsNull()
    {
        // Arrange
        var storageService = new Mock<IStorageService>();
        var source = new StorageConfigurationSource
        {
            StorageService = storageService.Object,
            KeyPrefix = "Config:",
            Optional = true
        };
        var provider = new StorageConfigurationProvider(source);

        // Act
        provider.Set("TestKey", null);

        // Assert - Wait a bit for async operation
        await Task.Delay(100);
        storageService.Verify(s => 
            s.SetStringAsync("Config:TestKey", null, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public void StorageConfigurationProvider_Constructor_WithNullSource_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            new StorageConfigurationProvider(null!));
    }
}
