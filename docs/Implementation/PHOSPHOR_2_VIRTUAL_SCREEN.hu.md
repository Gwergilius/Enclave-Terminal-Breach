# PHOSPHOR 2.0 – Virtuális Képernyő: Implementációs Döntések

[English] | **Magyar**

> **Hatókör:** Implementációs döntések és tervezési indoklások a PHOSPHOR 2.0 virtuális képernyő puffer és compositor megvalósításához. A publikus API specifikációhoz lásd [PHOSPHOR-Requirements.md](../Architecture/PHOSPHOR-Requirements.md), az architektúra leíráshoz [PHOSPHOR-VirtualScreen-Architecture.md](../Architecture/PHOSPHOR-VirtualScreen-Architecture.md).

---

## Összefoglalás

A PHOSPHOR 2.0 felváltja az 1.x szekvenciális (top-to-bottom) renderelési modelljét egy **virtuális képernyő puffer + compositor** pipeline-ra. Ez lehetővé teszi az átlapoló UI elemeket (popupok, toast értesítések, mozgó panelek), amelyek kompozitálhatók és diff-elhetők a fizikai konzolra írás előtt.

A változás **major verzióbumpot** igényel (`phosphor-v1.1.0` → `phosphor-v2.0.0`), mivel a `CharStyle` namespace-e megváltozik és az `IPhosphorInputLoop` interfész breaking jellegű új tagokat kap. A régi `IPhosphorCanvas` API `[Obsolete]` jelöléssel megmarad a fokozatos migráció érdekében.

---

## 1. döntés — `CharStyle` átköltöztetése az `Enclave.Common.Drawing`-ba

**Döntés:** A `CharStyle` átkerült az `Enclave.Phosphor`-ból az `Enclave.Common.Drawing` névtérbe.

**Indoklás:** A `VirtualCell` (platformfüggetlen adatprimitív) függ a `CharStyle`-tól. Ha a `CharStyle` maradt volna az `Enclave.Phosphor`-ban, a `VirtualCell`-nek is ott kellene lennie — annak ellenére, hogy a cellákat konzol, MAUI és Blazor portok között is meg kell osztani. A `CharStyle` áthelyezése az `Enclave.Common.Drawing`-ba biztosítja, hogy a virtuális képernyő adatmodellje teljesen platformfüggetlen marad.

**Megvalósítás:** Ahelyett, hogy minden fájlhoz hozzáadtuk volna a `using Enclave.Common.Drawing;` direktívát az `Enclave.Phosphor` projektben, egyetlen `_using.cs` fájl kapott projekt-szintű `global using`-ot. Ez konzisztens azzal, ahogy a megoldás más projektjei kezelik a sűrűn használt névtereket:

```csharp
// Phosphor/_using.cs
global using Enclave.Common.Drawing;
```

Ugyanezt a `global using`-ot hozzáadtuk a tesztprojekt meglévő `_using.cs` fájljához is.

**Breaking change:** Minden külső kód, amely `Enclave.Phosphor.CharStyle`-ra hivatkozott, mostantól `Enclave.Common.Drawing.CharStyle`-t kell használjon. Ez az egyik oka a major verzióbumpnak.

---

## 2. döntés — `VirtualCell` mint `readonly record struct`

**Döntés:** A `VirtualCell` `readonly record struct`, nem class.

**Indoklás:**

| Tulajdonság | Következmény |
|-------------|-------------|
| `readonly` | A cellák immutábilisak; új cella hozzárendelése mindig felváltja a régit. Ez megakadályozza azokat a rejtett hibákat, ahol egy cella referenciája a bufferbe tárolás után módosul. |
| `record` | A strukturális egyenlőség (`==`) automatikusan generálódik. A Compositor **Diff fázisa** `compositeBuffer[row, col] != _physicalBuffer[row, col]` összehasonlítással dolgozik — ez allokatív overhead nélkül, helyesen működik. |
| `struct` | A cellák értékként tárolódnak a `VirtualCell[,]` tömbben. Egy 80×24-es képernyőhöz egyetlen tömb kell 1920 struktúrával (≈ 7,5 KB). Class esetén 1920 heap objektum + referencia overhead kellene. |

**Opcionális `Style` paraméter:** A `CharStyle Style = CharStyle.Normal` alapértelmezés hozzáadásra került, hogy az általános eset — karakter kiírása az alapértelmezett stílusban — csak egy argumentumot igényeljen:

```csharp
new VirtualCell('═')                     // CharStyle.Normal (alapértelmezett)
new VirtualCell('║', CharStyle.Dark)     // explicit stílus
```

Ezt a kód-review után adtuk hozzá az első implementáció után, visszafelé kompatibilis módon.

---

## 3. döntés — Transzparencia `'\0'`-val, nem külön flag-gel

**Döntés:** Egy cella akkor transzparens, ha `Character == '\0'`. Nincs külön `IsTransparent` boolean.

**Indoklás:** Egy külön flag növelné a struct méretét és minden cellíráshoz feltételt adna. A `'\0'` konvenció tömör és egyértelmű: a `'\0'` valódi karakterérték sohasem kerül szándékosan megjelenítésre terminál UI-ban. A Compositor Recompose fázisa egyetlen feltételt ellenőriz:

```csharp
if (cell.Character != '\0')
    compositeBuffer[row, col] = cell;
```

A `VirtualCell.Empty` (`'\0'`, Normal) a struct alapértelmezett értéke, ami azt jelenti, hogy egy frissen allokált `VirtualCell[,]` tömb automatikusan transzparens cellákkal töltődik fel — nincs szükség explicit inicializálási ciklusra a `Layer` konstruktorában.

**`VirtualCell.Space` vs `VirtualCell.Empty`:** A két statikus mező különböző célt szolgál:

| Mező | Karakter | Jelentés |
|------|----------|----------|
| `VirtualCell.Empty` | `'\0'` | Transzparens — a compositor az alatta lévő réteget mutatja |
| `VirtualCell.Space` | `' '` | Átlátszatlan szóköz — a háttérszínt festi erre a cellára |

A Compositor minden Recompose pass előtt `Space`-szel (nem `Empty`-vel) inicializálja a kompozit buffert, hogy a látható rétegek által le nem fedett képernyőterületek is látható üres karakterként jelenjenek meg, nem pedig undefined állapotban maradjanak.

---

## 4. döntés — `Layer` konkrét osztályként (nincs `ILayer`)

**Döntés:** A `Layer` egy `sealed class`, nincs hozzá `ILayer` interfész.

**Indoklás:** A `Layer` **adattároló** — egy menedzselt buffer bounds és Z-order metaadatokkal. Nincsenek külső függőségei, és nincs platformspecifikus viselkedése. Interfészel való absztrahálása egyetlen reális szcenárióban sem tenné lehetővé a helyettesítést:

- **Tesztek** valódi `Layer` példányokat hoznak létre a `FakeVirtualScreen.AddLayer()`-en keresztül, és a tényleges cella értékeken végeznek assertiont a `GetCell()`-lel. Egy mock layer *kevesebb* lefedettséget nyújtana, nem többet.
- **Platform portok** (MAUI, Blazor) nem igényelnek különböző `Layer` implementációt. Az architektúradokumentum kifejezetten kimondja, hogy a `Layer` az `Enclave.Common`-ban él, és megosztott a platformok között. Csak az Emit fázis (`IPhosphorWriter`) platformspecifikus.
- **Komponens renderelés** (PHOSPHOR 2.1) a `LayerWriter`-t használja absztrakcióhatárként. A `LayerWriter` konkrét `Layer`-re hivatkozik és relatív koordinátákat fordít; a teszthatár a `LayerWriter`-nél lesz, nem a `Layer`-nél.

A megkülönböztetés ugyanazt a mintát követi, amelyet a megoldás más pontjain is alkalmazunk: **a szolgáltatásokat interfészekkel absztrahálják; az adattárolókat nem.**

---

## 5. döntés — `DirtyRegionTracker` mint `internal`

**Döntés:** A `DirtyRegionTracker` egy `internal sealed class`, nem publikus API.

**Indoklás:** A `DirtyRegionTracker` a `VirtualScreen` implementációs részlete. Teljes API-ja (`Invalidate`, `Flush`, `HasRegions`) a `IVirtualScreen`-en keresztül elérhető, így nincs felhasználási eset a közvetlen, Phosphor assemblyn kívüli elérésre.

Az internal láthatóság megakadályozza, hogy a hívók megkerüljék a `VirtualScreen`-t és közvetlenül manipulálják a dirty-region listát — ami deszinkronizálhatná a compositort.

**Tesztelés:** Mivel a `DirtyRegionTracker` internal, az unit tesztek a `VirtualScreen` publikus API-ján keresztül fedik le. Ez szándékos: a tesztek az *megfigyelhető kontraktust* ellenőrzik (invalidálás → flush a várt régiókat adja vissza), nem a belső lista struktúráját.

**Merge heurisztika:** Amikor két dirty régió átfedi egymást, bounding unionjukba olvadnak össze, nem tárolódnak külön. Ez némileg nagyobb rekomponálási területet eredményez kevesebb compositor passért cserébe. Tipikusan 2–5 aktív réteggel rendelkező terminál UI-kban ez mindig nettó nyereség.

---

## 6. döntés — `IPhosphorInputLoop` kiegészítése `ReadKey`-vel és `Dispatch`-csel

**Döntés:** Két új metódus kerül az `IPhosphorInputLoop`-ba:

```csharp
ConsoleKeyInfo ReadKey(CancellationToken ct);
void Dispatch(ConsoleKeyInfo key);
```

**Indoklás:** A `PhosphorRenderLoop`-nak finomgranulált vezérlésre van szüksége az olvasás-dispatch-rekomponálás ciklus felett. A meglévő `Run()` metódus fekete doboz — beolvas egy billentyűt, dispatch-eli, és ciklusonként nem ad lehetőséget compositor munka beillesztésére. A `ReadKey` és `Dispatch` primitívek kiemelése lehetővé teszi, hogy a `PhosphorRenderLoop` a dispatch és a következő olvasás közé illessze be a rekomponálási lépést:

```
ReadKey(ct) → Dispatch(key) → [compositor flush ha dirty] → ReadKey(ct) → …
```

**Breaking change:** Ez a major verzióbump másik oka. Az `IPhosphorInputLoop` minden meglévő implementációjához hozzá kell adni mindkét metódust. A production `PhosphorInputLoop` és a `TestPhosphorInputLoop` test double is frissítésre került.

**Blokkoló korlát:** A `PhosphorInputLoop.ReadKey` blokkol, amíg az `Console.ReadKey()` vissza nem tér, majd ellenőrzi a cancellation tokent. A teljes lemondáshoz tehát egy billentyűleütés szükséges a feloldáshoz. Ez konzisztens a meglévő `Run()` viselkedéssel, és elfogadható egy interaktív terminál alkalmazás esetén.

---

## 7. döntés — `PhosphorRenderLoop` eseményvezérelt, nem timer-alapú

**Döntés:** A render loop billentyűzetbemenetre blokkol. Nincs háttér polling timer.

**Indoklás:** Egy fix frekvenciájú timer (pl. 60 fps):
- Szükségtelen CPU terhelést okozna, amikor a UI inaktív.
- Thread-safe hozzáférést igényelne minden layer bufferhez minden ticken.
- Semmi vizuális előnnyel nem járna szöveges módú UI-ban — a karakterek csak felhasználói vagy rendszeresemények hatására változnak.

Az eseményvezérelt modell ezzel szemben csak akkor vált ki rekomponálást, amikor `IVirtualScreen.HasDirtyRegions` igaz — ami csak akkor fordul elő, ha egy komponens `Invalidate`-et hív a rétege frissítése után.

**Toast / auto-dismiss kivétel:** Az időzítő alapon eltűnő komponensek (toast értesítések) egy `CancellationTokenSource` callback-ből invalidálják magukat — ők az invalidálás forrása, nem a render loop:

```csharp
// Toast komponensen belül:
_cts.CancelAfter(displayDuration);
_cts.Token.Register(() => _screen.Invalidate(_bounds));
```

Ez a render loopot tisztán és polling-mentesen tartja.

---

## 8. döntés — `IPhosphorCanvas` `[Obsolete]` jelöléssel, nem törölve

**Döntés:** Az `IPhosphorCanvas` és az `AnsiPhosphorCanvas` `[Obsolete(..., error: false)]` dekorációt kap törlés helyett.

**Indoklás:** A RAVEN jelenleg az `IPhosphorCanvas`-t használja szekvenciális boot-szekvenciájához és fázis-rendereléséhez. Az API törlése megszakítaná a RAVEN-t, mielőtt az `IVirtualScreen`-alapú modellre migrálható. Az `[Obsolete]` `error: false`-szal:
- A meglévő kód figyelmeztetéssel, nem hibával fordítható — nincs azonnali törés.
- A figyelmeztetés kommunikálja, hogy migráció várható.
- Az API a migrációs ablak alatt teljesen funkcionális marad.

Miután a RAVEN fázisai `IVirtualScreen`-re portolódtak, az `[Obsolete]` típusok `error: true`-ra lesznek frissítve, majd egy következő major verzióban el lesznek távolítva.

---

## 9. döntés — A fizikai buffer `Space`-szel inicializálva

**Döntés:** A Compositor `_physicalBuffer`-e (az utoljára kiírt állapot) `VirtualCell.Space`-szel inicializálódik, nem `VirtualCell.Empty`-vel.

**Indoklás:** A Diff fázis csak ott küld kiírást, ahol `compositeBuffer[r, c] != _physicalBuffer[r, c]`. Ha a fizikai buffer `Empty`-vel (`'\0'`) indulna és az első kompozit `Space` karaktereket tartalmazna, minden szóköz kiküldésre kerülne — technikailag helyes, de rossz okból. `Space`-szel kezdve az első teljes flush csak a nem-szóköz cellákat küldi el, ami a helyes minimális halmaz egy üres képernyővel induló terminál inicializálásához.

---

## 10. döntés — `CharStyle.Normal = 0` (enum újrarendezés)

**Döntés:** A `CharStyle` enum tagjait újrarendezzük, hogy a `Normal` legyen az első (nulla értékű) tag:

```csharp
// Előtte                  // Utána
Background = 0             Normal     = 0   ← alapértelmezett
Dark       = 1             Background = 1
Normal     = 2             Dark       = 2
Bright     = 3             Bright     = 3
```

**Motiváció — a `default(VirtualCell)` illeszkedési probléma:**

A `VirtualCell` struct. Amikor a CLR egy `VirtualCell[,]` tömböt allokál, `default(VirtualCell)` értékekkel tölti fel, ami minden bájtot nulláz. Az eredeti enum sorrendnél ez `('\0', CharStyle.Background)` értéket ad — transzparens (`'\0'`), de váratlan stílusértékkel. A `VirtualCell.Empty` `new('\0', CharStyle.Normal)`-ként volt definiálva, tehát `default(VirtualCell) != VirtualCell.Empty`, annak ellenére, hogy mindkettő szemantikailag transzparens.

A `Normal = 0`-ra való átrendezés pontosan egyenlővé teszi `default(VirtualCell) == VirtualCell.Empty`:

| | Átrendezés előtt | Átrendezés után |
|--|--|--|
| `default(VirtualCell)` | `('\0', Background=0)` ≠ Empty | `('\0', Normal=0)` **= Empty** ✓ |
| Frissen allokált buffer tartalma | technikailag Background | `VirtualCell.Empty` |

**Megfigyelés — `Layer.Clear()` és az implicit függőség:**

Mivel `VirtualCell.Empty == default(VirtualCell)`, felmerülhet a `MemoryMarshal.CreateSpan(...).Fill(VirtualCell.Empty)` helyettesítése `Array.Clear(_buffer)`-rel (ami az összes bájtot nullázza SIMD `memset(0)` útvonalon). Ez a csere **expliciten elutasítva**.

Az `Array.Clear` itt csak azért lenne helyes, mert `CharStyle.Normal == 0`. A `0` numerikus érték nem jelenik meg a forráskódban — a függőség láthatatlan. Egy jövőbeli fejlesztő, aki az enumot átrendezi (vagy új nulla értékű tagot ad hozzá), nem látja a `Layer.Clear`-t olyan helynek, amelyet frissíteni kell. A hiba csendes lenne és nehéz diagnosztizálni.

A `Layer.Clear()` ezért megtartja az explicit `.Fill(VirtualCell.Empty)` alakot:

```csharp
// Megtartva — névvel tölt, helyes a CharStyle belső numerikus értékeitől függetlenül
public void Clear()
{
    if (_buffer.Length == 0) return;
    MemoryMarshal
        .CreateSpan(ref _buffer[0, 0], _buffer.Length)
        .Fill(VirtualCell.Empty);
}
```

A `Normal = 0` illesztés továbbra is értékes: garantálja, hogy egy frissen allokált `VirtualCell[,]` (amelyet a CLR nulla bájtokkal tölt fel) egyenlő legyen a `VirtualCell.Empty`-vel, és ne egy váratlan `('\0', CharStyle.Background)` értékkel. Ez a helyességi előny megmarad — csak az azt kihasználó kódnak kell azt expliciten tennie.

**Szemantikai indoklás:** A `Normal` mint nulla értékű tag a legtermészetesebb sorrend. Ez a rendszer „alapállapota" — ugyanúgy, ahogy `false = 0` a `bool` esetén, vagy `default` bármely nullable típusnál. A `Background`, `Dark` és `Bright` a `Normal`-hoz képest specializációk.

**Breaking change hatóköre:** A `Background`, `Dark` és `Normal` numerikus értékei változnak (`Bright = 3` érintetlen marad). Ez csak akkor breaking change, ha a `CharStyle` egész számként kerül szerializálásra. A kódbázis átvizsgálása megerősíti, hogy ilyen szerializálás nem létezik — a `CharStyle` kizárólag runtime renderelési jelzőként használatos, mindig névvel hivatkozva.

---

## Érintett fájlok

### Új fájlok

| Fájl | Leírás |
|------|--------|
| `Common/Drawing/CharStyle.cs` | `CharStyle` enum, átköltözve az `Enclave.Phosphor`-ból |
| `Common/Drawing/VirtualCell.cs` | Virtuális cella primitív |
| `Phosphor/_using.cs` | Projekt-szintű `global using Enclave.Common.Drawing` |
| `Phosphor/Layer.cs` | Téglalap alakú virtuális képernyőréteg |
| `Phosphor/DirtyRegionTracker.cs` | Internal merge-and-flush dirty region tracker |
| `Phosphor/IVirtualScreen.cs` | Virtuális képernyő interfész |
| `Phosphor/VirtualScreen.cs` | Alapértelmezett virtuális képernyő implementáció |
| `Phosphor/IPhosphorCursor.cs` | Kurzor pozicionálás interfész |
| `Phosphor/AnsiPhosphorCursor.cs` | ANSI CUP (`ESC[row;colH`) implementáció |
| `Phosphor/Compositor.cs` | Recompose → Diff → Emit pipeline |
| `Phosphor/PhosphorRenderLoop.cs` | Eseményvezérelt render loop |
| `Phosphor.Tests/FakeVirtualScreen.cs` | `IVirtualScreen` test double |
| `Phosphor.Tests/DirtyRegionTrackerTests.cs` | Dirty region merge/flush tesztek |
| `Phosphor.Tests/CompositorTests.cs` | Recompose, diff, emit, transzparencia tesztek |

### Módosított fájlok

| Fájl | Változás |
|------|----------|
| `Phosphor/IPhosphorInputLoop.cs` | `ReadKey(ct)` és `Dispatch(key)` hozzáadva |
| `Phosphor/PhosphorInputLoop.cs` | Új interfész tagok implementálva |
| `Phosphor/IPhosphorCanvas.cs` | `[Obsolete]` jelölés |
| `Phosphor/AnsiPhosphorCanvas.cs` | `[Obsolete]` jelölés |
| `Phosphor.Tests/TestPhosphorInputLoop.cs` | Új interfész tagok implementálva |
| `Phosphor.Tests/_using.cs` | `global using Enclave.Common.Drawing` hozzáadva |

### Törölt fájlok

| Fájl | Ok |
|------|----|
| `Phosphor/CharStyle.cs` | Típus átköltözött a `Common/Drawing/CharStyle.cs`-be |

---

[English]: ./PHOSPHOR_2_VIRTUAL_SCREEN.md
