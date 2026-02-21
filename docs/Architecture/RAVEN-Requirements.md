# RAVEN Requirements

**English** | [Magyar]

## Purpose

RAVEN is the **enhanced console UI** iteration of the ECHELON Terminal Breach System. It builds on the Core module's [Password Solver][Algorithm] implementations but replaces the sequential stdin/stdout interface of [SPARROW][SPARROW] with a **full-screen, colour-capable terminal UI**. RAVEN represents the first production-quality console deployment, emulating the original RobCo Terminal NX-12 hardware used by the Enclave SIGINT Division at Raven Rock Site-R.

## Platform Context

| Property | Value |
|----------|-------|
| **Platform codename** | RAVEN |
| **Version series** | 2.x.y |
| **Hardware emulation** | RobCo Terminal NX-12 (SIGINT Console, 180 lbs, laboratory use) |
| **Environment** | Console application, full-screen ANSI/VT100 capable terminal |
| **Lore period** | April–August 2076 (RAVEN v2.3.1 first UOS breach) |

## Technical Constraints

- **Console application**: Full-screen ANSI/VT100 colour terminal (no GUI framework).
- **I/O**: Direct console addressing via `PHOSPHOR 1.0` abstraction layer; no sequential stdin/stdout.
- **Colour**: Monochrome green phosphor palette by default; switchable via colour theme configuration.
- **Input**: Keyboard navigation (function keys, arrow keys, Enter, Escape); no mouse in PHOSPHOR 1.0.

## Version and Identity

- **Product name**: RAVEN (from the application's `Product` attribute and `IPlatformInfoService.ProjectCodename`).
- **Version**: Taken from the assembly version (e.g. `2.0.0`), consistent with GitVersion `raven-v*` tag series.

## Language

All application output (prompts, messages, labels, boot sequence text) **must be in English.** Language selection or localization is out of scope for RAVEN.

---

## Colour System

### Design Philosophy

RAVEN uses a **four-level phosphor brightness model** rather than a full RGB palette. This is intentional: authentic RobCo terminals used single-colour phosphor displays with brightness variation as the only visual distinction. The model is implemented via the platform-agnostic [`ColorValue`][ColorValue] type from the Shared project.

### Palette Levels

Each colour theme defines four brightness levels plus a background colour:

| Level | Role | Monochrome Green (default) |
|-------|------|---------------------------|
| `Background` | Screen background | `#0C190C` |
| `Dark` | Borders, inactive elements, dim text | `#1A4D1A` |
| `Normal` | Standard text, module loading messages | `#339933` |
| `Bright` | Headers, `OK` status, emphasis, active elements | `#66FF66` |
| `Inverse` | High-priority status (e.g. `OMEGA-7`) | Swapped fg/bg |

### Colour Theme Configuration

RAVEN supports multiple colour themes selectable at runtime or via configuration. All themes follow the same five-slot model but use different hue values.

Planned themes for RAVEN 2.0 (See [Palette.md]):

| Theme key | Description |
|-----------|-------------|
| `green` | Classic monochrome green phosphor (default) |
| `amber` | Amber phosphor (period-accurate alternative) |
| `white` | White-on-black (high-contrast) |
| `blue` | Enclave blue accent theme |

Theme selection is stored in platform configuration (see [Configuration][Configuration]).

> **PHOSPHOR 1.1 note:** Full independent foreground/background colour per cell is a PHOSPHOR 1.1 feature. PHOSPHOR 1.0 supports theme-level colour switching only (all text in the active theme's palette).

---

## Configuration

RAVEN extends the configuration infrastructure established for SPARROW and described in [ConfigurationInfrastructureSummary][ConfigInfra]. Configuration is loaded from an embedded `appsettings.json` resource and overridden by command-line arguments, following the same priority chain.

### Priority (highest to lowest)

1. **Command-line arguments**
2. **appsettings.json** (embedded resource)
3. **Built-in defaults**

### Command-Line Arguments

```bash
raven [options]

Options:
  -i, --intelligence <level>    Solver intelligence level (default: 1)
                                  0 = Random (HOUSE gambit)
                                  1 = Smart (Best-bucket)
                                  2 = Genius (Tie-breaker)
                                Aliases: house, bucket, tie

  -t, --theme <name>            Colour theme (default: green)
                                  green | amber | white | blue

  -w, --words <file>            Word list file path (optional)

  -h, --help                    Show help message and exit
  -v, --version                 Show version information and exit
```

### Configuration File (embedded appsettings.json)

```json
{
  "Raven": {
    "Intelligence": 1,
    "WordListPath": null,
    "Theme": "green",
    "Startup": {
      "SkipBootSequence": false,
      "ShowLoadTime": true
    }
  },
  "Platform": {
    "ProjectCodename": "RAVEN",
    "PlatformName": "ROBCO TERMINAL NX-12",
    "Timing": {
      "LineDelay": "200 ms",
      "SlowDelay": "500 ms",
      "OkStatusDelay": "250 ms",
      "ProgressUpdate": "0 ms",
      "ProgressDuration": "0 ms",
      "WarningPause": "1200 ms",
      "FinalPause": "600 ms"
    }
  }
}
```

> **Note:** `ProgressUpdate` and `ProgressDuration` are `0 ms` in RAVEN because PHOSPHOR 1.0 does not implement animated progress bars (see Boot Sequence – Phase 4). This keeps RAVEN lore-accurate as a POC-era terminal.

### IPlatformInfoService

RAVEN provides a `RavenPlatformInfoService : IPlatformInfoService` implementation. All timing constants, platform name, and boot sequence texts are sourced from this service, making them configuration-driven and testable. See [PlatformServicesSummary][PlatformSvcs] for the interface contract.

---

## Boot Sequence

The `StartupBadgePhase` of SPARROW/RAVEN 1.x is replaced by a multi-phase **Boot Sequence** that emulates RobCo Terminal NX-12 initialisation. The boot sequence is the first phase executed by `PhaseRunner` and runs before the main application UI.

All boot sequence text, timing, and structure are documented in [ECHELON_Boot_Sequence.md][BootSeq]. The RAVEN-specific details are summarised below.

### Phase 1 – System Initialisation

```
╔═══════════════════════════════════════════╗
║ ROBCO TERMINAL NX-12                      ║
║ BIOS v1.4.2.8 - RobCo Industries          ║
╚═══════════════════════════════════════════╝

Detecting Enclave SIGINT module..........................OK
Validating cryptographic signature.......................OK
Verifying clearance level...........................OMEGA-7
```

- Box drawing characters rendered in `Dark`; header text in `Bright`; `OK` in `Bright`; `OMEGA-7` in `Inverse`.
- Each line displayed after `LineDelay` (200 ms). `OK` / clearance level appears after additional `OkStatusDelay` (250 ms).

### Phase 2 – Project Header

```
╔═══════════════════════════════════════════╗
║ PROJECT RAVEN                             ║
║ Ver v2.0.0 - Enclave SIGINT Division      ║
╚═══════════════════════════════════════════╝

[BOOT SEQUENCE INITIATED]
```

- Version is read from `ProductInfo` (assembly version / `IPlatformInfoService.Version`).
- 500 ms pause (`SlowDelay`) before proceeding.

### Phase 3 – Module Loading

Sequential display of technical module lines with `OK` status per entry:

```
>> Initializing quantum decryption core...........OK
>> Loading RobCo UOS exploit database.............OK
...
```

- `>>` prompt in `Dark`; description in `Normal`; `OK` in `Bright`.
- Per-line delay: `LineDelay` (200 ms); `OK` appears after `OkStatusDelay` (250 ms).
- Module list is sourced from `IPlatformInfoService.SystemModules`.

### Phase 4 – System Integrity Check

```
[SYSTEM INTEGRITY CHECK]

Core modules verification...............................OK
Exploit library verification............................OK
Stealth mode verification...............................OK
```

> **RAVEN does NOT display animated progress bars.** The POC-era hardware predates the progress bar UI introduced in GHOST v3.0.0. `ProgressUpdate` and `ProgressDuration` are both `0 ms`; the integrity check uses simple `OK` lines only.

### Phase 5 – Authorisation Warning

```
WARNING: Unauthorized access to government 
         terminals is a federal offense.
         
AUTHORIZATION: ENCLAVE PERSONNEL ONLY
CLEARANCE LEVEL: OMEGA-7 VERIFIED
```

- `WARNING:` in `Bright`; remaining text in `Normal`.
- `WarningPause` (1200 ms) pause for readability.

### Phase 6 – Ready State

```
[RAVEN READY]
>> Awaiting target terminal connection...
```

- `FinalPause` (600 ms) before transitioning to main application UI.

### Skip Functionality

When `Startup.SkipBootSequence` is `true` in configuration, the boot sequence is bypassed entirely and the application transitions directly to the main UI. This is intended for development and automated testing; the option is disabled by default.

---

## PHOSPHOR Abstraction Layer

RAVEN introduces the **PHOSPHOR** console UI abstraction, which isolates all terminal rendering from application logic. PHOSPHOR is versioned independently of the RAVEN application version.

### PHOSPHOR 1.0 (Target for RAVEN 2.0.0)

PHOSPHOR 1.0 provides:

- **Full-screen canvas**: Clears and owns the entire terminal window on startup.
- **Sequential line rendering**: Lines are written top-to-bottom; no direct cursor positioning.
- **Colour output**: Foreground colour selection from the active theme's four palette levels (Background, Dark, Normal, Bright). The entire output uses one theme; per-cell colour is not supported.
- **Structured layout primitives**: Bordered panels (`╔═╗║╚╝`), section headers, key-hint sidebars (see [ConsoleUI mockup][ConsoleUI]).
- **Input handling**: Keyboard events routed to the active screen (Enter, Escape, function keys F1–F10, arrow keys). Mouse is not supported in PHOSPHOR 1.0.

The `IConsoleIO` interface of SPARROW/RAVEN 1.x is superseded by the PHOSPHOR rendering API. SPARROW's sequential `IConsoleIO` remains available as an internal compatibility shim if needed for testing.

### PHOSPHOR 1.1 (Future – not in RAVEN 2.0.0 scope)

Planned extensions for a later minor release:

- **Direct screen addressing**: Cursor can be positioned at any `(row, col)` coordinate.
- **Window system**: Overlapping window regions with relative coordinate addressing.
- **Popup windows**: Modal dialogs rendered on top of the current screen.
- **Toast notifications**: Transient overlay messages with auto-dismiss.

### PHOSPHOR 1.2 (Future – not in RAVEN 2.0.0 scope)

- **Mouse / pointing device support**: Click and scroll events routed to the active screen or window.

---

## Application UI
> This UI will be used from Raven 2.1.0. In the initial Raven version (2.0.0) we would use the legacy "UI" inherited from SPARROW (but together with the Raven Boot screen.

The full-screen layout follows the mockup defined in [ConsoleUI.md][ConsoleUI]:

```
 ┌────────────┐
┌┤RAVEN V2.x.x├───────────────────────────────────────┤Mode: INPUT├─┤F1:Help├─┐
|└────────────┘                                                               |
| F2:Remove Passwords   ┌┤ Passwords ├──────────────────────────────────────┐ |
| F3:Start Hacking      |  WORD1 WORD2 WORD3 WORD4 WORD5 WORD6 WORD7 WORD8  | |
| F4:Reset              └───────────────────────────────────────────────────┘ |
| F9:Config                                                                   |
| F10:Exit                                                                    |
|                                                                             |
├───────────────┐                                                             |
| Autocomplete  |                                                             |
| list:         |                                                             |
| WORD1         |                                                             |
| ...           |                                                             |
├───────────────┘                                                             |
| > Input line (with blinking cursor)                                         |
└┤INFO | StatusBar with info message                                         ├┘
```

### Screen Sections

| Section | Description |
|---------|-------------|
| **Title bar** | Application name + version (top-left), Mode indicator, F1:Help |
| **Sidebar** | Function key hints: F2 Remove, F3 Start Hacking, F4 Reset, F9 Config, F10 Exit |
| **Password panel** | Bordered panel showing current candidate list in multi-column layout |
| **Autocomplete panel** | Word list filtered by current input (from `words.txt` or candidate list in remove mode) |
| **Input line** | Current text entry with blinking cursor |
| **Status bar** | INFO prefix + contextual message (errors, hints, candidate count) |

---

## Application Flow

### RAVEN 1.3.x (current)

RAVEN 1.3.2 uses PHOSPHOR 1.0 with the legacy phase flow and a **replay loop**:

1. **Startup badge** – Run once at startup (product name, version, load time, intelligence, dictionary source).
2. **Replay loop** (exit only via Ctrl+C or Alt+F4):
   - **DataInputPhase** – Load or enter password candidates.
   - **HackingLoopPhase** – Suggest guesses, read match count, narrow candidates until “Correct. Terminal cracked.” or “No candidates” / “No candidates left.”
   - **“Press any key to play again…”** – Wait for any key.
   - **Clear screen** – Theme-aware clear via `IPhosphorCanvas.ClearScreen()`.
   - Loop back to DataInputPhase (new scope, fresh `IGameSession`).

**Ctrl+C** is handled at the top level: the event is cancelled so the process is not terminated with an error; a flag stops the loop, the canvas is disposed, and the process exits with code 0.

### Phase Execution Order (RAVEN 2.x planned)

```
PhaseRunner:
  1. BootSequencePhase   ← replaces StartupBadgePhase
  2. MainUiPhase         ← full-screen interactive UI (replaces DataInputPhase + HackingLoopPhase)
```

### Main UI State Machine

The main interactive screen follows the same state machine defined in [StateMachine.md][StateMachine], adapted for full-screen console rendering:

| State | Description |
|-------|-------------|
| `PasswordEntry` | Default. Password panel visible, input active, autocomplete from `words.txt`. |
| `PasswordDeletion` | Password panel items selectable; autocomplete shows existing candidates only. |
| `HackingGame` | Input hidden; recommended guess shown; match count selection active. |
| `GameOver` | Result panel shown (Win / Lose); F4:Reset available. |

### Navigation

| Key | Action |
|-----|--------|
| `F1` | Show help overlay |
| `F2` | Switch to `PasswordDeletion` state |
| `F3` | Switch to `HackingGame` state (requires ≥ 2 passwords) |
| `F4` | Reset to `PasswordEntry` state |
| `F9` | Open configuration panel |
| `F10` | Exit application |
| `Enter` | Submit current input / confirm selection |
| `Escape` | Cancel / return to previous state where applicable |
| `↑ ↓` | Navigate autocomplete list |
| `Tab` | Accept top autocomplete suggestion into input line |

---

## Core Integration

RAVEN depends on [Enclave.Echelon.Core][Core] and uses the same interfaces as SPARROW:

- `IPasswordSolver.GetBestGuess(candidates)` / `GetBestGuesses(candidates)` — recommended guess.
- `IPasswordSolver.NarrowCandidates(candidates, guess, matchCount)` — candidate filtering after each terminal response.
- `ISolverFactory` — selects the active solver strategy based on `RavenOptions.Intelligence`.
- `IGameSession` — shared session state between boot, input, and hacking phases.

---

## Testing

Testing follows the project's standard strategy (see [testing.md][Testing]):

- **Unit tests** (`xUnit`, `Shouldly`): All service, phase, and ViewModel logic. Mock-based (`Mock.Of` + `.AsMock()`); no real console I/O in unit tests.
- **BDD / Integration tests** (`ReqNRoll`): Key user journeys (password entry, hacking loop, game over) against a `TestConsoleIO` double.
- **UI tests** (`Playwright`): Out of scope for RAVEN (console-only platform).

Coverage thresholds remain: line ≥ 80%, branch ≥ 90% (enforced by CI).

---

## Summary

| Aspect | Requirement |
|--------|-------------|
| Role | Enhanced console UI; replaces SPARROW's sequential I/O with full-screen ANSI terminal |
| I/O | Full-screen PHOSPHOR 1.0 abstraction; keyboard navigation; no mouse in 2.0.0 |
| Identity | Product name RAVEN; version from assembly (raven-v* tag series, 2.x.y) |
| Language | All UI output in English; no localization in this release |
| Colour | Four-level phosphor palette; four switchable themes (green default); via `ColorValue` |
| Configuration | CLI args, embedded appsettings.json, built-in defaults; `IPlatformInfoService` for timing |
| Boot sequence | 6-phase RobCo NX-12 boot emulation; no progress bars (PHOSPHOR 1.0 / POC-era lore) |
| PHOSPHOR | 1.0: full-screen, sequential rendering, colour themes, keyboard input |
| PHOSPHOR 1.1 | Future: direct addressing, window system, popups, toasts |
| PHOSPHOR 1.2 | Future: mouse / pointing device support |
| State machine | PasswordEntry → PasswordDeletion / HackingGame → GameOver → PasswordEntry |
| Core integration | Same IPasswordSolver / ISolverFactory / IGameSession as SPARROW |
| Testing | xUnit + Shouldly (unit), ReqNRoll (BDD); no Playwright for console |

[Algorithm]: ./Algorithm.md
[SPARROW]: ./SPARROW-Requirements.md
[ColorValue]: ./ColorValue-Design-Decision.md
[ConfigInfra]: ./ConfigurationInfrastructureSummary.md
[PlatformSvcs]: ./PlatformServicesSummary.md
[BootSeq]: ../Design/ECHELON_Boot_Sequence.md
[ConsoleUI]: ../Design/ConsoleUI.md
[StateMachine]: ./StateMachine.md
[Core]: ../../README.md
[Testing]: ../../.cursor/rules/testing.md
[Magyar]: ./RAVEN-Requirements.hu.md
[Palette.md]: ../Design/Palette.md