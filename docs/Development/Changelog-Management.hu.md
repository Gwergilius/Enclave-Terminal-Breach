# Changelog kezelés

**[English]** | Magyar

**Hibrid stratégia:** központi CHANGELOG (gyökér) – release összefoglaló; komponens changelogok (docs/CHANGELOG.md, src/.../CHANGELOG.md) – részletes változások. CHANGELOG fájlok **csak angolul**, nincs .hu.md.

**Feature branchen:** csak az érintett komponens changelogja, [Unreleased] alá bejegyzés (Added/Changed/Fixed stb.). **PR merge után:** changelog még dátum nélkül. **Changelog véglegesítés:** egyetlen megengedett közvetlen commit main-re; [Unreleased]→[X.Y.Z] vagy [YYYY-MM-DD] (docs); új üres [Unreleased]; központi CHANGELOG összefoglaló + link; csak changelog fájlok; commit „chore(scope): finalize changelog…”; tag a véglegesítés után.

**Admin bypass:** CSAK changelog véglegesítés; SOHA kód, doc tartalom. Biztonsági ellenőrzés: git status csak changelog, git diff csak changelog változás. Kategóriák: Added, Changed, Deprecated, Removed, Fixed, Security. Központi szabály: összefoglaló + link, ne duplikáld a komponens tartalmát.

[English]: ./Changelog-Management.md
