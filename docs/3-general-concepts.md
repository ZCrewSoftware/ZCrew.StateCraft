# General Concepts

This document covers foundational concepts that apply throughout StateCraft.

## Handler Signatures

StateCraft provides three handler signatures for all configurable callbacks: synchronous, asynchronous with `Task`, and
asynchronous with `ValueTask`. This pattern applies consistently to:

- State lifecycle handlers (`OnActivate`, `OnEntry`, `OnExit`, `OnDeactivate`, `OnStateChange`)
- Action handlers
- Trigger handlers
- Initial state providers
- Transition conditions

### Synchronous Handlers

Use synchronous handlers for quick, non-blocking operations:

```csharp
// Entry handler
.OnEntry(() => Console.WriteLine("Entered state"))

// Action handler
.WithAction(action => action
    .Invoke(() => _counter++))

// Trigger handler
.ThenInvoke(sm => sm.Transition(Transition.Next))
```

Synchronous handlers are internally wrapped to integrate with the async state machine.

### Asynchronous Task Handlers

Use `Task`-returning handlers for operations that perform I/O or await other async work:

```csharp
// Entry handler with cancellation
.OnEntry(async token => await InitializeAsync(token))

// Action handler with cancellation
.WithAction(action => action
    .Invoke(async token => await ProcessDataAsync(token)))

// Trigger handler with cancellation
.ThenInvoke(async (sm, token) => await sm.Transition(Transition.Next, token))
```

All async handlers receive a `CancellationToken` parameter for cooperative cancellation.

### Asynchronous ValueTask Handlers

Use `ValueTask`-returning to easily integrate with existing methods that return `ValueTask`:

```csharp
// Entry handler returning ValueTask
.OnEntry(token => _cache.TryGetValueAsync(key, out var value))

// Action handler returning ValueTask
.WithAction(action => action
    .Invoke(token => ProcessAsync(token)))  // Returns ValueTask
```

`ValueTask` handlers are usually ideal for hot paths where synchronous completion; but, internally the `ValueTask` will
always be captured as a `Task`.

## Initial State

Every state machine requires an initial state configured via `WithInitialState()`.
The initial state is entered every time `Activate()` is called.

### Static Initial State

The simplest form specifies a fixed starting state:

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithState(State.Idle, state => state)
    .Build();
```

### Dynamic Initial State

Use a provider function when the initial state must be determined at activation time:

```csharp
// Synchronous provider
.WithInitialState(() => LoadStateFromDatabase())

// Async Task provider
.WithInitialState(async token => await LoadStateFromDatabaseAsync(token))

// Async ValueTask provider
.WithInitialState(token => LoadStateFromDatabaseAsync(token))  // Returns ValueTask<TState>
```

### Parameterized Initial State

Initial states can carry typed parameters:

```csharp
// Static state with parameter
.WithInitialState<JobConfig>(State.Processing, config)

// Provider returning tuple (type parameter is usually omitted)
.WithInitialState<JobConfig>(async token =>
{
    var config = await LoadConfigAsync(token);
    return (State.Processing, config);
})
```

The target state must be configured with a matching parameter type:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobConfig>()
    .OnEntry(config => Console.WriteLine($"Processing: {config.Name}")))
```

### Multi-Parameter Initial State

Initial states can carry up to 4 typed parameters:

```csharp
// Static state with two parameters
.WithInitialState<JobConfig, UserContext>(State.Processing, config, userContext)

// Provider returning tuple
.WithInitialState<JobConfig, UserContext>(async token =>
{
    var config = await LoadConfigAsync(token);
    var context = await LoadContextAsync(token);
    return (State.Processing, config, context);
})

// Three and four parameters follow the same pattern
.WithInitialState<T1, T2, T3>(State.X, p1, p2, p3)
.WithInitialState<T1, T2, T3, T4>(State.X, p1, p2, p3, p4)
```

The target state must be configured with matching parameter types:

```csharp
.WithState(State.Processing, state => state
    .WithParameters<JobConfig, UserContext>()
    .OnEntry((config, context) =>
        Console.WriteLine($"Processing {config.Name} for {context.User}")))
```

## Build and Validation

The `Build()` method creates a state machine instance from the configuration.

### Basic Build

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithState(State.Idle, state => state)
    .Build();
```

### Build with Validation

Pass `StateMachineBuildOptions.Validate` to perform build-time validation:

```csharp
var machine = configuration.Build(StateMachineBuildOptions.Validate);
```

Validation catches configuration errors before runtime:

| Validation                 | Error Example                                                                  |
|----------------------------|--------------------------------------------------------------------------------|
| Duplicate states           | `"A is duplicated"` or `"A<int> is duplicated"`                                |
| Invalid transition targets | `"To B(A) → B has no matching next state"`                                     |
| Unreachable transitions    | `"To B(A) → B is unreachable because it is shadowed by a previous transition"` |

Validation throws `InvalidOperationException` with all errors if any are found.

### Configuration Reuse

The configuration object is reusable. Each `Build()` call creates an independent state machine instance:

```csharp
var configuration = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithState(State.Idle, state => state
        .WithTransition(Transition.Start, State.Running))
    .WithState(State.Running, state => state);

// Create multiple independent instances
var machine1 = configuration.Build();
var machine2 = configuration.Build();
var machine3 = configuration.Build();
```

## Best Practices

### Validate in Unit Tests

Use `Build(StateMachineBuildOptions.Validate)` in unit tests to catch configuration errors during development, not in
production:

```csharp
// In your unit tests
[Fact]
public void Configuration_WhenBuildingWithValidation_ShouldBeValid()
{
    // Arrange
    var configuration = CreateStateMachineConfiguration();

    // Act
    var machine = configuration.Build(StateMachineBuildOptions.Validate);

    // Assert
    Assert.NotNull(machine);
}

// In production code
public IStateMachine<State, Transition> CreateStateMachine()
{
    return CreateStateMachineConfiguration().Build();  // No validation overhead
}
```

This catches mistakes like typos in state names or missing state configurations during testing while avoiding the
validation overhead in production.

### Honor Cancellation Tokens

Always listen to cancellation tokens, especially in actions and triggers.
The state machine cancels tokens when transitioning out of a state or deactivating:

```csharp
// Good: Respects cancellation
.WithAction(action => action
    .Invoke(async token =>
    {
        while (!token.IsCancellationRequested)
        {
            await ProcessNextItemAsync(token);
        }
    }))

// Good: Pass token to async operations
.WithAction(action => action
    .Invoke(async token =>
    {
        await httpClient.GetAsync(url, token);
        await Task.Delay(1000, token);
    }))

// Avoid: Ignoring cancellation
.WithAction(action => action
    .Invoke(async _ =>
    {
        while (true)  // Never terminates!
        {
            await ProcessNextItemAsync();
        }
    }))
```

### Reuse Configurations

Create configurations once and reuse them rather than rebuilding from scratch each time:

```csharp
// Good: Create configuration once, reuse it
public class OrderStateMachineFactory
{
    private readonly IStateMachineConfiguration<OrderState, OrderTransition> configuration;

    public OrderStateMachineFactory()
    {
        this.configuration = StateMachine
            .Configure<OrderState, OrderTransition>()
            .WithInitialState(OrderState.Pending)
            .WithState(OrderState.Pending, state => state
                .WithTransition(OrderTransition.Process, OrderState.Processing))
            .WithState(OrderState.Processing, state => state
                .WithTransition(OrderTransition.Complete, OrderState.Completed))
            .WithState(OrderState.Completed, state => state);
    }

    public IStateMachine<OrderState, OrderTransition> Create()
    {
        return this.configuration.Build();
    }
}

// Avoid: Rebuilding configuration for each instance
public IStateMachine<OrderState, OrderTransition> CreateOrder()
{
    // Configuration rebuilt every time - wasteful
    return StateMachine
        .Configure<OrderState, OrderTransition>()
        .WithInitialState(OrderState.Pending)
        // ... all configuration repeated ...
        .Build();
}
```

Reusing configurations:
- Reduces memory allocations
- Improves performance
- Ensures consistency across instances
- Makes testing easier (test the configuration once)

## Next Steps

- [State Lifecycle](./4-state-lifecycle.md) - Detailed lifecycle handler documentation
- [Actions](./5-actions.md) - Long-running interruptible state work
- [Parameterless Transitions](./6-parameterless-transitions.md) - Simple state-to-state transitions
- [Parameterized Transitions](./7-parameterized-transitions.md) - Transitions that carry typed data
- [Mapped Transitions](./8-mapped-transitions.md) - Automatic parameter conversion
- [Reentrant Transitions](./9-reentrant-transitions.md) - Same-parameter transitions
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals
- [Exception Handling](./11-exception-handling.md) - Error handling strategies
