using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;

namespace Enclave.Common.Tests.Drawing;

/// <summary>
/// Unit tests for <see cref="Size"/>.
/// </summary>
[UnitTest, TestOf(nameof(Size))]
public sealed class SizeTests
{
    [Fact]
    public void Empty_HasZeroWidthAndHeight()
    {
        // Arrange & Act
        var s = Size.Empty;

        // Assert
        s.Width.ShouldBe(0);
        s.Height.ShouldBe(0);
    }

    public static readonly IEnumerable<object?[]> ConstructorTestData = new (int width, int height)[]
    {
        (0, 0),
        (10, 20),
        (100, 50)
    }.ToTestData();

    [Theory, MemberData(nameof(ConstructorTestData))]
    public void Constructor_FromPoint_CreatesSizeWithPointCoordinates(int width, int height)
    {
        // Arrange
        var p = new Point(width, height);

        // Act
        var s = new Size(p);

        // Assert
        s.Width.ShouldBe(width);
        s.Height.ShouldBe(height);
    }

    public static readonly IEnumerable<object?[]> AddTestData = new (Size start, Size delta)[]
    {
        (new (10, 20), new(5, 10)),
        (new (0, 0), new(-3, -4)),
        (new (50, 50), new(25, -10))
    }.ToTestData();

    [Theory, MemberData(nameof(AddTestData))]
    public void Add_WithDeltas_ReturnsCorrectSize(Size s, Size delta)
    {
        // Arrange
        var expectedWidth = s.Width + delta.Width;
        var expectedHeight = s.Height + delta.Height;

        // Act
        var result = s.Add(delta.Width, delta.Height);

        // Assert
        result.Width.ShouldBe(expectedWidth);
        result.Height.ShouldBe(expectedHeight);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void Add_WithOtherSize_ReturnsSum(Size s, Size delta)
    {
        // Arrange
        var expectedWidth = s.Width + delta.Width;
        var expectedHeight = s.Height + delta.Height;

        // Act
        var result = s.Add(delta);

        // Assert
        result.Width.ShouldBe(expectedWidth);
        result.Height.ShouldBe(expectedHeight);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void Subtract_WithOtherSize_ReturnsDifference(Size s, Size delta)
    {
        // Arrange
        var expectedWidth = s.Width - delta.Width;
        var expectedHeight = s.Height - delta.Height;

        // Act
        var result = s.Subtract(delta);

        // Assert
        result.Width.ShouldBe(expectedWidth);
        result.Height.ShouldBe(expectedHeight);
    }


    public static readonly IEnumerable<object?[]> ScaleTestData = new (Size start, double factor)[]
    {
        (new (10, 20), 2.0),
        (new (0, 0), 1.5),
        (new (50, 50), 0.5)
    }.ToTestData();

    [Theory, MemberData(nameof(ScaleTestData))]
    public void Scale_WithFactor_ReturnsScaledSize(Size s, double factor )
    {
        // Arrange
        var expectedWidth = (int)(s.Width * factor);   
        var expectedHeight = (int)(s.Height * factor);

        // Act
        var result = s.Scale(factor);

        // Assert
        result.Width.ShouldBe(expectedWidth);
        result.Height.ShouldBe(expectedHeight);
    }

    [Theory, MemberData(nameof(ScaleTestData))]
    public void AsPoint_ReturnsPointWithSameDimensions(Size s, double _)
    {
        // Arrange

        // Act
        var p = s.AsPoint();

        // Assert
        p.X.ShouldBe(s.Width);
        p.Y.ShouldBe(s.Height);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void OperatorPlus_TwoSizes_ReturnsSum(Size s, Size delta)
    {
        // Arrange
        var expectedWidth = s.Width + delta.Width;
        var expectedHeight = s.Height + delta.Height;

        // Act
        var result = s + delta;

        // Assert
        result.Width.ShouldBe(expectedWidth);
        result.Height.ShouldBe(expectedHeight);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void OperatorMinus_TwoSizes_ReturnsDifference(Size s, Size delta)
    {
        // Arrange
        var expectedWidth = s.Width - delta.Width;
        var expectedHeight = s.Height - delta.Height;

        // Act
        var result = s - delta;

        // Assert
        result.Width.ShouldBe(expectedWidth);
        result.Height.ShouldBe(expectedHeight);
    }

    [Theory, MemberData(nameof(ScaleTestData))]
    public void OperatorMultiply_ScalarTimesSize_ReturnsScaledSize(Size s, double factor)
    {
        // Arrange
        var expectedWidth = (int)(s.Width * factor);
        var expectedHeight = (int)(s.Height * factor);

        // Act
        var result = factor * s;

        // Assert
        result.Width.ShouldBe(expectedWidth);
        result.Height.ShouldBe(expectedHeight);
    }

    [Theory, MemberData(nameof(ScaleTestData))]
    public void OperatorMultiply_SizeTimesScalar_ReturnsScaledSize(Size s, double factor)
    {
        // Arrange
        var expectedWidth = (int)(s.Width * factor);
        var expectedHeight = (int)(s.Height * factor);

        // Act
        var result = s * factor;

        // Assert
        result.Width.ShouldBe(expectedWidth);
        result.Height.ShouldBe(expectedHeight);
    }

    [Theory, MemberData(nameof(ScaleTestData))]
    public void ValueEquality_EqualSizes_AreEqual(Size s, double _)
    {
        // Arrange
        var a = new Size(s.Width, s.Height);
        var b = new Size(s.Width, s.Height);

        // Assert
        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void ValueEquality_DifferentSizes_AreNotEqual(Size a, Size b)
    {
        // Arrange
        // Arrange
        (a.Width != b.Width || a.Height != b.Height).ShouldBeTrue("PRE: Test sizes should be different"); // Sanity check that sizes are actually different

        // Assert
        a.ShouldNotBe(b);
        (a != b).ShouldBeTrue();
    }
}
