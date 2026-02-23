using static System.Math;


namespace Enclave.Common.Drawing;

/// <summary>
/// Represents a rectangle defined by its upper-left corner coordinates and size.
/// </summary>
/// <param name="x">The left-coordinate of the upper-left corner of the rectangle.</param>
/// <param name="y">The top-coordinate of the upper-left corner of the rectangle.</param>
/// <param name="width">The width of the rectangle.</param>
/// <param name="height">The height of the rectangle.</param>
public readonly record struct Rectangle(int x, int y, int width, int height)
{
    /// <summary>
    /// Represents an <see cref="Enclave.Common.Drawing.Rectangle"/> structure with its properties left uninitialized.
    /// </summary>
    public static readonly Rectangle Empty = new(0, 0, 0, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="Enclave.Common.Drawing.Rectangle"/> class with the specified location and size.
    /// </summary>
    /// <param name="location">A <see cref="Enclave.Common.Drawing.Point"/> that specifies the coordinates of the upper-left corner of the rectangle.</param>
    /// <param name="size">A <see cref="Enclave.Common.Drawing.Size"/> that specifies the width and height of the rectangle.</param>
    public Rectangle(Point location, Size size)
        : this(location.X, location.Y, size.Width, size.Height)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Enclave.Common.Drawing.Rectangle "/> class using the specified top-left and bottom-right <see cref="Enclave.Common.Drawing.Point"/>s.
    /// </summary>
    /// <remarks>The rectangle includes both the top-left and bottom-right points. The width and height are
    /// calculated as the difference between the corresponding coordinates, plus one.</remarks>
    /// <param name="topLeft">The coordinates of the top-left corner of the rectangle.</param>
    /// <param name="bottomRight">The coordinates of the bottom-right corner of the rectangle.</param>
    public Rectangle(Point topLeft, Point bottomRight)
        : this(
              x: topLeft.X, 
              y: topLeft.Y, 
              width: bottomRight.X - topLeft.X + 1, 
              height: bottomRight.Y - topLeft.Y + 1)
    {
    }

    public readonly int X => x;
    public readonly int Y => y;
    public readonly int Width => Max(0, width);
    public readonly int Height => Max(0, height);
    public readonly int Left => X;
    public readonly int Right => X + Width - 1;
    public readonly int Top => Y;
    public readonly int Bottom => Y + Height - 1;
    public readonly Point Location => new(X, Y);
    public readonly Size Dimension => new(Width, Height);
    public readonly Point Center => new(X + Width / 2, Y + Height / 2); 

    /// <summary>
    /// Tests whether the <see cref="Enclave.Common.Drawing.Rectangle"/> has no area.
    /// </summary>
    public readonly bool IsEmpty => Width <= 0 || Height <= 0;

    /// <summary>
    /// Returns a new <see cref="Enclave.Common.Drawing.Rectangle"/> that is offset from the current rectangle by the specified horizontal and vertical
    /// amounts.
    /// </summary>
    /// <param name="x">The amount to offset the rectangle horizontally.</param>
    /// <param name="y">The amount to offset the rectangle vertically.</param>
    /// <returns>A new Rectangle that is offset by the specified amounts. The width and height remain unchanged.</returns>
    public readonly Rectangle Offset(int x, int y)
        => new(Location.Add(x, y), Dimension);

    //
    // Summary:
    //     Adjusts the location of this rectangle by the specified amount.
    //
    // Parameters:
    //   pos:
    //     Amount to offset the location.
    /// <summary>
    /// Returns a new <see cref="Enclave.Common.Drawing.Rectangle"/> that is offset from the current rectangle by the specified amount.
    /// </summary>
    /// <param name="pos">A <see cref="Point"/> that specifies the amount to offset the location of the rectangle.</param>
    /// <returns>A new <see cref="Rectangle"/> whose location is offset by the specified amount, with the same size as the
    /// original rectangle.</returns>
    public readonly Rectangle Offset(Point pos)
        => new(Location + pos, Dimension);


    /// <summary>
    /// Creates and returns an enlarged copy of the specified <see cref="Enclave.Common.Drawing.Rectangle"/>
    /// structure. The copy is enlarged by the specified amount. The original <see cref="Enclave.Common.Drawing.Rectangle"/>
    /// structure remains unmodified.
    /// </summary>
    /// <param name="x">The amount, in units, to increase the width of the rectangle. Can be negative to shrink the rectangle
    /// horizontally.</param>
    /// <param name="y">The amount, in units, to increase the height of the rectangle. Can be negative to shrink the rectangle
    /// vertically.</param>
    /// <returns>A new Rectangle instance representing the enlarged (or shrunken) rectangle.</returns>
    public readonly Rectangle Inflate(int x, int y)
        => new (Location, Dimension.Add(x,y));

    /// <summary>
    /// Returns a new <see cref="Enclave.Common.Drawing.Rectangle"/> that is inflated by the specified width and height values.
    /// </summary>
    /// <param name="size">A <see cref="Enclave.Common.Drawing.Size"/> structure that specifies the amount to inflate the rectangle horizontally and
    /// vertically.</param>
    /// <returns>A <see cref="Enclave.Common.Drawing.Rectangle"/> that represents the inflated rectangle. The width and height are increased by the
    /// corresponding values in <paramref name="size"/>.
    /// </returns>
    public readonly Rectangle Inflate(Size size)
        => new(Location, Dimension + size);

    /// <summary>
    /// Creates a <see cref="Enclave.Common.Drawing.Rectangle"/> that represents the intersection of this rectangle and the specified rectangle. 
    /// If the rectangles do not intersect, returns an empty rectangle.
    /// </summary>
    /// <param name="other">The <see cref="Enclave.Common.Drawing.Rectangle"/> to intersect with the current rectangle.</param>
    /// <returns>A <see cref="Enclave.Common.Drawing.Rectangle"/> that represents the area common to both rectangles. 
    /// If there is no intersection, an empty rectangle is returned.</returns>
    public readonly Rectangle Intersect(Rectangle other)
    {
        int left = Max(X, other.X);
        int top = Max(Y, other.Y);
        int width = Min(Right, other.Right) - left + 1;
        int height = Min(Bottom, other.Bottom) - top + 1;

        return width > 0 && height > 0 
            ? new Rectangle(left, top, width, height) 
            : Empty;
    }

    /// <summary>
    /// Determines whether this rectangle intersects with the specified <see cref="Enclave.Common.Drawing.Rectangle"/>.
    /// </summary>
    /// <param name="rect">The rectangle to test for intersection with this rectangle.</param>
    /// <returns>true if this rectangle and the specified rectangle intersect; otherwise, false.</returns>
    public readonly bool IntersectsWith(Rectangle rect)
    {
        var intersection = Intersect(rect);
        return !intersection.IsEmpty;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Enclave.Common.Drawing.Point"/> intersects with the current shape.
    /// </summary>
    /// <param name="pt">The <see cref="Enclave.Common.Drawing.Point"/> to test for intersection with the shape.</param>
    /// <returns>true if the specified point intersects with the shape; otherwise, false.</returns>
    public readonly bool IntersectsWith(Point pt) => Contains(pt);

    /// <summary>
    /// Returns a <see cref="Enclave.Common.Drawing.Rectangle"/> that represents the union of the current rectangle and the specified rectangle.    
    /// </summary>
    /// <remarks>
    /// The union of two rectangles is the smallest bounding box that can contain both of the rectangles.
    /// </remarks>
    /// <param name="other">The <see cref="Enclave.Common.Drawing.Rectangle"/> to combine with the current rectangle to form the union.</param>
    /// <param name="canBeInvalid">
    /// When true, allows returning a rectangle with zero/negative dimensions. When false (default), returns <see cref="Enclave.Common.Drawing.Rectangle.Empty"/> instead.
    /// </param>
    /// <returns>A <see cref="Enclave.Common.Drawing.Rectangle"/> that bounds both the current rectangle and the specified rectangle. If the resulting union has zero
    /// or negative width or height, and invalid recangle cannot be returned, then an empty rectangle is returned.
    /// </returns>
    public readonly Rectangle Union(Rectangle other, bool canBeInvalid = false)
    {
        int left = Min(X, other.X);
        int top = Min(Y, other.Y);
        int width = Max(Right, other.Right) - left + 1;
        int height = Max(Bottom, other.Bottom) - top + 1;

        return canBeInvalid || (width > 0 && height > 0) 
            ? new Rectangle(left, top, width, height)
            : Empty;
    }

    /// <summary>
    /// Determines whether the specified point is contained within this rectangle.
    /// </summary>
    /// <param name="pt">The point to test for inclusion within the rectangle.</param>
    /// <returns>true if the specified point lies within the bounds of this rectangle; otherwise, false.</returns>
    public readonly bool Contains(Point pt) => Contains(pt.X, pt.Y);

    /// <summary>
    /// Determines whether the specified point is contained within this rectangle.
    /// </summary>
    /// <param name="x">The x-coordinate of the point to test for containment.</param>
    /// <param name="y">The y-coordinate of the point to test for containment.</param>
    /// <returns>true if the point defined by x and y is within the bounds of this rectangle; otherwise, false.</returns>
    public readonly bool Contains(int x, int y)
        => X <= x && x <= Right && Y <= y && y <= Bottom;

    /// <summary>
    /// Determines whether the current rectangle completely contains the specified <see cref="Enclave.Common.Drawing.Rectangle"/>.
    /// </summary>
    /// <param name="other">The <see cref="Enclave.DrawingRectangle"/> rectangle to test for containment within the current rectangle.</param>
    /// <returns>true if the specified <see cref="Enclave.DrawingRectangle"/> is entirely contained within the current rectangle; otherwise, false.</returns>
    public readonly bool Contains(Rectangle other) 
        => Contains(other.Location) && Contains(other.Right, other.Bottom);


    //
    // Summary:
    //     Converts the attributes of this Enclave.Rectangle to a human-readable
    //     string.
    //
    // Returns:
    //     A string that contains the position, width, and height of this Enclave.Rectangle
    //     structure ¾ for example, {left=20, top=20, width=100, height=50}.
    public override readonly string ToString() => $"({Left}, {Top}, {Width}, {Height})";

    public static Rectangle operator +(Rectangle r, Point p) => r.Offset(p);
    public static Rectangle operator +(Point p, Rectangle r) => r.Offset(p);
}
