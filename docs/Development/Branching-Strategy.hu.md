# Branch stratégia

**[English]** | Magyar

Git workflow és branch kezelés. **GitHub Flow** variáns: PR-only feature fejlesztés, fázis alapú scope (sparrow, raven, ghost, echelon, core, arch, ci). Main védett; admin bypass **csak** changelog véglegesítésre.

**Branch formátum:** `<type>/<scope>[-description]` – docs/, feature/, refactor/, fix/, test/, chore/. **Workflow:** main pull → feature branch → commit (Conventional Commits) → push → PR → review → Squash and Merge (vagy Merge commit) → changelog véglegesítés (admin bypass, csak changelog fájlok) → tag.

**Admin bypass:** CSAK [Unreleased]→verzió/dátum, új [Unreleased], központi CHANGELOG összefoglaló; SOHA kód, doc tartalom, config. Hotfix: hotfix branch tagból, PR, merge, changelog véglegesítés, tag. Részletek: [Commit-Conventions][Commit-Conventions], [Changelog-Management][Changelog-Management].

[English]: ./Branching-Strategy.md
[Commit-Conventions]: ./Commit-Conventions.hu.md
[Changelog-Management]: ./Changelog-Management.hu.md
