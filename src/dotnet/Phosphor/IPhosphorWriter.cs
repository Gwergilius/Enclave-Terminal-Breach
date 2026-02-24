using Enclave.Shared.IO;

namespace Enclave.Phosphor;

/// <summary>
/// Low-level primitive for sequential text output with style support.
/// All rendering in PHOSPHOR 1.0 is top-to-bottom, left-to-right.
/// </summary>
public interface IPhosphorWriter: IConsoleWriter
{
    /// <summary>
    /// Current character style. All subsequent Write/WriteLine calls use this style
    /// until it is changed. Defaults to <see cref="CharStyle.Normal"/>.
    /// </summary>
    CharStyle Style { get; set; }
}
