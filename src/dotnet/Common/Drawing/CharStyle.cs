namespace Enclave.Common.Drawing;

/// <summary>
/// Character style selecting which palette slot to use for foreground colour.
/// </summary>
public enum CharStyle
{
    /// <summary>Standard body text, module loading messages.</summary>
    Normal,

    /// <summary>Renders text in the theme's background colour — produces an effective highlight/inverse effect against normal content.</summary>
    Background,

    /// <summary>Borders, inactive elements, dim separators.</summary>
    Dark,

    /// <summary>Headers, OK status, active/selected elements, emphasis.</summary>
    Bright
}
