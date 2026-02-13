# Coverage report

**English** | [Magyar]

Script and instructions for generating code coverage reports for the Enclave Terminal Breach solution.

## Location

Tools live under **`~/tools/coverage`** (repository root: `tools/coverage/`). This keeps coverage automation separate from source (`src/`) and documentation (`docs/`).

## Requirements

- **PowerShell 7+** (see [Development Environment](../../.cursor/rules/development-environment.md))
- **.NET SDK** (e.g. 10.x; see `src/global.json`)
- **dotnet-coverage** (global tool):
  ```bash
  dotnet tool install -g dotnet-coverage
  ```
- **ReportGenerator** (global tool):
  ```bash
  dotnet tool install -g ReportGenerator
  ```

## Usage

### Basic

From **repository root**:
```powershell
.\tools\coverage\run-coverage.ps1
```

From **`src/`** (so `global.json` and solution are in scope):
```powershell
cd src
..\tools\coverage\run-coverage.ps1 -SolutionPath .
```

The script will:
- Find the solution file (`.slnx` or `.sln`) in the current directory or under `src/`
- Run **only tests marked with `[UnitTest]`** (xunit.categories) for coverage – integration, e2e, smoke, etc. are excluded by default
- Generate an HTML report (excluding test projects and test framework)
- Open the HTML report in the browser

### Filtered coverage

By default the script uses **`Category=UnitTest`** so only tests with the `[UnitTest]` attribute are run. This keeps coverage based on unit tests only.

```powershell
# Default: unit tests only (no need to pass -Filter)
.\tools\coverage\run-coverage.ps1

# All tests (no filter) – use only if you intentionally want full-suite coverage
.\tools\coverage\run-coverage.ps1 -Filter ""

# Integration tests only
.\tools\coverage\run-coverage.ps1 -Filter "Category=IntegrationTest"

# Custom filter (same syntax as dotnet test --filter)
.\tools\coverage\run-coverage.ps1 -Filter "FullyQualifiedName~Password"
```

### Single- or multi-module coverage

To run only unit tests for one or more modules and see coverage for those types only (uses the `[TestOf("ModuleName")]` attribute):

```powershell
.\tools\coverage\run-coverage.ps1 -Module Password
.\tools\coverage\run-coverage.ps1 -Module Password,PasswordValidator
.\tools\coverage\run-coverage.ps1 -Modules Password,PasswordValidator
```

You can use **-Module** or **-Modules** (alias); multiple modules are comma-separated. Default (no `-Module`/`-Modules`) remains: all unit tests run and the report covers the full production codebase.

### Custom output path

```powershell
.\tools\coverage\run-coverage.ps1 -OutputPath "CustomTestResults"
```

## Output

- **`TestResults/coverage.xml`** – Raw coverage data (XML)
- **`TestResults/html/index.html`** – HTML report (opens automatically)

The output folder (default `TestResults`) is **cleaned before each run**.

## How test projects are found

The script does **not** use a hardcoded list of test projects. It runs **`dotnet test <solution>`** once:

- The solution file (`.sln` or `.slnx`) contains all projects; every test project in the solution is executed.
- Both **`.sln`** and **`.slnx`** are supported. The script looks in the current directory first, then under `src/` relative to the repository root.

Adding a new test project to the solution is enough for it to be included in coverage.

## Rules

1. **Generate coverage only with this script** – do not run `dotnet-coverage` or `reportgenerator` manually for this repo.
2. **Tests run for coverage:** by default only tests with the **`[UnitTest]`** attribute (xunit.categories) are executed. Integration, e2e, smoke and other categories do not affect the coverage report.
3. **Included in the report:** only production assemblies whose name starts with **Enclave**. Assemblies matching **\*.Tests** or **\*.Test.\*** (test projects and test infrastructure) are excluded. Third-party assemblies (FluentValidation, Spectre.Console, Moq, xunit, etc.) are also excluded so the line coverage percentage reflects only production code.
4. **The output folder is deleted** before each run.

## Troubleshooting

| Problem | Solution |
|--------|----------|
| `dotnet-coverage` not found | `dotnet tool install -g dotnet-coverage` |
| `reportgenerator` not found | `dotnet tool install -g ReportGenerator` |
| No solution file found | Run from repo root or from `src/`, or pass `-SolutionPath path\to\solution` or `path\to\folder` |
| Coverage collection fails | Run `dotnet test` first and fix any failing tests |
| HTML report does not open | Open `TestResults/html/index.html` manually in a browser |

## Coverage thresholds (CI)

The script **`assert-coverage-thresholds.ps1`** checks that line and branch coverage meet minimum percentages (default: line ≥ 80%, branch ≥ 95%). It is used in the PR pipeline; you can run it locally:

```powershell
.\tools\coverage\run-coverage.ps1 -OutputPath TestResults
pwsh -File .\tools\coverage\assert-coverage-thresholds.ps1 -CoverageXmlPath TestResults/coverage.xml -LineMinimum 80 -BranchMinimum 95
```

(If the coverage XML is Cobertura format, the script parses it directly; otherwise it uses ReportGenerator to convert first.)

## References

- [dotnet-coverage](https://github.com/tonerdo/coverlet) (coverlet)
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator)

[//]: #References
[Magyar]: ./README.hu.md
