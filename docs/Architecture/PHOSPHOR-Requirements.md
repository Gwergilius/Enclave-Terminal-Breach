# PHOSPHOR Requirements

**English** | [Magyar]

## Overview

**PHOSPHOR** is the console UI abstraction layer of the ECHELON Terminal Breach System. It sits between the application logic (phases, ViewModels) and the operating system's terminal/console API, providing a structured, testable, and lore-accurate rendering surface.

PHOSPHOR is versioned independently of the platform applications. Each RAVEN release targets a specific PHOSPHOR version; later PHOSPHOR versions introduce new UI capabilities without breaking existing consumers.

> **In-universe context:** PHOSPHOR is the display driver stack of the RobCo Terminal NX-12. The name refers to the phosphor-coated CRT screen used in all RobCo terminals of the pre-war era.

### Relationship to `IConsoleIO`

The existing `IConsoleIO` interface (RAVEN 1.x / SPARROW) provides sequential stdin/stdout I/O with no cursor positioning or colour support. PHOSPHOR supersedes `IConsoleIO` as the primary rendering abstraction for RAVEN 2.x and later. `IConsoleIO` is retained as an internal compatibility shim for unit tests that do not require full-screen rendering.

---

## Version Roadmap

| Version | Target release | Scope |
|---------|---------------|-------|
| **PHOSPHOR 1.0** | RAVEN 2.0.0 | Full-screen canvas, sequential rendering, colour themes, keyboard input |
| **PHOSPHOR 1.1** | RAVEN 2.x (future) | Direct cursor addressing, window system, popups, toasts |
| **PHOSPHOR 1.2** | RAVEN 2.x (future) | Mouse / pointing device support |

---

## PHOSPHOR 1.0

### Goals

- Own and manage the entire terminal window (full-screen canvas).
- Provide colour output via a four-level phosphor palette; support multiple switchable themes.
- Expose structured layout primitives that match the [ConsoleUI mockup][ConsoleUI].
- Route keyboard events to registered handlers.
- Be fully testable without a real terminal (headless / test double).

### Non-Goals (deferred to later versions)

- Direct cursor addressing at arbitrary `(row, col)` positions → PHOSPHOR 1.1.
- Overlapping window regions or popup dialogs → PHOSPHOR 1.1.
- Mouse / pointing device events → PHOSPHOR 1.2.
- Animated progress bars (omitted intentionally; RAVEN is a POC-era terminal with no progress bar UI).

---

### Colour Model

#### CharStyle

Every text write operation carries a `CharStyle` value that selects which palette slot to use for the foreground colour:

| `CharStyle` | Role |
|-------------|------|
| `Background` | Renders text in the theme's background colour — against a normally-coloured cell this produces an effective highlight/inverse effect |
| `Dark` | Borders, inactive elements, dim separators |
| `Normal` | Standard body text, module loading messages |
| `Bright` | Headers, `OK` status, active/selected elements, emphasis |

```csharp
public enum CharStyle { Background, Dark, Normal, Bright }
```

All colour values are represented as [`ColorValue`][ColorValue] (platform-agnostic R/G/B/A record from `Enclave.Common`).

#### Built-in Themes

#### PhosphorTheme

A theme maps each `CharStyle` to a concrete `ColorValue`:

```csharp
public sealed record PhosphorTheme(
    string Key,
    IReadOnlyDictionary<CharStyle, ColorValue> Palette);
```

Four themes ship with PHOSPHOR 1.0. Hex values are taken from [Palette.md][Palette]:

| Key | Name | `Background` | `Dark` | `Normal` | `Bright` |
|-----|------|-------------|--------|----------|---------|
| `green` | Classic Green Phosphor (default) | `#0C190C` | `#1A4D1A` | `#339933` | `#66FF66` |
| `amber` | Amber Phosphor | `#190C00` | `#653811` | `#996600` | `#FFBB33` |
| `white` | White Phosphor | `#0A0A0A` | `#4D4D4D` | `#999999` | `#E6E6E6` |
| `blue` | Blue Phosphor | `#000519` | `#1A417B` | `#3366CC` | `#66BBFF` |

The active theme is selected at startup from `RavenOptions.Theme`; runtime switching is a PHOSPHOR 1.1 consideration.

> **Highlight rendering:** Elements requiring special visual prominence (e.g. `OMEGA-7`) are rendered with `CharStyle.Background`. Because the foreground is set to the theme's background colour, such text appears visually distinct against normal content — a low-cost inverse effect without a dedicated escape sequence.

---

### Canvas Management

PHOSPHOR 1.0 owns the entire terminal window from the moment `IPhosphorCanvas.Initialize()` is called until `Dispose()` is invoked.

**Initialization responsibilities:**

- Hide the system cursor.
- Set console title to `"RAVEN v{version} – ENCLAVE SIGINT"` (or equivalent).
- Clear the screen with the active theme's `Background` colour.
- Set console encoding to UTF-8 (required for box-drawing characters).
- Record terminal dimensions (`Width` × `Height` in columns and rows).

**Teardown responsibilities:**

- Restore the system cursor.
- Optionally restore the original console title.
- Flush any pending output.

**Terminal resize:** PHOSPHOR 1.0 does not support dynamic resize. If the terminal is resized after initialization, behaviour is undefined. A minimum terminal size of **80 columns × 24 rows** is enforced; startup aborts with a plain-text error if the terminal is smaller.

---

### Rendering Primitives

All rendering in PHOSPHOR 1.0 is **sequential** (top-to-bottom, left-to-right). There is no cursor positioning to arbitrary coordinates.

PHOSPHOR exposes a single low-level primitive: **`IPhosphorWriter`**. Higher-level UI elements (title bar, status bar, sidebar, panels, input line) are **application-layer concerns** built on top of `IPhosphorWriter` by the consuming platform project (e.g. `Enclave.Raven`). On Blazor or MAUI those same UI concepts are expressed as native components or controls — they do not use PHOSPHOR at all.

#### Text Output

```csharp
public interface IPhosphorWriter
{
    /// <summary>
    /// Current character style. All subsequent Write/WriteLine calls use this style
    /// until it is changed. Defaults to <see cref="CharStyle.Normal"/>.
    /// </summary>
    CharStyle Style { get; set; }

    /// <summary>Writes text at the current cursor position using <see cref="Style"/>.</summary>
    void Write(string text);

    /// <summary>Writes text followed by a newline using <see cref="Style"/>.
    /// When called with no argument, advances to the next line only.</summary>
    void WriteLine(string? text = null);
}
```

**Usage pattern:**
```csharp
writer.Style = CharStyle.Dark;
writer.Write(">> ");
writer.Style = CharStyle.Normal;
writer.Write("Loading module... ");
writer.Style = CharStyle.Bright;
writer.WriteLine("OK");
```

The `Style` property is the extension point for future inline control characters: a control character embedded in the output string can set `Style` mid-write, enabling seamless style transitions without changing the `IPhosphorWriter` interface.

To produce a highlight effect, set `Style = CharStyle.Background` — the theme's background colour becomes the foreground, making the text visually stand out against normal content (used e.g. for `OMEGA-7`).

> **Layer boundary:** `IPhosphorWriter` is the ceiling of the PHOSPHOR layer. Structured UI elements — bordered panels, title bars, status bars, sidebars, input lines — are built by the application layer on top of `IPhosphorWriter`. In `Enclave.Raven` these are dedicated renderer classes (`TitleBarRenderer`, `StatusBarRenderer`, etc.). In GHOST (Blazor) or MAUI they are native components; PHOSPHOR is not involved.

---

### Keyboard Input

PHOSPHOR 1.0 provides a polling-based keyboard input loop. The application registers handlers for specific key events; PHOSPHOR dispatches events on each keypress.

```csharp
public interface IPhosphorKeyboardHandler
{
    /// <summary>
    /// Called when the user presses a key.
    /// Return true to consume the event (stop further processing); false to pass through.
    /// </summary>
    bool OnKeyPressed(ConsoleKeyInfo key);
}

public interface IPhosphorInputLoop
{
    /// <summary>Registers a keyboard handler. Handlers are called in registration order.</summary>
    void Register(IPhosphorKeyboardHandler handler);

    /// <summary>
    /// Starts the blocking input loop. Returns when <see cref="Stop"/> is called.
    /// </summary>
    void Run(CancellationToken cancellationToken = default);

    /// <summary>Signals the input loop to exit cleanly.</summary>
    void Stop();
}
```

#### Standard Key Bindings (consumed by PHOSPHOR itself)

| Key | Action |
|-----|--------|
| `F1` | Trigger help overlay (application-level handler) |
| `F10` | Signal exit (application-level handler) |
| `Ctrl+C` | Cancel / graceful shutdown |

All other keys are forwarded to registered application handlers in order.

#### Character Input Buffer

For the input line, PHOSPHOR maintains a mutable character buffer:

- Printable characters are appended.
- `Backspace` removes the last character.
- `Enter` fires an `OnInputSubmitted(string buffer)` callback and clears the buffer.
- `Escape` fires `OnInputCancelled()` and clears the buffer.
- `↑` / `↓` fire `OnNavigateAutocomplete(direction)` for list navigation.
- `Tab` fires `OnAutocompleteAccepted()` to accept the highlighted autocomplete suggestion.

---

### Testability

The entire PHOSPHOR API is interface-based. Test doubles can be injected in unit and integration tests, recording all render calls without touching a real terminal.

Required test doubles (in `Enclave.Phosphor.Tests`):

| Interface | Test double responsibility |
|-----------|---------------------------|
| `IPhosphorWriter` | Record all `Write` / `WriteLine` calls as `IReadOnlyList<(string Text, CharStyle Style)>`; capture `Style` at the moment of each call |
| `IPhosphorInputLoop` | Simulate key presses by injecting `ConsoleKeyInfo` events programmatically |

Application-layer renderers (TitleBar, StatusBar, Sidebar, Panel, InputLine) have their own test doubles in `Enclave.Raven.Tests`, where they are tested against `TestPhosphorCanvas`.

This mirrors the role that `TestConsoleIO` plays for SPARROW/RAVEN 1.x unit tests.

---

### DI Registration

PHOSPHOR components are registered in `Startup.ConfigureServices` as singletons (one canvas per process lifetime):

```csharp
// PHOSPHOR 1.0 – full-screen console canvas
services.AddSingleton<PhosphorTheme>(
    _ => PhosphorThemeFactory.Create(ravenOptions.Theme));
services.AddSingleton<IPhosphorCanvas, AnsiPhosphorCanvas>();
services.AddSingleton<IPhosphorWriter>(sp =>
    sp.GetRequiredService<IPhosphorCanvas>());
services.AddSingleton<IPhosphorInputLoop, PhosphorInputLoop>();
```

Application-layer renderers (TitleBar, StatusBar, etc.) are registered separately in `Enclave.Raven`'s startup, as they belong to the application layer, not to PHOSPHOR.

---

## PHOSPHOR 1.1 (Planned)

> **Status:** Design intent only. Not in scope for RAVEN 2.0.0.

### Direct Screen Addressing

PHOSPHOR 1.1 introduces a coordinate system (`Column`, `Row`) that allows any renderer to write to any cell of the canvas. This unlocks:

- In-place update of a single status line without full redraw.
- Cursor positioned at a specific input field location.
- Differential rendering (only dirty cells redrawn).

```csharp
public interface IPhosphorCursor
{
    void MoveTo(int column, int row);
    (int Column, int Row) Position { get; }
}
```

### Window System

A **window** is a rectangular region of the canvas with its own coordinate origin. Windows can overlap. The window stack determines render order (topmost window rendered last, paints over lower windows).

```csharp
public interface IPhosphorWindow
{
    int Left { get; }
    int Top { get; }
    int Width { get; }
    int Height { get; }
    int ZOrder { get; }

    void Render(IPhosphorWriter writer);
    void Show();
    void Hide();
    void Close();
}
```

### Popup Windows

A **popup** is a modal window that blocks keyboard input to all windows below it in the Z-order until it is dismissed. Typical uses: confirmation dialogs, error alerts, help overlay.

### Toast Notifications

A **toast** is a non-modal, auto-dismissing overlay message rendered in a fixed corner of the canvas (default: bottom-right). It disappears after a configurable duration (default: 3 seconds).

```csharp
public interface IPhosphorToastService
{
    void Show(string message, TimeSpan? duration = null, CharStyle style = CharStyle.Bright);
}
```

---

## PHOSPHOR 1.2 (Planned)

> **Status:** Design intent only. Not in scope for RAVEN 2.0.0.

### Mouse / Pointing Device Support

PHOSPHOR 1.2 enables VT220-compatible mouse event reporting (ANSI escape `?1000h`). Supported events:

- Left click: fires `OnClick(column, row)` on the topmost window or element at the coordinate.
- Right click: fires `OnRightClick(column, row)`.
- Scroll wheel: fires `OnScroll(column, row, delta)`.

Mouse support is opt-in (disabled by default; enabled via configuration). On terminals that do not support mouse reporting, PHOSPHOR 1.2 degrades gracefully to keyboard-only operation.

```csharp
public interface IPhosphorMouseHandler
{
    bool OnClick(int column, int row);
    bool OnRightClick(int column, int row);
    bool OnScroll(int column, int row, int delta);
}
```

---

## Summary

| Aspect | PHOSPHOR 1.0 | PHOSPHOR 1.1 | PHOSPHOR 1.2 |
|--------|-------------|-------------|-------------|
| Canvas ownership | Full-screen | Full-screen | Full-screen |
| Rendering model | Sequential (top-to-bottom) | Direct addressing, differential | Direct addressing, differential |
| Colour | Theme-level (4 levels + inverse) | Theme-level | Theme-level |
| Window system | None | Overlapping windows, Z-order | Overlapping windows |
| Popups / Toasts | None | Yes | Yes |
| Keyboard input | Yes | Yes | Yes |
| Mouse input | No | No | Yes |
| Progress bars | No (POC-era lore) | Optional | Optional |
| Testability | Interface-based test doubles | Interface-based | Interface-based |
| Target release | RAVEN 2.0.0 | RAVEN 2.x (future) | RAVEN 2.x (future) |

[ColorValue]: ../Architecture/ColorValue-Design-Decision.md
[ConsoleUI]: ./ConsoleUI.md
[Palette]: ./Palette.md
[Magyar]: ./PHOSPHOR-Requirements.hu.md
