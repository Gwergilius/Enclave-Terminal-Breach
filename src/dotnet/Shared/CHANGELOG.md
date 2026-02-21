# Enclave.Shared Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable changes to the Enclave.Shared project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

### Changed
- **IConsoleIO** extended for PHOSPHOR: `Title`, `OutputEncoding`, `GetDimensions()`, `Flush()`. **ConsoleIO** delegates to `Console`; **TestConsoleIO** uses configurable `Dimensions`.

### Added
- **Phases/** – `IPhase` (common contract for all phases; moved from Enclave.Raven and Enclave.Sparrow).
- **Services/** – `IPhaseRunner`, `PhaseRunner` (orchestrates phases in order; moved from Enclave.Raven and Enclave.Sparrow).
- **Enclave.Shared.Tests** – `PhaseRunnerTests` (moved from Enclave.Raven.Tests and Enclave.Sparrow.Tests).

## [0.1.0] - 2026-02-20

### Added
- **Enclave.Shared** project – shared abstractions for SPARROW and RAVEN.
- **IO/** – `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader` (moved from Enclave.Raven).
- **Models/** – `IGameSession`, `GameSession` (moved from Enclave.Raven).
- **Errors/** – `DuplicatedPassword` (moved from Enclave.Raven).
- **Enclave.Shared.Tests** – `ConsoleIntReaderTests`, `TestConsoleIO`, `GameSessionTests`.
