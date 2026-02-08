# Branching Strategy

Git workflow and branch management strategy for the Enclave Terminal Breach project.

## Overview

This project uses a **GitHub Flow** variant optimized for solo development with evolutionary phases. The strategy emphasizes:

- Clean, linear history through Pull Requests
- Phase-based branch organization (SPARROW → RAVEN → GHOST → ECHELON)
- Semantic versioning per platform
- Protected main branch with PR-only merges

## Branch Structure

### Main Branch

**`main`**
- Always deployable and stable
- Protected: Requires Pull Request for all changes
- No direct commits allowed (including administrators)
- Source of truth for the project

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

### 5. Cleanup
```bash
# After merge, delete remote branch (GitHub does this automatically)
# Delete local branch
git checkout main
git pull
git branch -d feature/sparrow-stdin-input
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
- `chore` - Maintenance tasks

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

### Tagging

Create annotated tags for releases:
```bash
# Tag a release
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
- ✅ Do not allow bypassing settings
- ✅ Include administrators (strict mode)

## Best Practices

### Do:
- ✅ Create PR for every change to main
- ✅ Write descriptive commit messages
- ✅ Keep feature branches short-lived (1-3 days)
- ✅ Rebase on main before creating PR (if needed)
- ✅ Delete merged branches
- ✅ Tag releases consistently

### Don't:
- ❌ Commit directly to main
- ❌ Use generic commit messages ("update", "fix")
- ❌ Let branches drift far from main
- ❌ Force push to main
- ❌ Skip PR review (even if you're alone)

## Emergency Hotfixes

For critical fixes to released versions:
```bash
# Create hotfix branch from tag
git checkout -b hotfix/sparrow-v0.1.1 sparrow-v0.1.0

# Make fix
git commit -m "fix(sparrow): Resolve stdin buffer overflow"

# Tag new patch version
git tag -a sparrow-v0.1.1 -m "Hotfix: stdin buffer overflow"

# Merge to main via PR
git checkout main
git merge hotfix/sparrow-v0.1.1
git push origin main sparrow-v0.1.1
```

## Migration from Azure DevOps

**Historical note:** This project was migrated from Azure DevOps to GitHub to showcase software evolution. The git history starts fresh from the GitHub migration point.

**Original repository:** Azure DevOps (private, archived)  
**Current repository:** GitHub (public, active)

## References

- [GitHub Flow] - GitHub's branching model
- [Conventional Commits] - Commit message specification
- [Semantic Versioning] - Version numbering scheme

[GitHub Flow]: https://docs.github.com/en/get-started/quickstart/github-flow
[Conventional Commits]: https://www.conventionalcommits.org/
[Semantic Versioning]: https://semver.org/
[Commit-Conventions.md]: ./Commit-Conventions.md