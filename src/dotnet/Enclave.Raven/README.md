# Enclave.Raven

RAVEN – DOS-style console proof-of-concept for Enclave Terminal Breach. PHOSPHOR 1.0 canvas, colour themes, sequential stdin/stdout. Active development for 1.3.x / 2.0.x.

## Contents

| Folder / area | Description |
|---------------|-------------|
| **Configuration/** | `RavenOptions`, `RavenIntelligence`, `RavenStartupOptions` – config binding and solver level mapping. |
| **Phases/** | Phase interfaces and implementations: `IStartupBadgePhase`, `IResetScopePhase`, `IDataInputPhase`, `IHackingLoopPhase`, `IPlayAgainPhase`; `StartupBadgePhase`, `ResetScopePhase`, `DataInputPhase`, `HackingLoopPhase`, `PlayAgainPhase`, `CandidateListFormatter`. |
| **Services/** | `IPhaseRegistry`, `PhaseRegistry` (scoped; receives `IEnumerable<IPhase>` from DI; `GetPhase(name)` returns `Result<IPhase>`); `ICurrentScope`, `CurrentScopeHolder`; `INavigationService`, `NavigationService`; `IExitRequest`, `ExitRequest`; `IProductInfo` (see ProductInfo). |
| **IO/** | `ConsoleKeyboardHandler` – `IPhosphorReader` implementation delegating to `IConsoleIO`. |
| **Application** | Main loop: initializes canvas, resolves phases from `ICurrentScope` + `IPhaseRegistry`, runs until `NavigateTo("Exit")`. |
| **Startup** | `ConfigureServices` – DI registration (scoped phases and PhaseRegistry, singleton navigation/scope/canvas, IProductInfo, etc.). |
| **ProductInfo / IProductInfo** | Product name and version; `ProductInfo.GetCurrent()`; registered as singleton in DI; used by Application and StartupBadgePhase. |

## Dependencies

- **Enclave.Common** – FluentResults, configuration, drawing (e.g. Rectangle).
- **Enclave.Echelon.Core** – Password solver, domain models.
- **Enclave.Phosphor** – `IPhosphorCanvas`, `IPhosphorWriter`, `IPhosphorReader`, themes.
- **Enclave.Shared** (≥ 1.0.0) – `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader`; `IGameSession`, `GameSession`; `IPhase`, `INavigationService`, `ApplicationExit`; shared phases and models. RAVEN requires Shared 1.0.0 or later.

## Tests

- **Enclave.Raven.Tests** – Unit tests for Startup, Application (orchestration), phases (DataInput, StartupBadge, HackingLoop, etc.), **CurrentScopeHolder**, **ExitRequest**, **NavigationService**, **PhaseRegistry**, CandidateListFormatter, RavenIntelligence, ProductInfo. IO and Models tests in **Enclave.Shared.Tests** (including **ApplicationExit**). **Enclave.Common.Tests** – Rectangle (including operator Point+Rectangle). **Enclave.Phosphor.Tests** – AnsiPhosphorCanvas (including Style setter with invalid enum value).

## See also

- [Source root README](../README.md) – folder structure and solution
- [Phase Navigation State Machine](../../../docs/Architecture/Phase-Navigation-State-Machine.md) – runner loop, IPhaseRegistry, scoped resolution
- [RAVEN Requirements](../../../docs/Architecture/RAVEN-Requirements.md) – long-term goals
- [RAVEN 2.0.0 Specification](../../../docs/Architecture/RAVEN-2.0.0-Specification.md) – current target spec
