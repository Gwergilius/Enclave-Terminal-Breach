namespace Enclave.Phosphor;

/// <summary>
/// Manages the full-screen terminal canvas. PHOSPHOR 1.0 owns the entire terminal window
/// from <see cref="Initialize"/> until <see cref="Dispose"/>.
/// </summary>
/// <remarks>
/// The canvas instance also implements <see cref="IPhosphorWriter"/>; DI resolves both from the same singleton.
/// </remarks>
public interface IPhosphorCanvas : IPhosphorWriter, IDisposable
{
    /// <summary>
    /// Terminal width in columns. Valid after <see cref="Initialize"/>.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Terminal height in rows. Valid after <see cref="Initialize"/>.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Initializes the canvas: hide cursor, set title, clear screen, set UTF-8, record dimensions.
    /// Enforces minimum 80×24; throws if terminal is smaller.
    /// </summary>
    /// <param name="title">Console title (e.g. "RAVEN v{version} – ENCLAVE SIGINT").</param>
    /// <returns>This canvas instance for fluent chaining.</returns>
    IPhosphorCanvas Initialize(string title);

    /// <summary>
    /// Clears the screen and re-applies the theme background. Use between rounds (e.g. after "Press any key").
    /// </summary>
    void ClearScreen();
}
