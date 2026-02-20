# Enclave.Common

Project-independent utilities and extensions shared across Enclave Terminal Breach. No dependency on Echelon/Core or platform code.

## Contents

| Folder / area | Description |
|---------------|-------------|
| **Extensions/** | Static extension methods (e.g. `ResourceExtensions` for loading embedded resources). |
| **Errors/** | Error types used with FluentResults (e.g. `NotFoundError` for missing resources). |

## Main types

- **ResourceExtensions** – Load embedded resources from an assembly or type: `GetResourceStream`, `GetResourceString`, `GetJsonResource<T>`. Paths are normalized (e.g. `/` and `-` handled for manifest names). Used by Core for the word list and by tests.
- **NotFoundError** – FluentResults error for “resource not found”; carries optional messages (e.g. available manifest names).

## Dependencies

- **FluentResults** – `Result<T>` return types for extensions that can fail (e.g. missing resource).

## Tests

Unit tests: **Enclave.Echelon.Common.Tests** (under `src/tests/Unit/`), including `ResourceExtensionsTests`.

## See also

- [Source root README](../README.md) – folder structure and solution
- [Central CHANGELOG](../../CHANGELOG.md) – project-wide releases
