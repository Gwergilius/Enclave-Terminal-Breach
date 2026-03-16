# Enclave.Sparrow Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable changes to the Enclave.Sparrow project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

### Changed
- **Startup and Program** – Align with shared and test infrastructure changes. No functional change.

## [1.2.3] - 2026-03-03

### Changed
- **Enclave.Phosphor 2.0 / Enclave.Common 2.0** – Updated dependencies. `CharStyle` import updated to `Enclave.Common.Drawing`; `IPhosphorCanvas`/`AnsiPhosphorCanvas` usage continues via the deprecated path. No functional changes for SPARROW.

## [1.2.2] - 2026-02-24

### Changed
- **Enclave.Shared 1.0.0 compatibility** – Refactor to align with Enclave.Shared 1.0.0. No new features or breaking changes from SPARROW’s perspective; dependency and usage updated so SPARROW requires Shared ≥ 1.0.0.

## [1.2.1] - 2026-02-21

### Changed
- **Use Enclave.Shared** – Replaced local IO, Models, and Errors with references to `Enclave.Shared`. Removed `IO/`, `Models/`, `Errors/` from this project.
- IO tests (ConsoleIntReader, TestConsoleIO) and Models tests (GameSessionTests) moved to **Enclave.Shared.Tests**.
- **Use Enclave.Shared for PhaseRunner** – Replaced local `IPhaseRunner`, `PhaseRunner`, and `IPhase` with references to `Enclave.Shared`. Removed `Services/` from this project. PhaseRunner tests moved to **Enclave.Shared.Tests**.

### Added
- **ProjectReference** to `Enclave.Shared` for `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader`, `IGameSession`, `GameSession`, `DuplicatedPassword`.

[Unreleased]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.2.3...HEAD
[1.2.3]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.2.2...sparrow-v1.2.3
[1.2.2]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.2.1...sparrow-v1.2.2
[1.2.1]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.2.0...sparrow-v1.2.1
