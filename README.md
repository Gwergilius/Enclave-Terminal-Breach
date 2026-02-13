# Enclave Terminal Breach

**English** | [Magyar]

[![License-MIT-badge]][License-MIT]
[![.NET-badge]][Dotnet]

Multi-platform Fallout terminal hacking assistant - Complete evolution from SPARROW prototype to ECHELON deployment.

> **In-universe**: Enclave-developed breach tool for RobCo Unified Operating System terminals. See [Project History](docs/Lore/Project-History.md) for complete ECHELON backstory.

## 🎮 What is this?

A **terminal hacking assistant** application that helps solve the terminal hacking mini-game in Bethesda's Fallout games (Fallout 3, Fallout: New Vegas, Fallout 4, and Fallout 76).

**Important:** This is NOT a recreation of the minigame itself. This is an external helper tool that analyzes password patterns and suggests optimal guesses.

## 🚀 Project Status

**Current Phase:** 📝 Documentation & Planning

| Component | Status |
|-----------|--------|
| Documentation | 🚧 In Progress |
| Architecture | 🚧 In Progress |
| SPARROW (DOS PoC) | 🚧 In Progress |
| RAVEN (Console) | 📋 Planned |
| GHOST (Web/Blazor) | 📋 Planned |
| ECHELON (MAUI Mobile) | 📋 Planned |

## 📚 Project Evolution

This repository documents the complete development evolution:

1. **Excel Prototype** (Pre-SPARROW) - Research phase using VBA macros
2. **SPARROW** - DOS 3.11 proof of concept (stdin/stdout)
3. **RAVEN** - Console application with screen positioning
4. **GHOST** - Web/SIGNET deployment (Blazor PWA)
5. **ECHELON** - Mobile Pip-Boy version (MAUI)

Each phase represents a significant architectural milestone, culminating in the final ECHELON v2.1.7 deployment.

## 🏗️ Technology Stack

- **.NET 10.0** - Primary framework
- **C# 12.0** - Programming language
- **MAUI** - Cross-platform mobile UI
- **Blazor** - Progressive Web App
- **xUnit** - Unit testing
- **ReqNRoll** - Integration/E2E testing
- **Playwright** - UI testing

## 📖 Documentation

- [Project History] - Complete ECHELON backstory (Coming soon)
- [Algorithm] - Password elimination algorithm (Coming soon)
- [Architecture] - System design documents (Coming soon)
- [Coding Standards] - Development guidelines (Coming soon)

## 📁 Source code

Folder structure, shared components (Common, Core, tests, test helpers), solution, and build/style configuration are described in the **[src/README][src README]**. Open the solution from `src/Enclave.Echelon.slnx`. Run the build from the **src/** folder: 

```Powershell
cd src
dotnet build Enclave.Echelon.slnx
```

For **code coverage** reports, see [tools/coverage/README](tools/coverage/README.md).

## 🤝 Contributing

This is a personal portfolio project, but feedback and suggestions are welcome! See coding standards in [.cursor/rules/][Coding Standards] for contribution guidelines.

## 📜 License

This project is licensed under the MIT License - see the [LICENSE] file for details.

## 🎯 Acknowledgments

- Bethesda Game Studios for the Fallout franchise
- The Fallout community for inspiration
- RobCo Industries (fictional) for the UOS we're breaching

---

**Disclaimer:** This is a fan project and is not affiliated with Bethesda Softworks or Bethesda Game Studios.

[external-links]: #References
[LICENSE]: ./LICENSE
[License-MIT]: https://opensource.org/licenses/MIT
[Dotnet]: https://dotnet.microsoft.com/
[Project History]: ./docs/Lore/Project-History.md "Complete ECHELON backstory"
[Algorithm]: ./docs/Architecture/Algorithm.md "Password elimination algorithm"
[Architecture]: ./docs/Architecture/README.md "System design documents"
[Coding Standards]: ./.cursor/rules/README.md "Development guidelines"
[src README]: ./src/README.md "Source code structure and configuration"
[Magyar]: ./README.hu.md

[Image-links]: #Image-references
[License-MIT-badge]: https://img.shields.io/badge/License-MIT-yellow.svg
[.NET-badge]: https://img.shields.io/badge/.NET-10.0-512BD4
