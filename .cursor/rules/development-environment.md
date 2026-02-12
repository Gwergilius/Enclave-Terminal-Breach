# Development Environment Rules

## PowerShell Version

**This project uses PowerShell 7.5.4 or later**

### Requirements

- All PowerShell scripts and modules must be compatible with PowerShell 7+
- Do not use older PowerShell features or cmdlets (before 7.0 version)
- Test all scripts on PowerShell 7.5.4 before committing

### Module Installation

- All module installations use `CurrentUser` scope by default (no administrator privileges required)
- The `AzureModuleHelper.psm1` module handles module installation and management
- Scripts automatically fall back to `CurrentUser` scope if administrator privileges are not available

### Compatibility Notes

- PowerShell 7.5.4 is the default version on Windows and Azure Automation Accounts
- Use `$PSVersionTable.PSVersion` to check PowerShell version
- Ensure all cmdlets and syntax are compatible with Windows PowerShell 7.5.4

---
alwaysApply: true
---
