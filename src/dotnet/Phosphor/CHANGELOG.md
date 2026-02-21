# Enclave.Phosphor Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable changes to the Enclave.Phosphor project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

### Changed
- **AnsiPhosphorCanvas** injects `IConsoleIO` instead of using `System.Console`; fully unit testable with `TestableConsoleIO`. Phosphor references Enclave.Shared.

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
