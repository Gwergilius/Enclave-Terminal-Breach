# Enclave.Shared

Shared abstractions and implementations for Enclave projects: IO, Models, Errors, and future UI types. Used by SPARROW and RAVEN console apps.

## Contents

| Folder / area | Description |
|---------------|-------------|
| **IO/** | Console input/output: `IConsoleIO`, `ConsoleIO` (stdin/stdout), `ConsoleIntReader` (validated integer prompt). PHOSPHOR-aligned (no cursor, no colour). |
| **Models/** | `IGameSession`, `GameSession` – shared game state between data-input and hacking phases; candidate list and word length. |
| **Errors/** | `DuplicatedPassword` – FluentResults error when a candidate is already in the list. |

## Dependencies

- **Enclave.Common** – `NotFoundError`, configuration.
- **Enclave.Echelon.Core** – `Password`, `InvalidPassword`, `ISolverConfiguration`-related types.
- **FluentResults** – `Result`, `Error`.

## Tests

- **Enclave.Shared.Tests** – Unit tests for `ConsoleIntReader` (via `TestConsoleIO`), `GameSession`. IO and Models tests shared across SPARROW and RAVEN.

## See also

- [Source root README](../README.md) – folder structure and solution
- [PHOSPHOR-Requirements](../../docs/Architecture/PHOSPHOR-Requirements.md) – console UI abstraction (Enclave.Shared IO aligns with PHOSPHOR 1.0)
