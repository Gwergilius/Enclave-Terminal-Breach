# Changelog

All notable changes to the Enclave Terminal Breach project.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- **Core – Password solver algorithm**: Implementation of the information-theoretic solver (see docs/Architecture/Algorithm.md). `IPasswordSolver` with `GetBestGuess`, `GetBestGuesses`, `CalculateInformationScore`, `NarrowCandidates`. Three strategies: **PasswordSolver** (tie-breaker by worst-case bucket), **BestScoreOnlySolver** (Excel-style), **RandomGuessSolver** (blind). Embedded word list `Resources/words.txt` (4–15 letters). Unit tests (`PasswordSolverTests`) and performance/convergence tests (`PasswordSolverAlgorithmPerformanceTests`) for random secret and adversarial scenarios; solver comparison doc and `PerformanceTest` category in Common.Test.Core.
- **Repository setup**: Initial README, folder structure (docs, src, .cursor/rules, .gitignore, LICENSE)
- **.cursor/rules/**: Coding standards and workspace (code-standards, communication, development-environment, documentation, naming-conventions, project-context, testing, workspace)
- **docs/Lore/**: ECHELON Project backstory (Project-History, UOS, Minigame, README)
- **docs/Architecture/**: Algorithm, StateMachine, PlatformServicesSummary, ConfigurationInfrastructureSummary, FutureArchitecture, ColorValue-Design-Decision, README; **resources/** (words.txt, README)
- **docs/Implementation/**: GHOST Blazor PWA implementation roadmap (Implementation-Plan, README)
- **docs/Development/**: Branching-Strategy, Commit-Conventions, Release-Process, GitVersion-Integration, Changelog-Management, README
- **.cursor/rules/development-workflow.md**: Mandatory rule for branching/commit and CHANGELOG update before commit
- **Hungarian translations (`.hu.md`)**: Full translations for all user-facing documentation (Lore, Architecture, Implementation, Development, READMEs). English remains primary; CHANGELOG files English-only. Language switcher on all English docs.

### Changed
- **Workflow**: Admin bypass guidelines for changelog finalization only (development workflow and rules)
- **docs/README.md**, **root README.md**: Document CHANGELOG and Development folder; links to Branching, Commit, Release, GitVersion, Changelog-Management
- **docs/CHANGELOG.md**: Documentation changelog (date-based sections)
- **.cursor/rules/documentation.md**: CHANGELOG files English-only; no `.hu.md`; no language switcher in CHANGELOG files
- **.gitignore**: Exclude all `Logs` and `logs` folders from version control (`[Ll]ogs/`)

See [Documentation Changelog](docs/CHANGELOG.md) for detailed documentation changes.
