using System.Diagnostics;
using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Shouldly;
using Xunit;

namespace Enclave.Common.Tests.Drawing;

/// <summary>
/// Unit tests for <see cref="Point"/>.
/// </summary>
[UnitTest, TestOf(nameof(Point))]
public sealed class PointTests
{
    public static readonly IEnumerable<object?[]> DirectionConstantsTestData = new (Point direction, int expectedX, int expectedY)[]
    {
        (Point.ToUp, 0, -1),
        (Point.ToDown, 0, 1),
        (Point.ToLeft, -1, 0),
        (Point.ToRight, 1, 0),
        (Point.Empty, 0, 0),
        (Point.Unit, 1, 1)
    }.ToTestData();

    [Theory, MemberData(nameof(DirectionConstantsTestData))]
    public void Constants_HaveExpectedValues(Point direction, int expectedX, int expectedY)
    {
        // Assert
        direction.X.ShouldBe(expectedX);
        direction.Y.ShouldBe(expectedY);
    }

    public static readonly IEnumerable<object?[]> AddTestData = new (Point start, Point delta)[]
    {
        (new (10, 20), new (5, -3)),
        (new (1, 0), new (-1, 0)),
        (new (-5, -5), new (15, 5))
    }.ToTestData();

    [Theory, MemberData(nameof(AddTestData))]
    public void Add_WithDeltas_ReturnsCorrectPoint(Point start, Point delta)
    {
        // Arrange
        var expectedX = start.X + delta.X;
        var expectedY = start.Y + delta.Y;

        // Act
        var result = start.Add(delta.X, delta.Y);

        // Assert
        result.X.ShouldBe(expectedX);
        result.Y.ShouldBe(expectedY);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void Add_WithOtherPoint_ReturnsSum(Point start, Point delta)
    {
        // Arrange 
        var expectedX = start.X + delta.X;
        var expectedY = start.Y + delta.Y;

        // Act
        var result = start.Add(delta);

        // Assert
        result.X.ShouldBe(expectedX);
        result.Y.ShouldBe(expectedY);
    }

    [Fact]
    public void Subtract_WithOtherPoint_ReturnsDifference()
    {
        // Arrange
        var a = new Point(10, 20);
        var b = new Point(3, 5);

        // Act
        var result = a.Subtract(b);

        // Assert
        result.X.ShouldBe(7);
        result.Y.ShouldBe(15);
    }

    [Fact]
    public void Scale_WithFactor_ReturnsScaledPoint()
    {
        // Arrange
        var p = new Point(10, 20);

        // Act
        var result = p.Scale(2.0);

        // Assert
        result.X.ShouldBe(20);
        result.Y.ShouldBe(40);
    }

    [Fact]
    public void Scale_WithFraction_TruncatesTowardZero()
    {
        // Arrange
        var p = new Point(10, 20);

        // Act
        var result = p.Scale(0.5);

        // Assert
        result.X.ShouldBe(5);
        result.Y.ShouldBe(10);
    }

    [Theory]
    [InlineData(30, 40)]
    [InlineData(10, 20)]
    [InlineData(3, 5)]
    public void AsSize_ReturnsSizeWithSameDimensions(int width, int height)
    {
        // Arrange
        var p = new Point(width, height);

        // Act
        var size = p.AsSize();

        // Assert
        size.Width.ShouldBe(width);
        size.Height.ShouldBe(height);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void DistanceTo_ReturnsDeltaAsSize(Point a, Point b)
    {
        // Arrange
        var expectedWidth = Math.Abs(b.X - a.X);
        var expectedHeight = Math.Abs(b.Y - a.Y);

        // Act
        var delta = a.DistanceTo(b);

        // Assert
        delta.Width.ShouldBe(expectedWidth);
        delta.Height.ShouldBe(expectedHeight);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void OperatorPlus_TwoPoints_ReturnsSum(Point a, Point b)
    {
        // Arrange
        var expectedX = a.X + b.X;
        var expectedY = a.Y + b.Y;

        // Act
        var result = a + b;

        // Assert
        result.X.ShouldBe(expectedX);
        result.Y.ShouldBe(expectedY);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void OperatorMinus_TwoPoints_ReturnsDifference(Point a, Point b)
    {
        // Arrange
        var expectedX = a.X - b.X;
        var expectedY = a.Y - b.Y;

        // Act
        var result = a - b;

        // Assert
        result.X.ShouldBe(expectedX);
        result.Y.ShouldBe(expectedY);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void OperatorPlus_PointAndSize_ReturnsOffsetPoint(Point p, Point q)
    {
        // Arrange
        var s = new Size(q);
        var expectedX = p.X + s.Width;
        var expectedY = p.Y + s.Height;

        // Act
        var result = p + s;

        // Assert
        result.X.ShouldBe(expectedX);
        result.Y.ShouldBe(expectedY);
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void OperatorMinus_PointAndSize_ReturnsOffsetPoint(Point p, Point q)
    {
        // Arrange
        var s = new Size(q);
        var expectedX = p.X - s.Width;
        var expectedY = p.Y - s.Height;

        // Act
        var result = p - s;

        // Assert
        result.X.ShouldBe(expectedX);
        result.Y.ShouldBe(expectedY);
    }

    public static readonly IEnumerable<object?[]> MultiplyTestData = new (Point point, double scalar)[]
    {
            (new (10, 20), 2.0),
            (new (0, 0), 5.0),
            (new (-5, -5), -3.0)
    }.ToTestData();

    [Theory, MemberData(nameof(MultiplyTestData))]
    public void OperatorMultiply_ScalarTimesPoint_ReturnsScaledPoint(Point p, double scalar)
    {
        // Arrange
        var expectedX = (int)(p.X * scalar);
        var expectedY = (int)(p.Y * scalar);

        // Act
        var result = scalar * p;

        // Assert
        result.X.ShouldBe(expectedX);
        result.Y.ShouldBe(expectedY);
    }

    [Theory, MemberData(nameof(MultiplyTestData))]
    public void OperatorMultiply_PointTimesScalar_ReturnsScaledPoint(Point p, double scalar)
    {
        // Arrange
        var expectedX = (int)(p.X * scalar); 
        var expectedY = (int)(p.Y * scalar);

        // Act
        var result = p * scalar;

        // Assert
        result.X.ShouldBe(expectedX);
        result.Y.ShouldBe(expectedY);
    }

    public static readonly IEnumerable<object?[]> ToStringTestData = new (Point point, string output)[]
    {
        (new (10, 20), "(10, 20)"),
        (new (0, 0), "(0, 0)"),
        (new (-5, -5), "(-5, -5)")
    }.ToTestData();
    
    [Theory, MemberData(nameof(ToStringTestData))]
    public void ToString_ReturnsReadableFormat(Point p, string expected)
    {
        // Arrange

        // Act
        var s = p.ToString();

        // Assert
        s.ShouldBe(expected);
    }

    [Theory, MemberData(nameof(ToStringTestData))]
    public void ValueEquality_EqualPoints_AreEqual(Point p, string _)
    {
        // Arrange
        var a = new Point(p.X, p.Y);
        var b = new Point(p.X, p.Y);

        // Assert
        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
    }

    [Theory, MemberData(nameof(AddTestData))]
    public void ValueEquality_DifferentPoints_AreNotEqual(Point a, Point b)
    {
        // Arrange
        (a.X != b.X || a.Y != b.Y).ShouldBeTrue("PRE: Test points should be different"); // Sanity check that points are actually different

        // Assert
        a.ShouldNotBe(b);
        (a != b).ShouldBeTrue();
    }
}
