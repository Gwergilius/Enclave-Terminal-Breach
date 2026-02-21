# Enclave.Common Changelog

[Keep a Changelog]: https://keepachangelog.com/en/1.0.0/
[Semantic Versioning]: https://semver.org/spec/v2.0.0.html

All notable changes to the Enclave.Common project.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

## [Unreleased]

### Added
- **ColorValue** – Platform-agnostic immutable colour record (R, G, B, A) with `FromHex`, `ToHex`, `ToCssRgba`, `ToCssRgb` for cross-platform use (MAUI, Blazor, console).

## [1.1.0] - 2026-02-14

### Added
- **ResourceExtensions** – `GetResourceStream`, `GetResourceString`, `GetJsonResource<T>` for loading embedded resources from `Assembly` or `Type`; path normalization for manifest names (`/`, `\`, `-`).
- **NotFoundError** – FluentResults error type for resource-not-found failures (e.g. with list of available manifest names).
