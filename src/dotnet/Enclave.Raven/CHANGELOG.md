# Enclave.Raven Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable changes to the Enclave.Raven project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

## [1.3.3] - 2026-02-24

Refactor for **Enclave.Shared 1.0.0** compatibility and internal cleanup. No new features or breaking changes from RAVEN’s perspective; RAVEN requires Shared ≥ 1.0.0.

### Added
- **IProductInfo + DI** – Product name and version via `IProductInfo`; `ProductInfo.GetCurrent()` registered as singleton; Application and StartupBadgePhase receive it via constructor.
- **PhaseRegistry scoped with IEnumerable<IPhase>** – PhaseRegistry and IPhaseRegistry are scoped; PhaseRegistry constructor receives `IEnumerable<IPhase>` from DI (all phases in scope). `GetPhase(phaseName)` returns `Result<IPhase>` (NotFoundError if not found). No phase name→Type map; Application resolves IPhaseRegistry from current scope and uses GetPhase(nextPhase).
- **Unit test coverage** – CurrentScopeHolder (lazy scope, ResetScope, Dispose); ExitRequest (IsExitRequested, RequestExit); NavigationService (NextPhase/NextPhaseArgs, NavigateTo normal/Exit, null args, case-insensitive Exit); PhaseRegistry (GetPhase found/not found, case-insensitive, null phases, duplicate names); ApplicationExit (default and custom message); Rectangle operator Point+Rectangle; AnsiPhosphorCanvas Style setter (invalid enum value coerced to Normal). NavigationService null-args branch covered.

### Changed
- **IPhaseRegistry** – Returns `Result<IPhase>` from `GetPhase(phaseName)` instead of throwing; uses Result pattern and NotFoundError (code standards).
- **Application** – Resolves IPhaseRegistry from `currentScope.CurrentScope.ServiceProvider` each loop iteration; uses `phaseRegistry.GetPhase(nextPhase)` and handles failed Result (break with that result).
- **AnsiPhosphorCanvas** – Style setter validates with `Enum.IsDefined<CharStyle>(value)` and coerces invalid values to CharStyle.Normal; foreground/background use `_theme.Palette[style]` directly (no GetColorForStyle). Style getter always returns the value the canvas will use.

## [1.3.2] - 2026-02-21

### Added
- **PHOSPHOR 1.0 integration** – Raven uses `IPhosphorCanvas`, `IPhosphorWriter`, and `IPhosphorReader` instead of raw `IConsoleIO`. Canvas initialised at startup; output goes through Phosphor (green theme). **ConsoleKeyboardHandler** implements `IPhosphorReader` delegating to `IConsoleIO`; registered in DI.
- **Replay loop** – After each HackingLoop run (win or “No candidates”), show “Press any key to play again…”, wait for key, clear screen (theme-aware), then run DataInput again. Exit only via Ctrl+C or Alt+F4.
- **Ctrl+C handling** – `Console.CancelKeyPress` handled at top level; `e.Cancel = true` and flag set so the main loop exits normally, canvas is disposed, process returns exit code 0.
- **IPhosphorCanvas.ClearScreen()** – Clears screen and re-applies theme background between rounds.

### Changed
- **Phases** – `StartupBadgePhase`, `DataInputPhase`, and `HackingLoopPhase` now take `IPhosphorWriter` and `IPhosphorReader` (replacing `IConsoleIO`). ReadInt uses `PhosphorReaderExtensions.ReadInt(reader, writer, …)`.
- **Program** – Runs StartupBadge once, then infinite replay loop (DataInput → HackingLoop → wait key → clear). No longer uses `IPhaseRunner` for the main loop.
- **Cursor** – Cursor shown during `ReadLine`/`ReadKey` in `ConsoleIO` so input wait is visible; hidden again after read.

---

## [1.3.1] - 2026-02-20

### Changed
- **Use Enclave.Shared** – Replaced local IO, Models, and Errors with references to `Enclave.Shared`. Removed `IO/`, `Models/`, `Errors/` from this project.
- IO tests (ConsoleIntReader, TestConsoleIO) and Models tests (GameSessionTests) moved to **Enclave.Shared.Tests**.

### Added
- **ProjectReference** to `Enclave.Shared` for `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader`, `IGameSession`, `GameSession`, `DuplicatedPassword`.

---

[Unreleased]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/raven-v1.3.3...HEAD
[1.3.3]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/raven-v1.3.2...raven-v1.3.3
[1.3.2]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/raven-v1.3.1...raven-v1.3.2
[1.3.1]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/raven-v1.3.0...raven-v1.3.1
