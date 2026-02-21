# RAVEN 2.0.0 – Release Specification

**English** | [Magyar]

## Overview

This document is the implementation specification for **RAVEN 2.0.0**. It defines exactly what changes relative to the 1.x codebase, what new components are introduced, the project structure, and the acceptance criteria for the release.

**Reference documents:**
- [RAVEN-Requirements.md][RAVEN-Req] – functional requirements
- [PHOSPHOR-Requirements.md][PHOSPHOR-Req] – UI abstraction layer requirements
- [ECHELON_Boot_Sequence.md][BootSeq] – boot sequence content and timing
- [ConfigurationInfrastructureSummary.md][ConfigInfra] – configuration patterns
- [StateMachine.md][StateMachine] – application state machine

---

## Scope

### In scope for 2.0.0

| Area | Description |
|------|-------------|
| **PHOSPHOR 1.0** | Full-screen ANSI canvas, four colour themes, keyboard input loop, layout primitives |
| **Boot sequence** | Six-phase RobCo NX-12 boot emulation replacing `StartupBadgePhase` |
| **Configuration** | `Theme` option, `SkipBootSequence` flag, embedded `Platform` section |
| **appsettings.json** | Extended with `Platform` timing block; `Startup.ShowBanner`/`ShowLoadTime` replaced |
| **Shared migration** | `IConsoleIO`/`ConsoleIO`/`ConsoleIntReader` and `IPhaseRunner`/`PhaseRunner` move to `Enclave.Echelon.Shared` |

### Out of scope for 2.0.0

| Area | Deferred to |
|------|-------------|
| **Main UI phase** | RAVEN 2.1.0 – `DataInputPhase` + `HackingLoopPhase` remain active for this release |
| PHOSPHOR 1.1 (direct cursor addressing, window system, popups, toasts) | RAVEN 2.x |
| PHOSPHOR 1.2 (mouse support) | RAVEN 2.x |
| Animated progress bars | RAVEN 2.x (PHOSPHOR 1.1+) |
| Runtime theme switching (hot-swap without restart) | RAVEN 2.x |
| `IPlatformInfoService` full implementation (boot texts from config) | Tracked separately |

---

## Breaking Changes vs. 1.x

| Component | 1.x | 2.0.0 |
|-----------|-----|-------|
| `RavenStartupOptions.ShowBanner` | `bool` (show/hide badge) | **Removed** |
| `RavenStartupOptions.ShowLoadTime` | `bool` | **Removed** |
| `RavenStartupOptions.SkipBootSequence` | – | **Added** `bool` (default `false`) |
| `RavenOptions.Theme` | – | **Added** `string` (default `"green"`) |
| `StartupBadgePhase` | Active phase, runs first | **Replaced** by `BootSequencePhase` |
| `DataInputPhase` | Active phase | **Retained** – still active in 2.0.0; replaced in 2.1.0 |
| `HackingLoopPhase` | Active phase | **Retained** – still active in 2.0.0; replaced in 2.1.0 |
| `IConsoleIO` / `ConsoleIO` / `ConsoleIntReader` | Defined in `Enclave.Raven` | **Moved** to `Enclave.Echelon.Shared`; namespace changes |
| `IPhaseRunner` / `PhaseRunner` | Defined in `Enclave.Raven` | **Moved** to `Enclave.Echelon.Shared`; namespace changes |
| `PhaseRunner` phase list | `[StartupBadge, DataInput, HackingLoop]` | **Changed** to `[BootSequence, DataInput, HackingLoop]` |
| `appsettings.json` Startup section | `ShowBanner`, `ShowLoadTime` | `SkipBootSequence` |

> **Note on Shared migration:** `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader`, `IPhaseRunner`, and `PhaseRunner` are moved to `Enclave.Echelon.Shared`. Both `Enclave.Sparrow` and `Enclave.Raven` add a project reference to `Enclave.Echelon.Shared`; their existing using statements are updated accordingly. The move does not change behaviour.

---

## Project Structure

The `Enclave.Phosphor` project structure (directory layout, all interfaces and implementations) is defined in [ProjectStructure.md][ProjectStructure]. This section covers only the changes within `Enclave.Raven` itself.

### Modified files

| File | Change |
|------|--------|
| `Configuration/RavenStartupOptions.cs` | Remove `ShowBanner`/`ShowLoadTime`; add `SkipBootSequence` |
| `Configuration/RavenOptions.cs` | Add `Theme` property |
| `appsettings.json` | Rewrite Startup section; add Platform timing block |
| `Startup.cs` | Add `Enclave.Echelon.Shared` reference; register PHOSPHOR services; swap `StartupBadgePhase` → `BootSequencePhase` in phase runner |
| `Phases/IStartupBadgePhase.cs` | Mark `[Obsolete]` |
| `Phases/StartupBadgePhase.cs` | Mark `[Obsolete]`; retain for test compatibility |
| `IO/IConsoleIO.cs` | **Deleted** – moved to `Enclave.Echelon.Shared` |
| `IO/ConsoleIO.cs` | **Deleted** – moved to `Enclave.Echelon.Shared` |
| `IO/ConsoleIntReader.cs` | **Deleted** – moved to `Enclave.Echelon.Shared` |
| `Services/IPhaseRunner.cs` | **Deleted** – moved to `Enclave.Echelon.Shared` |
| `Services/PhaseRunner.cs` | **Deleted** – moved to `Enclave.Echelon.Shared` |

### New files (Phases)

| File | Description |
|------|-------------|
| `Phases/IBootSequencePhase.cs` | Interface for the six-phase boot emulation |
| `Phases/BootSequencePhase.cs` | Implementation; uses `IPhosphorWriter` + timing from `RavenOptions`/config |

### New files (RAVEN-layer renderers)

| File | Description |
|------|-------------|
| `Rendering/ITitleBarRenderer.cs` + `TitleBarRenderer.cs` | Full-width title bar with app badge, mode label, key hints |
| `Rendering/IStatusBarRenderer.cs` + `StatusBarRenderer.cs` | Full-width bottom status bar |
| `Rendering/ISidebarRenderer.cs` + `SidebarRenderer.cs` | Left-side function key list |
| `Rendering/IPanelRenderer.cs` + `PanelRenderer.cs` | Bordered box with optional title; used for password list and autocomplete |
| `Rendering/IInputLineRenderer.cs` + `InputLineRenderer.cs` | Prompt + buffer + blinking cursor |

### New files (Tests)

| File | Description |
|------|-------------|
| `tests/Unit/Enclave.Phosphor.Tests/Canvas/TestPhosphorCanvas.cs` | Records all Write/WriteLine calls; no real console |
| `tests/Unit/Enclave.Phosphor.Tests/Input/TestPhosphorInputLoop.cs` | Injects synthetic `ConsoleKeyInfo` events |
| `tests/Unit/Enclave.Raven.Tests/Phases/BootSequencePhaseTests.cs` | Verifies phase output and timing calls |

---

## Configuration Changes

### `RavenStartupOptions` (modified)

```csharp
/// <summary>
/// Startup options for RAVEN 2.0.0 (Raven:Startup section).
/// </summary>
public sealed class RavenStartupOptions
{
    /// <summary>
    /// When true, skips the boot sequence and transitions directly to the main UI.
    /// Intended for development and automated testing. Default: false.
    /// </summary>
    public bool SkipBootSequence { get; set; } = false;
}
```

### `RavenOptions` (modified)

```csharp
/// <summary>
/// Colour theme key. One of: "green" (default), "amber", "white", "blue".
/// Invalid values fall back to "green".
/// </summary>
public string Theme { get; set; } = "green";
```

### `appsettings.json` (rewritten)

```json
{
  "Raven": {
    "Intelligence": 1,
    "WordListPath": null,
    "Theme": "green",
    "Startup": {
      "SkipBootSequence": false
    }
  },
  "Platform": {
    "ProjectCodename": "RAVEN",
    "PlatformName": "ROBCO TERMINAL NX-12",
    "Timing": {
      "LineDelay":      "200 ms",
      "SlowDelay":      "500 ms",
      "OkStatusDelay":  "250 ms",
      "ProgressUpdate": "0 ms",
      "ProgressDuration":"0 ms",
      "WarningPause":   "1200 ms",
      "FinalPause":     "600 ms"
    }
  }
}
```

---

## PHOSPHOR 1.0 – Key Contracts

> PHOSPHOR provides only the low-level terminal primitives. The renderers below (`TitleBarRenderer`, etc.) are **RAVEN-layer components** that build on `IPhosphorWriter`; they live in `Enclave.Raven`, not in `Enclave.Phosphor`.

### Colour model

```csharp
// Models/CharStyle.cs  (Enclave.Phosphor)
public enum CharStyle { Background, Dark, Normal, Bright }

// Models/PhosphorTheme.cs  (Enclave.Phosphor)
public sealed record PhosphorTheme(
    string Key,
    IReadOnlyDictionary<CharStyle, ColorValue> Palette);
```

`ColorValue` is the existing platform-agnostic R/G/B/A type from `Enclave.Common`.

### Canvas & writer

```csharp
// Canvas/IPhosphorCanvas.cs  (Enclave.Phosphor)
public interface IPhosphorCanvas : IPhosphorWriter, IDisposable
{
    int Width  { get; }
    int Height { get; }
    PhosphorTheme Theme { get; }
    void Initialize();   // hide cursor, clear screen, UTF-8, validate ≥80×24
    void Clear();        // full-screen clear with Background colour
}

// Canvas/IPhosphorWriter.cs  (Enclave.Phosphor)
public interface IPhosphorWriter
{
    CharStyle Style { get; set; }
    void Write(string text);
    void WriteLine(string? text = null);
}
```

### Theme factory

```csharp
// Themes/PhosphorThemeFactory.cs  (Enclave.Phosphor)
public static class PhosphorThemeFactory
{
    public static PhosphorTheme Create(string key) => key switch
    {
        "amber" => Amber,
        "white" => White,
        "blue"  => Blue,
        _       => Green,
    };

    public static readonly PhosphorTheme Green = new("green", new Dictionary<CharStyle, ColorValue>
    {
        [CharStyle.Background] = new(0x0C, 0x19, 0x0C),
        [CharStyle.Dark]       = new(0x1A, 0x4D, 0x1A),
        [CharStyle.Normal]     = new(0x33, 0x99, 0x33),
        [CharStyle.Bright]     = new(0x66, 0xFF, 0x66),
    });
    // Amber, White, Blue defined analogously (see PHOSPHOR-Requirements.md)
}
```

---

## RAVEN-Layer UI Components

These components are defined in `Enclave.Raven` and depend on `IPhosphorWriter` from `Enclave.Phosphor`. They are the console equivalents of Blazor components or MAUI controls in the other platform projects.

### Renderer interfaces

```csharp
// Rendering/ITitleBarRenderer.cs
public interface ITitleBarRenderer
{
    /// <param name="appBadge">e.g. "RAVEN V2.0.0"</param>
    /// <param name="modeLabel">e.g. "Mode: INPUT"</param>
    /// <param name="hints">e.g. [("F1", "Help")]</param>
    void Render(string appBadge, string modeLabel,
                IEnumerable<(string Key, string Label)> hints);
}

// Rendering/IStatusBarRenderer.cs
public interface IStatusBarRenderer
{
    void Render(string prefix, string message,
                CharStyle messageStyle = CharStyle.Normal);
}

// Rendering/ISidebarRenderer.cs
public interface ISidebarRenderer
{
    /// <param name="items">(Key, Label, Enabled) triples</param>
    void Render(IEnumerable<(string Key, string Label, bool Enabled)> items);
}

// Rendering/IPanelRenderer.cs
public interface IPanelRenderer
{
    void BeginPanel(int width, string? title = null);
    void PanelLine(string content, CharStyle style = CharStyle.Normal);
    void EndPanel(int width);
}

// Rendering/IInputLineRenderer.cs
public interface IInputLineRenderer
{
    void Render(string buffer);
    void UpdateBuffer(string buffer);
}
```

### Screen composition & redraw

The full RAVEN screen is composed by `BootSequencePhase` and (in 2.1.0) `MainUiPhase` in the following order:

```
┌┤RAVEN V2.x.x├──────────────────────────────────┤Mode: INPUT├─┤F1:Help├─┐
|└────────────┘                                                            |
| F2:Remove Passwords  ┌┤ Passwords ├───────────────────────────────────┐ |
| F3:Start Hacking     |  WORD1 WORD2 WORD3 WORD4 WORD5 WORD6 WORD7     | |
| F4:Reset             └────────────────────────────────────────────────┘ |
| F9:Config                                                                |
| F10:Exit                                                                 |
├──────────────┐                                                           |
| Autocomplete |                                                           |
| WORD1        |                                                           |
| ...          |                                                           |
├──────────────┘                                                           |
| > input text here_                                                       |
└┤INFO | Status message                                                   ├┘
```

Render order per redraw: `canvas.Clear()` → `TitleBar` → `Sidebar` → `Panel(s)` → `InputLine` → `StatusBar`.

**Redraw strategy:** full-screen redraw on each state change. Optimisation deferred to PHOSPHOR 1.1.

### Keyboard input

```csharp
// Input/IPhosphorKeyboardHandler.cs
public interface IPhosphorKeyboardHandler
{
    /// <returns>true = event consumed; false = pass through</returns>
    bool OnKeyPressed(ConsoleKeyInfo key);
}

// Input/IPhosphorInputLoop.cs
public interface IPhosphorInputLoop
{
    void Register(IPhosphorKeyboardHandler handler);
    void Run(CancellationToken cancellationToken = default);
    void Stop();
}

// Input/IPhosphorInputBuffer.cs
public interface IPhosphorInputBuffer
{
    string Current { get; }
    void Append(char c);
    void Backspace();
    void Clear();

    event Action<string>  InputSubmitted;   // Enter
    event Action          InputCancelled;   // Escape
    event Action<int>     AutocompleteNavigated; // ↑(−1) / ↓(+1)
    event Action          AutocompleteAccepted;  // Tab
}
```

### Theme factory

```csharp
// Themes/PhosphorThemeFactory.cs
public static class PhosphorThemeFactory
{
    public static PhosphorTheme Create(string key) => key switch
    {
        "amber" => Amber,
        "white" => White,
        "blue"  => Blue,
        _       => Green,   // default + unknown keys → green
    };

    public static readonly PhosphorTheme Green = new("green",
        Background: new ColorValue(0x0C, 0x19, 0x0C),
        Dark:       new ColorValue(0x1A, 0x4D, 0x1A),
        Normal:     new ColorValue(0x33, 0x99, 0x33),
        Bright:     new ColorValue(0x66, 0xFF, 0x66));

    public static readonly PhosphorTheme Amber = new("amber",
        Background: new ColorValue(0x19, 0x0C, 0x00),
        Dark:       new ColorValue(0x65, 0x38, 0x11),
        Normal:     new ColorValue(0x99, 0x66, 0x00),
        Bright:     new ColorValue(0xFF, 0xBB, 0x33));

    public static readonly PhosphorTheme White = new("white",
        Background: new ColorValue(0x0A, 0x0A, 0x0A),
        Dark:       new ColorValue(0x4D, 0x4D, 0x4D),
        Normal:     new ColorValue(0x99, 0x99, 0x99),
        Bright:     new ColorValue(0xE6, 0xE6, 0xE6));

    public static readonly PhosphorTheme Blue = new("blue",
        Background: new ColorValue(0x00, 0x05, 0x19),
        Dark:       new ColorValue(0x1A, 0x41, 0x7B),
        Normal:     new ColorValue(0x33, 0x66, 0xCC),
        Bright:     new ColorValue(0x66, 0xBB, 0xFF));
}
```

---

## Boot Sequence Phase

`BootSequencePhase` replaces `StartupBadgePhase`. It executes the six sub-phases defined in [ECHELON_Boot_Sequence.md][BootSeq] for the RAVEN/Console POC platform.

```csharp
// Phases/IBootSequencePhase.cs
public interface IBootSequencePhase : IPhase { }

// Phases/BootSequencePhase.cs – constructor signature
public sealed class BootSequencePhase(
    IPhosphorWriter writer,
    RavenOptions options,
    ProductInfo productInfo) : IBootSequencePhase
```

### Sub-phase execution

| Sub-phase | Key output | Timing |
|-----------|-----------|--------|
| 1. System Init | NX-12 BIOS box, `Detecting…OK`, `Verifying…OMEGA-7` | `LineDelay` per line; `OkStatusDelay` before each status token |
| 2. Project Header | `PROJECT RAVEN` box, `[BOOT SEQUENCE INITIATED]` | `SlowDelay` pause after box |
| 3. Module Loading | `>> Module name…OK` × N lines | `LineDelay` per line; `OkStatusDelay` before `OK` |
| 4. Integrity Check | Simple `OK` lines (no progress bars) | `LineDelay` per line |
| 5. Auth Warning | `WARNING:` + auth/clearance lines | `WarningPause` pause |
| 6. Ready State | `[RAVEN READY]`, `>> Awaiting…` | `FinalPause` pause |

**Skip:** When `options.Startup.SkipBootSequence == true`, `Run()` returns immediately.

**Colour mapping:**

| Element | CharStyle |
|---------|-----------|
| Box drawing chars (`╔═╗║╚╝`) | `Dark` |
| Box header text | `Bright` |
| Module description text | `Normal` |
| `>>` prompt | `Dark` |
| `OK` status token | `Bright` |
| `OMEGA-7` token | `Background` (renders theme background colour as foreground – effective highlight) |
| `WARNING:` label | `Bright` |
| Warning body text | `Normal` |
| `[RAVEN READY]` | `Bright` |

---

## DI Registration (`Startup.cs`)

```csharp
public static IServiceCollection ConfigureServices(
    IServiceCollection services,
    IConfiguration configuration)
{
    // --- existing registrations (unchanged) ---
    // RavenOptions, IConfiguration, IGameSession, IRandom,
    // IPasswordSolver × 3, ISolverConfiguration, ISolverFactory
    // (see current Startup.cs)

    // --- configuration: read Theme ---
    var ravenOptions = /* bind from configuration */;
    services.AddSingleton(ravenOptions);

    // --- PHOSPHOR 1.0 (Enclave.Phosphor layer) ---
    services.AddSingleton<PhosphorTheme>(
        _ => PhosphorThemeFactory.Create(ravenOptions.Theme));
    services.AddSingleton<IPhosphorCanvas, AnsiPhosphorCanvas>();
    services.AddSingleton<IPhosphorWriter>(
        sp => sp.GetRequiredService<IPhosphorCanvas>());
    services.AddSingleton<IPhosphorInputBuffer, PhosphorInputBuffer>();
    services.AddSingleton<IPhosphorInputLoop,   PhosphorInputLoop>();

    // --- RAVEN-layer renderers (Enclave.Raven, built on IPhosphorWriter) ---
    services.AddSingleton<ITitleBarRenderer,  TitleBarRenderer>();
    services.AddSingleton<IStatusBarRenderer, StatusBarRenderer>();
    services.AddSingleton<ISidebarRenderer,   SidebarRenderer>();
    services.AddSingleton<IPanelRenderer,     PanelRenderer>();
    services.AddSingleton<IInputLineRenderer, InputLineRenderer>();

    // --- phases (2.0.0 list) ---
    services.AddScoped<IBootSequencePhase, BootSequencePhase>();
    services.AddScoped<IDataInputPhase,    DataInputPhase>();
    services.AddScoped<IHackingLoopPhase,  HackingLoopPhase>();

    // --- keep obsolete startup badge registered for test compat ---
    services.AddScoped<IStartupBadgePhase, StartupBadgePhase>();

    // --- phase runner: 2.0.0 order ---
    services.AddSingleton<IPhaseRunner>(sp => new PhaseRunner(
        sp.GetRequiredService<IServiceScopeFactory>(),
        [typeof(IBootSequencePhase), typeof(IDataInputPhase), typeof(IHackingLoopPhase)]));

    return services;
}
```

---

## Implementation Plan

Development proceeds in five sequential milestones. Each milestone is a mergeable unit with passing tests.

### Milestone 1 – Shared migration

**Goal:** `IConsoleIO`/`ConsoleIO`/`ConsoleIntReader` and `IPhaseRunner`/`PhaseRunner` moved to `Enclave.Echelon.Shared`; both Sparrow and Raven updated to reference Shared.

Tasks:
- Create `Enclave.Echelon.Shared` project; add to solution.
- Move `IO/IConsoleIO.cs`, `IO/ConsoleIO.cs`, `IO/ConsoleIntReader.cs` → `Enclave.Echelon.Shared/IO/`.
- Move `Services/IPhaseRunner.cs`, `Services/PhaseRunner.cs` → `Enclave.Echelon.Shared/Services/`.
- Update namespaces; add `<ProjectReference>` to `Enclave.Sparrow.csproj` and `Enclave.Raven.csproj`.
- Fix all broken using statements in both projects and their test projects.
- Unit tests: `Enclave.Echelon.Shared.Tests` project created; existing IO and PhaseRunner tests migrated.

**Done when:** CI green; no code remains in `Enclave.Sparrow/IO/`, `Enclave.Sparrow/Services/IPhaseRunner*`, `Enclave.Raven/IO/`, `Enclave.Raven/Services/IPhaseRunner*`.

---

### Milestone 2 – Configuration & theme factory

**Goal:** Configuration model updated; `CharStyle` and `PhosphorTheme` (Dictionary-based) implemented; no runtime change yet.

Tasks:
- Update `RavenStartupOptions`: remove `ShowBanner`/`ShowLoadTime`, add `SkipBootSequence`.
- Add `Theme` to `RavenOptions`.
- Rewrite `appsettings.json` with new Startup section and Platform timing block.
- Implement `CharStyle` enum (`Background`, `Dark`, `Normal`, `Bright`).
- Implement `PhosphorTheme` record with `IReadOnlyDictionary<CharStyle, ColorValue> Palette`.
- Implement `PhosphorThemeFactory` with all four built-in themes.
- Unit tests: `PhosphorThemeFactoryTests` – all four themes return correct hex values; unknown key falls back to green.

**Done when:** CI green; `PhosphorThemeFactory.Create("amber").Palette[CharStyle.Bright]` returns `#FFBB33`; unknown key returns green.

---

### Milestone 3 – PHOSPHOR 1.0 canvas & writer

**Goal:** `AnsiPhosphorCanvas` renders coloured text to a real terminal; `TestPhosphorCanvas` records calls for tests.

Tasks:
- Define `IPhosphorCanvas`, `IPhosphorWriter` interfaces (no `WriteInverse`; `Write`/`WriteLine` accept `CharStyle`).
- Implement `AnsiPhosphorCanvas`:
  - `Initialize()`: UTF-8, hide cursor, clear screen, validate ≥80×24.
  - `Style` property setter: emit ANSI foreground colour escape from `theme.Palette[Style]`.
  - `Write(text)`: write text to console using the currently active `Style`.
  - `WriteLine(text)`: write text (if any) then newline; no argument → newline only.
  - `Clear()`: `Console.Clear()` + reapply background.
  - `Dispose()`: restore cursor.
- Implement `TestPhosphorCanvas`: records all calls as `(string Text, CharStyle Style)` list, capturing the value of `Style` at the moment each `Write`/`WriteLine` is called.
- Unit tests: minimum terminal size enforcement; `TestPhosphorCanvas` recording.

**Done when:** CI green; `TestPhosphorCanvas` captures all writes with correct `CharStyle` values.

---

### Milestone 4 – RAVEN-layer renderers & input

**Goal:** All RAVEN-layer renderer implementations done and unit-tested against `TestPhosphorCanvas`.

Tasks:
- Implement all renderers in `Enclave.Raven/Rendering/`: `TitleBarRenderer`, `StatusBarRenderer`, `SidebarRenderer`, `PanelRenderer`, `InputLineRenderer` — each depends on `IPhosphorWriter`.
- Implement `PhosphorInputBuffer` (character buffer, events) — in `Enclave.Phosphor`.
- Implement `PhosphorInputLoop` (polling `Console.KeyAvailable`) — in `Enclave.Phosphor`.
- Implement `TestPhosphorInputLoop` (inject `ConsoleKeyInfo` from test code) — in `Enclave.Phosphor.Tests`.
- Unit tests in `Enclave.Raven.Tests`: each renderer's `CharStyle` usage and box-drawing character placement, using `TestPhosphorCanvas` as the writer double.
- Unit tests in `Enclave.Phosphor.Tests`: `PhosphorInputBuffer` — Append, Backspace, Enter/Escape/Tab/Arrow events.

**Done when:** CI green; all renderer and input tests pass; no real console required in tests.

---

### Milestone 5 – Boot sequence phase & phase runner swap

**Goal:** `BootSequencePhase` renders all six sub-phases; `StartupBadgePhase` retired; runner updated to 2.0.0 order.

Tasks:
- Implement `BootSequencePhase` using `IPhosphorWriter` + timing from `RavenOptions`/config.
- Module list: hardcoded initial set from [ECHELON_Boot_Sequence.md][BootSeq] Phase 3.
- `OMEGA-7` rendered with `CharStyle.Background` (effective highlight against normal foreground).
- Unit tests:
  - `WhenSkipBootSequence_WritesNothing`: assert zero write calls on `TestPhosphorCanvas`.
  - `AllSixSubPhasesRender`: verify key output tokens present.
  - `Omega7IsRenderedWithBackgroundStyle`: verify `CharStyle.Background` used for `OMEGA-7`.
- Update `Startup.cs`: register PHOSPHOR services; swap phase runner to `[BootSequencePhase, DataInputPhase, HackingLoopPhase]`.
- Mark `StartupBadgePhase` as `[Obsolete]`.

**Done when:** CI green; boot sequence tests pass; skip flag works; `raven -t amber` launches with amber theme and sequential data-entry UI.

---

## Acceptance Criteria

### Functional

- [ ] Application launches; boot sequence renders all six sub-phases with correct colour levels (verified by visual inspection and unit tests).
- [ ] `--skip-boot` / `Startup.SkipBootSequence = true` bypasses boot sequence; sequential data entry UI (`DataInputPhase`) appears immediately.
- [ ] `-t green|amber|white|blue` selects the corresponding theme; invalid value silently falls back to `green`.
- [ ] Sequential data-entry and hacking loop (RAVEN 1.x behaviour) fully functional after boot sequence.
- [ ] `F10` / `Ctrl+C` exits cleanly; cursor restored.

### Non-functional

- [ ] `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader`, `IPhaseRunner`, `PhaseRunner` no longer exist in `Enclave.Raven` or `Enclave.Sparrow`; both projects reference `Enclave.Echelon.Shared`.
- [ ] Minimum terminal size 80×24 enforced at PHOSPHOR canvas initialisation; startup aborts with a plain-text error on smaller terminals.
- [ ] All PHOSPHOR interfaces have at least one test double implementation (`TestPhosphorCanvas`, `TestPhosphorInputLoop`).
- [ ] No production code uses `WriteInverse`; all colour selection goes through `CharStyle`.
- [ ] Line coverage ≥ 80%; branch coverage ≥ 90% (CI threshold unchanged).
- [ ] `dotnet build` with no warnings (PHOSPHOR types non-nullable throughout).

### Lore / aesthetic

- [ ] `OMEGA-7` rendered with `CharStyle.Background` (effective inverse highlight).
- [ ] Box drawing characters use `CharStyle.Dark`.
- [ ] `OK` status tokens use `CharStyle.Bright`.
- [ ] No progress bars rendered (POC-era fidelity).
- [ ] Module loading lines: `>>` in `Dark`, description in `Normal`, `OK` in `Bright`.

---

## Out-of-scope Decisions (Recorded)

These were considered and explicitly deferred:

| Decision | Rationale |
|----------|-----------|
| `IPlatformInfoService` for boot module list | Config-driven module list is a nice-to-have; hardcoded list is acceptable for 2.0.0 |
| Differential / dirty-cell rendering | Full redraw is sufficient at 80×24; PHOSPHOR 1.1 scope |
| Runtime theme switching | Requires canvas re-init; PHOSPHOR 1.1 scope |
| `words.txt` autocomplete as async stream | Synchronous load is fine for a ≤100k word list on startup |
| ANSI mouse mode (`?1000h`) | PHOSPHOR 1.2 scope |

---

## References

[RAVEN-Req]: ./RAVEN-Requirements.md
[PHOSPHOR-Req]: ../Design/PHOSPHOR-Requirements.md
[ProjectStructure]: ./ProjectStructure.md
[BootSeq]: ../Design/ECHELON_Boot_Sequence.md
[ConfigInfra]: ./ConfigurationInfrastructureSummary.md
[StateMachine]: ./StateMachine.md
[Magyar]: ./RAVEN-2.0.0-Specification.hu.md
