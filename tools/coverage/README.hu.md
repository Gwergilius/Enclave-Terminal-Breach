# Coverage riport generálás

**[English]** | Magyar

A dokumentáció leírja, hogyan lehet coverage riportokat generálni az **Enclave Terminal Breach** solution-höz.

## Előfeltételek

A coverage riporthoz szükséges eszközök:

### 1. dotnet-coverage
```bash
dotnet tool install -g dotnet-coverage
```

### 2. ReportGenerator
```bash
dotnet tool install -g ReportGenerator
```

Továbbá: **PowerShell 7+**, **.NET SDK** (pl. 10.x, lásd `src/global.json`).

## Használat

### Alapvető használat

**Repó gyökeréből:**
```powershell
.\tools\coverage\run-coverage.ps1
```

**src/ mappából** (így a `global.json` és a solution érvényesül):
```powershell
cd src
..\tools\coverage\run-coverage.ps1 -SolutionPath .
```

A szkript:
- **Csak a `[UnitTest]` attribútummal megjelölt teszteket** futtatja coverage gyűjtéshez – integration, e2e, smoke stb. alapértelmezetten nincsenek benne
- Generál egy HTML riportot (teszt projektek és teszt framework nélkül)
- Automatikusan megnyitja a riportot a böngészőben

### Szűrt coverage riport

Alapértelmezetten a **`Category=UnitTest`** szűrő van használatban, tehát csak a `[UnitTest]` attribútummal jelölt tesztek futnak.

```powershell
# Alapértelmezett: csak unit tesztek (nem kell -Filter)
.\tools\coverage\run-coverage.ps1

# Összes teszt (nincs szűrő) – csak ha szándékosan teljes suite coverage-t szeretnél
.\tools\coverage\run-coverage.ps1 -Filter ""

# Csak integration tesztek
.\tools\coverage\run-coverage.ps1 -Filter "Category=IntegrationTest"

# Egyedi filter (ugyanaz a szintaxis, mint dotnet test --filter)
.\tools\coverage\run-coverage.ps1 -Filter "FullyQualifiedName~Password"
```

### Egy vagy több modul coverage-ja

Ha csak egy vagy több modul unit tesztjeit szeretnéd futtatni és csak azoknak a típusoknak a coverage-át látni (a `[TestOf("ModulNeve")]` attribútum alapján):

```powershell
.\tools\coverage\run-coverage.ps1 -Module Password
.\tools\coverage\run-coverage.ps1 -Module Password,PasswordValidator
.\tools\coverage\run-coverage.ps1 -Modules Password,PasswordValidator
```

A **-Module** és **-Modules** (alias) egyaránt használható; több modul vesszővel elválasztva. Alapértelmezett (nincs `-Module`/`-Modules`): minden unit teszt fut, a riport a teljes prod kódra készül.

### Kimeneti útvonal megadása

```powershell
.\tools\coverage\run-coverage.ps1 -OutputPath "CustomTestResults"
```

## Kimeneti fájlok

- **`TestResults/coverage.xml`** – nyers coverage adatok XML formátumban
- **`TestResults/html/index.html`** – HTML riport (automatikusan megnyílik)

A kimeneti mappa (alapértelmezett: `TestResults`) **minden futtatás előtt törlődik**.

## Tesztprojektek megkeresése

A szkript **nem** használ beégetett projektlistát. Egyszer futtatja a **`dotnet test <solution>`** parancsot:

- A solution (`.sln` vagy `.slnx`) tartalmazza az összes projektet; a solutionben lévő minden tesztprojekt lefut.
- **`.sln`** és **`.slnx** is támogatott. A szkript először az aktuális mappában, majd a repó gyökeréhez képest a **`src/`** alatt keresi a solutiont.

Új tesztprojekt solutionhöz adásával automatikusan belekerül a coverage-ba.

## Szabályok

1. **Coverage riportot KIZÁRÓLAG ezzel a szkripttel generálj!** Ne futtass manuálisan `dotnet-coverage` vagy `reportgenerator` parancsokat.
2. **Coverage-hoz alapértelmezetten** csak a **`[UnitTest]`** attribútummal (xunit.categories) jelölt tesztek futnak. Az integration, e2e, smoke és egyéb kategóriák nem befolyásolják a riportot.
3. **A riportban csak** az **Enclave** prefixű *production* assemblyk szerepelnek (pl. `Enclave.Common`, `Enclave.Echelon.Core`). A **\*.Tests** és **\*.Test.\*** mintának megfelelő assemblyk (tesztprojektek, teszt infrastruktúra) ki vannak zárva. A külső assemblyk (FluentValidation, Spectre.Console, Moq, xunit stb.) szintén kizárva, így a line coverage % csak a prod kódra vonatkozik.
4. **A kimeneti mappa törlődik** minden futtatás előtt.

## Hibaelhárítás

| Probléma | Megoldás |
|----------|----------|
| dotnet-coverage nem található | `dotnet tool install -g dotnet-coverage` |
| reportgenerator nem található | `dotnet tool install -g ReportGenerator` |
| Nem található solution | Futtasd a repó gyökeréből vagy `src/`-ból, vagy add meg: `-SolutionPath útvonal` |
| Coverage gyűjtés sikertelen | Futtasd először: `dotnet test`, és javítsd a törő teszteket |
| HTML riport nem nyílik meg | Nyisd meg kézzel: `TestResults/html/index.html` |

## Kapcsolódó dokumentáció

- [dotnet-coverage (coverlet)](https://github.com/tonerdo/coverlet)
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator)
- [Forráskód és build](../src/README.md) – solution, build, tesztek

[//]: #References
[English]: ./README.md
