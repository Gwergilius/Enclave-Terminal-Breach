# PHOSPHOR komponens-architektúra

[English] | **Magyar**

## Áttekintés

Ez a dokumentum definiálja a PHOSPHOR-alapú terminál UI renderelés komponensmodelljét.
Bevezeti az egymásba ágyazható komponensfát relatív koordinátákkal, határolt rendereléssel, valamint egy MVVM-alapú vezérlési modellt az állapottal rendelkező és animált UI folyamatokhoz.

Az architektúra célja, hogy támogassa a RAVEN, majd később az ECHELON/GHOST UI fejlődését úgy, hogy közben az alacsony szintű renderelési primitívek (`VirtualScreen` + `Layer`) stabilak maradjanak.

## Célok

- Újrafelhasználható, magasabb szintű UI komponensek biztosítása (például szegélyezett dobozok és folyamatsorok).
- Egymásba ágyazható komponensek támogatása relatív pozicionálással.
- Clipping kényszerítése, hogy a gyermekkomponens ne írhasson a saját határain kívülre.
- A renderelés elválasztása az állapot- és időzítési logikától.
- Determinisztikus, jól tesztelhető animáció-vezérlés fenntartása.

## Nem célok

- A PHOSPHOR alap renderelési primitíveinek lecserélése.
- WPF/MAUI-szerű retained-mode vizuális framework bevezetése.
- Egér-alapú layout vagy eseményútválasztás bevezetése ebben a mérföldkőben.

## Komponensmodell

A PHOSPHOR két komponenskategóriát különböztet meg.

### Layer komponensek

A `LayerComponent` példányok saját, dedikált `Layer`-rel rendelkeznek.
Ablakszerű, z-orderrel kezelt felületekhez valók (például popupok, overlay-ek, toast régiók vagy mozgó panelek).

Jellemzők:

- Abszolút képernyőpozicionálás.
- Explicit z-order kezelés.
- Független invalidációs tartomány.

### Tartalom komponensek

A `ContentComponent` példányok a szülő writer kontextusába renderelnek, és nem hoznak létre dedikált `Layer`-t.
Szerkezeti és szöveges tartalomhoz készültek (például szegélyezett szövegblokkok, folyamatsorok, címkék, progress sorok).

Jellemzők:

- Relatív koordináták a szülő határain belül.
- Nincs extra layer költség.
- Teljesen egymásba ágyazhatók.

## Relatív renderelés LayerWriter-rel

A `LayerWriter` egy clippinget végző koordinátafordító wrapper egy cél `Layer` fölött.

Feladatai:

- Lokális komponenskoordináták abszolút képernyőkoordinátára fordítása.
- Minden írás a komponens aktuális határaira vágása.
- Gyermek writer scope létrehozása a `Clip(relativeBounds)` hívással.

Következmény:

- A komponens mindig úgy renderel, mintha a bal felső origója `(0,0)` lenne.
- A szülő biztonságosan tud gyermekeket komponálni kézi abszolút koordinátaszámítás nélkül.

## Komponensfa és renderelési szerződés

A renderelési szerződés szándékosan minimális.

```csharp
public interface IComponent
{
    Rectangle Bounds { get; }
    void Render(LayerWriter writer);
}
```

A konténer komponensek úgy renderelik a gyermekeket, hogy a szülő writerből clippingelt child writert képeznek.

```csharp
foreach (var child in Children)
{
    var childWriter = writer.Clip(child.Bounds);
    child.Render(childWriter);
}
```

## Lifecycle és állapot

A komponenspéldányok lehetnek:

- Állapotmentesek (tiszta projekció az aktuális bemeneti értékekből).
- Állapottal rendelkezők (például fokozatos pont-kiírás egy folyamatsorban).

Az állapottal rendelkező komponensek nem futtatnak saját aszinkron időzítési ciklust. Csak determinisztikus állapotátmeneti metódusokat adnak (például `Advance()`, `Complete()`).

## MVVM bevezetése PHOSPHOR-hoz

A komponens-architektúra MVVM-mel együtt ad tiszta szétválasztást a renderelés, az állapot és a folyamatvezérlés között.

### Felelősségi mátrix

| Réteg | Felelősség | Mit nem csinálhat |
|------|------------|-------------------|
| Model | Domain adatok és szabályok (candidate szavak, likeness számítások, boot metaadatok) | UI időzítés, közvetlen renderelés |
| ViewModel | Állapotátmenetek, szekvenciák, időzítés, parancskezelés, invalidációs triggerek | Közvetlen alacsony szintű terminál-írás |
| View (komponensek) | Aktuális állapot renderelése writer kontextusba | Async várakozás, üzleti döntések |

### Miért jó itt az MVVM

- A boot sequence effektusai időzítésérzékenyek, ezért egységtesztelhető orchestráció kell.
- A komponensek hordozhatók maradnak console és jövőbeli frontendek között.
- A renderelési logika snapshot-alapon külön tesztelhető az orchestrációtól.

## Invalidáció és render loop

A `ViewModel` így vezérli a frissítést:

1. Állapotot módosít egy vagy több komponensen.
2. Invalidációt kér az érintett tartományokra.
3. A render loop újrarajzolja az invalidált régiót.

Szabályok:

- A komponensek nem hívhatnak belül `Task.Delay`-t.
- A komponensek kerüljék a közvetlen globális képernyőszolgáltatás-hozzáférést; invalidációs szerződésen keresztül kommunikáljanak.
- Minden animációs ütemezés a `ViewModel` felelőssége.

## Boot sequence leképezés (példa)

Az [ECHELON Boot Sequence][boot-sequence] természetesen illeszkedik ehhez a modellhez.

- `BorderedBoxComponent`: statikus, keretezett fejléc blokkok.
- `ProcessBarComponent`: címke + fokozatos pontsor + lezáró állapot token (`OK`).
- Későbbi `ProgressBarComponent`: százalékos és/vagy kitöltéses vizualizáció hosszabb műveletekhez.

A `BootScreenViewModel` vezérli a fázissorrendet és időzítést, míg a komponensek kizárólag az aktuális állapot-pillanatképet renderelik.

## Kezdő implementációs mérföldkövek

### 1. mérföldkő

- `LayerWriter` implementálása clippinggel és koordinátafordítással.
- `BorderedBoxComponent` implementálása.
- Fókuszált unit tesztek clippingre és keret renderelésre.

### 2. mérföldkő

- `ProcessBarComponent` állapotátmeneteinek implementálása.
- `BootScreenViewModel` orchestráció bevezetése a folyamatsorok fölött.
- Determinisztikus időzítési absztrakció bevezetése tesztekhez.

### 3. mérföldkő

- `LayerComponent` use-case-ek bevezetése (popup/overlay felületek).
- Invalidáció aggregáció több komponensfrissítéshez.

## Kompatibilitás és továbbfejlődés

Ez a modell megtartja a PHOSPHOR 1.0 kompatibilitását, miközben lehetővé teszi a fokozatos bővítést PHOSPHOR 1.1+ irányban (window rendszer, további widgetek, gazdagabb kompozíció).

Ugyanez a komponensfa-koncepció később Blazor és MAUI frontendekre is leképezhető anélkül, hogy a terminálspecifikus renderelő kód újrahasznosítására lenne szükség.

[//]: #References
[English]: ./PHOSPHOR-component-architecture.md
[boot-sequence]: ../Design/ECHELON_Boot_Sequence.hu.md