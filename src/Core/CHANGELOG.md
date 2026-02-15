# Enclave.Echelon.Core Changelog

All notable changes to the Enclave.Echelon.Core project.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.0] - 2026-02-14

### Changed
- **Solver implementations renamed (Lore-aligned):** `PasswordSolver` → `TieBreakerPasswordSolver`, `BestScoreOnlySolver` → `BestBucketPasswordSolver`, `RandomGuessSolver` → `HouseGambitPasswordSolver`. Corresponding test classes renamed.
- **Abstract base class:** All solvers now inherit from `PasswordSolverBase`, which provides virtual defaults for `GetBestGuess`, `CalculateInformationScore`, and `NarrowCandidates`; only `GetBestGuesses` is abstract. Implementations override only where strategy differs.

### Added
- **Password solver** – `IPasswordSolver` with `GetBestGuess`, `GetBestGuesses`, `CalculateInformationScore`, `NarrowCandidates`. Implementations (Lore-aligned): `TieBreakerPasswordSolver` (ECHELON), `BestBucketPasswordSolver` (RAVEN), `HouseGambitPasswordSolver` (SPARROW HOUSE gambit).
- **Models** – `ScoreInfo` (score and worst-case bucket for a guess); `Password` extended with match-count cache and used by solver.
- **Embedded word list** – `Resources/words.txt` (4–15 letter words) for candidate source and tests; loaded via `Enclave.Common` ResourceExtensions.
- **Unit tests** – `TieBreakerPasswordSolverTests`, `BestBucketPasswordSolverTests`, `HouseGambitPasswordSolverTests` (GetBestGuess, GetBestGuesses, CalculateInformationScore, NarrowCandidates; Algorithm.md examples and edge cases).
- **Performance / convergence tests** – `PasswordSolverAlgorithmPerformanceTests` (random secret and adversarial; 4-step cap; solver comparison). Tagged with `[PerformanceTest]`.
