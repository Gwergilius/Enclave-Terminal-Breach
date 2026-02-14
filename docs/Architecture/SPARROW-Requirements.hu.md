# SPARROW követelmények

**Magyar** | [English]

## Cél

A SPARROW a Core modul [jelszó-solver][Algorithm] implementációinak **legegyszerűbb felülete**. **Proof of concept**: annak bemutatása, hogy az algoritmus működik és használható minimális, szkriptelhető környezetben.

## Technikai megkötések

- **Konzolalkalmazás**: fekete–fehér (nincs ANSI szín vagy képernyőpozícionálás).
- **I/O**: Csak szekvenciális be- és kimenet a **stdin** és **stdout** csatornákon.
- **Nincs** kurzorpozícionálás és karakter szintű színváltás.

## Verzió és identitás

- **Terméknév**: SPARROW (az alkalmazás Product attribútuma).
- **Verzió**: Az assembly verziója (pl. `1.1.0`).

## Nyelv

**Az alkalmazás teljes kimenete** (promptok, üzenetek, feliratok) **angol nyelvű.** A nyelvválasztás vagy lokalizáció nem tartozik a SPARROW hatókörébe; későbbi release-ben jöhet szóba.

## Alkalmazás menet

### 1. Indítási badge

Indításkor egy rövid szöveg és a betöltési idő:

```
SPARROW 1.1.0
Loading system profiles...537 ms
```

A verziószám az alkalmazás Assembly Version; a terméknév az alkalmazás ProductName attribútuma. A „Loading system profiles” sor a betöltés idejét jelzi (pl. ezredmásodpercben).

### 2. Adatbekérés mód (jelöltek gyűjtése)

Az alkalmazás kéri a jelszójelölteket, és addig fogad bemenetet, amíg a felhasználó üres sorral nem jelez befejezést. Minden bemeneti prompt kérdőjellel vagy kettősponttal záródik.

- **Első prompt**: `Enter password candidates:` — majd vár a bemenetre.
- **Későbbi promptok**: `Enter more password candidates (empty line to finish):` — minden nem üres sor után ismétlődik. A felhasználó addig adhat meg további sorokat, amíg **üres sort** nem küld.

**Beviteli szabályok:**

- Egy sorban egy vagy több szó állhat, egy vagy több szóközzel elválasztva.
- **Szóhossz**: Az első elfogadott szó hossza rögzíti a kötelező hosszt. Bármely későbbi, ettől eltérő hosszú szó **elutasításra kerül**: hibaüzenet, a bevitel folytatódik (a hibás szó nem kerül a listába).
- **Törlés**: A `-` előtagú szó (pl. `-TERMS`) kiveszi a szót a jelöltek listájából. Az összehasonlítás **kis- és nagybetűre nem érzékeny** (case-insensitive), ahogy a bevitel és a jelöltek egyeztetése is.
- **Duplikátum**: Ha egy már a listában lévő szót újra megadnak, **figyelmen kívül hagyjuk** és figyelmeztető üzenet jelenik meg.

**Minden sikeres beviteli sor után:**

- Kiírjuk a jelenlegi jelöltek számát.
- Kiírjuk a jelölteket **többoszlopos** elrendezésben; az oszlopok számát a szóhossz határozza meg. A szavak **ábécésorrendben** (vízszintesen az oszlopokban).

### 3. Jelszó feltörés mód (solver hurok)

Az adatbekérés befejezése (üres sor) után az alkalmazás jelszófeltörés módba vált, és a Core [solver][Algorithm]-jét használja:

1. **Legjobb tipp**: A solver meghívása (pl. `IPasswordSolver.GetBestGuess` vagy `GetBestGuesses`) a jelenlegi jelöltek listájával. A javasolt tipp megjelenítése: pl. `Suggested guess: \`xxxx\`.` **Minden bemeneti prompt** (jelöltek, találatszám stb.) **kérdőjellel vagy kettősponttal záródik** (`?` vagy `:`).
2. **Terminál válasz**: Kérés a Fallout terminál által visszaadott találatszámra, pl. `Match count? _`, majd a felhasználó bemenetének olvasása (egész szám).
3. **Nincs nyerés** (találat &lt; szóhossz):
   - `NarrowCandidates(candidates, guess, matchCount)` hívása a lista szűkítéséhez.
   - A fennmaradó jelöltek száma és a lista kiírása (ugyanaz a többoszlopos, ábécés rendezés, mint a beviteli módban).
   - Ismétlés az 1. lépéstől a szűkített listával.
4. **Nyerés** (találat = szóhossz): Gratulációs üzenet, majd **kilépés**.
5. **Kilépés**: A felhasználó bármikor megszakíthatja az alkalmazást (pl. Ctrl+C).

## Core integráció

- Az alkalmazás az [Enclave.Echelon.Core][Core] függőségét használja, és az [IPasswordSolver][Algorithm] felületet:
  - `GetBestGuess(candidates)` vagy `GetBestGuesses(candidates)` a javasolt tipp lekéréséhez.
  - `NarrowCandidates(candidates, guess, matchCount)` a terminál válasza utáni szűkítéshez.
- A jelöltek és a tipp típusa a Core `Password` (vagy megfelelő) modelljével igazodik.

## Példa: teljes játékmenet

Az alábbi példa egy **16 ötbetűs jelöltet** tartalmazó teljes menetet mutat. A titkos jelszó (a felhasználó számára ismeretlen; a találatszámokat a Fallout terminál adja) **TIRES**. A solver tippeket ad; a felhasználó minden tipphez megadja a találatszámot, amíg a terminál nincs feltörve.

**Jelöltek (1 szó, majd 7 szó, majd 8 szó, majd üres sor):**  
TERMS | TEXAS TIRES TANKS SALES SALTY SAUCE SAVES | DANTA DHOBI LILTS OAKUM ALEFS BLOCK BRAVE CHAIR.

**Titkos:** TIRES → így a felhasználó által megadott találatszámok: TERMS tippre → 3; TIRES tippre → 5 (nyerés).

```
SPARROW 1.1.0
Loading system profiles...537 ms

Enter password candidates:
TERMS

1 candidate(s):
TERMS

Enter more password candidates (empty line to finish):
TEXAS TIRES TANKS SALES SALTY SAUCE SAVES

8 candidate(s):
SALES  SALTY  SAUCE  SAVES
TANKS  TEXAS  TERMS  TIRES

Enter more password candidates (empty line to finish):
DANTA DHOBI LILTS OAKUM ALEFS BLOCK BRAVE CHAIR

16 candidate(s):
ALEFS  BLOCK  BRAVE  CHAIR
DANTA  DHOBI  LILTS  OAKUM
SALES  SALTY  SAUCE  SAVES
TANKS  TEXAS  TERMS  TIRES

Enter more password candidates (empty line to finish):

Suggested guess: `TERMS`
Match count? 3

1 candidate(s):
TIRES

Suggested guess: `TIRES`
Match count? 5

Correct. Terminal cracked.
```

*(A gratulációs üzenet után az alkalmazás kilép.)*

## Összefoglaló

| Szempont | Követelmény |
|----------|-------------|
| Nyelv | Teljes UI kimenet angolul; nyelvválasztás ebben a release-ben nincs |
| Szerep | Legegyszerűbb UI; POC a Core PasswordSolver számára |
| I/O | Csak stdin/stdout; nincs szín vagy kurzorpozícionálás |
| Identitás | Terméknév SPARROW; verzió az Assembly Version alapján |
| Beviteli mód | Prompt a jelöltekre; szóközzel elválasztott szavak; üres sor = befejezés; hossz/duplikátum/törlés szabályok |
| Kimenet | Jelöltek száma + többoszlopos ábécés lista minden bevitel után |
| Feltörés mód | GetBestGuess → találatszám prompt → NarrowCandidates → ismétlés vagy gratuláció és kilépés |
| Kilépés | Nyerésnél gratuláció; bármikor Ctrl+C (vagy megfelelő) |

[Algorithm]: ./Algorithm.hu.md
[Core]: ../../README.hu.md
[English]: ./SPARROW-Requirements.md
