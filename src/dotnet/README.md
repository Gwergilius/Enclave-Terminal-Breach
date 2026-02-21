# Source Code

**English** | [Magyar]

.NET implementation of the Enclave Terminal Breach system. This folder (**src/dotnet/**) contains the C# solution, production code, tests, test helpers, and build/style configuration. The Excel prototype remains in [src/excel-prototype](../excel-prototype/) (not part of this solution).

## Folder structure

| Folder | Contents |
|--------|----------|
| **Common/** | [Enclave.Common] – project-independent utilities and extensions (e.g. `ResourceExtensions`, `StringExtensions`, `TimeSpanExtensions`). |
| **Core/** | [Enclave.Echelon.Core] – core business logic: Password Solver, Password Repository, domain models. References Common. |
| **Shared/** | [Enclave.Shared] – shared abstractions (IO, UI, etc.). Used by Raven and future apps. |
| **Enclave.Sparrow/** | SPARROW console app (DOS-style stdin/stdout). *Code freeze – use Enclave.Raven for new work.* |
| **Enclave.Raven/** | RAVEN console app (DOS-style stdin/stdout). Active development for 1.3.0. |
| **tests/Common.Test.Core/** | Shared test helpers and attributes (e.g. `[UnitTest]`, `TestOf`). |
| **tests/Unit/** | Unit test projects (Enclave.Echelon.Core.Tests, Enclave.Echelon.Common.Tests, Enclave.Sparrow.Tests, Enclave.Raven.Tests, Enclave.Shared.Tests). |
| **tests/Integration/** | *Planned.* Integration test projects. |
| **tests/E2E/** | *Planned.* End-to-end test projects (e.g. GHOST E2E). |

## Configuration (this folder)

These files in **src/dotnet/** apply to every project in this folder:

| File | Purpose |
|------|---------|
| **Enclave.Echelon.slnx** | Solution file. Open this to work with the codebase; build with `dotnet build Enclave.Echelon.slnx` from this folder. |
| **global.json** | SDK version selection (e.g. .NET 10). Run `dotnet build` / `dotnet test` from **src/dotnet/** to use the specified SDK. |
| **.editorconfig** | Code style and formatting for C# and project files. |
| **Directory.Build.props** | Shared MSBuild properties: `LangVersion`, `ImplicitUsings`, `Nullable`, company/copyright, common `NoWarn`. |
| **Directory.Packages.props** | [Central Package Management][CPM]: NuGet package versions; projects reference packages without a version. |
| **.runsettings** | Test run settings (e.g. code coverage). Use `dotnet test --settings .runsettings` when running from **src/dotnet/**. |

## Technology stack

- .NET 10.0
- C# 14
- MAUI (mobile), Blazor (web) – per phase
- xUnit + Shouldly + Moq (testing); ReqNRoll + Playwright (integration/E2E)

## Documentation

- [Coding standards] – development guidelines
- [Architecture] – system design
- [Coverage report] – how to generate code coverage (PowerShell script under `tools/coverage/`)

[//]: #References
[CPM]: https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management "Central Package Management"
[Magyar]: ./README.hu.md
[Enclave.Common]: ./Common/README.md
[Enclave.Echelon.Core]: ./Core/README.md
[Architecture]: ../../docs/Architecture/README.md
[Coding standards]: ../../.cursor/rules/README.md
[Coverage report]: ../../tools/coverage/README.md "Code coverage script and usage"
