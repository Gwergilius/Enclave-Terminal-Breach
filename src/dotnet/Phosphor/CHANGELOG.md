# Enclave.Phosphor Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable changes to the Enclave.Phosphor project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

## [2.1.0] - 2026-03-16

### Added
- **IComponent** – Interface for renderable components (Bounds, Render contract).
- **ICompositor** – Compositor abstraction for layer-based rendering (used by RAVEN screens).

### Changed
- **Compositor** – Implements ICompositor; existing behavior unchanged.
- **PhosphorTypewriter** – Updated for compositor integration where used.

## [2.0.0] - 2026-03-03

### Added
- **IVirtualScreen** – Interface for a layered virtual screen buffer: `AddLayer(Rectangle, zOrder)`, `RemoveLayer(Layer)`, `Invalidate(Rectangle)`, `HasDirtyRegions`, `FlushDirtyRegions()`, `GetLayersInRegion(Rectangle)`.
- **VirtualScreen** – Thread-safe default implementation of `IVirtualScreen`; layers in a lock-guarded list, dirty regions via `DirtyRegionTracker`.
- **Layer** – Virtual character buffer for a screen region: absolute `Bounds`, `ZOrder`, `IsVisible`; `GetCell`/`SetCell` (validates control characters via `Debug.Assert`), `Clear()` (fills with `VirtualCell.Empty`), `MoveTo(Point)` (repositions without clearing buffer).
- **DirtyRegionTracker** – Internal thread-safe dirty-region accumulator: `Invalidate(Rectangle)`, `Flush()`, `HasRegions`.
- **IPhosphorCursor** / **AnsiPhosphorCursor** – 0-based cursor positioning via ANSI CUP escape (`ESC[row+1;col+1H`).
- **Compositor** – Internal three-phase render engine: Recompose (Z-ordered compositing with `VirtualCell.IsEmpty` transparency), Diff (changed cells only), Emit (consecutive same-style runs with cursor-position deduplication to skip redundant `MoveTo` calls).
- **PhosphorRenderLoop** – Event-driven render loop: reads keys via `IPhosphorInputLoop.ReadKey(CancellationToken)`, dispatches to handlers, flushes dirty regions via Compositor after each key.
- **LayerWriter** – Streaming text writer onto a `Layer`: `Write(string)` handles `\n` as CRLF, clips at bounds, validates control characters; `SetCell(relCol, relRow, VirtualCell)`, `MoveTo(int, int)`, `Style` property.
- **PhosphorInputLoop.ReadKey(CancellationToken)** – Cancellable single-key read: pre-read cancel check, blocking console read, post-read cancel check, null-stream guard (`InvalidOperationException`).

### Changed
- **CharStyle** – Moved to `Enclave.Common.Drawing`. **Breaking:** callers must update `using Enclave.Phosphor` → `using Enclave.Common.Drawing`.
- **IPhosphorCanvas** / **AnsiPhosphorCanvas** – Marked `[Obsolete]`; retained for backward compatibility. Prefer `IVirtualScreen`-based API for new development.

## [1.1.0] - 2026-02-24

### Added
- **PhosphorTypewriter** – **IPhosphorWriter** decorator for typewriter-style output: characters are enqueued and sent one-by-one by a background loop. Delays from **ITimingOptions** (CharDelay for normal characters, LineDelay for `\n`); waits only the remaining time since the previous character (no extra delay if the caller already paused). Uses **Enclave.Common.Waiter** for testable delays (optional constructor parameter).
- **ITimingOptions** – Interface for timing values (CharDelay, CharDelayFast, LineDelay, SlowDelay, OkStatusDelay, ProgressUpdate, ProgressDuration, WarningPause, FinalPause). Consumed by PhosphorTypewriter; typically bound from `Platform:Timing` in the host app (e.g. RAVEN).

### Changed
- **AnsiPhosphorCanvas** – Injects `IConsoleIO` instead of using `System.Console`; fully unit testable with `TestableConsoleIO`. **Write(string?)** treats `null` as no-op (no exception). Phosphor references Enclave.Shared and Enclave.Common (Waiter).

## [1.0.0] - 2026-02-21

### Added
- **PHOSPHOR 1.0** – Console UI abstraction layer for the ECHELON Terminal Breach System.
- **CharStyle** – Enum for palette slots: Background, Dark, Normal, Bright.
- **PhosphorTheme** – Theme record mapping CharStyle to ColorValue.
- **PhosphorThemeFactory** – Built-in themes: green (default), amber, white, blue.
- **IPhosphorWriter** – Sequential text output with style (Write, WriteLine, Style).
- **IPhosphorCanvas** – Full-screen canvas management (Initialize, Dispose, Width, Height).
- **IPhosphorKeyboardHandler** – Keyboard event handler interface.
- **IPhosphorInputLoop** – Polling-based input loop (Register, Run, Stop).
- **AnsiPhosphorCanvas** – ANSI escape sequence implementation for real terminals.
- **PhosphorInputLoop** – Console.ReadKey-based implementation.
- **Test doubles** (Enclave.Phosphor.Tests): TestPhosphorWriter, TestPhosphorInputLoop.

[Unreleased]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/phosphor-v2.1.0...HEAD
[2.1.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/phosphor-v2.0.0...phosphor-v2.1.0
[2.0.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/phosphor-v1.1.0...phosphor-v2.0.0
[1.1.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/phosphor-v1.0.0...phosphor-v1.1.0
[1.0.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/releases/tag/phosphor-v1.0.0
