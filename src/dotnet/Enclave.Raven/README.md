# Enclave.Raven

RAVEN – DOS-style console proof-of-concept for Enclave Terminal Breach. Sequential stdin/stdout only; no cursor positioning or colour. Active development for 1.3.x / 2.0.x.

## Contents

| Folder / area | Description |
|---------------|-------------|
| **Configuration/** | `RavenOptions`, `RavenIntelligence`, `RavenStartupOptions` – config binding and solver level mapping. |
| **Phases/** | Phase interfaces and implementations: `IStartupBadgePhase`, `IDataInputPhase`, `IHackingLoopPhase`; `StartupBadgePhase`, `DataInputPhase`, `HackingLoopPhase`, `CandidateListFormatter`. |

## Dependencies

- **Enclave.Common** – FluentResults, configuration.
- **Enclave.Echelon.Core** – Password solver, domain models.
- **Enclave.Shared** – `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader`; `IGameSession`, `GameSession`; `DuplicatedPassword`, `IPhaseRunner`, `PhaseRunner`.

## Tests

- **Enclave.Raven.Tests** – Unit tests for Startup, phases (DataInput, StartupBadge, HackingLoop), PhaseRunner, CandidateListFormatter, RavenIntelligence, ProductInfo. IO and Models tests in **Enclave.Shared.Tests**.

## See also

- [Source root README](../README.md) – folder structure and solution
- [RAVEN-Requirements](../../docs/Architecture/RAVEN-Requirements.md) – long-term goals
- [RAVEN-2.0.0-Specification](../../docs/Architecture/RAVEN-2.0.0-Specification.md) – current target spec
