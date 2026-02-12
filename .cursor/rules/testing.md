# Testing Standards

## General Rules

- Write unit tests for all business logic
- Test classes in logical grouping
- Edge cases and error case testing
- Use **Mock** objects for external dependencies

## Testing dotnet projects

- Use xUnit testing framework
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
