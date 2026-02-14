# Source Code

**English** | [Magyar]

Multi-platform implementations of the Enclave Terminal Breach system. This folder contains all production code, tests, test helpers, the solution file, and build/style configuration.

## Folder structure

| Folder | Contents |
|--------|----------|
| **Common/** | [Enclave.Common](Common/) – project-independent utilities and extensions (e.g. `ResourceExtensions`, `StringExtensions`, `TimeSpanExtensions`). |
| **Core/** | [Enclave.Echelon.Core](Core/) – core business logic: Password Solver, Password Repository, domain models. References Common. |
| **excel-prototype/** | [Excel prototype](excel-prototype/) – first prototype (Excel/VBA); not built by the solution. |
| **tests/Common/** | *Planned.* Enclave.Tests.Common – shared test helpers and mocks (not tested itself). |
| **tests/Unit/** | *Planned.* Unit test projects (e.g. Enclave.Common.Tests, Enclave.Echelon.Core.Tests). |
| **tests/Integration/** | *Planned.* Integration test projects. |
| **tests/E2E/** | *Planned.* End-to-end test projects (e.g. GHOST E2E). |

## Configuration (this folder)

These files in **src/** apply to every project under **src/**:

| File | Purpose |
|------|---------|
| **Enclave.Echelon.slnx** | Solution file. Open this to work with the codebase; build with `dotnet build Enclave.Echelon.slnx` from this folder. |
| **global.json** | SDK version selection (e.g. .NET 10). The `dotnet` CLI resolves it from the current directory, so run `dotnet build` / `dotnet test` from **src/** to use the specified SDK. |
| **.editorconfig** | Code style and formatting for C# and project files under `src/`. |
| **Directory.Build.props** | Shared MSBuild properties: `LangVersion`, `ImplicitUsings`, `Nullable`, company/copyright, common `NoWarn`. |
| **Directory.Packages.props** | [Central Package Management][CPM]: NuGet package versions; projects reference packages without a version. |
| **.runsettings** | Test run settings (e.g. code coverage). Use `dotnet test --settings .runsettings` when running from **src/**. |

## Technology stack

- .NET 10.0
- C# 14
- MAUI (mobile), Blazor (web) – per phase
- xUnit + Shouldly + Moq (testing); ReqNRoll + Playwright (integration/E2E)

## Documentation

- [Coding standards][coding standards] – development guidelines
- [Architecture](../docs/Architecture/) – system design
- [Coverage report][coverage] – how to generate code coverage (PowerShell script under `tools/coverage/`)

[//]: #References
[CPM]: https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management "Central Package Management"
[coding standards]: ../.cursor/rules/README.md
[coverage]: ../tools/coverage/README.md "Code coverage script and usage"
[Magyar]: ./README.hu.md
