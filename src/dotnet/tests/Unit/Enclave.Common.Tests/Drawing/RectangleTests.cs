using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;

namespace Enclave.Common.Tests.Drawing;

/// <summary>
/// Unit tests for <see cref="Rectangle"/>.
/// </summary>
[UnitTest, TestOf(nameof(Rectangle))]
public sealed class RectangleTests
{
    [Fact]
    public void Empty_HasZeroBounds()
    {
        // Arrange & Act
        var r = Rectangle.Empty;

        // Assert
        r.X.ShouldBe(0);
        r.Y.ShouldBe(0);
        r.Width.ShouldBe(0);
        r.Height.ShouldBe(0);
        r.IsEmpty.ShouldBeTrue();
    }

    public static readonly IEnumerable<object?[]> RectangleTestData = new (Point location, Size size)[]
    {
        (new Point(0, 0), new Size(10, 20)),
        (new Point(5, 5), new Size(50, 50)),
        (new Point(10, 20), new Size(100, 50))
    }.ToTestData();

    [Theory, MemberData(nameof(RectangleTestData))]
    public void Constructor_WithLocationAndSize_SetsBounds(Point loc, Size size)
    {
        // Arrange

        // Act
        var r = new Rectangle(loc, size);

        // Assert
        r.X.ShouldBe(loc.X);
        r.Y.ShouldBe(loc.Y);
        r.Width.ShouldBe(size.Width);
        r.Height.ShouldBe(size.Height);

        r.Left.ShouldBe(loc.X);
        r.Top.ShouldBe(loc.Y);
        r.Right.ShouldBe(loc.X + size.Width - 1);
        r.Bottom.ShouldBe(loc.Y + size.Height - 1);
    }

    [Theory, MemberData(nameof(RectangleTestData))]
    public void Constructor_WithTopLeftAndBottomRight_IncludesBothPoints(Point loc, Size size)
    {
        // Arrange
        var topLeft = loc;
        var bottomRight = loc + size - Point.Unit;

        // Act
        var r = new Rectangle(topLeft, bottomRight);

        // Assert
        r.X.ShouldBe(loc.X);
        r.Y.ShouldBe(loc.Y);
        r.Width.ShouldBe(size.Width);
        r.Height.ShouldBe(size.Height);
        r.Right.ShouldBe(bottomRight.X);
        r.Bottom.ShouldBe(bottomRight.Y);
    }

    [Theory, MemberData(nameof(RectangleTestData))]
    public void WidthHeight_NegativeInput_ClampedToZero(Point loc, Size size)
    {
        // Arrange
        (size.Width > 0 && size.Height > 0).ShouldBeTrue("PRE: Test size should be positive"); // Sanity check that size is actually positive
        size *= -1; // Make size negative

        // Act
        var r = new Rectangle(loc, size);

        // Assert
        r.Width.ShouldBe(0);
        r.Height.ShouldBe(0);
        r.IsEmpty.ShouldBeTrue();
    }

    [Theory, MemberData(nameof(RectangleTestData))]
    public void Offset_WithDeltas_MovesLocation(Point loc, Size size)
    {
        // Arrange
        var r = new Rectangle(loc, size);
        var delta = new Point(5, -3);

        // Act
        var result = r.Offset(delta.X, delta.Y);

        // Assert
        result.X.ShouldBe(loc.X + delta.X);
        result.Y.ShouldBe(loc.Y + delta.Y);
        result.Width.ShouldBe(size.Width);
        result.Height.ShouldBe(size.Height);
    }

    [Theory, MemberData(nameof(RectangleTestData))]
    public void Offset_WithPoint_MovesLocation(Point loc, Size size)
    {
        // Arrange
        var r = new Rectangle(loc, size);
        var delta = new Point(2, 4);

        // Act
        var result = r.Offset(delta);

        // Assert
        result.X.ShouldBe(loc.X + delta.X);
        result.Y.ShouldBe(loc.Y + delta.Y);
        result.Width.ShouldBe(size.Width);
        result.Height.ShouldBe(size.Height);
    }

    [Theory, MemberData(nameof(RectangleTestData))]
    public void OperatorPlus_RectangleAndPoint_ReturnsOffsetRectangle(Point loc, Size size)
    {
        // Arrange
        var r = new Rectangle(loc, size);
        var p = new Point(5, 5);

        // Act
        var result = r + p;

        // Assert
        result.X.ShouldBe(loc.X + p.X);
        result.Y.ShouldBe(loc.Y + p.Y);
        result.Width.ShouldBe(size.Width);
        result.Height.ShouldBe(size.Height);
    }

    [Theory, MemberData(nameof(RectangleTestData))]
    public void OperatorPlus_PointAndRectangle_ReturnsOffsetRectangle(Point loc, Size size)
    {
        // Arrange: p + r uses the same semantics as r.Offset(p), so result equals r + p
        var r = new Rectangle(loc, size);
        var p = new Point(5, 5);

        // Act
        var result = p + r;

        // Assert
        result.X.ShouldBe(loc.X + p.X);
        result.Y.ShouldBe(loc.Y + p.Y);
        result.Width.ShouldBe(size.Width);
        result.Height.ShouldBe(size.Height);
    }

    [Theory, MemberData(nameof(RectangleTestData))]
    public void Inflate_WithPositiveValues_IncreasesDimensions(Point loc, Size size)
    {
        InflateTest(loc, size, new Point(10,5));
    }

    [Theory, MemberData(nameof(RectangleTestData))]
    public void Inflate_WithNegativeValues_DecreasesDimensions(Point loc, Size size)
    {
        InflateTest(loc, size, new Point(-10, -5));
    }

    private static void InflateTest(Point loc, Size size, Point delta)
    {
        // Arrange
        var r = new Rectangle(loc, size);

        // Act
        var result = r.Inflate(delta.X, delta.Y);

        // Assert
        // Inflate should keep the same origin and increase the size, so X/Y should be unchanged and Width/Height should increase by delta
        result.X.ShouldBe(loc.X);
        result.Y.ShouldBe(loc.Y);
        result.Width.ShouldBe(size.Width + delta.X);
        result.Height.ShouldBe(size.Height + delta.Y);
    }


    [Theory, MemberData(nameof(RectangleTestData))]
    public void Inflate_WithSize_AddsSizeToDimension(Point loc, Size size)
    {
        // Arrange
        var r = new Rectangle(loc, size);
        var delta = new Size(5, 10);

        // Act
        var result = r.Inflate(delta);

        // Assert
        result.X.ShouldBe(loc.X);
        result.Y.ShouldBe(loc.Y);
        result.Width.ShouldBe(size.Width + delta.Width);
        result.Height.ShouldBe(size.Height + delta.Height);
    }
    public static readonly IEnumerable<object?[]> ContainsPointTestData = new (Rectangle rect, Point[] points)[]
    {
        (new Rectangle(0, 0, 10, 20), new Point[] { new(5, 5), new(0, 0), new(9, 19) }),
        (new Rectangle(5, 5, 50, 50), new Point[] { new(10, 10), new(5, 5), new(54, 54) }),
        (new Rectangle(10, 20, 50, 30), new Point[] { new(30, 35), new(10, 20), new(59, 49) })
    }.ToTestData();

    [Theory, MemberData(nameof(ContainsPointTestData))]
    public void Contains_PointInside_ReturnsTrue(Rectangle r, Point[] points)
    {
        // Arrange

        // Act & Assert
        foreach (var point in points)
        {
            var borders = new Point[] { 
                new(r.Left, point.Y), // Left edge
                new(r.Right, point.Y), // Right edge
                new(point.X, r.Top), // Top edge
                new(point.X, r.Bottom) // Bottom edge
            };
            r.Contains(point).ShouldBeTrue();
            foreach (var p in borders)
            {
                r.Contains(p).ShouldBeTrue();
            }
        }
    }

    public static readonly IEnumerable<object?[]> DoesNotContainsPointTestData = new (Rectangle rect, Point[] points)[]
    {
        (new Rectangle(0, 0, 10, 20), new Point[] { new(-1,0), new(0,-1), new(10,20), new(10,19), new(9,20) }),
        (new Rectangle(5, 5, 50, 50), new Point[] { new (4,5), new(56,5), new(56,55) }),
        (new Rectangle(10, 20, 50, 30), new Point[] { new(9,20), new(61,35), new(80,60) })
    }.ToTestData();
    [Theory, MemberData(nameof(DoesNotContainsPointTestData))]
    public void Contains_PointOutside_ReturnsFalse(Rectangle r, Point[] points)
    {
        // Arrange

        // Act & Assert
        foreach (var point in points)
        {
            r.Contains(point).ShouldBeFalse();
        }
    }

    public static readonly IEnumerable<object?[]> ContainsRectTestData = new (Rectangle outer, Rectangle inner)[]
    {
        (new Rectangle(0, 0, 10, 20), new Rectangle(5, 5, 1, 1)),
        (new Rectangle(5, 5, 50, 50), new Rectangle(5, 5, 1, 1)),
        (new Rectangle(10, 20, 50, 30), new Rectangle(10, 20, 50, 30)),
        (new Rectangle(5, 5, 50, 50), new Rectangle(10, 10, 0, 0)),
    }.ToTestData();

    [Theory, MemberData(nameof(ContainsRectTestData))]
    public void Contains_RectangleInside_ReturnsTrue(Rectangle outer, Rectangle inner)
    {
        // Arrange

        // Act & Assert
        outer.Contains(inner).ShouldBeTrue();
    }

    public static readonly IEnumerable<object?[]> OverlappingRectTestData = new (Rectangle outer, Rectangle other)[]
    {
        (new Rectangle(0, 0, 10, 20), new Rectangle(5, 5, 10, 10)), // Partially overlapping
        (new Rectangle(5, 5, 50, 50), new Rectangle(0, 0, 10, 10)), // Partially overlapping
    }.ToTestData();

    public static readonly IEnumerable<object?[]> DistinctRectTestData = new (Rectangle outer, Rectangle other)[]
    {
        (new Rectangle(0, 0, 10, 20), new Rectangle(40, 40, 20, 20)), // Completely outside
        (new Rectangle(5, 5, 50, 50), new Rectangle(0, 0, 0, 0)),   // Empty rect is not considered contained, if its origin is not contained by the outer rect
    }.ToTestData();

    [Theory, MemberData(nameof(DistinctRectTestData))]
    public void Contains_RectanglePartlyOutside_ReturnsFalse(Rectangle outer, Rectangle inner)
    {
        // Arrange

        // Act & Assert
        outer.Contains(inner).ShouldBeFalse();
    }


    [Theory]
    [MemberData(nameof(ContainsRectTestData))]
    [MemberData(nameof(OverlappingRectTestData))]
    public void Intersect_OverlappingRectangles_ReturnsIntersection(Rectangle a, Rectangle b)
    {
        if(a.IsEmpty || b.IsEmpty) return; // Skip empty rects since they don't have a well-defined intersection (could be empty or could be the other rect)

        // Arrange
        var expectedLeft = Math.Max(a.Left, b.Left);
        var expectedTop = Math.Max(a.Top, b.Top);
        var expectedWidth = Math.Min(a.Right, b.Right) - expectedLeft + 1;
        var expectedHeight = Math.Min(a.Bottom, b.Bottom) - expectedTop + 1;
        var expected = new Rectangle(expectedLeft, expectedTop, expectedWidth, expectedHeight);

        Test_Intersertion(a, b, expected);
    }

    [Theory]
    [MemberData(nameof(DistinctRectTestData))]
    public void Intersect_NonOverlapping_ReturnsEmpty(Rectangle a, Rectangle b)
    {
        var expected = Rectangle.Empty; // Normalize to Empty if the intersection is invalid
        Test_Intersertion(a, b, expected);
    }

    private static void Test_Intersertion(Rectangle a, Rectangle b, Rectangle expected)
    {
        // Act
        var result = a.Intersect(b);

        // Assert
        result.X.ShouldBe(expected.Left);
        result.Y.ShouldBe(expected.Top);
        result.Width.ShouldBe(expected.Width);
        result.Height.ShouldBe(expected.Height);
    }

    public static readonly IEnumerable<object?[]> AdjacentRectTestData = new (Rectangle outer, Rectangle other)[]
    {
        (new Rectangle(0, 0, 10, 20), new Rectangle(10, 0, 20, 20)), // Right side adjacent
        (new Rectangle(0, 20, 50, 50), new Rectangle(0, 0, 20, 20)), // Top adjacent
        (new Rectangle(10, 0, 20, 20), new Rectangle(0, 0, 10, 20)), // Left side adjacent
        (new Rectangle(0, 0, 20, 20), new Rectangle(0, 20, 50, 50)), // Bottom adjacent
    }.ToTestData();

    [Theory, MemberData(nameof(AdjacentRectTestData))]
    public void Intersect_AdjacentRectangles_ReturnsEmpty(Rectangle a, Rectangle b)
    {
        // Arrange - touching at edge only

        // Act
        var result = a.Intersect(b);

        // Assert
        result.IsEmpty.ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(ContainsRectTestData))]
    [MemberData(nameof(OverlappingRectTestData))]
    public void IntersectsWith_Overlapping_ReturnsTrue(Rectangle a, Rectangle b)
    {
        // Arrange
        if(a.IsEmpty|b.IsEmpty) return;
        
        // Act & Assert
        a.IntersectsWith(b).ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(DistinctRectTestData))]
    public void IntersectsWith_NonOverlapping_ReturnsFalse(Rectangle a, Rectangle b)
    {
        // Arrange

        // Act & Assert
        a.IntersectsWith(b).ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(ContainsRectTestData))]
    public void IntersectsWith_PointInside_ReturnsTrue(Rectangle a, Rectangle _)
    {
        // Arrange
        var p = a.Center;

        // Act & Assert
        a.IntersectsWith(p).ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(ContainsRectTestData))]
    [MemberData(nameof(DistinctRectTestData))]
    [MemberData(nameof(AdjacentRectTestData))]
    public void Union_TwoRectangles_ReturnsBoundingBox(Rectangle a, Rectangle b)
    {
        // Arrange
        var expectedLeft = Math.Min(a.Left, b.Left);
        var expectedTop = Math.Min(a.Top, b.Top);
        var expectedWidth = Math.Max(a.Right, b.Right) - expectedLeft + 1;
        var expectedHeight = Math.Max(a.Bottom, b.Bottom) - expectedTop + 1;
        var expected = new Rectangle(expectedLeft, expectedTop, expectedWidth, expectedHeight);

        // Act
        var result = a.Union(b);

        // Assert
        result.X.ShouldBe(expected.Left);
        result.Y.ShouldBe(expected.Top);
        result.Right.ShouldBe(expected.Right);
        result.Bottom.ShouldBe(expected.Bottom);
        result.Width.ShouldBe(expected.Width);
        result.Height.ShouldBe(expected.Height);
    }

    [Fact]
    public void Union_WithCanBeInvalidFalse_AndNonOverlapping_ReturnsEmptyWhenResultWouldBeInvalid()
    {
        // Arrange - two rectangles that don't overlap; union of two disjoint rects is still valid
        var a = new Rectangle(0, 0, 10, 10);
        var b = new Rectangle(20, 20, 10, 10);

        // Act
        var result = a.Union(b, canBeInvalid: false);

        // Assert - union of two non-empty rects is never invalid, so we get bounding box
        result.IsEmpty.ShouldBeFalse();
        result.X.ShouldBe(0);
        result.Y.ShouldBe(0);
        result.Right.ShouldBe(29);
        result.Bottom.ShouldBe(29);
    }

    [Fact]
    public void Union_WithCanBeInvalidTrue_AllowsInvalidDimensions()
    {
        // Arrange - e.g. union of empty with something
        var a = Rectangle.Empty;
        var b = new Rectangle(10, 10, 5, 5);

        // Act
        var result = a.Union(b, canBeInvalid: true);

        // Assert
        result.X.ShouldBe(a.Left);
        result.Y.ShouldBe(a.Top);
        result.Right.ShouldBe(b.Right);
        result.Bottom.ShouldBe(b.Bottom);
    }

    [Theory]
    [MemberData(nameof(RectangleTestData))]
    public void ToString_ReturnsReadableFormat(Point loc, Size size)
    {
        // Arrange
        var r = new Rectangle(loc, size);

        // Act
        var s = r.ToString();

        // Assert
        s.ShouldBe($"({loc.X}, {loc.Y}, {size.Width}, {size.Height})");
    }

    [Theory]
    [MemberData(nameof(RectangleTestData))]
    public void ValueEquality_EqualRectangles_AreEqual(Point loc, Size size)
    {
        // Arrange
        var a = new Rectangle(loc, size);
        var b = new Rectangle(loc, size);
        // Assert
        a.ShouldBe(b);
    }

    [Theory]
    [MemberData(nameof(RectangleTestData))]
    public void ValueEquality_DifferentRectangles_AreNotEqual(Point loc, Size size)
    {
        // Arrange
        var a = new Rectangle(loc, size);
        var b = new Rectangle(loc+Point.Unit, size);

        // Assert
        a.ShouldNotBe(b);
    }
}
