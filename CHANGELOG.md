# Changelog

All notable changes to the Enclave Terminal Breach project.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.2] - 2026-02-16

### Fixed
- **Sonar issues**: Resolved findings in Core and tests. Added `==` and `!=` operators to `Password` with full unit test coverage (reference equality, null handling, case-insensitive word comparison).

## [1.1.1] - 2026-02-16

### Added
- **SPARROW unit tests** (Enclave.Sparrow.Tests): GameSession, DataInputPhase, HackingLoopPhase, StartupBadgePhase, PhaseRunner, Startup, ProductInfo, ConsoleIntReader, InvalidPassword, CandidateListFormatter. Coverage: ~98% line, ~93% branch.

### Changed
- **PhaseRunner** extracted from Program; Program uses IPhaseRunner.
- **ReadInt(min, max, defaultValue)** added to IConsoleIO; ConsoleIntReader helper for testability.
- **ProductInfo** extracted to public class.
- **Null checks** replaced with [NotNull] in DataInputPhase, HackingLoopPhase, StartupBadgePhase.
- **SetCandidate** simplified (direct indexer assignment).
- **ExcludeFromCodeCoverage** for Program (composition root), ConsoleIO (thin wrapper).

## [1.1.0] - 2026-02-14

### Added
- **SPARROW console app** (Enclave.Sparrow): DOS-style stdin/stdout UI with DI (Startup, ConfigureServices), phases (StartupBadge, DataInput, HackingLoop), shared GameSession, IPhase loop. Project name Enclave.Sparrow; Product SPARROW, Version 1.1.0.
- **Try the minigame** links in Algorithm.md and Minigame.md (Hackinal, Jetholt, OgnevOA).
- **README Acknowledgements**: Links to Bethesda Game Studios, Fallout Wiki, RobCo Industries, Hackinal, Jetholt.

### Changed
- **Core – Solver renames (Lore-aligned)**: `PasswordSolver` → `TieBreakerPasswordSolver`, `BestScoreOnlySolver` → `BestBucketPasswordSolver`, `RandomGuessSolver` → `HouseGambitPasswordSolver`. Abstract `PasswordSolverBase` with virtual GetBestGuess, CalculateInformationScore, NarrowCandidates; optional Random in HouseGambit/BestBucket.
- **Lore (Project-History)**: SPARROW/RAVEN/GHOST/ECHELON solver evolution (HOUSE gambit → best-bucket → DIVERGENCE → tie-breaker), version renumbering 1.x / 2.x / 3.x, NX-12 and Dr. Krane quotes.

See [Documentation Changelog](docs/CHANGELOG.md) for detailed documentation changes.

[Unreleased]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.1.2...HEAD
[1.1.2]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.1.1...sparrow-v1.1.2
[1.1.1]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.1.0...sparrow-v1.1.1
[1.1.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/releases/tag/sparrow-v1.1.0
