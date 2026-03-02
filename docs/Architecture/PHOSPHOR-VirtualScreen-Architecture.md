# PHOSPHOR Virtual Screen Architecture

**English** | [Magyar]

> **Scope:** This document describes the implementation architecture of the PHOSPHOR 1.1 virtual screen buffer and compositor. It supplements [PHOSPHOR-Requirements.md](./PHOSPHOR-Requirements.md), which defines the public API surface and version roadmap. Read that document first.

---

## Motivation

PHOSPHOR 1.0 uses sequential (top-to-bottom) rendering: text is written in order, and the physical console cursor position implicitly tracks progress. This model works for linear boot sequences and single-panel UIs, but breaks down once UI elements can overlap:

- A popup rendered on top of a panel must not permanently erase the panel beneath it when it closes.
- A toast notification must appear over all other content without triggering a full redraw.
- Moving a panel must restore whatever was visible in the vacated region.

Solving these problems requires knowing, at any given cell `(column, row)`, what *should* be visible — accounting for Z-order. A **virtual screen buffer** provides exactly this: a model of the intended screen state that can be recomposed at any time.

---

## Core Concepts

### 1. Virtual Cell

The virtual screen is a 2D grid of `VirtualCell` values. Each cell stores the character and style that the owning layer wants to render at that position. The cell does **not** store a Z-order value — Z-order is a property of the layer, not the cell.

```csharp
/// <summary>
/// A single character cell in the virtual screen buffer.
/// </summary>
public readonly record struct VirtualCell(char Character, CharStyle Style)
{
    public static readonly VirtualCell Empty = new(' ', CharStyle.Normal);
}
```

### 2. Layer

A layer is a rectangular region of the virtual screen owned by a single UI component (panel, popup, toast, etc.). Layers are ordered by `ZOrder`; higher values render on top.

```csharp
public sealed class Layer
{
    public int ZOrder { get; init; }
    public Rectangle Bounds { get; private set; }
    public bool IsVisible { get; set; } = true;

    // Internal buffer: (row, col) relative to Bounds.Location
    private readonly VirtualCell[,] _buffer;

    public Layer(Rectangle bounds, int zOrder)
    {
        Bounds = bounds;
        ZOrder = zOrder;
        _buffer = new VirtualCell[bounds.Height, bounds.Width];
        Clear();
    }

    public VirtualCell GetCell(int col, int row) =>
        _buffer[row - Bounds.Top, col - Bounds.Left];

    public void SetCell(int col, int row, VirtualCell cell) =>
        _buffer[row - Bounds.Top, col - Bounds.Left] = cell;

    public void Clear() =>
        Array.Fill(_buffer.Cast<VirtualCell>().ToArray(), VirtualCell.Empty);

    public void MoveTo(Point newLocation)
    {
        Bounds = new Rectangle(newLocation, Bounds.Size);
    }
}
```

> **Why no Z-order on the cell?** If Z-order were stored per cell, compositing would require sorting per cell — O(N·W·H). With Z-order on the layer, compositing is O(L·W·H) where L is the number of layers, which is always small.

### 3. Virtual Screen

The virtual screen maintains the ordered collection of layers. It is the single source of truth for what *should* be on the physical screen.

```csharp
public interface IVirtualScreen
{
    Size Size { get; }

    /// <summary>
    /// Registers a new layer. The layer is inserted into the Z-order at the
    /// position indicated by <see cref="Layer.ZOrder"/>.
    /// </summary>
    Layer AddLayer(Rectangle bounds, int zOrder);

    /// <summary>
    /// Removes a layer and invalidates the region it occupied.
    /// </summary>
    void RemoveLayer(Layer layer);

    /// <summary>
    /// Marks a rectangular region as requiring recomposition.
    /// Called by components after they write to their layer.
    /// </summary>
    void Invalidate(Rectangle region);

    /// <summary>
    /// Returns true if any dirty regions are pending recomposition.
    /// </summary>
    bool HasDirtyRegions { get; }

    /// <summary>
    /// Returns pending dirty regions and clears the tracker.
    /// </summary>
    IReadOnlyList<Rectangle> FlushDirtyRegions();

    /// <summary>
    /// Returns the layers intersecting <paramref name="region"/>, ordered by ZOrder ascending.
    /// </summary>
    IEnumerable<Layer> GetLayersInRegion(Rectangle region);
}
```

---

## Dirty Region Tracking

### Why regions, not cells

Tracking dirtiness at the individual cell level (a `bool[,]` dirty map) has two problems:

1. **Flush cost:** at 80×24 = 1 920 cells, iterating the entire map every frame is cheap — but at larger terminals (200×50 = 10 000 cells) it accumulates.
2. **Pattern mismatch:** UI changes almost always affect rectangular regions (a panel redraws, a popup appears). Accumulating dirty cells and then emitting one console write per cell is far slower than a single region-level operation.

Region-based tracking matches the natural granularity of UI events.

### DirtyRegionTracker

```csharp
internal sealed class DirtyRegionTracker
{
    private readonly List<Rectangle> _regions = [];
    private readonly object _lock = new();

    public bool HasRegions
    {
        get { lock (_lock) return _regions.Count > 0; }
    }

    public void Invalidate(Rectangle region)
    {
        lock (_lock)
        {
            // Merge with an existing overlapping region if possible,
            // otherwise append. This keeps the list short.
            for (int i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].IntersectsWith(region))
                {
                    _regions[i] = _regions[i].Union(region);
                    return;
                }
            }
            _regions.Add(region);
        }
    }

    public IReadOnlyList<Rectangle> Flush()
    {
        lock (_lock)
        {
            var snapshot = _regions.ToList();
            _regions.Clear();
            return snapshot;
        }
    }
}
```

**Merging heuristic:** Adjacent or overlapping dirty rectangles are unioned into one. This trades a slightly larger recomposition region for fewer compositor passes. For terminal UIs with typically 2–5 layers this is always a net win.

---

## Compositor (Painter's Algorithm)

The compositor translates a dirty region into a set of physical console writes. It operates in three phases:

```
Dirty region
    │
    ▼
┌─────────────────────────────┐
│ 1. RECOMPOSE                │  Layers → composite buffer
│    Painter's algorithm      │
└─────────────────────────────┘
    │
    ▼
┌─────────────────────────────┐
│ 2. DIFF                     │  Composite buffer vs. last-written buffer
└─────────────────────────────┘
    │
    ▼
┌─────────────────────────────┐
│ 3. EMIT                     │  Write changed cells to physical console
│    via IPhosphorWriter      │
└─────────────────────────────┘
```

### Phase 1: Recompose

The compositor iterates layers **in ascending Z-order** ("painter's algorithm": paint the bottom layer first, then higher layers paint over it). For each cell in the dirty region, the last layer that writes a non-transparent cell wins.

```csharp
private void Recompose(Rectangle region, VirtualCell[,] compositeBuffer)
{
    // Start with empty cells (background)
    FillRegion(compositeBuffer, region, VirtualCell.Empty);

    // Paint layers bottom-to-top
    foreach (var layer in _screen.GetLayersInRegion(region)
                                 .Where(l => l.IsVisible)
                                 .OrderBy(l => l.ZOrder))
    {
        var overlap = layer.Bounds.Intersect(region);
        if (overlap.IsEmpty) continue;

        for (int row = overlap.Top; row <= overlap.Bottom; row++)
        for (int col = overlap.Left; col <= overlap.Right; col++)
        {
            var cell = layer.GetCell(col, row);
            // Transparency: a cell with Character == '\0' is treated as transparent
            if (cell.Character != '\0')
                compositeBuffer[row, col] = cell;
        }
    }
}
```

**Transparency convention:** A cell whose `Character` is `'\0'` (default) is transparent — the compositor skips it and the layer below shows through. This allows panels with non-rectangular shapes (e.g. rounded corners with transparent cells) without a separate alpha channel.

### Phase 2: Diff

The diff compares the newly composed buffer against the `_physicalBuffer` (the last state written to the real console). Only cells that differ are emitted.

```csharp
private IReadOnlyList<CellWrite> Diff(Rectangle region,
    VirtualCell[,] compositeBuffer)
{
    var writes = new List<CellWrite>();
    for (int row = region.Top; row <= region.Bottom; row++)
    for (int col = region.Left; col <= region.Right; col++)
    {
        var composed = compositeBuffer[row, col];
        if (composed != _physicalBuffer[row, col])
        {
            writes.Add(new CellWrite(col, row, composed));
            _physicalBuffer[row, col] = composed;
        }
    }
    return writes;
}
```

### Phase 3: Emit

Cell writes are grouped by row and style to minimise ANSI escape sequences. A contiguous run of same-style cells on the same row is emitted as a single `Write` call.

```csharp
private void Emit(IReadOnlyList<CellWrite> writes)
{
    // Group by row, then by contiguous col + same style
    foreach (var rowGroup in writes.GroupBy(w => w.Row).OrderBy(g => g.Key))
    {
        // Build runs of adjacent same-style cells
        var run = new StringBuilder();
        CharStyle? runStyle = null;
        int? runStartCol = null;
        int? prevCol = null;

        foreach (var write in rowGroup.OrderBy(w => w.Col))
        {
            bool sameStyle = write.Cell.Style == runStyle;
            bool adjacent = prevCol.HasValue && write.Col == prevCol + 1;

            if (!sameStyle || !adjacent)
            {
                FlushRun(runStartCol, rowGroup.Key, runStyle, run);
                run.Clear();
                runStyle = write.Cell.Style;
                runStartCol = write.Col;
            }
            run.Append(write.Cell.Character);
            prevCol = write.Col;
        }
        FlushRun(runStartCol, rowGroup.Key, runStyle, run);
    }
}

private void FlushRun(int? col, int row, CharStyle? style, StringBuilder text)
{
    if (text.Length == 0) return;
    _cursor.MoveTo(col!.Value, row);
    _writer.Style = style!.Value;
    _writer.Write(text.ToString());
}
```

---

## Render Loop

The render loop is **event-driven**, not timer-driven. A background polling timer (30-60 fps) would introduce race conditions and unnecessary CPU usage when the UI is idle.

Instead, the loop blocks on keyboard input. Each keypress may cause a component to update its layer (setting dirty cells and calling `Invalidate`). After the handler returns, the compositor checks for dirty regions and recomposes only if needed.

```csharp
public sealed class PhosphorRenderLoop
{
    private readonly IVirtualScreen _screen;
    private readonly Compositor _compositor;
    private readonly IPhosphorInputLoop _input;

    public void Run(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            // Blocks until a key is available
            var key = _input.ReadKey(ct);

            // Dispatch to focused component → may call _screen.Invalidate(region)
            _input.Dispatch(key);

            // Recompose only if something changed
            if (_screen.HasDirtyRegions)
            {
                foreach (var region in _screen.FlushDirtyRegions())
                    _compositor.Flush(region);
            }
        }
    }
}
```

> **Timer use case:** Toasts with auto-dismiss timers are the one case where a time-triggered invalidation is needed. This is handled by the toast component itself registering a `CancellationTokenSource` timeout, which calls `_screen.Invalidate(toastBounds)` on expiry — not by polling the screen from a background thread.

---

## Layer Lifecycle and Invalidation Patterns

### Panel appears

```csharp
void ShowPanel(Rectangle bounds, int zOrder)
{
    var layer = _screen.AddLayer(bounds, zOrder);
    _panel.SetLayer(layer);
    _panel.Render();                  // writes to layer cells
    _screen.Invalidate(bounds);       // marks region for recomposition
}
```

### Panel closes

```csharp
void ClosePanel(Layer layer)
{
    var vacatedRegion = layer.Bounds;
    _screen.RemoveLayer(layer);
    _screen.Invalidate(vacatedRegion); // recompose reveals layers below
}
```

### Panel moves

```csharp
void MovePanel(Layer layer, Point newPosition)
{
    var oldRegion = layer.Bounds;
    layer.MoveTo(newPosition);
    _screen.Invalidate(oldRegion);     // old position: reveal layers below
    _screen.Invalidate(layer.Bounds);  // new position: paint layer at new location
}
```

### Content update (e.g. autocomplete list refresh)

```csharp
void RefreshAutocompleteList()
{
    _listLayer.Clear();
    RenderListItems(_listLayer);       // writes new cells to layer
    _screen.Invalidate(_listLayer.Bounds);
}
```

---

## Z-Order Conventions

Reserved Z-order slots ensure consistent stacking across all components:

| Z-order | Layer type |
|---------|------------|
| 0 | Background / wallpaper |
| 10 | Main content panels (title bar, sidebar, input area) |
| 20 | Secondary panels (password list, hint area) |
| 50 | Floating windows (movable dialogs) |
| 100 | Popup / modal dialogs |
| 200 | Toast notifications |
| 999 | Debug overlay (development only) |

Gaps between slots allow insertion of intermediate layers without renumbering.

---

## Testability

The virtual screen architecture is fully testable without a physical console:

| Component | Test double |
|-----------|-------------|
| `IVirtualScreen` | `FakeVirtualScreen`: records `Invalidate` calls, exposes layer contents |
| `IPhosphorWriter` (in Emit phase) | `RecordingPhosphorWriter`: captures all `MoveTo` + `Write` sequences |
| `IPhosphorInputLoop` | `ProgrammaticInputLoop`: injects `ConsoleKeyInfo` events synchronously |

**Example: verify that closing a popup invalidates the correct region**

```csharp
[Fact, UnitTest]
public void ClosePopup_InvalidatesPopupRegion()
{
    // Arrange
    var fakeScreen = new FakeVirtualScreen(new Size(80, 24));
    var popup = new PopupComponent(fakeScreen, bounds: new Rectangle(10, 5, 40, 10));
    popup.Show();
    fakeScreen.ClearInvalidations(); // ignore the Show() invalidation

    // Act
    popup.Close();

    // Assert
    fakeScreen.InvalidatedRegions.ShouldContain(r => r == new Rectangle(10, 5, 40, 10));
}
```

---

## Relationship to Platform Ports

The virtual screen is a **console-layer concern**. When porting to MAUI or Blazor, the concept maps as follows:

| PHOSPHOR / Console | MAUI | Blazor |
|--------------------|------|--------|
| `Layer` | `SKElement` / `AbsoluteLayout` child | `<div>` with `position: absolute` |
| Z-order | `ZIndex` / layout order | CSS `z-index` |
| `Compositor.Flush` | `InvalidateSurface()` + `SKCanvas.DrawRect` | DOM diffing (React/Blazor component tree) |
| `DirtyRegionTracker` | `InvalidateRegion(rect)` | `StateHasChanged()` |
| `IPhosphorWriter` (Emit) | `SKPaint` / `DrawText` | `InnerText` / style binding |

The virtual screen itself (`IVirtualScreen`, `Layer`, `VirtualCell`) is in `Enclave.Common` and shared across platforms. Only the Emit phase (`IPhosphorWriter` implementation) is platform-specific.

---

## Implementation Checklist (PHOSPHOR 1.1)

- [ ] `VirtualCell` record in `Enclave.Common.Drawing`
- [ ] `Layer` class in `Enclave.Phosphor`
- [ ] `IVirtualScreen` + `VirtualScreen` implementation
- [ ] `DirtyRegionTracker` (internal, used by `VirtualScreen`)
- [ ] `Compositor` with Recompose → Diff → Emit pipeline
- [ ] `IPhosphorCursor` + `AnsiPhosphorCursor` (ANSI escape for cursor positioning)
- [ ] `PhosphorRenderLoop` replacing the sequential render approach
- [ ] `FakeVirtualScreen` test double in `Enclave.Phosphor.Tests`
- [ ] Unit tests for `DirtyRegionTracker` merge logic
- [ ] Unit tests for `Compositor` (recompose correctness, diff minimality)
- [ ] Integration test: popup show/close restores background layer

---

[Magyar]: ./PHOSPHOR-VirtualScreen-Architecture.hu.md
[PHOSPHOR-Requirements]: ./PHOSPHOR-Requirements.md
