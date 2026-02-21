using Enclave.Common.Models;
using Shouldly;
using Xunit;

namespace Enclave.Common.Tests.Models;

/// <summary>
/// Unit tests for <see cref="ColorValue"/>.
/// </summary>
[UnitTest, TestOf(nameof(ColorValue))]
public sealed class ColorValueTests
{
    [Fact]
    public void FromHex_ValidHex6_CreatesCorrectColor()
    {
        // Arrange & Act
        var color = ColorValue.FromHex("#66FF66");

        // Assert
        color.R.ShouldBe((byte)102);
        color.G.ShouldBe((byte)255);
        color.B.ShouldBe((byte)102);
        color.A.ShouldBe((byte)255);
    }

    [Fact]
    public void FromHex_SixCharacterHex_SetsAlphaTo255()
    {
        // Arrange & Act - 6-char hex must default alpha to opaque
        var color = ColorValue.FromHex("ABCDEF");

        // Assert
        color.A.ShouldBe((byte)255);
    }

    [Fact]
    public void FromHex_ValidHex8_CreatesCorrectColorWithAlpha()
    {
        // Arrange & Act
        var color = ColorValue.FromHex("#66FF6680");

        // Assert
        color.R.ShouldBe((byte)102);
        color.G.ShouldBe((byte)255);
        color.B.ShouldBe((byte)102);
        color.A.ShouldBe((byte)128);
    }

    [Fact]
    public void FromHex_WithoutHash_Works()
    {
        // Arrange & Act
        var color = ColorValue.FromHex("339933");

        // Assert
        color.R.ShouldBe((byte)0x33);
        color.G.ShouldBe((byte)0x99);
        color.B.ShouldBe((byte)0x33);
    }

    [Fact]
    public void ToHex_WithoutAlpha_Returns6DigitHex()
    {
        // Arrange
        var color = new ColorValue(102, 255, 102);

        // Act
        var hex = color.ToHex(includeAlpha: false);

        // Assert
        hex.ShouldBe("#66FF66");
    }

    [Fact]
    public void ToHex_DefaultIncludeAlpha_Returns6DigitHex()
    {
        // Arrange
        var color = new ColorValue(102, 255, 102);

        // Act
        var hex = color.ToHex();

        // Assert
        hex.ShouldBe("#66FF66");
    }

    [Fact]
    public void ToHex_WithAlpha_Returns8DigitHex()
    {
        // Arrange
        var color = new ColorValue(102, 255, 102, 128);

        // Act
        var hex = color.ToHex(includeAlpha: true);

        // Assert
        hex.ShouldBe("#66FF6680");
    }

    [Fact]
    public void ToHex_WithAlphaZero_Returns8DigitHex()
    {
        // Arrange
        var color = new ColorValue(0, 0, 0, 0);

        // Act
        var hex = color.ToHex(includeAlpha: true);

        // Assert
        hex.ShouldBe("#00000000");
    }

    [Fact]
    public void ToCssRgba_ReturnsValidString()
    {
        // Arrange
        var color = new ColorValue(102, 255, 102, 255);

        // Act
        var css = color.ToCssRgba();

        // Assert
        css.ShouldBe("rgba(102, 255, 102, 1.0)");
    }

    [Fact]
    public void ToCssRgba_WithAlphaZero_ReturnsValidString()
    {
        // Arrange
        var color = new ColorValue(0, 0, 0, 0);

        // Act
        var css = color.ToCssRgba();

        // Assert
        css.ShouldBe("rgba(0, 0, 0, 0.0)");
    }

    [Fact]
    public void ToCssRgba_WithAlpha128_ReturnsValidString()
    {
        // Arrange
        var color = new ColorValue(255, 255, 255, 128);

        // Act
        var css = color.ToCssRgba();

        // Assert
        css.ShouldBe("rgba(255, 255, 255, 0.5)");
    }

    [Fact]
    public void ToCssRgb_ReturnsValidString()
    {
        // Arrange
        var color = new ColorValue(102, 255, 102);

        // Act
        var css = color.ToCssRgb();

        // Assert
        css.ShouldBe("rgb(102, 255, 102)");
    }

    [Fact]
    public void ToCssRgb_WithBlack_ReturnsValidString()
    {
        // Arrange
        var color = ColorValue.Black;

        // Act
        var css = color.ToCssRgb();

        // Assert
        css.ShouldBe("rgb(0, 0, 0)");
    }

    [Fact]
    public void ToCssRgb_WithWhite_ReturnsValidString()
    {
        // Arrange
        var color = ColorValue.White;

        // Act
        var css = color.ToCssRgb();

        // Assert
        css.ShouldBe("rgb(255, 255, 255)");
    }

    [Fact]
    public void Transparent_ReturnsFullyTransparentBlack()
    {
        // Arrange & Act
        var color = ColorValue.Transparent;

        // Assert
        color.R.ShouldBe((byte)0);
        color.G.ShouldBe((byte)0);
        color.B.ShouldBe((byte)0);
        color.A.ShouldBe((byte)0);
    }

    [Fact]
    public void Transparent_ToHex_ReturnsTransparentHex()
    {
        // Arrange & Act
        var hex = ColorValue.Transparent.ToHex(includeAlpha: true);

        // Assert
        hex.ShouldBe("#00000000");
    }

    [Fact]
    public void Black_ToHex_ReturnsBlackHex()
    {
        // Arrange & Act
        var hex = ColorValue.Black.ToHex();

        // Assert
        hex.ShouldBe("#000000");
    }

    [Fact]
    public void White_ToHex_ReturnsWhiteHex()
    {
        // Arrange & Act
        var hex = ColorValue.White.ToHex();

        // Assert
        hex.ShouldBe("#FFFFFF");
    }

    [Theory]
    [InlineData("#1234")]
    [InlineData("#12345")]
    [InlineData("#1234567")]
    [InlineData("#123456789")]
    [InlineData("12345")]
    [InlineData("1234567")]
    public void FromHex_InvalidLength_ThrowsArgumentException(string hex)
    {
        // Arrange & Act & Assert
        var ex = Should.Throw<ArgumentException>(() => ColorValue.FromHex(hex));
        ex.ParamName.ShouldBe("hex");
    }

    [Theory]
    [InlineData("GGGGGG")]
    [InlineData("#12XX56")]
    [InlineData("ZZZZZZ")]
    [InlineData("00 000")]
    public void FromHex_InvalidHexCharacters_ThrowsFormatException(string hex)
    {
        // Arrange & Act & Assert - Convert.ToByte(s, 16) throws for non-hex digits
        Should.Throw<FormatException>(() => ColorValue.FromHex(hex));
    }

    [Fact]
    public void FromHex_Null_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => ColorValue.FromHex(null!));
    }
}
