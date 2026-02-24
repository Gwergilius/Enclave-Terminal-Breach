# Enclave.Common Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable changes to the Enclave.Common project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

## [1.3.0] - 2026-02-23

### Added
- **Drawing** – `Point`, `Size`, `Rectangle` as readonly record structs in `Enclave.Common.Drawing`: immutable 2D geometry with operators (Point±Point, Point±Size, Size±Size, scalar multiply), `DistanceTo`/`AsSize`/`AsPoint`, direction constants (`ToUp`, `ToDown`, `ToLeft`, `ToRight`). Rectangle: `Contains`, `Intersect`, `Union`, `Offset`, `Inflate`, `IntersectsWith`; `Dimension` property; negative width/height clamped to zero. Unit tests: `PointTests`, `SizeTests`, `RectangleTests`.

## [1.2.0] - 2026-02-21

### Added
- **ColorValue** – Platform-agnostic immutable colour record (R, G, B, A) with `FromHex`, `ToHex`, `ToCssRgba`, `ToCssRgb` for cross-platform use (MAUI, Blazor, console).
- **Configuration** – `EmbeddedResourceConfigurationExtensions.AddEmbeddedJsonFile()` for loading JSON config from embedded resources; `StorageConfigurationExtensions.AddStorageConfiguration()` and `StorageConfigurationProvider` for loading config from `IStorageService` with key prefix.
- **Assembly** – `IAssemblyProvider` and `AssemblyProvider` for product name and version from assembly attributes (testable/mockable).
- **IStorageService** – Interface for key-value string storage (used by storage-based configuration).

## [1.1.0] - 2026-02-14

### Added
- **ResourceExtensions** – `GetResourceStream`, `GetResourceString`, `GetJsonResource<T>` for loading embedded resources from `Assembly` or `Type`; path normalization for manifest names (`/`, `\`, `-`).
- **NotFoundError** – FluentResults error type for resource-not-found failures (e.g. with list of available manifest names).
