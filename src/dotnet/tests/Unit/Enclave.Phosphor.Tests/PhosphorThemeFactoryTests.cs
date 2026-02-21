using Enclave.Common.Models;

namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="PhosphorThemeFactory"/>.
/// </summary>
[UnitTest, TestOf(nameof(PhosphorThemeFactory))]

public sealed class PhosphorThemeFactoryTests
{
    [Fact]
    public void Create_GreenTheme_ReturnsCorrectPalette()
    {
        // Arrange & Act
        var theme = PhosphorThemeFactory.Create("green");

        // Assert
        theme.Key.ShouldBe("green");
        theme.Palette[CharStyle.Background].ShouldBe(ColorValue.FromHex("#0C190C"));
        theme.Palette[CharStyle.Dark].ShouldBe(ColorValue.FromHex("#1A4D1A"));
        theme.Palette[CharStyle.Normal].ShouldBe(ColorValue.FromHex("#339933"));
        theme.Palette[CharStyle.Bright].ShouldBe(ColorValue.FromHex("#66FF66"));
    }

    [Theory]
    [InlineData("green")]
    [InlineData("GREEN")]
    [InlineData(" Green ")]
    public void Create_GreenThemeKeyVariants_ReturnsSameTheme(string key)
    {
        // Arrange & Act
        var theme = PhosphorThemeFactory.Create(key);

        // Assert
        theme.Key.ShouldBe("green");
    }

    [Fact]
    public void Create_AmberTheme_ReturnsCorrectPalette()
    {
        // Arrange & Act
        var theme = PhosphorThemeFactory.Create("amber");

        // Assert
        theme.Key.ShouldBe("amber");
        theme.Palette[CharStyle.Background].ShouldBe(ColorValue.FromHex("#190C00"));
        theme.Palette[CharStyle.Bright].ShouldBe(ColorValue.FromHex("#FFBB33"));
    }

    [Fact]
    public void Create_WhiteTheme_ReturnsCorrectPalette()
    {
        // Arrange & Act
        var theme = PhosphorThemeFactory.Create("white");

        // Assert
        theme.Key.ShouldBe("white");
        theme.Palette[CharStyle.Normal].ShouldBe(ColorValue.FromHex("#999999"));
    }

    [Fact]
    public void Create_BlueTheme_ReturnsCorrectPalette()
    {
        // Arrange & Act
        var theme = PhosphorThemeFactory.Create("blue");

        // Assert
        theme.Key.ShouldBe("blue");
        theme.Palette[CharStyle.Dark].ShouldBe(ColorValue.FromHex("#1A417B"));
    }

    [Fact]
    public void Create_UnknownTheme_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        var ex = Should.Throw<ArgumentException>(() => PhosphorThemeFactory.Create("invalid"));
        ex.ParamName.ShouldBe("key");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrWhiteSpaceKey_ThrowsArgumentException(string? key)
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentException>(() => PhosphorThemeFactory.Create(key!));
    }

    [Fact]
    public void Default_ReturnsGreenTheme()
    {
        // Arrange & Act
        var theme = PhosphorThemeFactory.Default;

        // Assert
        theme.Key.ShouldBe("green");
    }
}
