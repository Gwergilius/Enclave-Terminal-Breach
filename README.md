# Enclave Terminal Breach

**English** | [Magyar]

[![Version (RAVEN)][version-badge]][releases]
[![License-MIT-badge]][License-MIT]
[![.NET-badge]][Dotnet]   
[![Quality Gate Status][quality-gate-status]][quality-gate-status-url]
[![Coverage][coverage-badge]][coverage-url]   

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

Folder structure is described in the **[src/README][src README]** (platforms: **dotnet**, excel-prototype; future: python, typescript). The .NET solution lives under **src/dotnet/**. Open the solution from `src/dotnet/Enclave.Echelon.slnx` and run the build from **src/dotnet/**:

```Powershell
cd src/dotnet
dotnet build Enclave.Echelon.slnx
```

For **code coverage** and quality (SonarCloud), see [tools/coverage/README](tools/coverage/README.md). The coverage badge is provided by [SonarCloud](https://sonarcloud.io) (free for public repos); set the `SONAR_TOKEN` secret after adding the project on SonarCloud.

## 🔄 CI / Pipeline

GitHub Actions (`.github/workflows/ci.yml`):

- **Push** (any branch): build, unit tests, and coverage run; **failures do not block** (you can push half-finished work and still see results). On main, GitVersion outputs the version when tests pass.
- **Pull request** (to main/master): build, unit tests, and coverage are **blocking**; the run **fails** if build/test fails or if line coverage is below 80% or branch coverage below 95%.

### Version from commit / PR message

Version bumps are driven by **commit messages** on feature branches and by **PR title/description** when merged. Configured in `GitVersion.yml`. **Direct commits to main are not allowed** (except e.g. Changelog updates).

| Context | Default | Trigger | Example |
|--------|--------|---------|--------|
| **Commit** (on a feature branch) | Build number only (`0.1.0+5` → `+6`) | `patch(scope):` in subject | `patch(fix): correct validation` → patch bump |
| **PR merge** (Squash and merge) | **Minor** (new feature) | Subject starts with `feat:` or `feat(scope):` | `feat: add Password model` → minor |
| **PR merge** | **Major** (breaking) | Subject contains `breaking-change:` or `BREAKING CHANGE:` | `breaking-change: remove API` → major |
| **PR merge** | **Patch** (fix only) | Subject starts with `patch:` or `patch(scope):` | `patch: fix typo` → patch |

Use **Squash and merge** for PRs so the PR title becomes the merge commit message and GitVersion can apply the rules above.

## 🤝 Contributing

This is a personal portfolio project, but feedback and suggestions are welcome! See coding standards in [.cursor/rules/][Coding Standards] for contribution guidelines.

## 📜 License

This project is licensed under the MIT License - see the [LICENSE] file for details.

## 🎯 Acknowledgments

- [Bethesda Game Studios][Bethesda] for the Fallout franchise
- The [Fallout community][Fallout Wiki] for inspiration
- [RobCo Industries][RobCo] (fictional) for the UOS we're breaching
- [Hackinal][hackinal] and [Jetholt Hacking][jetholt-hacking] for browser-based minigames to try the algorithm

## About the Developer

**Gwergilius (Gergely Tóth)**  
Cross-platform .NET developer with a passion for Fallout lore and software architecture.

This project demonstrates:
- Cross-platform development (Console, Blazor, MAUI)
- MVVM architecture
- Lore-driven design (fiction follows function)
- PHOSPHOR abstraction layer (inspired by fictional technology)

*"I found Dr. Krane's notes in a Vault-Tec storage facility. Turns out PHOSPHOR wasn't just fiction - it was good software architecture. So I built it."*

## Contact
- GitHub: [@gwergilius][Gwergilius-Github]
- LinkedIn: [Gwergilius][Gwergilius-LinkedIn]

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
[Gwergilius-LinkedIn]: https://www.linkedin.com/in/gwergilius/
[Gwergilius-Github]: https://github.com/Gwergilius/
[Bethesda]: https://bethesdagamestudios.com
[Fallout Wiki]: https://fallout.fandom.com
[RobCo]: https://fallout.fandom.com/wiki/RobCo_Industries
[hackinal]: https://hackinal.com/
[jetholt-hacking]: https://jetholt.com/hacking/

[Image-links]: #Image-references
[version-badge]: https://img.shields.io/github/v/release/Gwergilius/Enclave-Terminal-Breach?sort=semver&label=Version
[releases]: https://github.com/Gwergilius/Enclave-Terminal-Breach/releases

[quality-gate-status]: https://sonarcloud.io/api/project_badges/measure?project=Gwergilius_Enclave-Terminal-Breach&metric=alert_status
[quality-gate-status-url]: https://sonarcloud.io/summary/new_code?id=Gwergilius_Enclave-Terminal-Breach

[coverage-badge]: https://sonarcloud.io/api/project_badges/measure?project=Gwergilius_Enclave-Terminal-Breach&metric=coverage
[coverage-url]: https://sonarcloud.io/summary/new_code?id=Gwergilius_Enclave-Terminal-Breach

[License-MIT-badge]: https://img.shields.io/badge/License-MIT-yellow.svg
[.NET-badge]: https://img.shields.io/badge/.NET-10.0-512BD4
