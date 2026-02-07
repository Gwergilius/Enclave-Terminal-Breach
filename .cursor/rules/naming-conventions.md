# Naming Conventions

## General Rules

- Every identifier should be named using English language

## C# Naming Conventions

- **Classes**: PascalCase (e.g. `AssetExtractionService`)
- **Methods**: PascalCase (e.g. `ExtractAssets`)
- **Private fields**: camelCase with underscore (e.g. `_extractionService`)
- **Properties**: PascalCase (e.g. `IsExtracting`)
- **Constants**: UPPER_CASE (e.g. `MAX_BUFFER_SIZE`)

## Naming Patterns

- Class names are nouns (e.g. Person, NotificationService, HtmlClientFactory)
- Function names are verbs or verbs with subject (e.g. Initialize, CalculateResult, GetClient)
- Property names are nouns (e.g. Color, Instance, InvocationDepth)
- Boolean properties should have "Is", "Can", or "Has" prefix, but only where it adds value
- Boolean fields should be prefixed similarly using "_is", "_can" or "_has" prefixes

---
alwaysApply: true
---
