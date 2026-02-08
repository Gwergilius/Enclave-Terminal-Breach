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

**Create Pull Request and merge:**

```bash
# Push feature branch
git push origin feature/sparrow-stdin-input

# Create PR
gh pr create \
  --title "feat(sparrow): Add stdin password input" \
  --body "Implements stdin input with validation and buffer overflow protection."

# Review and merge
gh pr merge --squash
```

**After merge:** Component changelog contains changes in `[Unreleased]` section, but dates are NOT yet assigned.

### Changelog Finalization (Direct Commit on Main)

**IMPORTANT:** This is the ONLY scenario where direct commits to main are permitted.

**Requirements:**
- ✅ Feature PR must be merged first
- ✅ Only changelog date updates (no code changes)
- ✅ Both central and component changelogs updated together
- ✅ Use admin bypass privilege

**Process:**

```bash
# After feature PR is merged, checkout main
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


# 3. Commit directly to main (admin bypass)
git add CHANGELOG.md src/Enclave.Echelon.SPARROW/CHANGELOG.md
git commit -m "chore(sparrow): finalize v0.1.0 changelog

- Update SPARROW CHANGELOG.md (Unreleased → 0.1.0)
- Update central CHANGELOG.md with release summary
- Finalize changelog after PR #XX merge"

# 4. Push directly to main (uses admin bypass)
git push origin main

# 5. Create tag
git tag -a sparrow-v0.1.0 -m "SPARROW v0.1.0 - DOS Proof of Concept

First working implementation of password elimination solver.
- Stdin/stdout interaction
- Input validation
- Buffer overflow protection"

# 6. Push tag
git push origin sparrow-v0.1.0
```

**Critical Rules:**
- ✅ Update BOTH changelogs in ONE commit
- ✅ Component: Move `[Unreleased]` → `[X.Y.Z]`
- ✅ Component: Create new empty `[Unreleased]` section
- ✅ Central: Add release summary with link to component changelog
- ✅ Use date format: `YYYY-MM-DD`
- ✅ Commit directly to main (admin bypass)
- ✅ Create tag AFTER changelog finalization
- ❌ Do NOT include code changes in this commit
- ❌ Do NOT bypass PR for feature work

### Documentation Changelog Finalization

**Special handling:** Documentation uses date-based sections, not version tags

**Process:**

```bash
# After documentation PR is merged
git checkout main
git pull

# 1. Update docs changelog with date
# File: docs/CHANGELOG.md

## [2026-02-10] - Development Workflow Documentation  # ← Date, not version

### Added
- **GitVersion-Integration.md**: Automated versioning guide
  * Installation and configuration
  * Multi-platform versioning
  * CI/CD integration

- **Changelog-Management.md**: Changelog strategy guide
  * Hybrid changelog approach
  * Workflow documentation
  * Admin bypass guidelines

## [Unreleased]


# 2. Update central changelog
# File: CHANGELOG.md

## Documentation Updates - 2026-02-10

### Added
- Development workflow documentation
  * GitVersion integration guide
  * Changelog management strategy
  * Branch protection and admin bypass guidelines

See [Documentation CHANGELOG](./docs/CHANGELOG.md) for detailed changes.


# 3. Direct commit to main (admin bypass)
git add CHANGELOG.md docs/CHANGELOG.md
git commit -m "docs: finalize development workflow documentation changelog

- Update docs/CHANGELOG.md with date-based section (2026-02-10)
- Update central CHANGELOG.md with summary
- Finalize after PR #XX merge"

git push origin main
```

**Documentation rules:**
- ✅ Use date-based sections: `[YYYY-MM-DD]`
- ✅ Do NOT use version tags
- ✅ Group related documentation updates
- ✅ Update central CHANGELOG with summary + link
- ✅ Direct commit to main (admin bypass)

## Admin Bypass: When and How

### When Admin Bypass is Permitted

**ONLY for changelog finalization:**
- ✅ Updating `[Unreleased]` → date/version
- ✅ Creating new empty `[Unreleased]` section
- ✅ Updating central CHANGELOG.md with summary
- ✅ After successful PR merge
- ✅ No code changes included

**NEVER for:**
- ❌ Feature development
- ❌ Bug fixes
- ❌ Refactoring
- ❌ Code changes of any kind
- ❌ Documentation content updates
- ❌ Dependency updates

### Admin Bypass Workflow

```bash
# 1. Ensure you're on latest main
git checkout main
git pull

# 2. Verify no uncommitted changes
git status

# 3. Edit ONLY changelog files
# - Component CHANGELOG: [Unreleased] → [Version/Date]
# - Central CHANGELOG: Add summary

# 4. Commit with clear message
git add CHANGELOG.md {component}/CHANGELOG.md
git commit -m "chore(scope): finalize changelog

- Update component CHANGELOG (Unreleased → X.Y.Z)
- Update central CHANGELOG with summary
- Finalize after PR #XX merge"

# 5. Push directly to main
git push origin main

# 6. Create tag (for platform releases)
git tag -a {platform}-vX.Y.Z -m "Release message"
git push origin {platform}-vX.Y.Z
```

### Safety Checks Before Admin Bypass

**Before pushing directly to main, verify:**

1. **Only changelog files modified:**
```bash
git status
# Should ONLY show:
# modified: CHANGELOG.md
# modified: docs/CHANGELOG.md (or component CHANGELOG)
```

2. **No code changes:**
```bash
git diff --cached
# Review changes - should be ONLY changelog updates
```

3. **Proper commit message:**
```bash
# Message should start with "chore:" and mention finalization
```

4. **Feature PR already merged:**
```bash
# Verify main has the feature code
git log --oneline -10
```

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
git commit -m "chore(sparrow): finalize v0.1.0 changelog"
git push origin main
git tag -a sparrow-v0.1.0 -m "SPARROW v0.1.0"
git push origin sparrow-v0.1.0

# Release RAVEN (on same commit or different)
git commit -m "chore(raven): finalize v0.3.0 changelog"
git push origin main
git tag -a raven-v0.3.0 -m "RAVEN v0.3.0"
git push origin raven-v0.3.0

# Central changelog shows both
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
# 1. Create hotfix branch from tag
git checkout -b hotfix/sparrow-v0.1.1 sparrow-v0.1.0

# 2. Fix bug and update changelog
# File: src/Enclave.Echelon.SPARROW/CHANGELOG.md
## [Unreleased]

### Fixed
- Buffer overflow on passwords longer than 8 characters

git commit -m "fix(sparrow): resolve buffer overflow

+semver: patch"

# 3. Create PR and merge
git push origin hotfix/sparrow-v0.1.1
gh pr create --title "fix(sparrow): Resolve buffer overflow"
gh pr merge --squash

# 4. Finalize changelog (admin bypass on main)
git checkout main
git pull

# Update changelogs: [Unreleased] → [0.1.1]
git commit -m "chore(sparrow): finalize v0.1.1 hotfix changelog"
git push origin main

# 5. Tag hotfix
git tag -a sparrow-v0.1.1 -m "Hotfix: buffer overflow"
git push origin sparrow-v0.1.1
```

## Best Practices

### Do:
- ✅ Update component changelog in feature branch
- ✅ Use clear, descriptive entries
- ✅ Finalize changelogs via admin bypass AFTER PR merge
- ✅ Update both changelogs together (one commit)
- ✅ Link from central to component changelogs
- ✅ Use appropriate categories (Added, Changed, Fixed, etc.)
- ✅ Include dates in YYYY-MM-DD format
- ✅ Keep central changelog concise (summary + link)
- ✅ Add `[Unreleased]` comparison link
- ✅ Create tags AFTER changelog finalization

### Don't:
- ❌ Update central changelog in feature branches
- ❌ Finalize changelogs before PR merge
- ❌ Use admin bypass for code changes
- ❌ Duplicate content between central and component changelogs
- ❌ Skip changelog updates in feature branches
- ❌ Use vague entries like "bug fixes" or "improvements"
- ❌ Update changelogs after tagging (always before)
- ❌ Mix multiple platform releases in one changelog entry
- ❌ Forget to create new `[Unreleased]` section after release
- ❌ Bypass PR workflow for anything except changelog finalization

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
- [Branching Strategy] - Git workflow and branch management
- [Release Process] - Complete release workflow

[Keep a Changelog]: https://keepachangelog.com/
[Semantic Versioning]: https://semver.org/
[Conventional Commits]: https://www.conventionalcommits.org/
[Branching Strategy]: ./Branching-Strategy.md
[Release Process]: ./Release-Process.md
