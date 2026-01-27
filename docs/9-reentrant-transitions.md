# Reentrant Transitions

Reentrant transitions pass the previous state's parameter to the next state unchanged.
They are only available from parameterized states and do not require a parameter when calling
`StateMachine.Transition(...)` at runtime.

`WithSameParameter()` is a convenience for `WithMappedParameter(previous => previous)`. For transitions that need to
convert the parameter to a different type, see [Mapped Transitions](./8-mapped-transitions.md).

## Configuration

Use `WithSameParameter()` to pass the parameter through unchanged:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Retry, t => t
        .WithSameParameter()
        .To(State.Processing)))
```

This re-enters `State.Processing` with the same `JobData` parameter.
The full lifecycle runs: `OnExit` for the current state, then `OnEntry` for the re-entered state.

### Transitioning to a Different State

Reentrant transitions are not limited to re-entering the same state. `WithSameParameter()` can target any state that
accepts the same parameter type:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Pause, t => t
        .WithSameParameter()
        .To(State.Paused)))

.WithState(State.Paused, state => state
    .WithParameter<JobData>()
    .OnEntry(job => Console.WriteLine($"Paused job: {job.Name}")))
```

### Shortcut

On parameterized states, the shortcut `.WithTransition(transition, state)` uses `WithSameParameter()` to preserve the
current parameter:

```csharp
// Shortcut (from a parameterized state)
.WithTransition(Transition.Retry, State.Processing)

// Equivalent to
.WithTransition(Transition.Retry, t => t.WithSameParameter().To(State.Processing))
```

> **Note:** This differs from parameterless states, where the same shortcut creates a
> [parameterless transition](./6-parameterless-transitions.md).

## No Runtime Parameter Required

Like mapped transitions, reentrant transitions do not require a parameter at runtime:

```csharp
await machine.Transition(Transition.Retry, cancellationToken);
```

This is the key distinction from [parameterized transitions](./7-parameterized-transitions.md), which require a
parameter at runtime.

## Conditional Transitions

### Conditions Before WithSameParameter

Conditions added before `WithSameParameter()` receive the previous state's parameter:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Retry, t => t
        .If(job => job.RetryCount < 3)
        .WithSameParameter()
        .To(State.Processing)))
```

### Conditions After WithSameParameter

This has no real impact compared to placing the conditions before `WithSameParameter()`.
Conditions added after `WithSameParameter()` receive the same parameter (since it is passed through unchanged):

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Retry, t => t
        .WithSameParameter()
        .If(job => job.Items.Count > 0)
        .To(State.Processing)))
```

### Combining Conditions

Conditions can be placed both before and after `WithSameParameter()`.
This has no real impact compared to placing the conditions before `WithSameParameter()`.
All conditions are evaluated in order: conditions added before are evaluated first, followed by conditions added after:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Retry, t => t
        .If(job => job.RetryCount < 3)      // Evaluated first (receives JobData)
        .WithSameParameter()
        .If(job => job.Items.Count > 0)     // Evaluated second (receives same JobData)
        .To(State.Processing)))
```

## Example

```csharp
enum State { Idle, Processing, Paused }
enum Transition { Start, Pause, Resume, Retry }

record JobData(string Name, IReadOnlyList<string> Items, int RetryCount = 0);

var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)

    .WithState(State.Idle, state => state
        .WithTransition<JobData>(Transition.Start, State.Processing))

    .WithState(State.Processing, state => state
        .WithParameter<JobData>()
        .WithAction(action => action
            .Invoke(async (job, token) =>
            {
                foreach (var item in job.Items)
                {
                    token.ThrowIfCancellationRequested();
                    await ProcessItemAsync(item, token);
                }
            }))
        // Reentrant: re-enter same state with same parameter
        .WithTransition(Transition.Retry, t => t
            .If(job => job.RetryCount < 3)
            .WithSameParameter()
            .To(State.Processing))
        // Reentrant: move to different state with same parameter
        .WithTransition(Transition.Pause, State.Paused))

    .WithState(State.Paused, state => state
        .WithParameter<JobData>()
        .OnEntry(job => Console.WriteLine($"Paused: {job.Name}"))
        // Resume back to processing with same data (shortcut)
        .WithTransition(Transition.Resume, State.Processing))

    .Build();

await machine.Activate(cancellationToken);

// Parameterized: provide JobData
await machine.Transition(Transition.Start, new JobData("Import", ["a.csv", "b.csv"]), cancellationToken);

// Reentrant: no parameter needed
await machine.Transition(Transition.Pause, cancellationToken);

// Reentrant: no parameter needed
await machine.Transition(Transition.Resume, cancellationToken);

await machine.Deactivate(cancellationToken);
```

## Best Practices

### Avoid Shadowing with Non-Conditional Transitions

When multiple transitions share the same trigger, they are evaluated in configuration order. The first match wins.
A non-conditional transition always matches, so any subsequent transitions with the same trigger are unreachable:

```csharp
// Bad: Second transition is shadowed and unreachable
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Retry, State.Processing)       // Always matches
    .WithTransition(Transition.Retry, t => t                  // Never reached
        .If(job => job.RetryCount < 3)
        .WithSameParameter()
        .To(State.Processing)))

// Good: Place conditional transitions before non-conditional ones
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Retry, t => t                  // Checked first
        .If(job => job.RetryCount < 3)
        .WithSameParameter()
        .To(State.Processing))
    .WithTransition(Transition.Retry, State.Failed))          // Fallback
```

Build-time validation (`Build(StateMachineBuildOptions.Validate)`) catches this as an error.

### Avoid Long-Running Operations in Conditions

Conditions are evaluated while the state machine's lock is held. Long-running conditions block all other transitions:

```csharp
// Avoid: Slow condition blocks state machine
.WithTransition(Transition.Retry, t => t
    .If(async (job, token) => await database.CanRetryAsync(job.Id, token))
    .WithSameParameter()
    .To(State.Processing))

// Good: Cache results in OnEntry or actions, check in conditions
.WithTransition(Transition.Retry, t => t
    .If(job => job.RetryCount < 3)
    .WithSameParameter()
    .To(State.Processing))
```

### Avoid Side Effects in Conditions

Conditions should be pure checks without side effects or stateful changes. Conditions may be evaluated multiple times
across different resolution attempts, and not all conditions on a transition are guaranteed to be checked due to
short-circuit evaluation:

```csharp
// Avoid: Side effects in conditions
.WithTransition(Transition.Retry, t => t
    .If(job =>
    {
        retryCount++;              // Side-effect — may run multiple times
        return retryCount < 3;
    })
    .WithSameParameter()
    .To(State.Processing))

// Good: Pure condition check
.WithTransition(Transition.Retry, t => t
    .If(job => job.RetryCount < 3)
    .WithSameParameter()
    .To(State.Processing))
```

## Next Steps

- [Mapped Transitions](./8-mapped-transitions.md) - Parameter type conversion between states
- [Parameterless Transitions](./6-parameterless-transitions.md) - Transitions without data
- [Parameterized Transitions](./7-parameterized-transitions.md) - Transitions that carry typed data
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals
- [Exception Handling](./11-exception-handling.md) - Error handling strategies
