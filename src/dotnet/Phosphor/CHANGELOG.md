# Enclave.Phosphor Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable changes to the Enclave.Phosphor project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

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

[Unreleased]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/phosphor-v1.1.0...HEAD
[1.1.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/phosphor-v1.0.0...phosphor-v1.1.0
[1.0.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/releases/tag/phosphor-v1.0.0
