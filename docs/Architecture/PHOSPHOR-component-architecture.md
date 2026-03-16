# PHOSPHOR Component Architecture

**English** | [Magyar]

## Overview

This document defines the component model for PHOSPHOR-based terminal UI rendering.
It introduces a nested component tree with relative coordinates, bounded rendering, and an MVVM-aligned orchestration model for stateful and animated UI flows.

This architecture is intended to support the RAVEN and later ECHELON/GHOST UI evolution while keeping the low-level rendering primitive (`VirtualScreen` + `Layer`) stable.

## Goals

- Provide reusable, higher-level UI components (for example bordered boxes and process rows).
- Support nested components with relative positioning.
- Enforce clipping so child rendering cannot write outside component bounds.
- Separate rendering from state and timing logic.
- Keep animation orchestration deterministic and testable.

## Non-Goals

- Replacing PHOSPHOR core rendering primitives.
- Introducing a retained-mode visual framework comparable to WPF/MAUI controls.
- Introducing mouse-driven layout or event routing in this milestone.

## Component Model

PHOSPHOR distinguishes two component categories.

### Layer Components

`LayerComponent` instances own a dedicated `Layer`.
They are used for window-like, z-ordered surfaces (for example popup windows, overlays, toast regions, or movable panels).

Characteristics:

- Absolute placement on screen.
- Explicit z-order management.
- Independent invalidation region.

### Content Components

`ContentComponent` instances render into the parent writer context and do not allocate a dedicated `Layer`.
They are intended for structural and textual content (for example bordered text blocks, process rows, labels, progress rows).

Characteristics:

- Relative coordinates within parent bounds.
- Zero additional layer overhead.
- Fully nestable.

## Relative Rendering with LayerWriter

`LayerWriter` is a clipped coordinate translator over a target `Layer`.

Responsibilities:

- Translate local component coordinates to absolute screen coordinates.
- Clip all writes to the current component bounds.
- Create child writer scopes through `Clip(relativeBounds)`.

Implication:

- A component always renders as if its top-left origin were `(0,0)`.
- Parent components can safely compose children without manual absolute coordinate calculations.

## Component Tree and Rendering Contract

The rendering contract is intentionally minimal.

```csharp
public interface IComponent
{
    Rectangle Bounds { get; }
    void Render(LayerWriter writer);
}
```

Container components render children by deriving a child writer from the parent writer with clipping.

```csharp
foreach (var child in Children)
{
    var childWriter = writer.Clip(child.Bounds);
    child.Render(childWriter);
}
```

## Lifecycle and State

Component instances may be:

- Stateless (pure projection from current input values).
- Stateful (for example progressive dot rendering in a process row).

Stateful components do not own asynchronous timing loops. They only expose deterministic state-transition methods (for example `Advance()`, `Complete()`).

## MVVM Introduction for PHOSPHOR

The component architecture is paired with MVVM to keep rendering, state, and workflow orchestration separate.

### Responsibility Matrix

| Layer | Responsibility | Must Not Do |
|------|----------------|-------------|
| Model | Domain data and rules (candidate words, likeness calculations, boot metadata) | UI timing, direct rendering |
| ViewModel | State transitions, sequencing, timing, command handling, invalidation triggers | Direct low-level terminal writes |
| View (Components) | Rendering of current state to writer context | Async delays, business decisions |

### Why MVVM Here

- Boot sequence effects are timing-sensitive and should remain unit-testable.
- Components remain portable between console and future frontends.
- Render logic can be snapshot-tested separately from orchestration logic.

## Invalidation and Render Loop

`ViewModel` drives updates by:

1. Mutating state on one or more components.
2. Requesting invalidation for affected bounds.
3. Letting the render loop redraw the invalid region.

Rules:

- Components must not call `Task.Delay` internally.
- Components should avoid direct access to global screen services except through well-defined invalidation contracts.
- All animation pacing belongs to `ViewModel`.

## Boot Sequence Mapping (Example)

The boot sequence from [ECHELON Boot Sequence][boot-sequence] maps naturally to this model.

- `BorderedBoxComponent`: static framed header blocks.
- `ProcessBarComponent`: label + progressive dot sequence + terminal status token (`OK`).
- Future `ProgressBarComponent`: percentage and/or fill visualization for longer operations.

`BootScreenViewModel` orchestrates phase order and timing, while components only render the current state snapshot.

## Initial Implementation Milestones

### Milestone 1

- Implement `LayerWriter` with clipping and coordinate translation.
- Implement `BorderedBoxComponent`.
- Add focused unit tests for clipping and frame rendering.

### Milestone 2

- Implement `ProcessBarComponent` state transitions.
- Introduce `BootScreenViewModel` orchestration over process rows.
- Add deterministic timing abstraction for tests.

### Milestone 3

- Introduce `LayerComponent` use cases (popup/overlay surfaces).
- Add invalidation aggregation for multiple component updates.

## Compatibility and Evolution

This model keeps PHOSPHOR 1.0 compatible while enabling progressive enhancements in PHOSPHOR 1.1+ (window system, additional widgets, richer composition).

The same conceptual tree can later be mapped to Blazor and MAUI frontends without reusing terminal-specific rendering code.

[//]: #References
[Magyar]: ./PHOSPHOR-component-architecture.hu.md
[boot-sequence]: ../Design/ECHELON_Boot_Sequence.md