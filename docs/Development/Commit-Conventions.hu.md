# Commit konvenciók

**[English]** | Magyar

[Conventional Commits] specifikáció: tiszta, elemzésre alkalmas history, automatikus changelog, szemantikus verzió.

**Formátum:** `<type>(<scope>): <subject>` + opcionális body + footer. **Type:** feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert. **Scope:** sparrow, raven, ghost, echelon, core, shared, algorithm, ui, tests, lore, arch, impl, dev, ci, build, deps. **Subject:** felszólító mód, max 50 karakter, első betű kis. **Footer:** BREAKING CHANGE:, Fixes #123, Co-authored-by:.

**Példák:** feat(sparrow): add password elimination logic; fix(raven): resolve cursor positioning; docs(lore): add ECHELON backstory; chore(scope): finalize changelog (admin bypass). Breaking change: scope után `!` vagy footer BREAKING CHANGE:. WIP commit feature branchen megengedett; squash merge előtt.

[English]: ./Commit-Conventions.md
[Conventional Commits]: https://www.conventionalcommits.org/
