# Forráskód

**[English]** | Magyar

Az Enclave Terminal Breach többplatformos implementációi. Ebben a mappában van a teljes termék kód, tesztek, teszt segédletek, a solution fájl és a build/stílus konfiguráció.

## Mappa szerkezet

| Mappa | Tartalom |
|--------|----------|
| **Common/** | [Enclave.Common](Common/) – projektfüggetlen utilityk és kiterjesztések (pl. ResourceExtensions, StringExtensions, TimeSpanExtensions). |
| **Core/** | [Enclave.Echelon.Core](Core/) – core üzleti logika: Password Solver, Password Repository, domain modellek. A Commonra hivatkozik. |
| **excel-prototype/** | [Excel prototípus](excel-prototype/) – első prototípus (Excel/VBA); nem része a solution buildjének. |
| **tests/Common/** | *Tervezett.* Enclave.Tests.Common – közös teszt segédletek és mockok (ezt nem teszteljük). |
| **tests/Unit/** | *Tervezett.* Unit teszt projektek (pl. Enclave.Common.Tests, Enclave.Echelon.Core.Tests). |
| **tests/Integration/** | *Tervezett.* Integrációs teszt projektek. |
| **tests/E2E/** | *Tervezett.* End-to-end teszt projektek (pl. GHOST E2E). |

## Konfiguráció (ebben a mappában)

A **src/** mappában lévő fájlok a **src/** alatti minden projektre vonatkoznak:

| Fájl | Cél |
|------|-----|
| **Enclave.Echelon.slnx** | Solution fájl. Innen nyitható a kódbázis; build: `dotnet build Enclave.Echelon.slnx` ebből a mappából. |
| **global.json** | SDK verzió (pl. .NET 10). A `dotnet` CLI a aktuális mappából keresi; ezért a `dotnet build` / `dotnet test` futtatását **src/**-ból érdemes indítani, hogy a kiválasztott SDK érvényesüljön. |
| **.editorconfig** | Kódstílus és formázás a `src/` alatti C# és projektfájlokra. |
| **Directory.Build.props** | Közös MSBuild tulajdonságok: LangVersion, ImplicitUsings, Nullable, company/copyright, közös NoWarn. |
| **Directory.Packages.props** | [Central Package Management][CPM]: NuGet csomagverziók; a projektek verzió nélkül hivatkoznak a csomagokra. |
| **.runsettings** | Tesztfuttatási beállítások (pl. code coverage). **src/**-ból: `dotnet test --settings .runsettings`. |

## Technológiai stack

- .NET 10.0
- C# 14
- MAUI (mobil), Blazor (web) – fázisonként
- xUnit + Shouldly + Moq (tesztelés); ReqNRoll + Playwright (integráció/E2E)

## Dokumentáció

- [Kódolási szabályok][coding standards] – fejlesztési irányelvek
- [Architektúra](../docs/Architecture/) – rendszertervezés

[//]: #References
[CPM]: https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management "Central Package Management"
[coding standards]: ../.cursor/rules/README.hu.md
[English]: ./README.md
