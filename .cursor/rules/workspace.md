# Workspace Structure

## Repository Root

The repository root is referred to as `~` in all documentation and communication.

The actual path on the developer's machine may vary (OneDrive sync, mapped drives, etc.).
Do not hardcode absolute paths. Always use `~`-relative references in documentation.

**Example:** `~/docs/Architecture` means the `docs/Architecture` folder inside the repository root.

## Git Commit Policy

**CRITICAL: NEVER commit changes automatically. All commits must be done manually by the user.**

- Do NOT use `git commit`, `git add`, or any git commands that modify the repository state
- Do NOT stage files for commit unless explicitly requested by the user
- You MAY prepare commit messages, show `git status`, or explain what would be committed
- The user handles all git operations manually

---
alwaysApply: true
---
