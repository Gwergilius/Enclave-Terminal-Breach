# Blazor PWA UI – Pip-Boy terminal plan

## Summary
- Single main screen with two states: **Input** (add/remove words) and **Hacking** (guess + match entry).
- Pip-Boy-style monochrome terminal (palettes and font presets already available).
- Bottom status bar shows last message (info/warn/error) and also pops a toast in the bottom-right corner.
- Configuration panel as an overlay to pick palette, font, and UI language (EN/HU). Passwords/tips remain English words.
- Escape is the universal close/reset: closes overlays, exits hacking back to input (reset), and exits app when in input.
- User preferences (palette, font, UI language, toast timing) persisted in localStorage.

## Layout (wireframes)

### Input mode

![Input-Mock]

Status bar: [INFO/WARN/ERR] last message (shows mode)   
Toast overlay: bottom-right stack (auto-timeout + close)   

### Hacking mode
![Hacking-Mock]

Esc: reset game state and return to Input (no separate controls row)   
Status bar + Toast overlay same as Input   

### Configuration panel (overlay)

Hotkey: C (if command line is not active)

```
┌─ CONFIG ───────────────────────┐
│ Palette: (radio/grid, hotkeys) │
│  (1) green (2) amber (3) blue  │
│  ... existing palettes ...     │
│ Font:                          │
│  (1) Consolas (2) Fira Code... │
│ Language:                      │
│  (E) English (H) Hungarian     │
│ Toast timeout (s): [ 4 ]       │
│ [A] Apply                      │
│ [Esc] Close (no apply)         │
└────────────────────────────────┘
```

### Help panel (overlay)

Hotkey: F1

```
┌─ HELP ─────────────────────────────┐
│ Short game description + keys      │
│  - Input: 1 Add, 2 Remove, 3 Start │
│  - Hacking: Enter to submit tip    │
│  - Chips: 0..len                   │
│  - Esc: close/reset                │
│ [Esc] Close                        │
└────────────────────────────────────┘
```

**Todo:** Context dependent Help panel

## Key components (Web)
- `TerminalShell` (layout): header + content (Input/Hacking view) + status bar + toast overlay + Config/Help overlays.
- `WordGrid`: fixed-width word cells, viewport wrap, max 20 items.
- `CommandLine`: single-line input, blink/invert cursor, up/down + Tab + Enter + Backspace, autocomplete list; when focused, global hotkeys are disabled.
- `MenuPanel` (Input): 1/2/3 options; CommandLine only visible/active in Add/Remove sub-state; Esc exits app.
- `BestGuessPanel` (Hacking): top1 guess display and auto-fill.
- `MatchChips`: actual match values 0..len, hotkey + click; hotkeys off while command line focused.
- `ConfigPanel`: palette/font/language picker + toast timeout; persists to localStorage; Esc closes.
- `HelpPanel`: rules/keys overlay (Esc closes).
- `StatusBar`: last message + mode; intensity per type.
- `ToastOverlay`: queued messages, bottom-right; auto-timeout (configurable); max visible = 1 (sequential).

## State and logic
- Mode switching: Input ↔ Hacking. Start Hacking resets command line and pre-fills best guess. Win/Lose/Reset (Esc) returns to Input, clears chips/best guess, resets command line, emits status message.
- Word limit: max 20; over-limit → error (status + toast).
- Length rules: empty list → ≥4 and present in words.txt; non-empty → match first word length + not duplicate + in words.txt.
- WordGrid click: in Input/Remove fills the command line; in Hacking always fills as the tentative tip.
- Autocomplete source: Input/Add → words.txt; Input/Remove → PasswordList; Hacking → in-play PasswordList.
- Chips appear only after Entered tip; hotkey/click selects, solver updates, new best guess + status.
- Hotkey gating: when command line has focus, global hotkeys (menu, config, chips, help) are ignored.

## Status/Toast pipeline (MediatR)
- `StatusMessageNotification` (Shared/Core): message, type (Info/Warning/Error), timestamp, optional code (short error code for diagnostics/FluentResults).
- Handlers:
  - Core: log handler (Serilog/ILogger).
  - Web: status bar handler (updates last message).
  - Web: toast queue handler (enqueue, overlay consumes; timeout configurable, default 4s).

## Open details to finalize
- Error codes shorthand (e.g., ERR_LIMIT, ERR_LEN, ERR_DUP, ERR_DICT) for status/toast and logs.
- Toast timing default 4s; max visible toasts fixed to 1 (sequential display).
- Config persistence: palette/font/language/toast timeout in localStorage.
- Help content: concise rules + keymap summary (bilingual strings).


[//]: #References-and-image-links

[Input-Mock]: ../Images/UI-elements/UI-mockup-Input.svg
[Hacking-Mock]: ../Images/UI-elements/UI-mockup-Hacking.svg