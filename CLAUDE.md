# Enclave Terminal Breach – Claude Code Configuration

## Rules

At the start of every session, read ALL files in the `.cursor/rules/` directory and follow every
rule defined in them. These files are the authoritative source for coding standards, communication
style, workflow, and project context.

Files to read:
- `.cursor/rules/project-context.md`
- `.cursor/rules/communication.md`
- `.cursor/rules/code-standards.md`
- `.cursor/rules/naming-conventions.md`
- `.cursor/rules/development-environment.md`
- `.cursor/rules/development-workflow.md`
- `.cursor/rules/documentation.md`
- `.cursor/rules/testing.md`
- `.cursor/rules/workspace.md`

## Quick Reference

- **Language:** C# 14 / .NET 10.0.103 (SDK); .NET 8.0.418 also installed but not targeted
- **Solution:** `Enclave.Echelon.slnx` (`.slnx` format — never `.sln`)
- **UI Platforms:** Blazor PWA (GHOST), .NET MAUI (ECHELON)
- **Communication:** Hungarian (tegező), code/docs in English
- **Git:** Never commit automatically — user handles all git operations
- **Tests:** xUnit + Shouldly; always use `[UnitTest, TestOf(nameof(...))]` attributes
- **Errors:** FluentResults (`Result<T>`), no bare exceptions for flow control
- **IDE split:** VS Code = architecture/docs/codegen · VS2026 Community = build/test/analysis
- **Builds/tests:** NEVER auto-execute — prepare the command, user runs it in VS2026
