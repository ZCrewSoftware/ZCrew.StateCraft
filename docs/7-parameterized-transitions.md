# Parameterized Transitions

Parameterized transitions carry typed data to the next state.
The parameter is provided at runtime when triggering the transition and is received by the target state's handlers.

## Configuration

### Full Configuration

Use `WithParameter<T>()` to declare that the transition carries a parameter:

```csharp
.WithState(State.Idle, state => state
    .WithTransition(Transition.Start, t => t
        .WithParameter<JobConfig>()
        .To(State.Processing)))

.WithState(State.Processing, state => state
    .WithParameter<JobConfig>()
    .OnEntry(config => Console.WriteLine($"Processing: {config.Name}")))
```

The target state must be configured with a matching parameter type via `WithParameter<T>()`.

### Shortcut

The shortcut passes the type parameter directly on `WithTransition`:

```csharp
// Shortcut
.WithTransition<JobConfig>(Transition.Start, State.Processing)

// Equivalent to
.WithTransition(Transition.Start, t => t.WithParameter<JobConfig>().To(State.Processing))
```

## Triggering Parameterized Transitions

At runtime, provide the parameter when calling `Transition`:

```csharp
var config = new JobConfig { Name = "Import", Items = items };

// Parameterized transition requires the parameter
await machine.Transition(Transition.Start, config, cancellationToken);
```

Calling the parameterless `Transition(transition)` overload on a parameterized transition will throw.

## From Parameterized States

When the previous state also has a parameter, both parameter types are tracked independently.
The previous state's parameter is available in conditions before `WithParameter<T>()`, while the new parameter is
available in conditions after:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobConfig>()
    .WithTransition(Transition.Fail, t => t
        .WithParameter<ErrorReport>()
        .To(State.Failed)))

.WithState(State.Failed, state => state
    .WithParameter<ErrorReport>()
    .OnEntry(error => Console.WriteLine($"Failed: {error.Message}")))
```

At runtime:

```csharp
await machine.Transition(Transition.Fail, new ErrorReport { Message = "Timeout" }, cancellationToken);
```

## Conditional Transitions

### Conditions Before WithParameter

Conditions added before `WithParameter<T>()` are placed on the initial transition configuration.

From a parameterless state, these conditions receive no parameters:

```csharp
.WithState(State.Idle, state => state
    .WithTransition(Transition.Start, t => t
        .If(() => hasCapacity)
        .WithParameter<JobConfig>()
        .To(State.Processing)))
```

From a parameterized state, these conditions receive the previous state's parameter:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobConfig>()
    .WithTransition(Transition.Fail, t => t
        .If(config => config.RetryCount >= 3)
        .WithParameter<ErrorReport>()
        .To(State.Failed)))
```

### Conditions After WithParameter

Conditions added after `WithParameter<T>()` receive the transition's parameter (the value that will be passed to the
next state):

```csharp
.WithState(State.Idle, state => state
    .WithTransition(Transition.Start, t => t
        .WithParameter<JobConfig>()
        .If(config => config.Items.Count > 0)
        .To(State.Processing)))
```

### Combining Conditions

Conditions can be placed both before and after `WithParameter<T>()`.
All conditions are evaluated in order: conditions added before `WithParameter<T>()` are evaluated first, followed by
conditions added after:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobConfig>()
    .WithTransition(Transition.Fail, t => t
        .If(config => config.RetryCount >= 3)       // Evaluated first (receives JobConfig from state)
        .WithParameter<ErrorReport>()
        .If(error => error.IsFatal)                  // Evaluated second (receives ErrorReport from transition)
        .To(State.Failed)))
```

## Example

```csharp
enum State { Idle, Processing, Failed, Completed }
enum Transition { Start, Fail, Complete }

record JobConfig(string Name, IReadOnlyList<string> Items, int RetryCount = 0);
record JobResult(int ItemsProcessed);
record ErrorReport(string Message);

var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)

    .WithState(State.Idle, state => state
        .WithTransition<JobConfig>(Transition.Start, State.Processing))

    .WithState(State.Processing, state => state
        .WithParameter<JobConfig>()
        .WithAction(action => action
            .Invoke(async (config, token) =>
            {
                foreach (var item in config.Items)
                {
                    token.ThrowIfCancellationRequested();
                    await ProcessItemAsync(item, token);
                }
            }))
        .WithTransition<ErrorReport>(Transition.Fail, State.Failed)
        .WithTransition<JobResult>(Transition.Complete, State.Completed))

    .WithState(State.Failed, state => state
        .WithParameter<ErrorReport>()
        .OnEntry(error => Console.WriteLine($"Failed: {error.Message}")))

    .WithState(State.Completed, state => state
        .WithParameter<JobResult>()
        .OnEntry(result => Console.WriteLine($"Done: {result.ItemsProcessed} items")))

    .Build();

await machine.Activate(cancellationToken);

var config = new JobConfig("Import", ["a.csv", "b.csv"]);
await machine.Transition(Transition.Start, config, cancellationToken);
await machine.Transition(Transition.Complete, new JobResult(2), cancellationToken);

await machine.Deactivate(cancellationToken);
```

## Best Practices

### Avoid Shadowing with Non-Conditional Transitions

When multiple transitions share the same trigger, they are evaluated in configuration order. The first match wins.
A non-conditional transition always matches, so any subsequent transitions with the same trigger are unreachable:

```csharp
// Bad: Second transition is shadowed and unreachable
.WithState(State.Idle, state => state
    .WithTransition<JobConfig>(Transition.Start, State.Processing)   // Always matches
    .WithTransition(Transition.Start, t => t                         // Never reached
        .WithParameter<JobConfig>()
        .If(config => config.IsPriority)
        .To(State.PriorityProcessing)))

// Good: Place conditional transitions before non-conditional ones
.WithState(State.Idle, state => state
    .WithTransition(Transition.Start, t => t                         // Checked first
        .WithParameter<JobConfig>()
        .If(config => config.IsPriority)
        .To(State.PriorityProcessing))
    .WithTransition<JobConfig>(Transition.Start, State.Processing))  // Fallback
```

Build-time validation (`Build(StateMachineBuildOptions.Validate)`) catches this as an error.

This also applies to parameter type matching, where broader parameter types may shadow other transitions:

```csharp
// Bad: Second transition is shadowed and unreachable
.WithState(State.Idle, state => state
    .WithTransition<object>(Transition.Start, State.Processing)  // Always matches
    .WithTransition<string>(Transition.Start, State.Processing)) // Never reached

// Good: Place more specific parameter types before broader ones
.WithState(State.Idle, state => state
    .WithTransition<string>(Transition.Start, State.Processing)  // Receives string parameters
    .WithTransition<object>(Transition.Start, State.Processing)) // Receives all other types
```

### Avoid Long-Running Operations in Conditions

Conditions are evaluated while the state machine's lock is held. Long-running conditions block all other transitions:

```csharp
// Avoid: Slow condition blocks state machine
.WithTransition(Transition.Start, t => t
    .WithParameter<JobConfig>()
    .If(async (config, token) => await database.CanProcessAsync(config, token))
    .To(State.Processing))

// Good: Cache results in OnEntry or actions, check in conditions
.WithTransition(Transition.Start, t => t
    .WithParameter<JobConfig>()
    .If(config => config.Items.Count > 0)
    .To(State.Processing))
```

### Avoid Side Effects in Conditions

Conditions should be pure checks without side effects or stateful changes. Conditions may be evaluated multiple times
across different resolution attempts, and not all conditions on a transition are guaranteed to be checked due to
short-circuit evaluation:

```csharp
// Avoid: Side effects in conditions
.WithTransition(Transition.Start, t => t
    .WithParameter<JobConfig>()
    .If(config =>
    {
        config.StartedAt = DateTime.UtcNow;  // Side-effect — unreliable timing
        return config.Items.Count > 0;
    })
    .To(State.Processing))

// Good: Pure condition check
.WithTransition(Transition.Start, t => t
    .WithParameter<JobConfig>()
    .If(config => config.Items.Count > 0)
    .To(State.Processing))
```

## Next Steps

- [Mapped Transitions](./8-mapped-transitions.md) - Automatic parameter conversion
- [Reentrant Transitions](./9-reentrant-transitions.md) - Same-parameter transitions
- [Parameterless Transitions](./6-parameterless-transitions.md) - Transitions without data
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals
- [Exception Handling](./11-exception-handling.md) - Error handling strategies
