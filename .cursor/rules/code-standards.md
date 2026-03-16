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

## Thread synchronization (lock)

- Use **`System.Threading.Lock`** (.NET 9+) for mutual exclusion: `private readonly Lock _lock = new();` then `lock (_lock) { ... }`.
- Do not use `object` or `Monitor` for new code when targeting .NET 9 or later.
- Do not use `await` inside a `lock`; locks are thread-bound and code after `await` may run on another thread.

---
alwaysApply: true
---
