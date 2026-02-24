# Project Structure & Layer Architecture

**English** | [Magyar]

## Overview

The ECHELON solution is organised into four reusable library layers plus the platform-specific application projects. This document defines the responsibility boundary of each project, the dependency rules between them, and the rationale for separating them.

---

## Layer Definitions

### `Enclave.Common` — Global utilities

**Reusability target:** Any .NET project in any solution.

`Enclave.Common` contains infrastructure and utility code that has **no knowledge of the ECHELON domain**. Components here could be extracted into a NuGet package and used by a completely different product without modification.

**Canonical contents:**
- Configuration infrastructure (`EmbeddedJsonResourceConfigurationProvider`, `StorageConfigurationExtensions`, `StorageConfigurationSource`, `StorageConfigurationProvider`)
- Resource helpers (`ResourceExtensions`, `EmbeddedResourceConfigurationExtensions`)
- Generic FluentResults error types that have no domain meaning (`NotFoundError`)
- Service abstractions that are universally useful (`IStorageService`)

**Dependency rule:** References only the BCL and approved third-party infrastructure packages (`FluentResults`, `Microsoft.Extensions.*`). **Never** references Core, Shared, or any application project.

---

### `Enclave.Echelon.Core` — Business logic kernel

**Reusability target:** Every ECHELON UI (SPARROW, RAVEN, GHOST/Blazor, MAUI).

`Enclave.Echelon.Core` contains the **domain model and algorithm implementations**. It is the reason the application exists. Any UI that wants to solve the terminal hacking minigame must reference Core.

**Canonical contents:**
- `Password` model and `PasswordValidator`
- `IPasswordSolver` and implementations (`HouseGambitPasswordSolver`, `BestBucketPasswordSolver`, `TieBreakerPasswordSolver`)
- `ISolverFactory` / `SolverFactory`, `ISolverConfiguration`, `SolverLevel`
- `IRandom` / `GameRandom`
- Domain errors (`InvalidPassword`)
- Embedded `words.txt` resource

**Dependency rule:** References `Enclave.Common` only. **Never** references Shared, PHOSPHOR, or any application project.

---

### `Enclave.Echelon.Shared` — Cross-product application layer *(new)*

**Reusability target:** All ECHELON products within this solution.

`Enclave.Echelon.Shared` contains code that is **not part of the business logic** (i.e. not in Core) but is **used by more than one product** (SPARROW, RAVEN, GHOST, MAUI). The distinction from Common is that Shared *does* understand the ECHELON domain; it just is not the algorithm itself.

**Canonical contents:**

| Component | Current location | Notes |
|-----------|-----------------|-------|
| `IGameSession` / `GameSession` | `Enclave.Sparrow`, `Enclave.Raven` (duplicated) | Application-level session model; wraps Core `Password` objects |
| `ProductInfo` | `Enclave.Sparrow`, `Enclave.Raven` (duplicated) | Reads product name/version from assembly attributes |
| `IPhase` | `Enclave.Sparrow`, `Enclave.Raven` (duplicated) | Phase execution contract |
| `IPhaseRunner` / `PhaseRunner` | `Enclave.Sparrow`, `Enclave.Raven` (duplicated) | Runs phases in order via `IServiceScopeFactory` |
| `CandidateListFormatter` | `Enclave.Sparrow`, `Enclave.Raven` (duplicated) | Multi-column alphabetical candidate list formatter |
| `DuplicatedPassword` error | `Enclave.Sparrow`, `Enclave.Raven` (duplicated) | FluentResults error with domain meaning |
| `IConsoleIO` / `ConsoleIO` / `ConsoleIntReader` | `Enclave.Sparrow`, `Enclave.Raven` (duplicated) | Sequential stdin/stdout abstraction (SPARROW runtime; RAVEN legacy/test shim) |

> **Naming note:** The namespace is `Enclave.Echelon.Shared`; the assembly/project is named `Enclave.Echelon.Shared` (`Enclave.Echelon.Shared.csproj`). This keeps the `Enclave.Echelon.*` naming convention consistent with Core.

**Dependency rule:** References `Enclave.Common` and `Enclave.Echelon.Core`. **Never** references PHOSPHOR or any application project.

---

### `Enclave.Phosphor` — Console UI abstraction layer *(new)*

**Reusability target:** All ECHELON console-platform products (RAVEN and any future console variant).

`Enclave.Phosphor` contains the full-screen ANSI terminal rendering engine described in [PHOSPHOR-Requirements.md][PHOSPHOR-Req]. It is extracted into its own project because:

1. It is a **significant, versioned subsystem** (PHOSPHOR 1.0 / 1.1 / 1.2) with its own evolution path.
2. It has **no dependency** on RAVEN-specific configuration or phases — only on colour model types from Shared.
3. It can be reused by any future console-platform application without pulling in RAVEN's application logic.
4. It enables **isolated testing** of the rendering engine without the application layer.

**Canonical contents:**

```
Enclave.Phosphor/
├── Models/
│   ├── CharStyle.cs
│   ├── PhosphorTheme.cs
│   └── KeyboardEvent.cs
├── Themes/
│   ├── IPhosphorThemeProvider.cs
│   └── PhosphorThemeFactory.cs
├── Canvas/
│   ├── IPhosphorCanvas.cs
│   ├── IPhosphorWriter.cs
│   └── AnsiPhosphorCanvas.cs
└── Input/
    ├── IPhosphorKeyboardHandler.cs
    ├── IPhosphorInputLoop.cs
    ├── IPhosphorInputBuffer.cs
    └── PhosphorInputLoop.cs
```

**Dependency rule:** References `Enclave.Common` only (for ColorValue). Never references Core, Shared, or any application project.

> **`ColorValue` placement:** `ColorValue` is a platform-agnostic R/G/B/A record used by the colour palette. It belongs in `Enclave.Common` (platform-neutral, domain-free value type) rather than Shared or PHOSPHOR, so it can also be used by GHOST (Blazor) and MAUI without a PHOSPHOR dependency.

---

## Solution Dependency Graph

```
┌────────────────────────────────────────────────────────────────────┐
│                        Application layer                           │
│                                                                    │
│  Enclave.Sparrow    Enclave.Raven    Enclave.Ghost   Enclave.MAUI  │
│       (1.x)            (2.x)          (Blazor)       (Android)     │
└──────┬───────────────────┬───────────────┬──────────────┬──────────┘
       │                   │               │              │
       │         ┌─────────┘               │              │
       │         │  Enclave.Phosphor       │              │
       │         │  (console UI layer)     │              │
       │         └─────────────────────────┘─ ─ ─(future)─┘
       │                   │
       └────────┬──────────┘
                │
       ┌────────▼───────────────────────────┐
       │        Enclave.Echelon.Shared      │
       │  (cross-product application layer) │
       └────────┬───────────────────────────┘
                │
       ┌────────▼───────────────────────────┐
       │        Enclave.Echelon.Core        │
       │        (business logic / solver)   │
       └────────┬───────────────────────────┘
                │
       ┌────────▼───────────────────────────┐
       │          Enclave.Common            │
       │   (global utilities / infra)       │
       └────────────────────────────────────┘
```

Arrows point **upward** (higher layer depends on lower layer). No layer may reference anything above it.

---

## Filesystem Layout (target state)

```
src/dotnet/
├── Common/                         Enclave.Common
├── Core/                           Enclave.Echelon.Core
├── Shared/                         Enclave.Echelon.Shared           ← new
├── Phosphor/                       Enclave.Phosphor                 ← new
├── Enclave.Sparrow/
└── Enclave.Raven/

tests/
├── Common.Test.Core/               Shared test infrastructure
└── Unit/
    ├── Enclave.Echelon.Common.Tests/
    ├── Enclave.Echelon.Core.Tests/
    ├── Enclave.Echelon.Shared.Tests/    ← new
    ├── Enclave.Phosphor.Tests/          ← new
    ├── Enclave.Sparrow.Tests/
    └── Enclave.Raven.Tests/
```

---

## Migration: what moves from Sparrow/Raven into Shared

> **Context:** A két projekt közötti kódátfedés **szándékos bootstrap**: a Raven a Sparrow kódbázisát vette kiindulási alapnak, és a két projekt jelenleg tudatosan párhuzamosan él. A konszolidáció az `Enclave.Echelon.Shared`-be tervezett architektúrális lépés, nem hibajavítás. A természetes trigger az, amikor egy harmadik fogyasztó (GHOST/Blazor vagy MAUI) szintén igényelné ugyanezeket az osztályokat.
>
> **Context (EN):** The code overlap between `Enclave.Sparrow` and `Enclave.Raven` is **intentional bootstrapping** — Raven was started from Sparrow's codebase as a working foundation. The two projects are currently maintained in parallel by design. Consolidation into `Enclave.Echelon.Shared` is a planned architectural step, not a bug fix. The natural trigger is when a third consumer (GHOST/Blazor or MAUI) would need the same code, making the duplication no longer sustainable.

The following files currently exist verbatim (or near-verbatim) in **both** `Enclave.Sparrow` and `Enclave.Raven`. They are candidates for migration to `Enclave.Echelon.Shared`.

| File(s) | Current namespace | New namespace |
|---------|------------------|---------------|
| `Models/IGameSession.cs` | `Enclave.Sparrow.Models` / `Enclave.Raven.Models` | `Enclave.Echelon.Shared.Models` |
| `Models/GameSession.cs` | same | same |
| `ProductInfo.cs` | `Enclave.Sparrow` / `Enclave.Raven` | `Enclave.Echelon.Shared` |
| `Phases/IPhase.cs` | `Enclave.Sparrow.Phases` / `Enclave.Raven.Phases` | `Enclave.Echelon.Shared.Phases` |
| `Services/IPhaseRunner.cs` | `Enclave.Sparrow.Services` / `Enclave.Raven.Services` | `Enclave.Echelon.Shared.Services` |
| `Services/PhaseRunner.cs` | same | same |
| `Phases/CandidateListFormatter.cs` | same | same |
| `Errors/DuplicatedPassword.cs` | `Enclave.Sparrow.Errors` / `Enclave.Raven.Errors` | `Enclave.Echelon.Shared.Errors` |
| `IO/IConsoleIO.cs` | `Enclave.Sparrow.IO` / `Enclave.Raven.IO` | `Enclave.Echelon.Shared.IO` |
| `IO/ConsoleIO.cs` | same | same |
| `IO/ConsoleIntReader.cs` | same | same |

After migration, both `Enclave.Sparrow.csproj` and `Enclave.Raven.csproj` add a reference to `Enclave.Echelon.Shared`. The `Configuration/` files (`*Options.cs`, `*Intelligence.cs`) **remain in their respective application projects** — they are product-specific and intentionally not shared.

> **Test impact:** `TestConsoleIO.cs` currently lives in both `Enclave.Sparrow.Tests` and `Enclave.Raven.Tests`. After migration it belongs in `Enclave.Echelon.Shared.Tests` (or `Common.Test.Core`) so both test projects can reference it.

---

## Project Reference Matrix (target state)

| Project | Common | Core | Shared | Phosphor |
|---------|:------:|:----:|:------:|:--------:|
| `Enclave.Common` | — | — | — | — |
| `Enclave.Echelon.Core` | ✓ | — | — | — |
| `Enclave.Echelon.Shared` | ✓ | ✓ | — | — |
| `Enclave.Phosphor` | ✓ | — | — | — |
| `Enclave.Sparrow` | ✓ | ✓ | ✓ | — |
| `Enclave.Raven` | ✓ | ✓ | ✓ | ✓ |
| `Enclave.Ghost` *(future)* | ✓ | ✓ | ✓ | — |
| `Enclave.MAUI` *(future)* | ✓ | ✓ | ✓ | — |

**Shared version:** Enclave.Sparrow and Enclave.Raven require **Enclave.Shared 1.0.0 or later** (breaking change at Shared 1.0.0). See [Enclave.Shared README](../../src/dotnet/Shared/README.md#version-compatibility) and [Shared CHANGELOG](../../src/dotnet/Shared/CHANGELOG.md).

---

## Decision log

### Why not put `GameSession` in Core?

`GameSession` is a stateful application-level session container. Core is stateless algorithm logic. Keeping them separate ensures Core stays free of session lifecycle concerns and remains purely functional and easily testable.

### Why not put `IPhase` / `PhaseRunner` in Common?

`IPhase` has no domain dependency but is conceptually tied to the ECHELON application lifecycle. Common is meant to be solution-agnostic. Placing infrastructure that is only meaningful within ECHELON in Shared keeps the Common boundary clean.

### Why is `Enclave.Phosphor` not just a folder in `Enclave.Raven`?

PHOSPHOR is a versioned subsystem (1.0 → 1.1 → 1.2) with its own test suite and an evolution path independent of RAVEN releases. Keeping it in a separate project enables isolated testing, clear versioning, and potential reuse by other console-based applications in the future without pulling in RAVEN's application logic.

### Why does `Enclave.Phosphor` not reference Core?

PHOSPHOR is a rendering engine. It knows nothing about passwords, solvers, or game sessions. Removing the Core dependency keeps PHOSPHOR reusable for any console application, not just ECHELON.

### Where does `ColorValue` live?

`ColorValue` is a platform-neutral R/G/B/A record. It belongs in `Enclave.Common` so that GHOST (Blazor) and MAUI can use the same palette definitions without taking a dependency on PHOSPHOR (which is a console-only rendering engine).

---

## References

[PHOSPHOR-Req]: ../Design/PHOSPHOR-Requirements.md
[RAVEN-Req]: ./RAVEN-Requirements.md
[Magyar]: ./ProjectStructure.hu.md
