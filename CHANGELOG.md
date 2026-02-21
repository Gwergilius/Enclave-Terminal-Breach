# Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html
[Documentation Changelog]: docs/CHANGELOG.md

All notable changes to the Enclave Terminal Breach project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

## [1.2.0] - 2026-02-16

### Added
- **SPARROW refactor (Core)**: **IRandom** and **GameRandom**; **ISolverFactory** / **SolverFactory** (DI-based solver selection from registered **IPasswordSolver** set); **SolverLevel** value object (aliases, ToString(prefix), FromInt/FromValue, TryParse); **IPasswordSolver.Level**; **ISolverConfiguration** (SparrowOptions). Solver and SolverLevel unit tests; CD workflow uses GitVersion and creates tag/release on main push.

### Changed
- **Source layout**: .NET solution and projects moved under **src/dotnet/**; **src/excel-prototype/** unchanged. Prepares for future **src/python/** and **src/typescript/** implementations. Added **src/README.md** and **src/README.hu.md**; updated root README, **src/dotnet/** READMEs, CI/CD workflows, and coverage docs/paths to use **src/dotnet/**.
- **CI**: Sonar OpenCover report path and format (coverage no longer 0%); CD release workflow uses GitVersion for version, creates and pushes tag when missing, then builds and publishes GitHub Release.

### Fixed
- Remaining Sonar issues (e.g. PasswordSolverBase argument null); version generation in CD workflow.

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

See [Documentation Changelog] for detailed documentation changes.

[Unreleased]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.2.0...HEAD
[1.2.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.1.2...sparrow-v1.2.0
[1.1.2]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.1.1...sparrow-v1.1.2
[1.1.1]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v1.1.0...sparrow-v1.1.1
[1.1.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/releases/tag/sparrow-v1.1.0
