# Changelog Management

Changelog maintenance strategy and guidelines for the Enclave Terminal Breach project.

## Overview

This project uses a **hybrid changelog strategy** to balance high-level release tracking with detailed component-specific changes. The approach provides:

- Quick release timeline overview (central CHANGELOG)
- Detailed component histories (per-platform changelogs)
- Clear separation between platforms and documentation
- Scalable structure for multi-platform evolution

## Changelog Structure

```
Enclave-Terminal-Breach/
├── CHANGELOG.md                           # Central release summary
├── docs/
│   └── CHANGELOG.md                       # Documentation changes
└── src/
    ├── Enclave.Echelon.SPARROW/
    │   └── CHANGELOG.md                   # SPARROW platform
    ├── Enclave.Echelon.RAVEN/
    │   └── CHANGELOG.md                   # RAVEN platform
    ├── Enclave.Echelon.GHOST/
    │   └── CHANGELOG.md                   # GHOST platform
    └── Enclave.Echelon.ECHELON/
        └── CHANGELOG.md                   # ECHELON platform
```

## Central CHANGELOG (Repository Root)

**Location:** `CHANGELOG.md`

**Purpose:** High-level release timeline and summary

**Contains:**
- Platform releases (tagged versions)
- Major documentation updates
- Breaking changes across platforms
- Security fixes
- Migration guides

**Format Example:**

```markdown
# Changelog

All notable releases of the Enclave Terminal Breach project.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Documentation
- GitVersion integration guide added
- Development workflow documentation completed

See [Documentation CHANGELOG](./docs/CHANGELOG.md) for details.

## [sparrow-v0.1.0] - 2026-02-15

### SPARROW - DOS Proof of Concept

First working implementation of the password elimination solver for DOS 3.11.

**Highlights:**
- Information-theoretic algorithm implementation
- Stdin/stdout terminal interaction
- Success rate: 28% on average difficulty

See [SPARROW CHANGELOG](./src/Enclave.Echelon.SPARROW/CHANGELOG.md) for complete details.

## Documentation Updates - 2026-02-10

### Added
- Complete lore documentation (Project History, UOS, Minigame mechanics)
- Architecture documentation (Algorithm, State Machine, Future plans)
- Development workflow (Branching, Commits, Releases, GitVersion)

See [Documentation CHANGELOG](./docs/CHANGELOG.md) for detailed changes.

[Unreleased]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v0.1.0...HEAD
[sparrow-v0.1.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/releases/tag/sparrow-v0.1.0
```

## Component Changelogs

### Documentation Changelog

**Location:** `docs/CHANGELOG.md`

**Purpose:** Detailed documentation change tracking

**Contains:**
- Documentation additions, changes, removals
- Date-based sections (not version tags)
- Detailed descriptions of what changed

**Format Example:**

```markdown
# Documentation Changelog

All notable documentation changes to the Enclave Terminal Breach project.

## [Unreleased]

## [2026-02-10] - Development Workflow Documentation

### Added
- **GitVersion-Integration.md**: Complete automated versioning guide
  * Installation and GitVersion.yml configuration
  * Multi-platform versioning with tag prefixes
  * MSBuild and CI/CD integration
  * Workflow examples and troubleshooting
  
- **Branching-Strategy.md**: Git workflow and branch management
  * GitHub Flow variant for solo development
  * Phase-based branch organization
  * PR workflow and merge strategies
  
- **Commit-Conventions.md**: Conventional Commits specification
  * Commit message format and examples
  * Type and scope definitions
  * Breaking change handling
  
- **Release-Process.md**: Version management and releases
  * Semantic versioning per platform
  * Pre-release checklist and workflow
  * Changelog maintenance guidelines

### Changed
- **Development/README.md**: Added GitVersion references

## [2026-02-08] - Architecture Documentation

### Added
- **Algorithm.md**: Information-theoretic password solver documentation
  * Formal proofs of correctness and convergence
  * Detailed examples with tables
  * Performance benchmarks (5 vs 10 letter words)
  * Python reference implementation
  * Academic references (Shannon, Cover & Thomas, Knuth)

### Changed
- Algorithm.md: Expanded from basic reference to publication-ready technical specification
```

### Platform Changelogs

**Location:** `src/Enclave.Echelon.{PLATFORM}/CHANGELOG.md`

**Purpose:** Platform-specific code changes

**Contains:**
- Feature additions
- Bug fixes
- Performance improvements
- Breaking changes
- Known limitations
- Version history aligned with git tags

**Format Example (SPARROW):**

```markdown
# SPARROW Changelog

All notable changes to the SPARROW (DOS Proof of Concept) platform.

## [Unreleased]

### Added
- Stdin password input validation

### Fixed
- Buffer overflow on long password inputs

## [0.1.0] - 2026-02-15

### Added
- Initial DOS 3.11 proof of concept implementation
- Information-theoretic password solver
- Stdin/stdout terminal interaction
- Basic password elimination algorithm
- Information score calculation

### Known Limitations
- Manual input required (no screen positioning)
- Limited to 4-8 character passwords
- No persistence between sessions
- Success rate: 28% average on medium difficulty

[Unreleased]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/sparrow-v0.1.0...HEAD
[0.1.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/releases/tag/sparrow-v0.1.0
```

## Workflow: When to Update

### During Feature Development

**Update:** Relevant component changelog ONLY

**Location:** Feature branch

**Process:**
```bash
# Working on SPARROW feature
git checkout -b feature/sparrow-stdin-input

# Edit component changelog
# File: src/Enclave.Echelon.SPARROW/CHANGELOG.md

## [Unreleased]

### Added
- Stdin password input with validation
- Buffer size checks for overflow prevention

# Commit changes
git add src/Enclave.Echelon.SPARROW/CHANGELOG.md
git commit -m "feat(sparrow): add stdin password input

+semver: minor"
```

**Rules:**
- ✅ Update component changelog in feature branch
- ✅ Add entry under `[Unreleased]` section
- ✅ Use appropriate category (Added, Changed, Fixed, etc.)
- ❌ Do NOT update central CHANGELOG.md
- ❌ Do NOT update other component changelogs
- ❌ Do NOT change `[Unreleased]` to version number

### After PR Merge to Main

**No changelog updates needed.**

The component changelog already contains the changes in the `[Unreleased]` section from the merged feature branch.

**Status:**
- ✅ Component changelog updated (from feature branch)
- ⏸️ Central CHANGELOG.md still untouched
- ⏸️ Version numbers still unreleased

### During Release Preparation

**Update:** Component changelog AND central changelog

**Location:** Main branch (in release preparation commit)

**Process:**

```bash
# On main branch, ready to release
git checkout main
git pull

# 1. Update component changelog
# File: src/Enclave.Echelon.SPARROW/CHANGELOG.md

## [0.1.0] - 2026-02-15  # ← Changed from [Unreleased]

### Added
- Stdin password input with validation
- Buffer size checks for overflow prevention

## [Unreleased]  # ← New empty section for future changes


# 2. Update central changelog
# File: CHANGELOG.md

## [sparrow-v0.1.0] - 2026-02-15

### SPARROW - DOS Proof of Concept

First working implementation with stdin input validation and buffer protection.

**Highlights:**
- Stdin/stdout terminal interaction
- Input validation and overflow protection
- Success rate: 28% average on medium difficulty

See [SPARROW CHANGELOG](./src/Enclave.Echelon.SPARROW/CHANGELOG.md) for complete details.


# 3. Commit both together
git add CHANGELOG.md src/Enclave.Echelon.SPARROW/CHANGELOG.md
git commit -m "chore(sparrow): prepare v0.1.0 release

- Update SPARROW CHANGELOG.md (Unreleased → 0.1.0)
- Update central CHANGELOG.md with release summary
- Bump version to 0.1.0"

# 4. Create tag
git tag -a sparrow-v0.1.0 -m "SPARROW v0.1.0 - DOS Proof of Concept

First working implementation of password elimination solver.
- Stdin/stdout interaction
- Input validation
- Buffer overflow protection"

# 5. Push
git push origin main sparrow-v0.1.0
```

**Critical Rules:**
- ✅ Update BOTH changelogs in ONE commit
- ✅ Component: Move `[Unreleased]` → `[X.Y.Z]`
- ✅ Component: Create new empty `[Unreleased]` section
- ✅ Central: Add release summary with link to component changelog
- ✅ Use date format: `YYYY-MM-DD`
- ✅ Commit BEFORE tagging
- ✅ Tag format: `{platform}-vX.Y.Z`

### Documentation Updates

**Special handling:** Documentation uses date-based sections, not version tags

**Process:**

```bash
# Working on documentation
git checkout -b docs/gitversion-integration

# Edit documentation changelog
# File: docs/CHANGELOG.md

## [Unreleased]

### Added
- **GitVersion-Integration.md**: Automated versioning guide
  * Installation and configuration
  * Multi-platform versioning
  * CI/CD integration

# Commit
git add docs/CHANGELOG.md
git commit -m "docs(dev): add GitVersion integration guide

+semver: none"

# After PR merge, when grouping documentation updates

# Update docs changelog with date
# File: docs/CHANGELOG.md

## [2026-02-10] - Development Workflow Documentation  # ← Date, not version

### Added
- **GitVersion-Integration.md**: Automated versioning guide
- **Changelog-Management.md**: Changelog strategy guide

# Update central changelog
# File: CHANGELOG.md

## Documentation Updates - 2026-02-10

### Added
- Development workflow documentation
  * GitVersion integration guide
  * Changelog management strategy

See [Documentation CHANGELOG](./docs/CHANGELOG.md) for detailed changes.

# Commit
git add CHANGELOG.md docs/CHANGELOG.md
git commit -m "docs: finalize development workflow documentation

- Update documentation CHANGELOG with date-based section
- Update central CHANGELOG with summary"
```

**Documentation rules:**
- ✅ Use date-based sections: `[YYYY-MM-DD]`
- ✅ Do NOT use version tags
- ✅ Group related documentation updates
- ✅ Update central CHANGELOG with summary + link

## Changelog Categories

Use these standard categories in order:

### Added
New features, new files, new capabilities

```markdown
### Added
- Stdin password input validation
- Buffer overflow protection
- Progress indicator during solving
```

### Changed
Changes to existing functionality

```markdown
### Changed
- Improved algorithm performance by 15%
- Updated UI color scheme to match Pip-Boy theme
- Refactored solver interface for better testability
```

### Deprecated
Features marked for removal (but still present)

```markdown
### Deprecated
- `ISolver.GetBestGuess()` - Use `GetBestGuesses()` instead
  Removal planned for v2.0.0
```

### Removed
Features that have been removed

```markdown
### Removed
- Legacy DOS 2.0 compatibility layer
- Obsolete `PasswordValidator` class
```

### Fixed
Bug fixes

```markdown
### Fixed
- Buffer overflow on passwords longer than 8 characters
- Incorrect match count calculation for repeated letters
- Memory leak in solver iteration loop
```

### Security
Security fixes and vulnerability patches

```markdown
### Security
- Fixed potential code injection in password input
- Updated dependencies to address CVE-2026-1234
```

## Single Source of Truth

**Principle:** Component changelogs are the source of truth.

**Central changelog rule:** Summary + link only

**Good central entry:**
```markdown
## [sparrow-v0.1.0] - 2026-02-15

### SPARROW - DOS Proof of Concept

First working implementation with input validation.

See [SPARROW CHANGELOG](./src/Enclave.Echelon.SPARROW/CHANGELOG.md) for details.
```

**Bad central entry (too detailed):**
```markdown
## [sparrow-v0.1.0] - 2026-02-15

### Added
- Stdin password input with validation
- Buffer size checks for overflow prevention
- Progress indicator during solving
- Error messages for invalid input

### Fixed
- Buffer overflow on long passwords
- Memory leak in solver loop

### Known Limitations
- Manual input required
- No screen positioning
...
```
*This duplicates the component changelog - just link to it instead.*

## Multi-Platform Releases

When releasing multiple platforms simultaneously:

**Option 1: Separate tags (Recommended)**

```bash
# Release SPARROW
git tag -a sparrow-v0.1.0 -m "SPARROW v0.1.0"

# Release RAVEN (on same commit or different)
git tag -a raven-v0.3.0 -m "RAVEN v0.3.0"

# Central changelog
## [sparrow-v0.1.0] - 2026-02-15
...

## [raven-v0.3.0] - 2026-02-15
...
```

**Option 2: Coordinated release**

```markdown
## Multiple Platforms - 2026-02-15

### SPARROW v0.1.0
DOS proof of concept release.
See [SPARROW CHANGELOG](./src/Enclave.Echelon.SPARROW/CHANGELOG.md).

### RAVEN v0.3.0
Console application release.
See [RAVEN CHANGELOG](./src/Enclave.Echelon.RAVEN/CHANGELOG.md).
```

## Hotfix Workflow

Hotfixes follow the same pattern but from a hotfix branch:

```bash
# Create hotfix branch from tag
git checkout -b hotfix/sparrow-v0.1.1 sparrow-v0.1.0

# Fix bug
git commit -m "fix(sparrow): resolve buffer overflow

+semver: patch"

# Update component changelog
# File: src/Enclave.Echelon.SPARROW/CHANGELOG.md

## [Unreleased]

### Fixed
- Buffer overflow on passwords longer than 8 characters

# Merge to main
git checkout main
git merge hotfix/sparrow-v0.1.1

# Prepare release (update both changelogs)
# Component: Unreleased → 0.1.1
# Central: Add 0.1.1 entry

git commit -m "chore(sparrow): prepare v0.1.1 hotfix release"
git tag -a sparrow-v0.1.1 -m "Hotfix: buffer overflow"
git push origin main sparrow-v0.1.1
```

## Automation Opportunities

### Changelog Generation (Future)

Consider tools for automated changelog generation:

**standard-version:**
```bash
npm install -g standard-version
standard-version --release-as patch
```

**conventional-changelog:**
```bash
npm install -g conventional-changelog-cli
conventional-changelog -p angular -i CHANGELOG.md -s
```

**Note:** Manual maintenance is recommended initially. Automation can be added once patterns are established.

## Best Practices

### Do:
- ✅ Update component changelog in feature branch
- ✅ Use clear, descriptive entries
- ✅ Update both changelogs during release (one commit)
- ✅ Link from central to component changelogs
- ✅ Use appropriate categories (Added, Changed, Fixed, etc.)
- ✅ Include dates in YYYY-MM-DD format
- ✅ Keep central changelog concise (summary + link)
- ✅ Add `[Unreleased]` comparison link

### Don't:
- ❌ Update central changelog in feature branches
- ❌ Duplicate content between central and component changelogs
- ❌ Skip changelog updates in feature branches
- ❌ Use vague entries like "bug fixes" or "improvements"
- ❌ Update changelogs after tagging (always before)
- ❌ Mix multiple platform releases in one changelog entry
- ❌ Forget to create new `[Unreleased]` section after release

## Template: New Component Changelog

When creating a new platform, use this template:

```markdown
# {PLATFORM} Changelog

All notable changes to the {PLATFORM} ({Description}) platform.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - YYYY-MM-DD

### Added
- Initial {PLATFORM} implementation
- Core features list

### Known Limitations
- Limitation 1
- Limitation 2

[Unreleased]: https://github.com/Gwergilius/Enclave-Terminal-Breach/compare/{platform}-v0.1.0...HEAD
[0.1.0]: https://github.com/Gwergilius/Enclave-Terminal-Breach/releases/tag/{platform}-v0.1.0
```

## References

- [Keep a Changelog] - Changelog format standard
- [Semantic Versioning] - Version numbering scheme
- [Conventional Commits] - Commit message standard
- [Release Process] - Complete release workflow

[Keep a Changelog]: https://keepachangelog.com/
[Semantic Versioning]: https://semver.org/
[Conventional Commits]: https://www.conventionalcommits.org/
[Release Process]: ./Release-Process.md
