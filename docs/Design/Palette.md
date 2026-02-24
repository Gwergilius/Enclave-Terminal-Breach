# Terminal Palette Colors

![Palette Colors]

## Green Monitor

```csharp
// Classic monochrome green phosphor palette (Hercules-style)
public static class GreenPalette
{
    public static readonly Color Background = Color.FromArgb(0x0C, 0x19, 0x0C); // Very dark green
    public static readonly Color Dark = Color.FromArgb(0x1A, 0x4D, 0x1A);  // Dark green
    public static readonly Color Normal = Color.FromArgb(0x33, 0x99, 0x33); // Medium green
    public static readonly Color Bright = Color.FromArgb(0x66, 0xFF, 0x66); // Bright phosphor green
}
```

## Amber Monitor

```csharp
// Classic monochrome amber phosphor palette (vintage terminal-style)
public static class AmberPalette
{
    public static readonly Color Background = Color.FromArgb(0x19, 0x0C, 0x00); // Very dark brown
    public static readonly Color Dark = Color.FromArgb(0x65, 0x38, 0x11);  // Dark amber 
    public static readonly Color Normal = Color.FromArgb(0x99, 0x66, 0x00); // Medium amber
    public static readonly Color Bright = Color.FromArgb(0xFF, 0xBB, 0x33); // Bright phosphor amber
}
```

## White Monitor

```csharp
// Classic monochrome white phosphor palette (vintage terminal-style)
public static class WhitePalette
{
    public static readonly Color Background = Color.FromArgb(0x0A, 0x0A, 0x0A); // Very dark gray (almost black)
    public static readonly Color Dark = Color.FromArgb(0x4D, 0x4D, 0x4D);  // Dark gray
    public static readonly Color Normal = Color.FromArgb(0x99, 0x99, 0x99); // Medium gray
    public static readonly Color Bright = Color.FromArgb(0xE6, 0xE6, 0xE6); // Bright phosphor white
}
```


## Blue Monitor

```csharp
// Classic monochrome blue phosphor palette (vintage terminal-style)
public static class PhosphorBluePalette
{
    public static readonly Color Background = Color.FromArgb(0x00, 0x05, 0x19); // Very dark blue
    public static readonly Color Dark = Color.FromArgb(0x1A, 0x41, 0x7B);  // Dark blue
    public static readonly Color Normal = Color.FromArgb(0x33, 0x66, 0xCC); // Medium blue
    public static readonly Color Bright = Color.FromArgb(0x66, 0xBB, 0xFF); // Bright phosphor blue
}
```

[//]: #References-and-image-links
[Palette Colors]: ../Images/UI-elements/Palette.svg
