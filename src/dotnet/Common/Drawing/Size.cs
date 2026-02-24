namespace Enclave.Common.Drawing;

public readonly record struct Size(int Width, int Height)
{
    public static readonly Size Empty = new (0, 0);

    public Size(Point point) : this(point.X, point.Y)
    {        
    }

    public readonly Size Add(int dw, int dh) => new(Width + dw, Height + dh);
    public readonly Size Add(Size d) => Add(d.Width, d.Height);
    public readonly Size Subtract(Size d) => new(Width - d.Width, Height - d.Height);
    public readonly Size Scale(double factor) => new((int)(Width * factor), (int)(Height * factor));
    public readonly Point AsPoint() => new(Width, Height);

    public static Size operator +(Size a, Size b) => a.Add(b);

    public static Size operator -(Size a, Size b) => a.Subtract(b);
    public static Size operator *(double factor, Size s) => s.Scale(factor);
    public static Size operator *(Size s, double factor) => factor * s;
}
