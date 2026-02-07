# Source Code

Multi-platform implementations of the Enclave Terminal Breach system.

## Structure (Planned)

### Phase 1: SPARROW
DOS 3.11 proof of concept with stdin/stdout interaction.

### Phase 2: RAVEN
Console application with screen positioning and PHOSPHOR abstraction layer.

### Phase 3: GHOST
Blazor Progressive Web App for SIGNET deployment.

### Phase 4: ECHELON
MAUI cross-platform mobile application for Pip-Boy integration.

## Shared Components

- **Core** - Business logic, algorithms, domain models
- **Shared** - ViewModels, state management, shared UI logic
- **TestHelpers** - Common testing utilities and mocks
- **Tests** - Unit tests, integration tests, UI tests

## Technology Stack

- .NET 10.0
- C# 12.0
- MAUI (mobile)
- Blazor (web)
- xUnit + ReqNRoll + Playwright (testing)

## Documentation

See [coding standards] for development guidelines.

[coding standards]: ../.cursor/rules/