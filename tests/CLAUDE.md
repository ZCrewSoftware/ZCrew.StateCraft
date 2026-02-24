## Test Commands

```bash

# Run all tests
dotnet test

# Run a specific test project
dotnet test --project tests/ZCrew.StateCraft.UnitTests
dotnet test --project tests/ZCrew.StateCraft.IntegrationTests

# Run a single test by method name (xunit v3 with Microsoft.Testing.Platform)
dotnet test --project tests/ZCrew.StateCraft.UnitTests --filter-method "ZCrew.StateCraft.UnitTests.StateTableTests.LookupState_WhenStateExists_ShouldReturnState"

# Run tests matching a pattern (wildcards supported)
dotnet test --project tests/ZCrew.StateCraft.UnitTests --filter-method "*LookupState*"

# Run all tests in a class
dotnet test --project tests/ZCrew.StateCraft.UnitTests --filter-class "ZCrew.StateCraft.UnitTests.StateTableTests"
```

## Testing

- Test projects have `InternalsVisibleTo` access automatically via Directory.Build.props
- Use `TestContext.Current.CancellationToken` for cancellation tokens instead of `default` or `CancellationToken.None`
- Test names should be formatted as `Member_{T_}_When_Should` and use `{T_}` to list type parameters to distinguish tests
- Only use `[Fact(Timeout = 5000)]` for tests with a clear deadlock risk (e.g., tests using synchronization primitives like `TaskCompletionSource`, `SemaphoreSlim`, or awaiting external signals)
- **Avoid `Task.Delay` for synchronization** - use proper synchronization primitives:
    - `TaskCompletionSource` to signal single events (e.g., trigger invoked, handler called)
    - `SemaphoreSlim` for counting multiple invocations or coordinating sequences
    - NSubstitute's `When...Do` to hook into mock invocations: `mock.When(x => x.Method()).Do(_ => tcs.TrySetResult())`
- **CurrentState timing**: `CurrentState` is set *after* `OnEntry` handlers complete. To reliably test state transitions triggered by async operations, signal completion from within the trigger's `ThenInvoke` callback after the transition completes, not from `OnEntry`

**Test Structure:**
- Follow the AAA (Arrange-Act-Assert) pattern with blank lines separating each section
- Add `// Arrange`, `// Act`, and `// Assert` comments to denote each section
- Keep Act and Assert sections separate (do not combine them)
- Create fresh instances in each test method rather than using shared fields to prevent cascading failures
- Name the instance under test descriptively (e.g., `behavior`, `stateMachine`, `activator`) — do not use `sut`
- Do not use `#region` blocks or decorative comment separators (e.g., `// ── Section ──────`) in any files
- When testing that a method throws, capture the call in a named variable in the Act section, then assert on it:
  ```csharp
  // Act
  var callOnEntry = () => behavior.CallOnEntry(_ => throw exception, token);

  // Assert
  await Assert.ThrowsAsync<InvalidOperationException>(callOnEntry);
  ```
- Use NSubstitute to assert that certain calls were made and verify call counts
