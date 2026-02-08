# Development Documentation

**English** | [Magyar]

Process, workflow, and contribution guidelines.

## Contents

- [Branching Strategy] - Git workflow and branch management
- [Commit Conventions] - Commit message standards and examples
- [Release Process] - Versioning, releases, and changelog management
- [GitVersion Integration] - Automated semantic versioning
- [Changelog Management] - Changelog maintenance strategy

## Quick Reference

### Start New Work

```bash
git checkout main && git pull
git checkout -b feature/your-feature
```

### Commit Your Changes

```bash
git add .
git commit -m "feat(scope): your change description

+semver: minor"
```

See [Commit Conventions] for detailed format guidelines.

### Update Changelog

```bash
# Edit relevant component changelog
# Example: src/Enclave.Echelon.SPARROW/CHANGELOG.md

## [Unreleased]

### Added
- Your feature description
```

See [Changelog Management] for complete guidelines.

### Check Current Version

```bash
dotnet-gitversion /showvariable SemVer
```

See [GitVersion Integration] for automated versioning.

### Create Pull Request

```bash
git push -u origin feature/your-feature
gh pr create --title "feat(scope): Your feature"
```

### After Merge

```bash
git checkout main && git pull
git branch -d feature/your-feature
```

### Create Release

```bash
# Verify next version
dotnet-gitversion /showvariable MajorMinorPatch

# Update changelogs (both component and central)
# Update CHANGELOG.md + component CHANGELOG.md

# Commit and tag
git add CHANGELOG.md src/Enclave.Echelon.{PLATFORM}/CHANGELOG.md
git commit -m "chore(platform): prepare vX.Y.Z release"
git tag -a phase-vX.Y.Z -m "Phase vX.Y.Z - Release title"
git push origin main phase-vX.Y.Z
```

See [Release Process] for complete release workflow.

## Automated Versioning

This project uses [GitVersion] to automatically calculate version numbers based on git history.

**Version is determined by:**
- Git tags (e.g., `sparrow-v0.1.0`)
- Branch name (e.g., `feature/*` gets `-alpha` suffix)
- Commit messages (e.g., `+semver: minor` bumps version)

**Quick version check:**
```bash
dotnet-gitversion
```

**Branch-specific versions:**
- `main` → `0.1.1` (ready for release)
- `feature/xyz` → `0.2.0-alpha.1+3`
- `phase/sparrow` → `1.0.0-beta.1+5`

See [GitVersion Integration] for comprehensive documentation.

## Changelog Strategy

This project uses a **hybrid changelog approach**:

- **Central CHANGELOG.md** (repository root): Release summaries and timeline
- **Component changelogs** (per platform/docs): Detailed change tracking

**Update changelogs during:**
- Feature development: Component changelog only (in feature branch)
- Release preparation: Both central and component (in release commit)

**Quick reference:**

| Stage | Update | Location |
|-------|--------|----------|
| Feature branch | Component changelog | `src/{Platform}/CHANGELOG.md` |
| Documentation | Documentation changelog | `docs/CHANGELOG.md` |
| Release prep | Central + Component | `CHANGELOG.md` + component |

See [Changelog Management] for complete strategy and workflows.

## Documentation Standards

All commits follow [Conventional Commits] specification.  
All releases follow [Semantic Versioning] scheme.  
All changelogs follow [Keep a Changelog] format.  
All versions automated via [GitVersion].

See individual documents for comprehensive guidelines.

[Branching Strategy]: ./Branching-Strategy.md
[Commit Conventions]: ./Commit-Conventions.md
[Release Process]: ./Release-Process.md
[GitVersion Integration]: ./GitVersion-Integration.md
[Changelog Management]: ./Changelog-Management.md
[Conventional Commits]: https://www.conventionalcommits.org/
[Semantic Versioning]: https://semver.org/
[Keep a Changelog]: https://keepachangelog.com/
[GitVersion]: https://gitversion.net/
[Magyar]: ./README.hu.md
