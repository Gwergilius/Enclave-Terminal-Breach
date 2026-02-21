# Testing Standards

## General Rules

- Write unit tests for all business logic
- Test classes in logical grouping
- Edge cases and error case testing
- Use **Mock** objects for external dependencies

## Testing dotnet projects

- Use xUnit testing framework
- **Unit test fixtures must be marked** with `[UnitTest, TestOf(nameof(SubjectType))]` attributes, where `SubjectType` is the type under test (e.g. `[UnitTest, TestOf(nameof(ColorValue))]` for `ColorValueTests`). *Motivation:* Without the `[UnitTest]` category, tests are excluded from the coverage report, since coverage tooling only runs tests in this category.
- Use Shouldly assertion library
- Theory tests for different input values
- Create Mock objects using Mock.Of. Setup and Verify them using .AsMock() extension method from Test.Core component (e.g.: obj.AsMock().Setup(...))

### Test Descriptions
- Test method names in English (convention)
- Test comments in English
- Arrange-Act-Assert with English comments
- Edge case descriptions in English
---
alwaysApply: true
---
