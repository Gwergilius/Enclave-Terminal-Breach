using System.Text;

namespace Enclave.Phosphor;

/// <summary>
/// Default implementation of <see cref="IScreenOptions"/> with configurable or default values.
/// </summary>
/// <param name="MinWidth">Minimum terminal width (default 80).</param>
/// <param name="MinHeight">Minimum terminal height (default 24).</param>
/// <param name="Encoding">Output encoding (default UTF-8). Stored as parameter to avoid property name clash with <see cref="OutputEncoding"/>.</param>
public sealed record ScreenOptions(
    int MinWidth = 80,
    int MinHeight = 24,
    Encoding? Encoding = null) : IScreenOptions
{
    /// <inheritdoc />
    public Encoding OutputEncoding => Encoding ?? System.Text.Encoding.UTF8;
}
