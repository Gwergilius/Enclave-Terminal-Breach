# Release Process

**English** | [Magyar]

Version management and release procedures for the Enclave Terminal Breach project.

## Overview

This project uses **Semantic Versioning** with platform-specific release tracks. Each phase (SPARROW, RAVEN, GHOST, ECHELON) maintains independent version numbers reflecting the in-universe development timeline.

## Versioning Scheme

### Semantic Versioning

All versions follow `MAJOR.MINOR.PATCH`:

- **MAJOR** - Breaking changes, incompatible API changes, new phase
- **MINOR** - New features, backward-compatible additions
- **PATCH** - Bug fixes, backward-compatible fixes

### Phase Versioning

Each phase has its own version track:
```
sparrow-v0.1.0     DOS proof of concept (Feb-Mar 2076)
sparrow-v0.2.0     Enhanced algorithm iteration

raven-v0.3.1       First NX-12 direct implementation (Apr-Aug 2076)
raven-v0.4.0       PHOSPHOR 1.0 abstraction layer (Sep 2076)

ghost-v1.0.0       Pip-Boy 2000 + SIGNET deployment (Sep 2076)
ghost-v1.2.0       PHOSPHOR 2.0 with 16-color support (Oct 2076)
ghost-v1.2.4       Neural pattern recognition (Nov 2076)

echelon-v2.0.0     Pip-Boy 3000 Mark III/IV (Feb 2077)
echelon-v2.1.0     Dictionary attack optimization (May 2077)
echelon-v2.1.7     Final pre-war version (Oct 2077)
```

### Lore and version alignment

**Decision:** Version numbers are not aligned to the Lore document. The project uses **real** project and component versions as the source of truth. The document [Project-History.md](../Lore/Project-History.md) is updated to use these actual versions when releases are made.

- **Source of truth:** Git tags, package versions, CHANGELOG.md, and project files (e.g. `.csproj`).
- **Project-History.md:** Reflects the real SPARROW, RAVEN, GHOST, ECHELON, and PHOSPHOR versions as they are released. We do not invent or adjust version numbers to match the lore narrative; we update the lore document to match the real versions.

## Version Numbering Rules

### MAJOR Version

Increment when:
- Moving to a new phase (SPARROW → RAVEN → GHOST → ECHELON)
- Breaking API changes in Core or Shared libraries
- Fundamental architecture changes
- Removing deprecated features

**Example:**
```
raven-v0.4.0 → ghost-v1.0.0
(New platform, new capabilities)
```

### MINOR Version

Increment when:
- Adding new features (backward-compatible)
- Significant improvements to existing features
- New optional dependencies
- Deprecating features (without removal)

**Example:**
```
ghost-v1.0.0 → ghost-v1.2.0
(Added PHOSPHOR 2.0 color palettes)
```

### PATCH Version

Increment when:
- Bug fixes
- Performance improvements
- Documentation updates
- Minor refactoring
- Dependency updates (no breaking changes)

**Example:**
```
ghost-v1.2.0 → ghost-v1.2.4
(Neural pattern recognition improvements)
```

## Release Workflow

### 1. Pre-Release Checklist

Before creating a release, verify:

- [ ] All tests pass (`dotnet test`)
- [ ] Code coverage meets threshold (>80%)
- [ ] No `TODO` or `FIXME` comments in release code
- [ ] Documentation updated (README, CHANGELOG)
- [ ] Version numbers updated in project files
- [ ] Build succeeds for all platforms
- [ ] Manual testing completed
- [ ] Breaking changes documented (if MAJOR version)

### 2. Update Version Numbers

**Update project files:**
```xml
<!-- Enclave.Echelon.SPARROW.csproj -->
<PropertyGroup>
  <Version>0.1.0</Version>
  <AssemblyVersion>0.1.0.0</AssemblyVersion>
  <FileVersion>0.1.0.0</FileVersion>
</PropertyGroup>
```

**Update README badges (if applicable):**
```markdown
![SPARROW Version](https://img.shields.io/badge/SPARROW-v0.1.0-green)
```

### 3. Update CHANGELOG

Maintain a `CHANGELOG.md` following [Keep a Changelog] format:
```markdown
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - 2026-02-15

### Added
- Initial SPARROW implementation
- DOS stdin/stdout password input
- Basic elimination algorithm
- Information score calculation

### Changed
- N/A (initial release)

### Deprecated
- N/A

### Removed
- N/A

### Fixed
- N/A

### Security
- N/A
```

### 4. Create Release Commit
```bash
git add .
git commit -m "chore(sparrow): prepare v0.1.0 release

- Update version to 0.1.0
- Update CHANGELOG.md
- Update README badges"
```

### 5. Create Git Tag

**Lightweight tag (simple):**
```bash
git tag sparrow-v0.1.0
```

**Annotated tag (recommended):**
```bash
git tag -a sparrow-v0.1.0 -m "SPARROW v0.1.0 - DOS Proof of Concept

First working implementation of the password elimination solver.

Features:
- Stdin/stdout interaction (DOS 3.11 compatible)
- Information-theoretic algorithm implementation
- Basic password elimination logic
- Success rate: 28% on average difficulty

Known Limitations:
- Manual input required
- No screen positioning
- Limited to 4-8 character passwords

Lore Context: February 2076 - Initial prototype phase"
```

### 6. Push Tag to GitHub
```bash
# Push tag
git push origin sparrow-v0.1.0

# Or push all tags
git push --tags
```

### 7. Create GitHub Release

**Via GitHub Web UI:**

1. Go to **Releases** → **Draft a new release**
2. **Choose tag:** `sparrow-v0.1.0`
3. **Release title:** `SPARROW v0.1.0 - DOS Proof of Concept`
4. **Description:** Copy from CHANGELOG + add highlights
5. **Attach binaries** (optional): see [Attach artifacts to a GitHub Release](#attach-artifacts-to-a-github-release) below.
6. **Mark as pre-release** (if alpha/beta)
7. **Publish release**

**Via GitHub CLI:**
```bash
gh release create sparrow-v1.1.0 \
  --title "SPARROW v1.1.0 - DOS Proof of Concept" \
  --notes-file release-notes.md \
  ./dist/Enclave-SPARROW-v1.1.0-win-x64.zip
```

### Attach artifacts to a GitHub Release

To offer downloadable executables or installers with a release:

**1. Build the app** (example: SPARROW):
```bash
# From repo root
VERSION=1.1.0
dotnet publish src/Enclave.Sparrow/Enclave.Sparrow.csproj \
  -c Release -r win-x64 --self-contained \
  -o dist/sparrow-win-x64
# Optional: Linux
dotnet publish src/Enclave.Sparrow/Enclave.Sparrow.csproj \
  -c Release -r linux-x64 --self-contained \
  -o dist/sparrow-linux-x64
```

**2. Package into archives** (use the naming convention below):
```bash
# Windows zip (from repo root)
cd dist && zip -r Enclave-SPARROW-v1.1.0-win-x64.zip sparrow-win-x64 && cd ..
# Or on Windows (PowerShell):
Compress-Archive -Path dist/sparrow-win-x64/* -DestinationPath dist/Enclave-SPARROW-v1.1.0-win-x64.zip
```

**3. Attach to the release**

- **GitHub Web UI:** Open the release (e.g. **Releases** → **SPARROW v1.1.0**) → **Edit** → scroll to *Attach binaries by dropping them here or selecting them* → drag the `.zip` (or `.tar.gz`) file → **Save**.
- **GitHub CLI:** Create the release with files, or add files to an existing release:
```bash
gh release upload sparrow-v1.1.0 dist/Enclave-SPARROW-v1.1.0-win-x64.zip
# Or create release and upload in one go:
gh release create sparrow-v1.1.0 --title "SPARROW v1.1.0" --notes "..." dist/Enclave-SPARROW-v1.1.0-win-x64.zip
```

Users can then download the archive from the release page and run the executable (e.g. `Enclave.Sparrow.exe` inside the zip on Windows). For an installer (e.g. MSI/Setup), you would add a separate build step (e.g. MSIX/installer tooling) and attach the resulting file the same way.

### 8. Announcement

**Update README.md:**
```markdown
## 🚀 Latest Releases

- **SPARROW v0.1.0** (2026-02-15) - DOS proof of concept
```

**Announce on social/community:**
- GitHub Discussions
- Project Discord/Slack (if applicable)
- Dev.to / Medium blog post (optional)

## Release Types

### Stable Release

Production-ready versions:
```
sparrow-v0.1.0
raven-v0.4.0
ghost-v1.2.4
echelon-v2.1.7
```

### Pre-Release

Alpha/Beta versions for testing:
```
raven-v0.4.0-alpha.1
ghost-v1.0.0-beta.2
echelon-v2.0.0-rc.1
```

**Pre-release suffixes:**
- `alpha.N` - Early testing, unstable
- `beta.N` - Feature complete, testing
- `rc.N` - Release candidate, final testing

**Example:**
```bash
git tag -a ghost-v1.0.0-beta.1 -m "GHOST v1.0.0 Beta 1"
```

Mark as pre-release in GitHub:
```bash
gh release create ghost-v1.0.0-beta.1 --prerelease
```

### Hotfix Release

Emergency patches to stable releases:
```
sparrow-v0.1.0 → sparrow-v0.1.1 (hotfix)
```

**Workflow:**
```bash
# Create hotfix branch from tag
git checkout -b hotfix/sparrow-v0.1.1 sparrow-v0.1.0

# Make fix
git commit -m "fix(sparrow): resolve buffer overflow in stdin reader"

# Tag hotfix
git tag -a sparrow-v0.1.1 -m "Hotfix: stdin buffer overflow"

# Merge to main
git checkout main
git merge hotfix/sparrow-v0.1.1
git push origin main sparrow-v0.1.1
```

## Changelog Generation

### Manual (Recommended for this project)

Maintain `CHANGELOG.md` manually following [Keep a Changelog]:
```markdown
## [0.2.0] - 2026-03-01

### Added
- Enhanced algorithm with tie-breaker logic
- Worst-case bucket size minimization

### Changed
- Improved information score calculation performance

### Fixed
- Buffer overflow on long password inputs
```

### Automated (Optional)

Use [standard-version] or [release-please] for automation:
```bash
npm install -g standard-version

# Generate changelog and bump version
standard-version

# With custom release type
standard-version --release-as minor
```

## Build Artifacts

### Packaging

Create platform-specific builds for release:
```bash
# SPARROW (Console app - .NET)
dotnet publish src/Enclave.Sparrow/Enclave.Sparrow.csproj \
  -c Release \
  -r win-x64 \
  --self-contained \
  -o dist/sparrow-win-x64

# Optional: single-file executable (smaller download)
dotnet publish src/Enclave.Sparrow/Enclave.Sparrow.csproj \
  -c Release \
  -r win-x64 \
  --self-contained \
  -p:PublishSingleFile=true \
  -o dist/sparrow-win-x64

# RAVEN (Console - Windows)
dotnet publish src/Enclave.Echelon.RAVEN \
  -c Release \
  -r win-x64 \
  --self-contained \
  -o dist/raven-win-x64

# GHOST (Web - Blazor)
dotnet publish src/Enclave.Echelon.GHOST \
  -c Release \
  -o dist/ghost-web

# ECHELON (Mobile - Android)
dotnet publish src/Enclave.Echelon.ECHELON \
  -c Release \
  -f net10.0-android \
  -o dist/echelon-android
```

### Asset Naming

Follow consistent naming:
```
Enclave-{PHASE}-v{VERSION}-{PLATFORM}.{EXT}

Examples:
Enclave-SPARROW-v1.1.0-win-x64.zip
Enclave-SPARROW-v1.1.0-linux-x64.tar.gz
Enclave-RAVEN-v0.4.0-win-x64.zip
Enclave-GHOST-v1.2.4-web.zip
Enclave-ECHELON-v2.1.7-android.apk
```

## Version History Tracking

### Git Tags

All releases are tagged:
```bash
# List all tags
git tag

# Filter by phase
git tag -l "sparrow-*"
git tag -l "raven-*"
git tag -l "ghost-*"
git tag -l "echelon-*"
```

### GitHub Releases

Browse releases: https://github.com/Gwergilius/Enclave-Terminal-Breach/releases

### CHANGELOG.md

Comprehensive changelog at repository root.

## Deprecation Policy

### Marking Features as Deprecated

When deprecating features:

1. Add `[Obsolete]` attribute (C#)
2. Update documentation
3. Log warning at runtime
4. Note in CHANGELOG under `### Deprecated`
5. Plan removal for next MAJOR version

**Example:**
```csharp
[Obsolete("Use ISolver.GetBestGuesses() instead. Will be removed in v3.0.0")]
public Password GetBestGuess(IEnumerable<Password> candidates)
{
    // Old implementation
}
```

**CHANGELOG:**
```markdown
### Deprecated
- ISolver.GetBestGuess() - Use GetBestGuesses() instead. Removal planned for v3.0.0
```

## Support Policy

### Active Versions

Only the **latest version of each phase** receives updates:

- SPARROW: Latest v0.x.x
- RAVEN: Latest v0.x.x
- GHOST: Latest v1.x.x
- ECHELON: Latest v2.x.x

### Security Patches

Critical security fixes backported to:
- Latest MAJOR version of each phase
- Previous MAJOR version (for 6 months after new MAJOR release)

## References

- [Semantic Versioning] - Version numbering specification
- [Keep a Changelog] - Changelog format standard
- [Conventional Commits] - Commit message standard
- [GitHub Releases] - GitHub release documentation

[Semantic Versioning]: https://semver.org/
[Keep a Changelog]: https://keepachangelog.com/
[Conventional Commits]: https://www.conventionalcommits.org/
[GitHub Releases]: https://docs.github.com/en/repositories/releasing-projects-on-github
[standard-version]: https://github.com/conventional-changelog/standard-version
[release-please]: https://github.com/googleapis/release-please
[Magyar]: ./Release-Process.hu.md