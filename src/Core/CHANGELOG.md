# Enclave.Echelon.Core Changelog

All notable changes to the Enclave.Echelon.Core project.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- **Password solver** – `IPasswordSolver` with `GetBestGuess`, `GetBestGuesses`, `CalculateInformationScore`, `NarrowCandidates`. Implementations: `PasswordSolver` (information score + worst-case tie-breaker), `BestScoreOnlySolver` (score only, random tie-break), `RandomGuessSolver` (blind random).
- **Models** – `ScoreInfo` (score and worst-case bucket for a guess); `Password` extended with match-count cache and used by solver.
- **Embedded word list** – `Resources/words.txt` (4–15 letter words) for candidate source and tests; loaded via `Enclave.Common` ResourceExtensions.
- **Unit tests** – `PasswordSolverTests` (GetBestGuess, GetBestGuesses, CalculateInformationScore, NarrowCandidates; Algorithm.md examples and edge cases).
- **Performance / convergence tests** – `PasswordSolverAlgorithmPerformanceTests` (random secret and adversarial; 4-step cap; solver comparison). Tagged with `[PerformanceTest]`.
