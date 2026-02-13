# Enclave.Common Changelog

All notable changes to the Enclave.Common project.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- **ResourceExtensions** – `GetResourceStream`, `GetResourceString`, `GetJsonResource<T>` for loading embedded resources from `Assembly` or `Type`; path normalization for manifest names (`/`, `\`, `-`).
- **NotFoundError** – FluentResults error type for resource-not-found failures (e.g. with list of available manifest names).
