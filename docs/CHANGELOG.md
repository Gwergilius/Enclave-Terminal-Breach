# Documentation Changelog

All notable documentation changes to the Enclave Terminal Breach project.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
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
