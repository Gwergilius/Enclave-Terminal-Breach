# Source Code

**English** | [Magyar]

This directory contains all source code for the Enclave Terminal Breach project, organized by **platform** so that .NET, Python, and TypeScript implementations can coexist.

## Folder structure

| Folder | Contents |
|--------|----------|
| **dotnet/** | [.NET implementation](dotnet/README.md) – C# solution (Common, Core, SPARROW, tests). Open the solution and run build from **src/dotnet/**. |
| **excel-prototype/** | Excel/VBA prototype (pre-SPARROW); not part of any solution. |
| **python/** | *Planned.* Python implementation. |
| **typescript/** | *Planned.* TypeScript implementation. |

## Quick start (.NET)

From the repository root:

```powershell
cd src/dotnet
dotnet build Enclave.Echelon.slnx
```

For solution structure, configuration, and coverage, see the [dotnet README](dotnet/README.md).

[Magyar]: ./README.hu.md
