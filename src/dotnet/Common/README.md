# Enclave.Common

Project-independent utilities and extensions shared across Enclave Terminal Breach. No dependency on Echelon/Core or platform code.

## Contents

| Folder / area   | Description |
|-----------------|-------------|
| **Assembly/**   | Types to read product name and version from an assembly (e.g. `IAssemblyProvider`, `AssemblyProvider`). |
| **Configuration/** | Configuration sources and extensions: embedded JSON resources, storage-based config. See [Configuration README][config-readme]. |
| **Drawing/**    | Immutable 2D geometry: `Point`, `Size`, `Rectangle` with operators and intersection/containment. |
| **Errors/**     | Error types used with FluentResults (e.g. `NotFoundError` for missing resources). |
| **Extensions/** | Static extension methods: `ResourceExtensions`, `EmbeddedResourceConfigurationExtensions`, `StorageConfigurationExtensions`. |
| **Models/**     | Shared value types (e.g. `ColorValue`). |
| **Services/**   | Abstractions (e.g. `IStorageService` for key-value string storage). |

## Main types

- **ResourceExtensions** – Load embedded resources from an assembly or type: `GetResourceStream`, `GetResourceString`, `GetJsonResource<T>`. Paths are normalized (e.g. `/` and `-` handled for manifest names).
- **NotFoundError** – FluentResults error for “resource not found”; carries optional messages (e.g. available manifest names).
- **ColorValue** – Platform-agnostic immutable colour (R, G, B, A) with `FromHex`, `ToHex`, `ToCssRgba`, `ToCssRgb`.
- **Point**, **Size**, **Rectangle** – Readonly record structs in `Enclave.Common.Drawing`; operators (Point±Point, Point±Size, Size±Size, scalar multiply); rectangle supports `Contains`, `Intersect`, `Union`, `Offset`, `Inflate`, `IntersectsWith`; `Dimension` for size.
- **EmbeddedResourceConfigurationExtensions** – `AddEmbeddedJsonFile()` to load JSON configuration from embedded resources.
- **StorageConfigurationExtensions** – `AddStorageConfiguration()` to load configuration from `IStorageService` with a key prefix.
- **IAssemblyProvider** / **AssemblyProvider** – Product and version from assembly; mockable for tests.
- **IStorageService** – Async key-value string storage interface used by storage configuration.

## Dependencies

- **FluentResults** – `Result<T>` return types for extensions that can fail (e.g. missing resource).
- **Microsoft.Extensions.Configuration** – Configuration builder and provider model for embedded and storage-based configuration.

## Tests

Unit tests: **Enclave.Common.Tests** (under `src/dotnet/tests/Unit/`), including `ResourceExtensionsTests`, `ColorValueTests`, `AssemblyProviderTests`, `EmbeddedResourceConfigurationTests`, `StorageConfigurationTests`, `PointTests`, `SizeTests`, `RectangleTests`.

## See also

- [Source root README][source-readme] – folder structure and solution
- [Central CHANGELOG][central-changelog] – project-wide releases

[//]: #References
[config-readme]: Configuration/README.md
[source-readme]: ../README.md
[central-changelog]: ../../CHANGELOG.md
