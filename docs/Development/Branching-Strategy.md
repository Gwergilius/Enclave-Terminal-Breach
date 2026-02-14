# Branching Strategy

**English** | [Magyar]

Git workflow and branch management strategy for the Enclave Terminal Breach project.

## Overview

This project uses a **GitHub Flow** variant optimized for solo development with evolutionary phases. The strategy emphasizes:

- Clean, linear history through Pull Requests
- Phase-based branch organization (SPARROW → RAVEN → GHOST → ECHELON)
- Semantic versioning per platform
- Protected main branch with limited admin bypass for administrative tasks
- PR-only workflow for all feature development

## Branch Structure

### Main Branch

**`main`**
- Always deployable and stable
- Protected: Requires Pull Request for all feature development
- Admin bypass permitted ONLY for changelog finalization
- Source of truth for the project
- **Releases live on main:** Each release (e.g. SPARROW v1.1.0) is merged to `main`; Git tags point to commits on `main`. All release versions are available from the main branch. Feature branches are temporary and are removed after their PR is merged.

**Branch Protection Settings:**
```yaml
☑ Require a pull request before merging
☑ Require approvals: 0 (solo development)
☐ Do not allow bypassing the above settings  # Admin bypass enabled
☑ Include administrators  # Admin follows same rules (except changelog finalization)
```

### Feature Branches

Feature branches are organized by **type** and **scope**:

```
<type>/<scope>[-<description>]
```

**Types:**
- `docs/` - Documentation updates
- `feature/` - New features and capabilities
- `refactor/` - Code restructuring
- `fix/` - Bug fixes
- `test/` - Test additions or improvements
- `chore/` - Build, dependencies, tooling

**Scopes (Project Phases):**
- `sparrow` - DOS proof of concept
- `raven` - Console application
- `ghost` - Blazor web application
- `echelon` - MAUI mobile application
- `core` - Shared core logic
- `arch` - Architecture changes
- `ci` - CI/CD pipeline

**Examples:**
```
docs/migration-from-console-repo
feature/sparrow-password-solver
feature/raven-boot-sequence
feature/ghost-blazor-ui
feature/echelon-maui-shell
refactor/core-dependency-injection
test/algorithm-convergence
chore/ci-github-actions
```

### Long-Running Phase Branches (Optional)

For major phases, consider long-running branches:

```
phase/sparrow    → merge to main when SPARROW complete
phase/raven      → merge to main when RAVEN complete
phase/ghost      → merge to main when GHOST complete
phase/echelon    → merge to main when ECHELON complete
```

**Pros:**
- Clear separation between phases
- Can cherry-pick fixes back to earlier phases
- Parallel development on different phases

**Cons:**
- More complex merge management
- Risk of divergence

**Recommendation:** Use only if working on multiple phases simultaneously.

## Workflow

### 1. Starting New Work

```bash
# Ensure main is up to date
git checkout main
git pull origin main

# Create feature branch
git checkout -b feature/sparrow-stdin-input

# Work on feature (multiple commits OK)
git add .
git commit -m "feat(sparrow): Add stdin password input"
```

### 2. Creating Pull Request

```bash
# Push to remote
git push -u origin feature/sparrow-stdin-input

# Create PR via GitHub UI or CLI
gh pr create \
  --title "feat(sparrow): Add stdin password input" \
  --body "Implements DOS-style stdin input for SPARROW prototype"
```

### 3. Pull Request Review

Even as a solo developer, review your own PR:
- ✅ Read through all changes
- ✅ Verify tests pass (when CI is setup)
- ✅ Check documentation is updated
- ✅ Ensure commit messages follow conventions
- ✅ Verify changelog entries are in [Unreleased] section

### 4. Merging

**Preferred: Squash and Merge**
- Consolidates feature branch commits into one
- Keeps main history clean
- Good for features with many WIP commits

**Alternative: Merge Commit**
- Preserves full feature branch history
- Use for significant milestones (e.g., completing a phase)

```bash
# After PR approval
# Use GitHub UI "Squash and merge" or "Merge commit"
```

### 5. Changelog Finalization (Admin Bypass)

**IMPORTANT:** This is the ONLY scenario where direct commits to main are permitted.

After PR merge, finalize changelogs using admin bypass:

```bash
# 1. Checkout main after PR merge
git checkout main
git pull

# 2. Update changelogs
# - Component CHANGELOG: [Unreleased] → [Version/Date]
# - Central CHANGELOG: Add summary

# 3. Commit directly to main
git add CHANGELOG.md {component}/CHANGELOG.md
git commit -m "chore(scope): finalize changelog

- Update component CHANGELOG (Unreleased → X.Y.Z)
- Update central CHANGELOG with summary
- Finalize after PR #XX merge"

# 4. Push directly to main (uses admin bypass)
git push origin main

# 5. Create tag (for releases)
git tag -a {platform}-vX.Y.Z -m "Release message"
git push origin {platform}-vX.Y.Z
```

**Rules for admin bypass:**
- ✅ ONLY for changelog finalization
- ✅ ONLY after successful PR merge
- ✅ ONLY changelog files modified
- ✅ Commit message starts with "chore:"
- ❌ NEVER for feature code
- ❌ NEVER for bug fixes
- ❌ NEVER for documentation content
- ❌ NEVER for any code changes

See [Changelog Management] for detailed changelog workflow.

### 6. Cleanup

```bash
# After changelog finalization and tagging
git checkout main
git pull

# Delete local feature branch
git branch -d feature/sparrow-stdin-input

# Remote branch already deleted by GitHub after PR merge
```

## Admin Bypass: Strict Usage Guidelines

### When Admin Bypass is Permitted

**ONLY for changelog finalization:**
- Moving `[Unreleased]` → `[Version]` or `[Date]`
- Creating new empty `[Unreleased]` section
- Updating central CHANGELOG.md with release summary
- After successful PR merge
- No code, documentation content, or configuration changes

**Example permitted commit:**
```bash
git commit -m "chore(sparrow): finalize v0.1.0 changelog

- Update SPARROW CHANGELOG.md (Unreleased → 0.1.0)
- Update central CHANGELOG.md with release summary"
```

### When Admin Bypass is FORBIDDEN

**NEVER bypass PR for:**
- ❌ Feature development (use `feature/*` branch + PR)
- ❌ Bug fixes (use `fix/*` branch + PR)
- ❌ Refactoring (use `refactor/*` branch + PR)
- ❌ Documentation content (use `docs/*` branch + PR)
- ❌ Tests (use `test/*` branch + PR)
- ❌ Dependencies (use `chore/*` branch + PR)
- ❌ Configuration changes (use `chore/*` branch + PR)
- ❌ ANY code modification
- ❌ Emergency hotfixes (still use `hotfix/*` branch + PR)

**Example FORBIDDEN direct commit:**
```bash
# WRONG! Never bypass PR for code changes
git commit -m "fix: quick typo fix in README"
git push origin main  # ← VIOLATION!

# CORRECT: Use PR even for tiny changes
git checkout -b fix/readme-typo
git commit -m "fix: correct typo in README"
git push origin fix/readme-typo
gh pr create --title "fix: Correct typo in README"
```

### Safety Verification Before Admin Bypass

Before pushing directly to main, verify:

```bash
# 1. Only changelog files modified
git status
# Should ONLY show:
# modified: CHANGELOG.md
# modified: docs/CHANGELOG.md (or component CHANGELOG)

# 2. No code changes
git diff --cached
# Review: should be ONLY changelog date/version updates

# 3. Proper commit message format
# Message must start with "chore:" and mention finalization

# 4. Feature PR already merged
git log --oneline -5
# Verify PR merge commit is present
```

## Commit Conventions

Follow [Conventional Commits] specification:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation only
- `style` - Formatting, missing semicolons
- `refactor` - Code restructuring
- `test` - Adding tests
- `chore` - Maintenance tasks (includes changelog finalization)

**Scopes:**
- Phase names: `sparrow`, `raven`, `ghost`, `echelon`
- Component areas: `core`, `ui`, `tests`, `arch`, `lore`, `impl`

**Examples:**
```
feat(sparrow): Implement password elimination algorithm
docs(lore): Add ECHELON Project backstory
refactor(core): Extract solver interface
test(algorithm): Add convergence test cases
chore(ci): Configure GitHub Actions workflow
chore(sparrow): finalize v0.1.0 changelog  ← Admin bypass commit
```

See [Commit-Conventions.md] for detailed guidelines.

## Release Strategy

### Versioning

Each phase uses **Semantic Versioning** (`MAJOR.MINOR.PATCH`):

```
sparrow-v0.1.0    → First DOS prototype
raven-v0.4.0      → Console with PHOSPHOR 1.0
ghost-v1.2.4      → Blazor PWA standard build
echelon-v2.1.7    → Final MAUI deployment
```

**Version components:**
- `MAJOR` - Breaking changes, new phase
- `MINOR` - New features, backward compatible
- `PATCH` - Bug fixes, small improvements

### Release Workflow

Complete release workflow with admin bypass:

```bash
# 1. Feature development (PR workflow)
git checkout -b feature/sparrow-stdin-input
# ... develop, commit, update component CHANGELOG [Unreleased]
git push origin feature/sparrow-stdin-input
gh pr create --title "feat(sparrow): Add stdin input"
gh pr merge --squash

# 2. Changelog finalization (admin bypass)
git checkout main
git pull
# Edit changelogs: [Unreleased] → [0.1.0], update central CHANGELOG
git commit -m "chore(sparrow): finalize v0.1.0 changelog"
git push origin main  # ← Uses admin bypass

# 3. Create tag
git tag -a sparrow-v0.1.0 -m "SPARROW v0.1.0 - DOS Proof of Concept"
git push origin sparrow-v0.1.0

# 4. Create GitHub Release
gh release create sparrow-v0.1.0 \
  --title "SPARROW v0.1.0 - DOS Proof of Concept" \
  --notes-file release-notes.md
```

### Tagging

Create annotated tags for releases:

```bash
# Tag a release (after changelog finalization)
git tag -a sparrow-v0.1.0 -m "SPARROW v0.1.0 - DOS proof of concept

First working implementation of password solver.
- Stdin/stdout interaction
- Basic elimination algorithm
- 28% success rate"

# Push tags
git push origin sparrow-v0.1.0
```

### GitHub Releases

Create GitHub Release for each tagged version:

1. Go to Releases → Draft a new release
2. Choose tag: `sparrow-v0.1.0`
3. Title: `SPARROW v0.1.0 - DOS Proof of Concept`
4. Description: Changelog and notable features
5. Attach binaries (optional)

## Branch Protection Rules

**Main branch protection (configured):**
- ✅ Require a pull request before merging
- ✅ Require approvals: 0 (solo development)
- ☐ Do not allow bypassing settings (admin bypass enabled)
- ✅ Include administrators (must use PR except for changelog finalization)

**Protection bypassed for:**
- Changelog finalization commits only

**Protection enforced for:**
- All feature development
- All bug fixes
- All refactoring
- All documentation content
- All configuration changes
- All dependency updates

## Best Practices

### Do:
- ✅ Create PR for every code or content change
- ✅ Write descriptive commit messages
- ✅ Keep feature branches short-lived (1-3 days)
- ✅ Rebase on main before creating PR (if needed)
- ✅ Delete merged branches
- ✅ Tag releases consistently
- ✅ Update changelogs in feature branches ([Unreleased])
- ✅ Finalize changelogs via admin bypass AFTER PR merge
- ✅ Verify only changelog files modified before admin bypass

### Don't:
- ❌ Commit directly to main (except changelog finalization)
- ❌ Use generic commit messages ("update", "fix")
- ❌ Let branches drift far from main
- ❌ Force push to main
- ❌ Skip PR review (even if you're alone)
- ❌ Bypass PR for code changes
- ❌ Finalize changelogs before PR merge
- ❌ Mix code changes with changelog finalization

## Emergency Hotfixes

For critical fixes to released versions:

```bash
# 1. Create hotfix branch from tag
git checkout -b hotfix/sparrow-v0.1.1 sparrow-v0.1.0

# 2. Make fix and update changelog [Unreleased]
git commit -m "fix(sparrow): Resolve stdin buffer overflow

+semver: patch"

# 3. Create PR and merge
git push origin hotfix/sparrow-v0.1.1
gh pr create --title "fix(sparrow): Resolve buffer overflow"
gh pr merge --squash

# 4. Finalize changelog (admin bypass)
git checkout main
git pull
# Update changelogs: [Unreleased] → [0.1.1]
git commit -m "chore(sparrow): finalize v0.1.1 hotfix changelog"
git push origin main  # ← Admin bypass

# 5. Tag hotfix
git tag -a sparrow-v0.1.1 -m "Hotfix: stdin buffer overflow"
git push origin main sparrow-v0.1.1
```

**Note:** Even hotfixes follow PR workflow for code changes; admin bypass only for changelog finalization.

## Migration from Azure DevOps

**Historical note:** This project was migrated from Azure DevOps to GitHub to showcase software evolution. The git history starts fresh from the GitHub migration point.

**Original repository:** Azure DevOps (private, archived)  
**Current repository:** GitHub (public, active)

## References

- [GitHub Flow] - GitHub's branching model
- [Conventional Commits] - Commit message specification
- [Semantic Versioning] - Version numbering scheme
- [Changelog Management] - Changelog maintenance strategy

[GitHub Flow]: https://docs.github.com/en/get-started/quickstart/github-flow
[Conventional Commits]: https://www.conventionalcommits.org/
[Semantic Versioning]: https://semver.org/
[Commit-Conventions.md]: ./Commit-Conventions.md
[Changelog Management]: ./Changelog-Management.md
[Magyar]: ./Branching-Strategy.hu.md
