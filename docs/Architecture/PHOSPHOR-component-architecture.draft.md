# Kérdés:
Szeretnék bevezetni magasabb szintű komponenseket saját (mint a React és a Blazor teszi). A Boot screen (lásd: ~/docs/design/ECHELON_Boot_Sequence.md) remek példa az ilyenekre:
- Bordered text (device header, system header megjelenítéshez)
- Process bar (title + typewriter-ként megjelenített pontsorozat, majd OK a befejezéskor)
   - Detecting/Verifiying/loading modules
- Progress bar (későbbi RAVEN/PHOSPHOR változatok)
Minden komponensnek saját ablaka kellene legyen, amiben relatív módon címez a rendereléskor. (Egymásba ágyazható ablakok kellenének -- komponens-tree, mint a React-nál).
Hogyan kellene elindulni a megvalósítással?

# Válasz (AI):

## A kulcskérdés: minden komponens kap saját Layer-t?
**Nem** — és ez a React/Blazor analógia fontos tanulsága. Ott sem kap minden `<div>` saját `position: absolute` réteget. Csak az "**ablak jellegű**" komponensek kapnak saját Layer-t (popup, toast, mozgó panel). A "**tartalom jellegű**" komponensek (BorderedBox, ProcessBar) a szülőjük layer-ébe renderelnek, relatív koordinátákkal.
Ez két komponens-típust jelent: 
```
LayerComponent         ← saját Layer-t kap, abszolút pozíció + Z-order
    └─ ContentComponent ← szülő layer-ébe ír, relatív koordinátákkal
            └─ ContentComponent ← egymásba ágyazható
```

---

## Az `ILayerWriter` — a relatív koordináta megoldása
A trükk egy **wrapper**, ami egy Layer fölé ül és a relatív koordinátákat abszolúttá fordítja:

```csharp
/// <summary>
/// Wraps a Layer and translates relative (component-local) coordinates
/// to absolute screen coordinates. Clips writes to the component's bounds.
/// </summary>
public sealed class LayerWriter
{
    private readonly Layer _layer;
    private readonly Rectangle _bounds; // absolute bounds of this component

    public LayerWriter(Layer layer, Rectangle bounds)
    {
        _layer = layer;
        _bounds = bounds;
    }

    /// <summary>Creates a child writer clipped to a sub-region (relative to this writer).</summary>
    public LayerWriter Clip(Rectangle relativeBounds)
    {
        var absolute = new Rectangle(
            _bounds.Left + relativeBounds.Left,
            _bounds.Top + relativeBounds.Top,
            relativeBounds.Width,
            relativeBounds.Height);

        // Intersect with our own bounds to enforce clipping
        var clipped = _bounds.Intersect(absolute);
        return new LayerWriter(_layer, clipped);
    }

    public void SetCell(int relCol, int relRow, VirtualCell cell)
    {
        int absCol = _bounds.Left + relCol;
        int absRow = _bounds.Top + relRow;
        if (_bounds.Contains(absCol, absRow))
            _layer.SetCell(absCol, absRow, cell);
    }

    public int Width => _bounds.Width;
    public int Height => _bounds.Height;
}
```

---

## Komponens interfész

```csharp
public interface IComponent
{
    /// <summary>Bounds relative to parent component (or screen if root).</summary>
    Rectangle Bounds { get; }

    /// <summary>
    /// Renders into the provided writer.
    /// The writer is already clipped to this component's bounds —
    /// use (0,0) as the top-left corner.
    /// </summary>
    void Render(LayerWriter writer);
}
```

Az egymásba ágyazás így működik egy container komponensben:

```csharp
public abstract class ContainerComponent : IComponent
{
    protected List<IComponent> Children { get; } = [];

    protected void RenderChildren(LayerWriter writer)
    {
        foreach (var child in Children)
        {
            var childWriter = writer.Clip(child.Bounds);
            child.Render(childWriter);
        }
    }
}
```

---

## A Boot Sequence konkrét komponensei

### `BorderedBoxComponent`
```
╔═══════════════════════════════════════════╗
║ ROBCO TERMINAL NX-12                      ║
║ BIOS v1.4.2.8 - RobCo Industries          ║
╚═══════════════════════════════════════════╝
```

```csharp
public sealed class BorderedBoxComponent(Rectangle bounds, IReadOnlyList<string> lines) 
    : IComponent
{
    public Rectangle Bounds => bounds;

    public void Render(LayerWriter writer)
    {
        // Top border
        writer.SetCell(0, 0, new VirtualCell('╔', CharStyle.Dark));
        for (int col = 1; col < writer.Width - 1; col++)
            writer.SetCell(col, 0, new VirtualCell('═', CharStyle.Dark));
        writer.SetCell(writer.Width - 1, 0, new VirtualCell('╗', CharStyle.Dark));

        // Content rows
        for (int i = 0; i < lines.Count && i + 1 < writer.Height - 1; i++)
        {
            int row = i + 1;
            writer.SetCell(0, row, new VirtualCell('║', CharStyle.Dark));
            WriteText(writer, 2, row, lines[i], CharStyle.Bright);
            writer.SetCell(writer.Width - 1, row, new VirtualCell('║', CharStyle.Dark));
        }

        // Bottom border
        int lastRow = writer.Height - 1;
        writer.SetCell(0, lastRow, new VirtualCell('╚', CharStyle.Dark));
        for (int col = 1; col < writer.Width - 1; col++)
            writer.SetCell(col, lastRow, new VirtualCell('═', CharStyle.Dark));
        writer.SetCell(writer.Width - 1, lastRow, new VirtualCell('╝', CharStyle.Dark));
    }
}
```

### `ProcessBarComponent`
```
Detecting Enclave SIGINT module..........................OK
```

Ez az érdekes — a typewriter effektus _állapotot_ jelent. A komponens nem "rajzol egyszerre", hanem idővel teljesedik ki. Ez azt jelenti, hogy a komponensnek **saját belső állapota** van és `Invalidate`-et hív magán, ahogy az animáció előrehalad:

```csharp
public sealed class ProcessBarComponent : IComponent
{
    private readonly string _label;
    private readonly int _dotCount;
    private int _renderedDots = 0;        // hány pont látszik most
    private ProcessBarState _state = ProcessBarState.Running;

    private IVirtualScreen _screen;       // Invalidate híváshoz
    private Layer _parentLayer;

    public Rectangle Bounds { get; }

    public void Render(LayerWriter writer)
    {
        WriteText(writer, 0, 0, _label, CharStyle.Normal);
        WriteText(writer, _label.Length, 0, 
            new string('.', _renderedDots), CharStyle.Dark);

        if (_state == ProcessBarState.Done)
            WriteText(writer, _label.Length + _dotCount, 0, "OK", CharStyle.Bright);
    }

    // Külső vezérlő (BootSequenceViewModel) hívja:
    public void AddDot()
    {
        _renderedDots = Math.Min(_renderedDots + 1, _dotCount);
        _screen.Invalidate(/* absolute bounds */);
    }

    public void Complete()
    {
        _renderedDots = _dotCount;
        _state = ProcessBarState.Done;
        _screen.Invalidate(/* absolute bounds */);
    }
}
```

---

## A BootSequenceViewModel vezérli az animációt

A komponensek **passzívak** — nem tudnak async várakozni. Az animáció logikája a ViewModelbe kerül:

```csharp
public sealed class BootScreenViewModel
{
    private readonly IVirtualScreen _screen;

    public async Task RunBootSequenceAsync(CancellationToken ct)
    {
        // Phase 1: Device header
        var deviceHeader = new BorderedBoxComponent(
            bounds: new Rectangle(2, 1, 45, 4),
            lines: ["ROBCO TERMINAL NX-12", "BIOS v1.4.2.8 - RobCo Industries"]);
        
        RenderComponent(deviceHeader);
        await Task.Delay(500, ct);

        // Phase 3: Module loading
        var modules = new[]
        {
            "Detecting Enclave SIGINT module",
            "Validating cryptographic signature",
            "Verifying clearance level",
        };

        foreach (var module in modules)
        {
            var bar = new ProcessBarComponent(module, dotCount: 20, ...);
            RenderComponent(bar);
            
            // Typewriter: pontok megjelenítése
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(BootTiming.LineDelay / 20, ct);
                bar.AddDot();
            }
            await Task.Delay(BootTiming.OkStatusDelay, ct);
            bar.Complete();
        }
    }
}
```

---

## Összefoglalva: hogyan indulj el?
Három lépésben érdemes megközelíteni:
**1. LayerWriter** — ez az alap, ez teszi lehetővé a relatív koordinátákat és a clippingot. Önmagában is tesztelhető, nincs függősége.
**2. BorderedBoxComponent** — statikus, nincs állapota, jól lefedi az összes box-drawing karaktert. Első "igazi" komponens.
**3. ProcessBarComponent** — itt jelenik meg az állapot és az Invalidate pattern; ez a legtanulságosabb darab.
A ProgressBar (GHOST/ECHELON platformra) csak ezután következik, és ugyanerre az alapra épül.
Szeretnéd, hogy a LayerWriter-rel és a BorderedBoxComponent-tel kezdjük el a tényleges implementációt?

# Kérdés:
1. Dokumentálni kellene ezt a komponens-koncepciót is a ~/docs/Architecture mappában. Fejtsd ki az MVVM megközelítés bevezetését is! (Gondolom, ezt tükrözi a ViewModel felvetése az előző javaslatban. Ha igen nagyon támogatom)
2. Claude kóddal akarom megcsinálni, VS Code segítségével. Kérlek, állíts össze egy nyitó promotot, amivel elindulhatunk