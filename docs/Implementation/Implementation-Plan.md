# Blazor PWA UI Implementation Steps (Pip-Boy Terminal)

**English** | [Magyar]

## Scope
- Single-screen Pip-Boy-style UI with Input and Hacking modes.
- Config/Help overlays, hotkey gating, status/toast pipeline, localization (EN/HU), localStorage prefs.

## Steps
1) Shared contracts
   - Define `StatusMessageNotification` (message, type Info/Warning/Error, timestamp, code).
   - Align FluentResults/validation errors to use short codes (e.g., ERR_LIMIT/ERR_LEN/ERR_DUP/ERR_DICT/ERR_TIMEOUT).
   - Add MediatR notification handlers (logging in Core; status/toast in Web).
2) Web services/state
   - `IStatusBarState` + impl: holds last message + mode, `Notify(...)`.
   - `IToastService`: queue (max visible = 1), configurable timeout (default 4s, from config), consumes MediatR events.
   - `IConfigService`: load/save palette, font, UI language, toast timeout via localStorage; notify UI on changes.
   - `IHotkeyService`: global hotkeys with gating when command line focused.
3) UI skeleton
   - `TerminalShell` layout: header (mode, [C] Config, [H] Help, Esc info), content slot (Input/Hacking), status bar, toast overlay, overlays.
   - `WordGrid`, `CommandLine`, `MenuPanel` (Input), `BestGuessPanel` (Hacking), `MatchChips`, `ConfigPanel`, `HelpPanel`, `StatusBar`, `ToastOverlay`.
4) Input mode logic
   - Word limit 20; length/dict/duplicate rules; errors via status+toast.
   - CommandLine only in Add/Remove state; WordGrid click fills in Remove state.
   - Hotkeys: 1 Add, 2 Remove, 3 Start, Esc Exit; disabled when cmdline focused.
5) Hacking mode logic
   - Start Hacking: reset cmdline, preload best guess (top1).
   - Entered tip shows match chips (actual values 0..len); hotkeys 0..len (disabled when cmdline focused).
   - Esc resets game state to Input (win/lose/give-up behavior unified).
6) Localization (EN/HU)
   - String resources for UI (menus, buttons, status texts, help). Password/tip words remain English.
   - Config panel language toggle; apply triggers rerender and persist to localStorage.
7) Config/Help overlays
   - Config: palette/font/lang/toast timeout, Apply or Esc to close without apply.
   - Help: brief rules + keymap; Esc to close.
8) Persistence & bootstrap
   - On app start, load config from localStorage; apply palette/font/lang/toast timeout; fallback to defaults.
   - On change, save to localStorage and notify UI.
9) UX polish
   - Blink/invert cursor, consistent palette usage, status color by type.
   - Toasts sequential (max visible = 1), auto-timeout configurable.
10) Testing
   - Unit tests for state/services; integration/E2E with ReqNRoll; hotkey gating and notification flow coverage.

[Magyar]: ./Implementation-Plan.hu.md








