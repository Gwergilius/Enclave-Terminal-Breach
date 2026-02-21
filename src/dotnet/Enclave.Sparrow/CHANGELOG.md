# Enclave.Sparrow Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable changes to the Enclave.Sparrow project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

## [1.2.1] - 2026-02-21

### Changed
- **Use Enclave.Shared** – Replaced local IO, Models, and Errors with references to `Enclave.Shared`. Removed `IO/`, `Models/`, `Errors/` from this project.
- IO tests (ConsoleIntReader, TestConsoleIO) and Models tests (GameSessionTests) moved to **Enclave.Shared.Tests**.
- **Use Enclave.Shared for PhaseRunner** – Replaced local `IPhaseRunner`, `PhaseRunner`, and `IPhase` with references to `Enclave.Shared`. Removed `Services/` from this project. PhaseRunner tests moved to **Enclave.Shared.Tests**.

### Added
- **ProjectReference** to `Enclave.Shared` for `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader`, `IGameSession`, `GameSession`, `DuplicatedPassword`.
