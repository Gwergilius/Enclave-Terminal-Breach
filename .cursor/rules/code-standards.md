# Code Standards

## C# Extension Methods

- C# Extension methods in `Extensions` folder
- `this` parameter is always the first parameter
- Null checks in all extension methods
- Throw ArgumentNullException for null parameters
- Detailed XML documentation with `<example>` blocks

## Dotnet Dependency Injection

- Use **Microsoft.Extensions.DependencyInjection**
- Register services in **ConfigureServices** method
- Use **IServiceProvider** to retrieve services

## Dotnet Error Handling

- Use FluentResults library for Result<T> types
- NotFoundError class for non-existent resources
- Detailed error messages with diagnostic information
- Throw Exception only if parameters are invalid
- Use **try-catch** blocks for I/O operations

## Classes and Methods

- All public methods with XML documentation

## Project Structure

- Extension classes in `Extensions/` folder
- Model classes in `Models/` folder
- Service classes in `Services/` folder
- Test classes in the corresponding `Tests/` folder

---
alwaysApply: true
---
