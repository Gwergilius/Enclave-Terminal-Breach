# Development Workflow (Mandatory)

The branching, commit, and release practices in **docs/Development** are mandatory. Follow them whenever you suggest or prepare git operations.

## Authority

- [Branching Strategy](docs/Development/Branching-Strategy.md) – branch naming, types, scopes, PR workflow
- [Commit Conventions](docs/Development/Commit-Conventions.md) – Conventional Commits format, types, scopes, subject/body/footer
- [Release Process](docs/Development/Release-Process.md) – versioning, tags, changelog format
- [GitVersion Integration](docs/Development/GitVersion-Integration.md) – version calculation, semver in commits

## Required Before Every Commit

1. **CHANGELOG.md** – Update before each commit. Add the change under `## [Unreleased]` in the appropriate section (`Added`, `Changed`, `Fixed`, etc.). Format follows [Keep a Changelog](https://keepachangelog.com/).
2. **Commit message** – Use Conventional Commits: `type(scope): subject`; optional body and footer (see Commit-Conventions.md).

## When Suggesting Branches or Commits

- Propose branch names from Branching-Strategy (e.g. `feature/ghost-blazor-ui`, `docs/arch-algorithm`).
- Propose commit messages that match Commit-Conventions (type, scope, imperative subject, optional `+semver:` footer).
- Remind the user to update CHANGELOG.md if they have not yet done so before committing.

## Before merging the SPARROW (1.0.0) PR

- **Update Lore version numbers:** Before merging the feat/SPARROW PR to main, update **docs/Lore/Project-History.md** and **docs/Lore/Project-History.hu.md** so that version numbers in the lore document reflect the real SPARROW 1.x release (see Release-Process: lore reflects real versions). Remind the user of this when they work on or mention the SPARROW PR.

---
alwaysApply: true
---
