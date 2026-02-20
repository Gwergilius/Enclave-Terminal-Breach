# Forráskód

**[English]** | Magyar

Az Enclave Terminal Breach .NET implementációja. Ebben a mappában (**src/dotnet/**) van a C# solution, a termék kód, tesztek, teszt segédletek és a build/stílus konfiguráció. Az Excel prototípus a [src/excel-prototype](../excel-prototype/) mappában maradt (nem része ennek a solutionnek).

## Mappa szerkezet

| Mappa | Tartalom |
|--------|----------|
| **Common/** | [Enclave.Common] – projektfüggetlen utilityk és kiterjesztések (pl. ResourceExtensions, StringExtensions, TimeSpanExtensions). |
| **Core/** | [Enclave.Echelon.Core] – core üzleti logika: Password Solver, Password Repository, domain modellek. A Commonra hivatkozik. |
| **Enclave.Sparrow/** | SPARROW konzol alkalmazás (DOS-stílusú stdin/stdout). |
| **tests/Common.Test.Core/** | Közös teszt segédletek és attribútumok (pl. `[UnitTest]`, `TestOf`). |
| **tests/Unit/** | Unit teszt projektek (Enclave.Echelon.Core.Tests, Enclave.Echelon.Common.Tests, Enclave.Sparrow.Tests). |
| **tests/Integration/** | *Tervezett.* Integrációs teszt projektek. |
| **tests/E2E/** | *Tervezett.* End-to-end teszt projektek (pl. GHOST E2E). |

## Konfiguráció (ebben a mappában)

A **src/dotnet/** mappában lévő fájlok erre a mappára vonatkoznak:

| Fájl | Cél |
|------|-----|
| **Enclave.Echelon.slnx** | Solution fájl. Innen nyitható a kódbázis; build: `dotnet build Enclave.Echelon.slnx` ebből a mappából. |
| **global.json** | SDK verzió (pl. .NET 10). A **src/dotnet/**-ból futtasd a `dotnet build` / `dotnet test` parancsokat. |
| **.editorconfig** | Kódstílus és formázás a C# és projektfájlokra. |
| **Directory.Build.props** | Közös MSBuild tulajdonságok: LangVersion, ImplicitUsings, Nullable, company/copyright, közös NoWarn. |
| **Directory.Packages.props** | [Central Package Management][CPM]: NuGet csomagverziók; a projektek verzió nélkül hivatkoznak a csomagokra. |
| **.runsettings** | Tesztfuttatási beállítások (pl. code coverage). **src/dotnet/**-ból: `dotnet test --settings .runsettings`. |

## Technológiai stack

- .NET 10.0
- C# 14
- MAUI (mobil), Blazor (web) – fázisonként
- xUnit + Shouldly + Moq (tesztelés); ReqNRoll + Playwright (integráció/E2E)

## Dokumentáció

- [Kódolási szabályok] – fejlesztési irányelvek
- [Architektúra] – rendszertervezés
- [Coverage riport] – code coverage készítése (PowerShell script: `tools/coverage/`)

[//]: #References
[CPM]: https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management "Central Package Management"
[English]: ./README.md

[Enclave.Common]: ./Common/README.hu.md
[Enclave.Echelon.Core]: ./Core/README.hu.md
[Architektúra]: ../../docs/Architecture/README.hu.md
[Kódolási szabályok]: ../../.cursor/rules/README.hu.md
[Coverage riport]: ../../tools/coverage/README.hu.md "Coverage script és használat"
