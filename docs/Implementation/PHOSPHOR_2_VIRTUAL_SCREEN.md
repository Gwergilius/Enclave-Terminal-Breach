# PHOSPHOR 2.0 – Virtual Screen Implementation Decisions

**English** | [Magyar]

> **Scope:** Implementation decisions and design rationale for the PHOSPHOR 2.0 virtual screen buffer and compositor. For the public API specification and feature list, see [PHOSPHOR-Requirements.md](../Architecture/PHOSPHOR-Requirements.md). For the architectural overview, see [PHOSPHOR-VirtualScreen-Architecture.md](../Architecture/PHOSPHOR-VirtualScreen-Architecture.md).

---

## Summary

PHOSPHOR 2.0 replaces the sequential (top-to-bottom) rendering model of 1.x with a **virtual screen buffer + compositor** pipeline. This enables overlapping UI layers (popups, toasts, movable panels) that can be composed and diffed before being written to the physical console.

The change is a **major version bump** (`phosphor-v1.1.0` → `phosphor-v2.0.0`) because it renames the `CharStyle` namespace and adds breaking members to `IPhosphorInputLoop`. The old `IPhosphorCanvas` API is preserved as `[Obsolete]` for incremental migration.

---

## Decision 1 — `CharStyle` moved to `Enclave.Common.Drawing`

**Decision:** `CharStyle` was moved from `Enclave.Phosphor` to `Enclave.Common.Drawing`.

**Rationale:** `VirtualCell` (a cross-platform data primitive) depends on `CharStyle`. If `CharStyle` had stayed in `Enclave.Phosphor`, `VirtualCell` would have to live in `Enclave.Phosphor` too — even though cells are intended to be shared across console, MAUI, and Blazor ports. Moving `CharStyle` to `Enclave.Common.Drawing` keeps the virtual screen data model fully platform-agnostic.

**Implementation:** Rather than adding `using Enclave.Common.Drawing;` to every file in the `Enclave.Phosphor` project, a single `_using.cs` was added with a project-level `global using`. This is consistent with how other projects in the solution handle frequently-used namespaces:

```csharp
// Phosphor/_using.cs
global using Enclave.Common.Drawing;
```

The same `global using` was added to the test project's existing `_using.cs`.

**Breaking change:** Any external code that referenced `Enclave.Phosphor.CharStyle` must now reference `Enclave.Common.Drawing.CharStyle`. This was one of the two reasons for the major version bump.

---

## Decision 2 — `VirtualCell` as `readonly record struct`

**Decision:** `VirtualCell` is a `readonly record struct`, not a class.

**Rationale:**

| Property | Consequence |
|----------|-------------|
| `readonly` | Cells are immutable; assigning a new cell always replaces the old one. This prevents subtle bugs where a cell reference is mutated after being stored in the buffer. |
| `record` | Structural equality (`==`) is generated automatically. The Compositor's **Diff phase** compares `compositeBuffer[row, col] != _physicalBuffer[row, col]` — this comparison is both correct and allocation-free. |
| `struct` | Cells are stored as values directly in the `VirtualCell[,]` buffer array. A 80×24 screen allocates one array of 1 920 structs (≈ 7.5 KB). A class would allocate 1 920 heap objects plus reference overhead. |

**Optional `Style` parameter:** `CharStyle Style = CharStyle.Normal` was added as a default so that the common case — writing a character in the default style — requires only one argument:

```csharp
new VirtualCell('═')                     // CharStyle.Normal (default)
new VirtualCell('║', CharStyle.Dark)     // explicit style
```

This was added after the initial implementation following code review, and is backward-compatible.

---

## Decision 3 — Transparency via `'\0'`, not a separate flag

**Decision:** A cell is transparent when `Character == '\0'`. There is no separate `IsTransparent` boolean.

**Rationale:** A separate flag would increase the struct size and add a condition to every cell write. The `'\0'` convention is both compact and unambiguous: a real character value of `'\0'` is never intentionally rendered in a terminal UI. The Compositor's Recompose phase checks a single condition:

```csharp
if (cell.Character != '\0')
    compositeBuffer[row, col] = cell;
```

`VirtualCell.Empty` (`'\0'`, Normal) is the struct's default value, which means a freshly allocated `VirtualCell[,]` buffer is automatically filled with transparent cells — no explicit initialisation loop is needed in `Layer`'s constructor.

**`VirtualCell.Space` vs `VirtualCell.Empty`:** The two static fields serve different purposes:

| Field | Character | Meaning |
|-------|-----------|---------|
| `VirtualCell.Empty` | `'\0'` | Transparent — compositor reveals layer below |
| `VirtualCell.Space` | `' '` | Opaque space — paints the background colour at this cell |

The Compositor initialises its composite buffer to `Space` (not `Empty`) before each Recompose pass, so that any screen area not covered by a visible layer renders as a visible blank character rather than falling through to an undefined state.

---

## Decision 4 — `Layer` as a concrete class (no `ILayer`)

**Decision:** `Layer` is a `sealed class` with no accompanying `ILayer` interface.

**Rationale:** `Layer` is a **data container** — a managed buffer with bounds and Z-order metadata. It has no external dependencies and no platform-specific behaviour. Abstracting it via an interface would not enable substitution in any realistic scenario:

- **Tests** create real `Layer` instances via `FakeVirtualScreen.AddLayer()` and assert on actual cell values via `GetCell()`. A mock layer would provide *less* coverage, not more.
- **Platform ports** (MAUI, Blazor) do not need a different `Layer` implementation. The architecture document explicitly states that `Layer` lives in `Enclave.Common` and is shared across platforms. Only the Emit phase (`IPhosphorWriter`) is platform-specific.
- **Component rendering** (PHOSPHOR 2.1) will use `LayerWriter` as its abstraction boundary. `LayerWriter` holds a reference to a concrete `Layer` and translates relative coordinates; the test boundary will be at `LayerWriter`, not at `Layer`.

The distinction follows the pattern applied elsewhere in the solution: **services are abstracted via interfaces; data containers are not.**

---

## Decision 5 — `DirtyRegionTracker` is `internal`

**Decision:** `DirtyRegionTracker` is an `internal sealed class`, not a public API.

**Rationale:** `DirtyRegionTracker` is an implementation detail of `VirtualScreen`. Its entire API (`Invalidate`, `Flush`, `HasRegions`) is surfaced through `IVirtualScreen`, so there is no use case for consuming it directly from outside the Phosphor assembly.

Making it internal prevents callers from bypassing `VirtualScreen` and directly manipulating the dirty-region list — which could desynchronise the compositor.

**Testing:** Because `DirtyRegionTracker` is internal, unit tests cover it indirectly via `VirtualScreen`'s public API. This is intentional: the tests verify the *observable contract* (invalidate → flush returns expected regions) rather than the internal list structure.

**Merge heuristic:** When two dirty regions overlap, they are merged into their bounding union rather than stored separately. This trades a marginally larger recomposition area for fewer compositor passes. For terminal UIs with typically 2–5 active layers, this is always a net win.

---

## Decision 6 — `IPhosphorInputLoop` extended with `ReadKey` and `Dispatch`

**Decision:** Two new methods were added to `IPhosphorInputLoop`:

```csharp
ConsoleKeyInfo ReadKey(CancellationToken ct);
void Dispatch(ConsoleKeyInfo key);
```

**Rationale:** `PhosphorRenderLoop` needs fine-grained control over the read-dispatch-recompose cycle. The existing `Run()` method is a black box — it reads a key, dispatches it, and loops, with no hook for interleaving compositor work. Extracting `ReadKey` and `Dispatch` as primitives allows `PhosphorRenderLoop` to insert the recompose pass between dispatch and the next read:

```
ReadKey(ct) → Dispatch(key) → [compositor flush if dirty] → ReadKey(ct) → …
```

**Breaking change:** This is the second reason for the major version bump. Any existing implementation of `IPhosphorInputLoop` must add both methods. The production `PhosphorInputLoop` and test double `TestPhosphorInputLoop` were updated accordingly.

**Blocking limitation:** `PhosphorInputLoop.ReadKey` blocks until the underlying `Console.ReadKey()` returns, then checks the cancellation token. Full cancellation therefore requires a keypress to unblock. This is consistent with the existing `Run()` behaviour and is acceptable for an interactive terminal application.

---

## Decision 7 — `PhosphorRenderLoop` is event-driven, not timer-driven

**Decision:** The render loop blocks on keyboard input. There is no background polling timer.

**Rationale:** A fixed-rate timer (e.g. 60 fps) would:
- Introduce unnecessary CPU usage when the UI is idle.
- Require thread-safe access to every layer buffer on every tick.
- Produce no visual benefit in a text-mode UI — characters only change in response to user or system events.

The event-driven model instead triggers recomposition only when `IVirtualScreen.HasDirtyRegions` is true, which happens only when a component calls `Invalidate` after updating its layer.

**Toast / auto-dismiss exception:** Components that auto-dismiss on a timer (toast notifications) invalidate themselves from a `CancellationTokenSource` callback — they are the source of the invalidation, not the render loop:

```csharp
// Inside a toast component:
_cts.CancelAfter(displayDuration);
_cts.Token.Register(() => _screen.Invalidate(_bounds));
```

This keeps the render loop itself clean and polling-free.

---

## Decision 8 — `IPhosphorCanvas` marked `[Obsolete]`, not deleted

**Decision:** `IPhosphorCanvas` and `AnsiPhosphorCanvas` are decorated with `[Obsolete(..., error: false)]` rather than being removed.

**Rationale:** RAVEN currently uses `IPhosphorCanvas` for its sequential boot-sequence and phase rendering. Deleting the API would break RAVEN before it has been migrated to the new `IVirtualScreen`-based model. `[Obsolete]` with `error: false` means:
- Existing code compiles with a warning, not an error — no immediate breakage.
- The warning communicates that migration is expected.
- The API remains fully functional during the migration window.

Once RAVEN's phases are ported to use `IVirtualScreen`, the `[Obsolete]` types will be promoted to `error: true` and then removed in a subsequent major version.

---

## Decision 9 — Physical buffer initialised to `Space`

**Decision:** The Compositor's `_physicalBuffer` (the last-written state) is initialised to `VirtualCell.Space`, not `VirtualCell.Empty`.

**Rationale:** The Diff phase only emits cells where `compositeBuffer[r, c] != _physicalBuffer[r, c]`. If the physical buffer started as `Empty` (`'\0'`) and the first composite contained `Space` characters, every space would be emitted — correct, but for the wrong reason. Starting with `Space` means the first full flush emits all non-space cells, which is the minimal correct set for initialising a terminal that starts with a blank screen.

---

## Decision 10 — `CharStyle.Normal = 0` (enum reorder)

**Decision:** The `CharStyle` enum members are reordered so that `Normal` is the first (zero-valued) member:

```csharp
// Before                  // After
Background = 0             Normal     = 0   ← default
Dark       = 1             Background = 1
Normal     = 2             Dark       = 2
Bright     = 3             Bright     = 3
```

**Motivation — the `default(VirtualCell)` alignment problem:**

`VirtualCell` is a struct. When a `VirtualCell[,]` buffer is allocated, the CLR fills it with `default(VirtualCell)`, which zeroes all bytes. With the original enum order, this produces `('\0', CharStyle.Background)` — transparent (`'\0'`), but with an unexpected style value. `VirtualCell.Empty` was defined as `new('\0', CharStyle.Normal)`, so `default(VirtualCell) != VirtualCell.Empty` even though both are semantically transparent.

Reordering to `Normal = 0` makes `default(VirtualCell) == VirtualCell.Empty` exactly:

| | Before reorder | After reorder |
|--|--|--|
| `default(VirtualCell)` | `('\0', Background=0)` ≠ Empty | `('\0', Normal=0)` **= Empty** ✓ |
| Freshly allocated buffer contents | technically Background | `VirtualCell.Empty` |

**Observation — `Layer.Clear()` and the implicit dependency:**

Since `VirtualCell.Empty == default(VirtualCell)`, one might replace `MemoryMarshal.CreateSpan(...).Fill(VirtualCell.Empty)` with `Array.Clear(_buffer)` (which zeroes all bytes via a SIMD `memset(0)` path). This substitution is **explicitly rejected**.

`Array.Clear` is only correct here because `CharStyle.Normal == 0`. The numeric value `0` never appears in the source — the dependency is invisible. A future reader who reorders the enum (or adds a new zero-valued member) would not see `Layer.Clear` as a call site that needs updating. The bug would be silent and hard to diagnose.

`Layer.Clear()` therefore retains the explicit `.Fill(VirtualCell.Empty)` form:

```csharp
// Kept — fills by name, correct regardless of CharStyle's internal numeric values
public void Clear()
{
    if (_buffer.Length == 0) return;
    MemoryMarshal
        .CreateSpan(ref _buffer[0, 0], _buffer.Length)
        .Fill(VirtualCell.Empty);
}
```

The `Normal = 0` alignment is still valuable: it guarantees that a freshly allocated `VirtualCell[,]` (filled by the CLR with zero bytes) equals `VirtualCell.Empty` rather than an unexpected `('\0', CharStyle.Background)` value. That correctness benefit stands — it is the code that exploits it that must do so explicitly.

**Semantic justification:** `Normal` as the zero value is the most natural ordering. It is the "base state" of the system — the same way `false = 0` for `bool` or `default` for any nullable type. `Background`, `Dark`, and `Bright` are specialisations relative to `Normal`.

**Breaking change scope:** The numeric values of `Background`, `Dark`, and `Normal` change (`Bright = 3` is unaffected). This is only breaking if `CharStyle` is serialised as an integer. Inspection of the codebase confirms no such serialisation exists — `CharStyle` is used exclusively as a rendering hint at runtime, always referenced by name.

---

## Affected Files

### New files

| File | Description |
|------|-------------|
| `Common/Drawing/CharStyle.cs` | `CharStyle` enum, moved from `Enclave.Phosphor` |
| `Common/Drawing/VirtualCell.cs` | Virtual cell primitive |
| `Phosphor/_using.cs` | Project-level `global using Enclave.Common.Drawing` |
| `Phosphor/Layer.cs` | Rectangular virtual screen layer |
| `Phosphor/DirtyRegionTracker.cs` | Internal merge-and-flush dirty region tracker |
| `Phosphor/IVirtualScreen.cs` | Virtual screen interface |
| `Phosphor/VirtualScreen.cs` | Default virtual screen implementation |
| `Phosphor/IPhosphorCursor.cs` | Cursor positioning interface |
| `Phosphor/AnsiPhosphorCursor.cs` | ANSI CUP (`ESC[row;colH`) implementation |
| `Phosphor/Compositor.cs` | Recompose → Diff → Emit pipeline |
| `Phosphor/PhosphorRenderLoop.cs` | Event-driven render loop |
| `Phosphor.Tests/FakeVirtualScreen.cs` | Test double for `IVirtualScreen` |
| `Phosphor.Tests/DirtyRegionTrackerTests.cs` | Dirty region merge/flush tests |
| `Phosphor.Tests/CompositorTests.cs` | Recompose, diff, emit, transparency tests |

### Modified files

| File | Change |
|------|--------|
| `Phosphor/IPhosphorInputLoop.cs` | Added `ReadKey(ct)` and `Dispatch(key)` |
| `Phosphor/PhosphorInputLoop.cs` | Implemented new interface members |
| `Phosphor/IPhosphorCanvas.cs` | Added `[Obsolete]` |
| `Phosphor/AnsiPhosphorCanvas.cs` | Added `[Obsolete]` |
| `Phosphor.Tests/TestPhosphorInputLoop.cs` | Implemented new interface members |
| `Phosphor.Tests/_using.cs` | Added `global using Enclave.Common.Drawing` |

### Deleted files

| File | Reason |
|------|--------|
| `Phosphor/CharStyle.cs` | Type moved to `Common/Drawing/CharStyle.cs` |

---

[Magyar]: ./PHOSPHOR_2_VIRTUAL_SCREEN.hu.md
