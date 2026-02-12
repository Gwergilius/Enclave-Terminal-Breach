# Release folyamat

**[English]** | Magyar

Szemantikus verzió (MAJOR.MINOR.PATCH) platformonként. Fázis verziók: sparrow-v0.1.0, raven-v0.4.0, ghost-v1.2.4, echelon-v2.1.7.

**Lore és verzió:** A verziószámokat nem a Lore dokumentumhoz igazítjuk. A projekt valódi (release) verzióit használjuk mindenütt; a [Project-History.md](../Lore/Project-History.md) ezeket a valódi verziókat tükrözi, és release-ekkor frissítjük. Forrás: Git tag-ek, csomagverziók, CHANGELOG.md, projektfájlok.

**Pre-release ellenőrzőlista:** tesztek, doc, verzió a projektfájlokban, build, manuális teszt. **Lépések:** verzió frissítés (csproj), CHANGELOG [Keep a Changelog], release commit, annotated tag (pl. sparrow-v0.1.0), push tag, GitHub Release (cím, leírás, opcionális binárisok). **Típusok:** stable, pre-release (alpha/beta/rc), hotfix (tagból branch, fix, PR, changelog, tag).

Changelog kézi (ajánlott) vagy standard-version/release-please. Build artifactok: platform-specifikus publish, elnevezés Enclave-{PHASE}-v{VERSION}-{PLATFORM}. Deprecation: [Obsolete], doc, CHANGELOG Deprecated, eltávolítás következő MAJOR-ban.

[English]: ./Release-Process.md
