# Commit Conventions

**English** | [Magyar]

Commit message standards for the Enclave Terminal Breach project.

## Overview

This project follows the [Conventional Commits] specification to maintain a clean, parseable git history that enables:

- Automated changelog generation
- Semantic versioning automation
- Clear communication of intent
- Easy filtering and searching of commits
- Consistent commit style across all phases

## Commit Message Format
```
<type>(<scope>): <subject>

<body>

<footer>
```

### Components

**Type** (required)
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation only changes
- `style` - Code style changes (formatting, missing semicolons, etc.)
- `refactor` - Code restructuring (no functionality change)
- `perf` - Performance improvements
- `test` - Adding or updating tests
- `build` - Build system or external dependencies
- `ci` - CI/CD configuration changes
- `chore` - Maintenance tasks (dependencies, tooling)
- `revert` - Reverting previous commit

**Scope** (optional but recommended)
- Phase names: `sparrow`, `raven`, `ghost`, `echelon`
- Component areas: `core`, `shared`, `ui`, `tests`, `algorithm`
- Documentation areas: `lore`, `arch`, `impl`, `dev`
- Infrastructure: `ci`, `build`, `deps`

**Subject** (required)
- Use imperative mood: "add" not "added" or "adds"
- Don't capitalize first letter
- No period at the end
- Maximum 50 characters
- Describe WHAT changed, not WHY (use body for WHY)

**Body** (optional)
- Wrap at 72 characters per line
- Explain WHAT and WHY, not HOW
- Separate from subject with blank line
- Use bullet points for multiple items

**Footer** (optional)
- Breaking changes: `BREAKING CHANGE: description`
- Issue references: `Fixes #123`, `Closes #456`
- Co-authors: `Co-authored-by: Name <email>`

## Type Descriptions

### feat - New Feature

New functionality added to the codebase.
```
feat(sparrow): implement password elimination algorithm

Add core solver logic with entropy-based guess selection.
Implements information-theoretic approach from Algorithm.md.

- Calculate information score for each candidate
- Select guess maximizing distinct match-count values
- Eliminate inconsistent candidates after each guess
```

### fix - Bug Fix

Correction of existing functionality.
```
fix(raven): resolve cursor positioning on boot screen

Cursor was positioned incorrectly on narrow terminals.
Now calculates position based on actual terminal width.

Fixes #42
```

### docs - Documentation

Documentation changes only (no code changes).
```
docs(lore): add ECHELON Project backstory

Complete pre-war development timeline (2075-2077).
Establishes context for SPARROW → RAVEN → GHOST → ECHELON evolution.
```

### refactor - Code Restructuring

Code changes that neither fix bugs nor add features.
```
refactor(core): extract solver interface

Move solver logic into ISolver interface.
Enables dependency injection and easier testing.
No behavior changes.
```

### test - Testing

Adding or updating tests.
```
test(algorithm): add convergence test cases

Verify algorithm converges in finite steps for all inputs.
Test cases cover 5, 10, and 15 letter word sets.
```

### chore - Maintenance

Build, tooling, dependencies.
```
chore(deps): update xUnit to 2.6.0

Update test framework dependency.
No API changes affecting our code.
```

## Scope Guidelines

### Phase Scopes

Use phase names when changes are specific to a platform:

- `sparrow` - DOS proof of concept code
- `raven` - Console application code
- `ghost` - Blazor web application code
- `echelon` - MAUI mobile application code

**Examples:**
```
feat(sparrow): add stdin password input
feat(raven): implement boot sequence animation
feat(ghost): add Blazor PWA shell
feat(echelon): integrate Pip-Boy UI theme
```

### Component Scopes

Use component names for shared code:

- `core` - Core business logic (Enclave.Core)
- `shared` - Shared ViewModels and state (Enclave.Shared)
- `algorithm` - Password solver algorithm
- `ui` - UI components or styling
- `tests` - Test infrastructure
- `build` - Build configuration

**Examples:**
```
feat(core): add password validation service
refactor(shared): extract GameSession base class
test(algorithm): add entropy calculation tests
```

### Documentation Scopes

Use documentation area names:

- `lore` - In-universe backstory
- `arch` - Architecture documentation
- `impl` - Implementation plans
- `dev` - Development process docs

**Examples:**
```
docs(lore): add RobCo UOS documentation
docs(arch): document PHOSPHOR abstraction layer
docs(impl): add GHOST implementation plan
docs(dev): add branching strategy
```

### Infrastructure Scopes

- `ci` - GitHub Actions, CI/CD
- `deps` - Dependencies
- `build` - Build system changes

**Examples:**
```
ci: add build and test workflow
chore(deps): update .NET to 10.0
build: configure solution-wide GitVersion
```

## Breaking Changes

Breaking changes MUST be indicated in the footer:
```
feat(core)!: change ISolver interface signature

BREAKING CHANGE: ISolver.GetBestGuess() now returns IResult<Password>
instead of Password. All implementations must be updated.

Migration: Wrap return values in Result.Ok() or Result.Fail().
```

Alternative syntax (using `!` after scope):
```
feat(core)!: change ISolver interface signature

ISolver.GetBestGuess() now returns IResult<Password>.
```

## Multi-Commit Features

For features requiring multiple commits, maintain consistency:
```
feat(ghost): scaffold Blazor project structure
feat(ghost): add TerminalShell component
feat(ghost): implement status bar state
feat(ghost): add toast notification service
```

Then create PR titled:
```
feat(ghost): Implement Blazor UI shell
```

## Commit Message Examples

### Good Examples ✅
```
feat(sparrow): add password elimination logic

Implements information-theoretic solver from Algorithm.md.
Success rate: 28% on average difficulty terminals.
```
```
docs(lore): add ECHELON Project timeline

Complete development history from SPARROW (2076) through
ECHELON v2.1.7 (2077). Includes PHOSPHOR evolution.
```
```
refactor(core): introduce dependency injection

Replace static dependencies with Microsoft.Extensions.DI.
Enables better testability and SOLID compliance.

BREAKING CHANGE: Services must be registered in DI container.
Migration guide added to docs/Architecture/DI-Migration.md
```
```
test(algorithm): verify tie-breaker logic

Add tests for worst-case bucket size minimization.
Covers DANTA/DHOBI/LILTS example from Algorithm.md.
```

### Bad Examples ❌
```
Update stuff
```
*Too vague, no type, no scope*
```
Added new feature
```
*Past tense, no scope, unclear what was added*
```
feat: Fix the bug where passwords weren't working correctly and also added some new UI components
```
*Multiple unrelated changes in one commit, wrong type (should be fix + feat separately)*
```
FEAT(CORE): ADD PASSWORD SOLVER!!!!
```
*All caps, exclamation marks, not following format*

## Revert Commits

When reverting a previous commit:
```
revert: feat(sparrow): add password elimination logic

This reverts commit a1b2c3d4e5f6.

Reason: Algorithm converges too slowly on hard difficulty.
Reverting until optimization is complete.
```

## Co-Authoring

When pair programming or incorporating contributions:
```
feat(raven): implement PHOSPHOR color palettes

Add support for Green, Amber, White, and Blue phosphor themes.
Each palette maintains 4 brightness levels.

Co-authored-by: Dr. Elizabeth Krane <krane@enclave.gov>
```

## Tools and Automation

### Commitizen

Use [commitizen] for interactive commit message creation:
```bash
npm install -g commitizen cz-conventional-changelog
git cz
```

### Commitlint

Validate commit messages in CI:
```bash
npm install -g @commitlint/cli @commitlint/config-conventional
```

Configuration (`.commitlintrc.json`):
```json
{
  "extends": ["@commitlint/config-conventional"],
  "rules": {
    "scope-enum": [2, "always", [
      "sparrow", "raven", "ghost", "echelon",
      "core", "shared", "algorithm", "ui", "tests",
      "lore", "arch", "impl", "dev",
      "ci", "build", "deps"
    ]]
  }
}
```

## Commit Frequency

### Do Commit:
- After completing a logical unit of work
- Before switching contexts or branches
- When tests pass (if you have tests)
- At natural stopping points

### Don't Commit:
- Broken code (unless explicitly WIP commit in feature branch)
- Half-finished thoughts
- Just to have something in git
- Without a clear message

## WIP Commits

In feature branches, WIP commits are acceptable but should be squashed before merge:
```
wip(ghost): experimenting with toast positioning

Trying different approaches. Will clean up before PR.
```

These will be squashed when merging PR to main.

## References

- [Conventional Commits] - Specification homepage
- [Semantic Versioning] - Version numbering scheme
- [Angular Commit Guidelines] - Inspiration for conventions
- [Commitizen] - Interactive commit tool

[Conventional Commits]: https://www.conventionalcommits.org/
[Semantic Versioning]: https://semver.org/
[Angular Commit Guidelines]: https://github.com/angular/angular/blob/main/CONTRIBUTING.md#commit
[Commitizen]: https://github.com/commitizen/cz-cli
[Magyar]: ./Commit-Conventions.hu.md