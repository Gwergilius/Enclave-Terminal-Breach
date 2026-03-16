# Development Environment

## Primary Tech Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Runtime | .NET | 10.0 |
| Language | C# | 14.0 |
| Web UI | Blazor PWA | .NET 10 |
| Mobile UI | .NET MAUI | .NET 10 |
| Unit Testing | xUnit + Shouldly | latest stable |
| BDD / E2E | ReqNRoll | latest stable |
| UI Testing | Playwright | latest stable |

## Future / Exploratory Platforms (not yet in scope)

These are planned alternatives to the Blazor and MAUI variants.
Do not generate code for these unless explicitly requested.

- **React.js** – potential browser SPA alternative to GHOST/Blazor
- **React Native** – potential mobile alternative to ECHELON/MAUI

## C# Language Features

Always use the most idiomatic C# 14 / .NET 10 features where appropriate:

- Primary constructors for classes and structs
- Collection expressions (`[1, 2, 3]`)
- `using` aliases for any type (including tuples and pointers)
- Inline arrays
- `nameof` in attribute constructors
- `params` with any collection type

## PowerShell

**This project uses PowerShell 7.5+ for tooling scripts only.**

- Scripts are for local tooling (coverage, CI helpers) — not for application logic
- All scripts must be compatible with PowerShell 7.5+
- Do not use Windows PowerShell (5.x) syntax or cmdlets

## Installed .NET SDKs

The following SDKs are installed on the developer's machine:

| SDK | Path |
|-----|------|
| 8.0.418 | `C:\Program Files\dotnet\sdk` |
| 10.0.103 | `C:\Program Files\dotnet\sdk` |

The project targets **.NET 10** (`10.0.103`). Do not generate code or project files targeting SDK versions not listed above.

## Solution Files

**This project uses the `.slnx` solution format** (new XML-based format, supported by VS2026 and .NET CLI).

- Always reference `Enclave.Echelon.slnx`, never `.sln`
- Do not suggest converting to `.sln` format

## Build

```powershell
cd src/dotnet
dotnet build Enclave.Echelon.slnx
```

## IDE Workflow

| Activity | Tool |
|----------|------|
| Architecture decisions | VS Code + Claude Code |
| Documentation | VS Code + Claude Code |
| Code generation | VS Code + Claude Code |
| Build & compilation | Visual Studio 2026 Community Edition |
| Test execution | Visual Studio 2026 Community Edition |
| Code analysis (Roslyn, SonarCloud) | Visual Studio 2026 Community Edition |

**CRITICAL: Claude Code must NEVER auto-execute builds or tests.**

- Prepare the command and explain what it does
- The user runs it manually in VS2026 or the terminal
- If build/test output is needed for diagnosis, the user will paste or `@terminal` reference it

## Solution Structure

```
src/
  dotnet/           # Primary .NET solution
    Enclave.Echelon.slnx
  excel-prototype/  # VBA macros (legacy research, read-only reference)
tools/
  coverage/         # Code coverage tooling (SonarCloud integration)
docs/               # Architecture, lore, and development documentation
```

---
alwaysApply: true
---
