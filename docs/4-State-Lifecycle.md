# State Lifecycle

This document covers the lifecycle handlers available in StateCraft and best practices for using them effectively.

## Overview

StateCraft provides hooks at key points in a state machine's lifecycle:

| Handler         | Description                               | Machine-Level option | State-Level option |
|-----------------|-------------------------------------------|----------------------|--------------------|
| `OnActivate`    | Called once when the state machine starts | ✘                    | ✔                  |
| `OnEntry`       | Called each time a state is entered       | ✘                    | ✔                  |
| `OnExit`        | Called each time a state is exited        | ✘                    | ✔                  |
| `OnStateChange` | Called during transitions                 | ✔                    | ✔                  |
| `OnDeactivate`  | Called once when the state machine stops  | ✘                    | ✔                  |

Each handler supports both synchronous and asynchronous signatures.
Each state-level handler will have the same parameters as the state, if any.

## State Machine Flow

### Activation Flow

When `Activate()` is called:

1. `OnActivate` - Activate the initial state
2. `OnEntry` - Initialize the initial state
3. Start initial state actions

### Transition Flow

During a transition from State A to State B:

1. Cancel and await previous state (State A) actions
2. `OnExit` - Clean up the previous state (State A)
3. `OnStateChange` (machine-level) - Machine-level state change
4. `OnStateChange` (state-level) - State-level state change (State B)
5. `OnEntry` - Initialize the next state (State B)
6. Start next state (State B) actions

### Deactivation Flow

When `Deactivate()` is called:

1. Cancel and await final state actions
2. `OnExit` - Clean up the final state
3. `OnDeactivate` - Deactivate the final state

## Activation

The `OnActivate` handler runs only during `Activate()`, not during regular transitions.

### Configuration

```csharp
// Synchronous
.WithState(State.Idle, state => state
    .OnActivate(currentState => Console.WriteLine($"Activated in {currentState}")))

// Asynchronous
.WithState(State.Idle, state => state
    .OnActivate(async (currentState, token) =>
    {
        await InitializeResourcesAsync(token);
        Console.WriteLine($"Activated in {currentState}");
    }))
```

### Parameterized States

For parameterized states, the handler receives both the state and its parameter:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobConfig>()
    .OnActivate((currentState, config) =>
        Console.WriteLine($"Activated in {currentState} with job: {config.Name}")))
```

## Entry and Exit

The `OnEntry` and `OnExit` handlers run during state transitions.

### Configuration

```csharp
.WithState(State.Running, state => state
    // Synchronous handlers
    .OnEntry(() => Console.WriteLine("Running"))
    .OnExit(() => Console.WriteLine("Stopping"))
    // Asynchronous handlers
    .OnEntry(async token => await RunAsync(token))
    .OnExit(async token => await StopAsync(token)))
```

### Parameterized States

For parameterized states, handlers receive the state parameter:

```csharp
.WithState(State.Running, state => state
    // Configure this state with a parameter
    .WithParameter<JobData>()
    // Synchronous parameterized handlers
    .OnEntry(job => Console.WriteLine($"Running job: {job.Id}"))
    .OnExit(job => Console.WriteLine($"Stopping job: {job.Id}"))
    // Asynchronous parameterized handlers
    .OnEntry(async (job, token) => await RunAsync(job, token))
    .OnExit(async (job, token) => await StopAsync(job, token)))
```

## State Change Handlers

StateCraft provides two levels of state change handlers:

### State-Level Handler

Configured on a specific state, called when transitioning **into** that state:

```csharp
.WithState(State.Running, state => state
    .OnStateChange((from, transition, to) =>
        Console.WriteLine($"Entering Running from {from} via {transition}")))
```

Since this is configured for a specific state, `to` will always be that state.
This is great for performing state changes using a rich persistence service or when using Domain-Driven-Design.
If the state has a parameter then that parameter will be present on the state-level handler:

```csharp
.WithState(State.Running, state => state
    // Configure this state with a parameter
    .WithParameter<JobData>()
    .OnStateChange((from, transition, to, job) =>
        Console.WriteLine($"Entering Running from {from} via {transition} for job: {job.Id}")))
```

### Machine-Level Handler

Configured on the state machine, called for **every** transition:

```csharp
.OnStateChange((from, transition, to) =>
    Console.WriteLine($"[{from}] --{transition}--> [{to}]"))
```

This is ideal for logging or persisting state changes.
This will **not** include the parameter when changing to a parameterized state.

## Deactivation

The `OnDeactivate` handler runs only during `Deactivate()`, not during regular transitions.

### Configuration

```csharp
.WithState(State.Running, state => state
    // Synchronous
    .OnDeactivate(currentState =>
        Console.WriteLine($"Deactivating from {currentState}"))
    // Asynchronous
    .OnDeactivate(async (currentState, token) =>
    {
        await ReleaseResourcesAsync(token);
        Console.WriteLine($"Deactivated from {currentState}");
    }))
```

### Parameterized States

If the state has a parameter then that parameter will be present on the deactivation handler:

```csharp
.WithState(State.Running, state => state
    .WithParameter<JobData>()
    // Synchronous
    .OnDeactivate((currentState, job) =>
        Console.WriteLine($"Deactivating from {currentState} for job: {job.Id}"))
    // Asynchronous
    .OnDeactivate(async (currentState, job, token) =>
    {
        await ReleaseResourcesAsync(token);
        Console.WriteLine($"Deactivated from {currentState} for job: {job.Id}");
    }))
```

## Lifecycle Example

```csharp
enum State { A, B }
enum Transition { Go }

var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.A)

    .OnStateChange((_, _, _) => Console.WriteLine("4. OnStateChange (machine-level)"))

    .WithState(State.A, state => state
        .OnActivate(_ => Console.WriteLine("1. OnActivate (State.A)"))
        .OnEntry(() => Console.WriteLine("2. OnEntry (State.A)"))
        .OnExit(() => Console.WriteLine("3. OnExit (State.A)"))
        .WithTransition(Transition.Go, State.B))

    .WithState(State.B, state => state
        .OnStateChange((_, _, _) => Console.WriteLine("5. OnStateChange (State.B)"))
        .OnEntry(() => Console.WriteLine("6. OnEntry (State.B)"))
        .OnExit(() => Console.WriteLine("7. OnExit (State.B)"))
        .OnDeactivate(_ => Console.WriteLine("8. OnDeactivate (State.B)")))

    .Build();

await machine.Activate(cancellationToken);
await machine.Transition(Transition.Go, cancellationToken);
await machine.Deactivate(cancellationToken);
```

Output:
```
1. OnActivate (State.A)
2. OnEntry (State.A)
3. OnExit (State.A)
4. OnStateChange (machine-level)
5. OnStateChange (State.B)
6. OnEntry (State.B)
7. OnExit (State.B)
8. OnDeactivate (State.B)
```

## Best Practices

### Keep Handlers Quick

Lifecycle handlers block the state machine. For long-running work, use state actions instead:

```csharp
// Good: Quick setup in OnEntry, long work in action
.WithState(State.Processing, state => state
    .OnEntry(() => InitializeCounters())
    .WithAction(action => action
        .Invoke(async token => await ProcessLargeDatasetAsync(token))))

// Avoid: Long work in OnEntry blocks state machine
.WithState(State.Processing, state => state
    .OnEntry(async token => await ProcessLargeDatasetAsync(token)))  // Blocks state machine until complete
```

### Use OnStateChange for Persistence

The machine-level `OnStateChange` handler is ideal for persisting state using a single update method:

```csharp
.OnStateChange(async (_, _, _, token) =>
{
    await this.repository.UpdateJob(this.jobData, token);
})
```

The state-level `OnStateChange` handler is ideal for persisting using rich update methods:

```csharp
.WithState(State.Running, state => state
    .WithParameter<JobData>()
    .OnStateChange(async (_, _, _, job, token) =>
    {
        await this.jobDataService.UpdateJobToRunning(job.Id, token);
    }))
.WithState(State.Stopping, state => state
    .WithParameter<JobData>()
    .OnStateChange(async (_, _, _, job, token) =>
    {
        await this.jobDataService.UpdateJobToStopping(job.Id, job.Reason, token);
    }))
```

### Idempotent Handlers

Design handlers to be idempotent when possible. This helps with recovery scenarios:

```csharp
.OnEntry(() =>
{
    // Idempotent: safe to call multiple times
    _cache.AddOrUpdate(key, value);
})
```

### Thread Safety

All lifecycle operations are thread-safe.
Concurrent transitions are synchronized by the state machine's internal lock.
All handlers are thread-safe: `OnActivate`, `OnEntry`, `OnStateChange` (both), `OnExit`, `OnDeactivate`.
You don't need additional synchronization in handlers unless accessing external shared state.

## Next Steps

- [Actions](./5-actions.md) - Long-running interruptible state work
- [Triggers](./6-triggers.md) - Autonomous transitions based on signals
- [Exception Handling](./7-exception-handling.md) - Error handling strategies
