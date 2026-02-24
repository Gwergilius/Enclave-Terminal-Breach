# Enclave.Raven

RAVEN – DOS-style console proof-of-concept for Enclave Terminal Breach. PHOSPHOR 1.0 canvas, colour themes, typewriter-style output (PhosphorTypewriter), sequential stdin/stdout. Active development for 1.4.x / 2.0.x.

## Contents

| Folder / area | Description |
|---------------|-------------|
| **Configuration/** | `RavenOptions`, `RavenIntelligence`, `RavenStartupOptions` – bind from `System` section; `TimingOptions` (implements `ITimingOptions`) from `Platform:Timing` for typewriter delays. Solver level mapping. |
| **Phases/** | Phase interfaces and implementations: `IStartupBadgePhase`, `IResetScopePhase`, `IDataInputPhase`, `IHackingLoopPhase`, `IPlayAgainPhase`; `StartupBadgePhase`, `ResetScopePhase`, `DataInputPhase`, `HackingLoopPhase`, `PlayAgainPhase`, `CandidateListFormatter`. |
| **Services/** | `IPhaseRegistry`, `PhaseRegistry` (scoped; receives `IEnumerable<IPhase>` from DI; `GetPhase(name)` returns `Result<IPhase>`); `ICurrentScope`, `CurrentScopeHolder`; `INavigationService`, `NavigationService`; `IExitRequest`, `ExitRequest`; `IProductInfo` (see ProductInfo). |
| **IO/** | `ConsoleKeyboardHandler` – `IPhosphorReader` implementation delegating to `IConsoleIO`. |
| **Application** | Main loop: initializes canvas, resolves phases from `ICurrentScope` + `IPhaseRegistry`, runs until `NavigateTo("Exit")`. |
| **Startup** | `ConfigureServices` – DI registration (scoped phases and PhaseRegistry, singleton navigation/scope/canvas, IProductInfo, TimingOptions, PhosphorTypewriter as IPhosphorWriter, etc.). |
| **ProductInfo / IProductInfo** | Product name and version; `ProductInfo.GetCurrent()`; registered as singleton in DI; used by Application and StartupBadgePhase. |

## Dependencies

- **Enclave.Common** – FluentResults, configuration, drawing (e.g. Rectangle).
- **Enclave.Echelon.Core** – Password solver, domain models.
- **Enclave.Phosphor** (≥ 1.1.0) – `IPhosphorCanvas`, `IPhosphorWriter` (RAVEN uses PhosphorTypewriter), `IPhosphorReader`, `ITimingOptions`, themes.
- **Enclave.Shared** (≥ 1.0.0) – `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader`; `IGameSession`, `GameSession`; `IPhase`, `INavigationService`, `ApplicationExit`; shared phases and models. RAVEN requires Shared 1.0.0 or later.

## Tests

- **Enclave.Raven.Tests** – Unit tests for Startup, Application (orchestration), phases (DataInput, StartupBadge, HackingLoop, etc.), **CurrentScopeHolder**, **ExitRequest**, **NavigationService**, **PhaseRegistry**, CandidateListFormatter, RavenIntelligence, ProductInfo, TimingOptions. IO and Models tests in **Enclave.Shared.Tests** (including **ApplicationExit**). **Enclave.Common.Tests** – Rectangle, Waiter, TimeSpanExtensions. **Enclave.Phosphor.Tests** – AnsiPhosphorCanvas, PhosphorTypewriter.

## See also

- [Source root README](../README.md) – folder structure and solution
- [Phase Navigation State Machine](../../../docs/Architecture/Phase-Navigation-State-Machine.md) – runner loop, IPhaseRegistry, scoped resolution
- [RAVEN Requirements](../../../docs/Architecture/RAVEN-Requirements.md) – long-term goals
- [RAVEN 2.0.0 Specification](../../../docs/Architecture/RAVEN-2.0.0-Specification.md) – current target spec
