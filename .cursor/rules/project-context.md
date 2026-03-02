# Project Context

## Developer Profile

The developer is a Senior .NET C# developer and Solution Architect with 20+ years of experience.

## Project Overview

**Enclave Terminal Breach** is a multi-platform terminal hacking assistant application that helps solve
the terminal hacking mini-game in Bethesda's Fallout games (Fallout 3, Fallout: New Vegas, Fallout 4,
and Fallout 76).

**Important:** This is NOT a recreation of the mini-game itself. It is an external helper tool that
analyzes password candidate lists, applies an elimination algorithm based on likeness scores,
and suggests optimal guesses.

### In-universe Identity

The application is an Enclave-developed breach tool for RobCo Unified Operating System terminals,
code-named **ECHELON**. See `docs/Lore/Project-History.md` for the full backstory.

## Project Evolution (Code Names)

Each phase is a distinct deliverable with its own architecture milestone:

| Phase | Name | Platform | Status |
|-------|------|----------|--------|
| 0 | Excel Prototype (Pre-SPARROW) | VBA macros | Done (research) |
| 1 | SPARROW | DOS 3.11 proof of concept (stdin/stdout) | In Progress |
| 2 | RAVEN | Console application with screen positioning | Planned |
| 3 | GHOST | Web deployment (Blazor PWA) | Planned |
| 3b | (Future) React.js Web | Browser SPA – React.js alternative to GHOST | Exploratory |
| 4 | ECHELON | Mobile Pip-Boy version (MAUI) | Planned |
| 4b | (Future) React Native Mobile | React Native alternative to ECHELON | Exploratory |

## Repository

- GitHub: https://github.com/Gwergilius/Enclave-Terminal-Breach
- Documentation root: `~/README.md` (where `~` refers to the repository root)
- Source code: `~/src/dotnet/` (.NET solution: `Enclave.Echelon.slnx`)

---
alwaysApply: true
---
