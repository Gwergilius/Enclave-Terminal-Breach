using System.Text;

namespace Enclave.Phosphor;

/// <summary>
/// Configuration for the terminal screen: minimum dimensions and output encoding.
/// The caller supplies an implementation (e.g. from app settings or defaults).
/// </summary>
public interface IScreenOptions
{
    /// <summary>Minimum terminal width in columns. Initialization fails if the terminal is narrower.</summary>
    int MinWidth { get; }

    /// <summary>Minimum terminal height in rows. Initialization fails if the terminal is shorter.</summary>
    int MinHeight { get; }

    /// <summary>Output encoding (e.g. UTF-8 for box-drawing characters).</summary>
    Encoding OutputEncoding { get; }
}
