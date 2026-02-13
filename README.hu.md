# Enclave Terminal Breach

**[English]** | Magyar

[![License-MIT-badge]][License-MIT]
[![.NET-badge]][Dotnet]

Többplatformos Fallout terminál feltörő asszisztens – a SPARROW prototípustól az ECHELON bevezetésig.

> **Univerzumban:** Az Enclave által fejlesztett feltörő eszköz a RobCo Unified Operating System terminálokhoz. Teljes ECHELON háttér: [Project History][Project History].

## 🎮 Mi ez?

Egy **terminál feltörő asszisztens** alkalmazás, amely a Bethesda Fallout játékok (Fallout 3, New Vegas, 4, 76) terminál feltörő minijátékának megoldásában segít.

**Fontos:** Ez NEM a minijáték újraalkotása, hanem egy külső segédprogram, amely elemzi a jelszómintákat és optimális tippeket javasol.

## 🚀 Projekt állapot

**Jelenlegi fázis:** 📝 Dokumentáció és tervezés

| Komponens | Állapot |
|-----------|--------|
| Dokumentáció | 🚧 Folyamatban |
| Architektúra | 🚧 Folyamatban |
| SPARROW (DOS PoC) | 🚧 Folyamatban |
| RAVEN (Konzol) | 📋 Tervezett |
| GHOST (Web/Blazor) | 📋 Tervezett |
| ECHELON (MAUI mobil) | 📋 Tervezett |

## 📚 Projekt evolúció

A repó a teljes fejlesztési evolúciót dokumentálja:

1. **Excel prototípus** (pre-SPARROW) – Kutatási fázis VBA makrókkal
2. **SPARROW** – DOS 3.11 proof of concept (stdin/stdout)
3. **RAVEN** – Konzol alkalmazás képernyőpozicionálással
4. **GHOST** – Web/SIGNET bevezetés (Blazor PWA)
5. **ECHELON** – Mobil Pip-Boy verzió (MAUI)

Minden fázis jelentős architektúra mérföldkő, a végső ECHELON v2.1.7 bevezetésig.

## 🏗️ Technológiai stack

- **.NET 10.0** – Elsődleges keretrendszer
- **C# 12.0** – Programozási nyelv
- **MAUI** – Többplatformos mobil UI
- **Blazor** – Progressive Web App
- **xUnit** – Unit tesztelés
- **ReqNRoll** – Integrációs/E2E tesztelés
- **Playwright** – UI tesztelés

## 📖 Dokumentáció

- [Project History] – Teljes ECHELON háttér
- [Algorithm] – Jelszó eliminációs algoritmus
- [Architecture] – Rendszertervezési dokumentumok
- [Coding Standards] – Fejlesztési irányelvek

## 📁 Forráskód

A mappa szerkezet, megosztott komponensek (Common, Core, tesztek, teszt segédletek), a solution és a build/stílus konfiguráció a **[src/README][src README]**-ben van leírva. A solution a `src/Enclave.Echelon.slnx` fájlból nyitható. A buildet a **src/** mappából kell futtatni: 

```Powershell
cd src
dotnet build Enclave.Echelon.slnx
```

A **code coverage** riporthoz lásd a [tools/coverage/README](tools/coverage/README.hu.md) fájlt.

## 🔄 CI / pipeline

GitHub Actions (`.github/workflows/ci.yml`):

- **Push** (bármely branch): build, unit tesztek és coverage futnak; a **hibák nem blokkolók** (félkész állapotban is be tudod küldeni, de látod az eredményt). Main-re sikeres teszt esetén a GitVersion kiírja a verziót.
- **Pull request** (main/master felé): a build, unit tesztek és a coverage **blokkolók**; a futtatás **sikertelen**, ha build/teszt elhasal, vagy a line coverage 80% alatt, illetve a branch coverage 95% alatt van.

### Verzió a commit / PR üzenetből

A verzióemelést a **commit üzenetek** (feature branchen) és a **PR címe/leírása** (merge-nél) vezérlik. Konfig: `GitVersion.yml`. A **main-re történő közvetlen commit tiltott** (kivéve pl. Changelog küldés).

| Kontextus | Alapértelmezett | Indító | Példa |
|-----------|------------------|--------|--------|
| **Commit** (feature branchen) | Csak build szám (`0.1.0+5` → `+6`) | `patch(scope):` a subjectben | `patch(fix): validáció javítás` → patch |
| **PR merge** (Squash and merge) | **Minor** (új feature) | Subject `feat:` vagy `feat(scope):` | `feat: Password modell` → minor |
| **PR merge** | **Major** (breaking) | Subject tartalmazza `breaking-change:` vagy `BREAKING CHANGE:` | `breaking-change: API eltávolítás` → major |
| **PR merge** | **Patch** (csak fix) | Subject `patch:` vagy `patch(scope):` | `patch: elírás javítása` → patch |

PR-nál használj **Squash and merge**-et, hogy a PR címe kerüljön a merge commit üzenetébe, és a GitVersion alkalmazza a fenti szabályokat.

## 🤝 Közreműködés

Személyes portfólió projekt, de a visszajelzés és javaslatok megköszönöm. A közreműködési irányelvek: [.cursor/rules/][Coding Standards].

## 📜 Licenc

MIT License – részletek: [LICENSE].

## 🎯 Köszönet

- Bethesda Game Studios a Fallout franchise-ért
- A Fallout közösség az inspirációért
- RobCo Industries (fiktív) az UOS-ért, amit feltörünk

---

**Disclaimer:** Rajongói projekt, nincs kapcsolat a Bethesda Softworks vagy Bethesda Game Studios-szal.

[English]: ./README.md
[Project History]: ./docs/Lore/Project-History.hu.md
[Algorithm]: ./docs/Architecture/Algorithm.hu.md
[Architecture]: ./docs/Architecture/README.hu.md
[Coding Standards]: ./.cursor/rules/README.hu.md
[src README]: ./src/README.hu.md "Forráskód szerkezet és konfiguráció"
[LICENSE]: ./LICENSE
[License-MIT]: https://opensource.org/licenses/MIT
[Dotnet]: https://dotnet.microsoft.com/
[License-MIT-badge]: https://img.shields.io/badge/License-MIT-yellow.svg
[.NET-badge]: https://img.shields.io/badge/.NET-10.0-512BD4
