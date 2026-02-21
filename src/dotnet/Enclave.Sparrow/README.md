# Enclave.Sparrow

SPARROW – DOS-style console proof-of-concept for Enclave Terminal Breach. Sequential stdin/stdout only; no cursor positioning or colour. *Code freeze – use Enclave.Raven for new development.*

## Contents

| Folder / area | Description |
|---------------|-------------|
| **Configuration/** | `SparrowOptions`, `SparrowIntelligence`, `SparrowStartupOptions` – config binding and solver level mapping. |
| **Phases/** | Phase interfaces and implementations: `IStartupBadgePhase`, `IDataInputPhase`, `IHackingLoopPhase`; `StartupBadgePhase`, `DataInputPhase`, `HackingLoopPhase`, `CandidateListFormatter`. |

## Dependencies

- **Enclave.Common** – FluentResults, configuration.
- **Enclave.Echelon.Core** – Password solver, domain models.
- **Enclave.Shared** – `IConsoleIO`, `ConsoleIO`, `ConsoleIntReader`; `IGameSession`, `GameSession`; `DuplicatedPassword`, `IPhaseRunner`, `PhaseRunner`.

## Tests

- **Enclave.Sparrow.Tests** – Unit tests for Startup, phases (DataInput, StartupBadge, HackingLoop), PhaseRunner, CandidateListFormatter, SparrowIntelligence, ProductInfo. IO and Models tests moved to **Enclave.Shared.Tests**.

## See also

- [Source root README] – folder structure and solution
- [SPARROW-Requirements] - direct goals motivating SPARROW versions
- [RAVEN-Requirements] – long-term goals (RAVEN extends SPARROW)

[Source root README]: ../README.md
[RAVEN-Requirements]: ../../../docs/Architecture/RAVEN-Requirements.md
[SPARROW-Requirements]: ../../../docs/Architecture/SPARROW-Requirements.md