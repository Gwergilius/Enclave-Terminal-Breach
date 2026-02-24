using Enclave.Common.Models;
using Enclave.Shared.IO;

namespace Enclave.Phosphor;

/// <summary>
/// Implementation of <see cref="IPhosphorCanvas"/> that uses <see cref="IConsoleIO"/> semantic operations only.
/// No escape sequences here; the console implementation (e.g. <see cref="Enclave.Shared.IO.ConsoleIO"/>) translates to ANSI or other protocols.
/// </summary>
public sealed class AnsiPhosphorCanvas : IPhosphorCanvas
{
    private readonly PhosphorTheme _theme;
    private readonly IConsoleIO _console;
    private readonly IScreenOptions _screenOptions;
    private bool _disposed;
    private bool _initialized;
    private CharStyle _style = CharStyle.Normal;

    /// <inheritdoc />
    public CharStyle Style
    {
        get => _style;
        set => _style = Enum.IsDefined<CharStyle>(value) ? value : CharStyle.Normal;
    }

    /// <inheritdoc />
    public int Width { get; private set; }

    /// <inheritdoc />
    public int Height { get; private set; }

    /// <summary>
    /// Initializes a new instance of <see cref="AnsiPhosphorCanvas"/> with the given theme, console, and screen options.
    /// </summary>
    /// <param name="theme">The phosphor theme for colour output.</param>
    /// <param name="console">Console abstraction for output and terminal properties.</param>
    /// <param name="screenOptions">Minimum dimensions and output encoding (e.g. from config).</param>
    public AnsiPhosphorCanvas(PhosphorTheme theme, IConsoleIO console, IScreenOptions screenOptions)
    {
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(screenOptions);
        _theme = theme;
        _console = console;
        _screenOptions = screenOptions;
    }

    /// <inheritdoc />
    public IPhosphorCanvas Initialize(string title)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(title);

        var (w, h) = _console.GetDimensions();
        if (w < _screenOptions.MinWidth || h < _screenOptions.MinHeight)
        {
            _console.OutputEncoding = _screenOptions.OutputEncoding;
            var message = $"Terminal too small: {w}×{h}. Minimum required: {_screenOptions.MinWidth}×{_screenOptions.MinHeight}.";
            _console.WriteLine(message);
            throw new InvalidOperationException(message);
        }

        Width = w;
        Height = h;

        _console.HideCursor();
        _console.Title = title;
        _console.OutputEncoding = _screenOptions.OutputEncoding;

        var bg = _theme.Palette[CharStyle.Background];
        _console.SetBackgroundColor(bg);
        _console.ClearScreen();

        _initialized = true;
        return this;
    }

    /// <inheritdoc />
    public void ClearScreen()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_initialized)
            throw new InvalidOperationException("Canvas must be initialized before clearing.");
        var bg = _theme.Palette[CharStyle.Background];
        _console.SetBackgroundColor(bg);
        _console.ClearScreen();
    }

    /// <inheritdoc />
    public void Write(string? value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (value is null)
            return;
        if (!_initialized)
            throw new InvalidOperationException("Canvas must be initialized before writing.");
        var color = _theme.Palette[Style];
        _console.SetForegroundColor(color);
        _console.Write(value);
    }

    /// <inheritdoc />
    public void WriteLine(string? value = null)
    {
        if (value is not null)
            Write(value);
        _console.WriteLine();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _console.ResetStyle();
        _console.ShowCursor();
        _console.Flush();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
