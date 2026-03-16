# Documentation Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable documentation changes to the Enclave Terminal Breach project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

### Added
- **PHOSPHOR component architecture docs (EN + HU)** – Added `docs/Architecture/PHOSPHOR-component-architecture.md` and `docs/Architecture/PHOSPHOR-component-architecture.hu.md`. The new document defines the component tree model (`LayerComponent` vs `ContentComponent`), `LayerWriter` clipping and relative coordinates, lifecycle boundaries, invalidate/render loop, MVVM responsibility matrix, and phased implementation milestones.
- **Enclave.Shared 1.0.0 compatibility** – Documented that RAVEN and SPARROW require Enclave.Shared ≥ 1.0.0 (breaking change). Shared README and CHANGELOG; Raven and Sparrow README Dependencies; Project Structure reference matrix note with links.
- **RAVEN 1.4.0 / PHOSPHOR 1.1.0** – Raven README: Configuration (System, Platform:Timing), Startup (PhosphorTypewriter, TimingOptions), Dependencies (Phosphor ≥ 1.1.0, ITimingOptions), Tests (TimingOptions, PhosphorTypewriter, Waiter). Root CHANGELOG and src/dotnet README updated for typewriter and 1.4.x.

### Changed
- **Architecture index docs (EN + HU)** – Updated `docs/Architecture/README.md` and `docs/Architecture/README.hu.md` to include links for PHOSPHOR component architecture.
- **Enclave.Raven CHANGELOG** – Released [1.4.0] (2026-02-24): typewriter effect, Platform.Timing, System config, IConsoleWriter null no-op. Version links unchanged (Unreleased, 1.4.0).
- **Enclave.Phosphor CHANGELOG** – Released [1.1.0]: PhosphorTypewriter, ITimingOptions, Waiter, Write(null) no-op.
- **Enclave.Raven CHANGELOG** – Released [1.3.3] (2026-02-24): refactor for Shared 1.0.0 compatibility, no new features or breaking changes. Version links updated (Unreleased, 1.3.3).
- **Phase Navigation State Machine** (EN + HU): Runner loop now describes resolution via `IPhaseRegistry.GetPhase(nextPhase)` returning `Result<IPhase>` (and handling failed Result). Registration section updated: PhaseRegistry is scoped and receives `IEnumerable<IPhase>` from DI; no name→Type map; phases registered as IPhase for collection.
- **Enclave.Raven README**: Contents table updated for current layout (Services, IO, Application, Startup, ProductInfo); dependencies include Phosphor; tests list includes CurrentScopeHolder, ExitRequest, NavigationService, PhaseRegistry, ApplicationExit, Rectangle, AnsiPhosphorCanvas. Link to Phase Navigation doc added.

## [1.2.0] - 2026-02-16

### Changed
- **Source layout docs**: Root README and README.hu now point to **src/dotnet/** for the .NET solution and build. **src/README.md** and **src/README.hu.md** added (platform overview: dotnet, excel-prototype, planned python/typescript). **src/dotnet/** READMEs updated (excel-prototype moved to parent; paths to docs/tools/.cursor fixed). **tools/coverage/** README and script updated for **src/dotnet/**.

## [1.1.0] - 2026-02-14

### Added
- **Architecture/Algorithm.md** (and **Algorithm.hu.md**): Section *Automated performance test (Fallout difficulty levels, 4-step cap)* with success-rate table (random secret, seed 17) and note that longer-word levels are not harder for the algorithm. **SolverComparison.md**: Comparison of TieBreaker, BestScoreOnly, and Random strategies under random secret and adversarial scenarios (tables and conclusion).
- **Repository**: Initial README and folder structure (docs, src, .cursor/rules, .gitignore, LICENSE)
- **.cursor/rules/**: Coding standards and workspace (code-standards, communication, development-environment, documentation, naming-conventions, project-context, testing, workspace)
- **Lore/**: ECHELON Project backstory and universe (Project-History, UOS, Minigame, README)
- **Architecture/** – Technical architecture and design
  * **Algorithm.md**: Password solver (information score, tie-breaker, convergence), Python usage/implementation, examples (SALES/TERMS/DANTA-LILTS), convergence speed (20 words, 5 vs 10 letters)
  * **StateMachine.md**: Terminal Hacker state machine (MAUI + Blazor, Boot → Main, PasswordEntry, etc.)
  * **PlatformServicesSummary.md**: Platform services design (RAVEN/GHOST/ECHELON, SIGNET), target file structure
  * **ConfigurationInfrastructureSummary.md**: Configuration system architecture
  * **FutureArchitecture.md**: Future Password Registry, Flyweight, caching
  * **ColorValue-Design-Decision.md**: Why custom ColorValue (platform-independent colors)
  * **README.md**: Index with links to all Architecture docs
- **Implementation/**: GHOST Blazor PWA implementation roadmap (shared contracts, web services, UI skeleton, input/hacking mode, localization, config/help, persistence, testing)
- **Development/** – Developer workflow and changelog management
  * **Branching-Strategy.md**: Git workflow, branch naming (feature/docs/fix/etc.), phase scopes, PR workflow
  * **Commit-Conventions.md**: Conventional Commits (types, scopes, subject/body/footer), examples
  * **Release-Process.md**: Semantic versioning per platform, pre-release checklist, release workflow
  * **GitVersion-Integration.md**: Automated versioning (tags, branch names, +semver: in commits)
  * **Changelog-Management.md**: Hybrid changelog strategy (central + component), when to update, categories
  * **README.md**: Quick reference (new work, commit, version check, PR, release)
- **.cursor/rules/development-workflow.md**: Mandatory rule for branching/commit conventions and CHANGELOG update before every commit (references docs/Development)
- **CHANGELOG.md**: Documentation changelog (date-based sections, Keep a Changelog) for tracking all doc changes
- **resources/**: **README.md** (words.txt format, usage, example sets); **words.txt** (word list for solver)
- **Hungarian translations (`.hu.md`)**: 
  - Full translations for all user-facing documentation; 
  - English remains primary per documentation standards. 
  - CHANGELOG files are English-only (no `.hu.md`). 
  - Added: 
    - **Lore/**: Project-History, UOS, Minigame, README; 
	- **Architecture/**: Algorithm, StateMachine, PlatformServicesSummary, ConfigurationInfrastructureSummary, FutureArchitecture, ColorValue-Design-Decision, README; 
	- **Implementation/**: Implementation-Plan, README; 
	- **Development/** Branching-Strategy, Commit-Conventions, Release-Process, GitVersion-Integration, Changelog-Management, README; 
	- **docs/** README; 
	- root, **resources/**, **src/**, **.cursor/rules/** READMEs. 
  - All `.hu.md` use reference-style links and mirror English structure; 
  - language switcher on each doc.

### Changed
- **README.md**: Document CHANGELOG.md and Development folder (structure, links to Branching/Commit/Release/GitVersion/Changelog-Management, reminder to update changelog before commit)
- **.cursor/rules/documentation.md**: CHANGELOG files are English-only; no `.hu.md` versions; no language switcher in CHANGELOG files
- **Architecture/ColorValue-Design-Decision**: Layer diagram now uses Mermaid-rendered SVG image (`.mmd.svg`); diagram source and links to PlantUML, DOT, and draw.io variants moved below the diagram; English doc uses English-only wording
- **Architecture/ColorValue-Design-Decision.hu.md**: Replaced single "Tesztelés és jövő" sentence with full **Tesztelés**, **Jövőbeli megfontolások** (optional System.Drawing bridge, possible extensions), **Összefoglalás**, and **Hivatkozások** sections to match the English structure
