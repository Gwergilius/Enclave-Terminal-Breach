namespace Enclave.Common.Drawing;

public readonly record struct Point(int X, int Y)
{
    public static readonly Point Empty = new(0, 0);
    public static readonly Point Unit = new(1, 1);
    public static readonly Point ToUp = new(0, -1);
    public static readonly Point ToDown = new(0, 1);
    public static readonly Point ToLeft = new(-1, 0);
    public static readonly Point ToRight = new(1, 0);   

    public readonly Point Add(int deltaX, int deltaY) => new(X + deltaX, Y + deltaY);
    public readonly Point Add(Point other) => Add(other.X, other.Y);
    public readonly Point Subtract(Point other) => Add(-other.X, -other.Y);
    public readonly Point Scale(double factor) => new((int)(X * factor), (int)(Y * factor));
    public readonly Size AsSize() => new(X, Y);
    public readonly Size DistanceTo(Point other) => new(Math.Abs(other.X - X), Math.Abs(other.Y - Y));

    // Point ± Point → Point
    public static Point operator +(Point a, Point b) => a.Add(b);
    public static Point operator -(Point a, Point b) => a.Subtract(b);

    // Point ± Dimension → Point
    public static Point operator +(Point p, Size s) => new(p.X + s.Width, p.Y + s.Height);
    public static Point operator -(Point p, Size s) => new(p.X - s.Width, p.Y - s.Height);

    // Scalar * Point → Point
    public static Point operator *(double factor, Point p) => p.Scale(factor);
    public static Point operator *(Point p, double factor) => factor * p;

    
    public override readonly string ToString()
    {
        return $"({X}, {Y})";
    }
}
