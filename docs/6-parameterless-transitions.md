# Parameterless Transitions

Parameterless transitions move between states without carrying any data.
They are the simplest type of transition and the default when no parameter configuration is specified.

## Configuration

### Full Configuration

Use the `WithTransition` method with a configuration callback to define a parameterless transition:

```csharp
.WithState(State.Idle, state => state
    .WithTransition(Transition.Start, t => t
        .WithNoParameters()
        .To(State.Running)))
```

### Implied WithNoParameters

`WithNoParameters()` is optional. When no parameter configuration is specified, calling `To()` directly implies
`WithNoParameters()`:

```csharp
// These two are equivalent
.WithTransition(Transition.Start, t => t.WithNoParameters().To(State.Running))
.WithTransition(Transition.Start, t => t.To(State.Running))
```

### Shortcut

The most concise form passes the transition and target state directly:

```csharp
// Shortcut
.WithTransition(Transition.Start, State.Running)

// Equivalent to
.WithTransition(Transition.Start, t => t.WithNoParameters().To(State.Running))
```

## From Parameterized States

When leaving a parameterized state, a parameterless transition drops the previous state's parameter:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Cancel, t => t
        .WithNoParameters()
        .To(State.Idle)))
```

The target state (`State.Idle`) does not receive any parameter.

## Conditional Transitions

Conditions control whether a transition is allowed. All conditions must pass for the transition to proceed.
The first `false` result short-circuits evaluation and the transition is skipped.

### Conditions Before WithNoParameters

Conditions added before `WithNoParameters()` are placed on the initial transition configuration.

From a parameterless state, these conditions receive no parameters:

```csharp
.WithState(State.Idle, state => state
    .WithTransition(Transition.Start, t => t
        .If(() => isReady)
        .WithNoParameters()
        .To(State.Running)))
```

From a parameterized state, these conditions receive the previous state's parameter:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Cancel, t => t
        .If(job => job.IsCancellable)
        .WithNoParameters()
        .To(State.Idle)))
```

### Conditions After WithNoParameters

Conditions added after `WithNoParameters()` do not receive any parameters, regardless of whether the previous state is
parameterized:

```csharp
.WithState(State.Idle, state => state
    .WithTransition(Transition.Start, t => t
        .WithNoParameters()
        .If(() => hasResources)
        .To(State.Running)))
```

### Combining Conditions

Conditions can be placed both before and after `WithNoParameters()`.
All conditions are evaluated in order: conditions added before `WithNoParameters()` are evaluated first, followed by
conditions added after:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Cancel, t => t
        .If(job => job.IsCancellable)           // Evaluated first (receives JobData)
        .WithNoParameters()
        .If(() => !isShuttingDown)              // Evaluated second (no parameters)
        .To(State.Idle)))
```

## Example

```csharp
enum State { Idle, Running, Paused, Stopped }
enum Transition { Start, Pause, Resume, Stop }

var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)

    .WithState(State.Idle, state => state
        .WithTransition(Transition.Start, State.Running))

    .WithState(State.Running, state => state
        .WithTransition(Transition.Pause, State.Paused)
        .WithTransition(Transition.Stop, State.Stopped))

    .WithState(State.Paused, state => state
        .WithTransition(Transition.Resume, State.Running)
        .WithTransition(Transition.Stop, State.Stopped))

    .WithState(State.Stopped, state => state)

    .Build();

await machine.Activate(cancellationToken);
await machine.Transition(Transition.Start, cancellationToken);
await machine.Transition(Transition.Pause, cancellationToken);
await machine.Transition(Transition.Resume, cancellationToken);
await machine.Transition(Transition.Stop, cancellationToken);
await machine.Deactivate(cancellationToken);
```

## Best Practices

### Avoid Shadowing with Non-Conditional Transitions

When multiple transitions share the same trigger, they are evaluated in configuration order. The first match wins.
A non-conditional transition always matches, so any subsequent transitions with the same trigger are unreachable:

```csharp
// Bad: Second transition is shadowed and unreachable
.WithState(State.Idle, state => state
    .WithTransition(Transition.Start, State.Running)          // Always matches
    .WithTransition(Transition.Start, t => t                  // Never reached
        .If(() => isPriority)
        .To(State.PriorityRunning)))

// Good: Place conditional transitions before non-conditional ones
.WithState(State.Idle, state => state
    .WithTransition(Transition.Start, t => t                  // Checked first
        .If(() => isPriority)
        .To(State.PriorityRunning))
    .WithTransition(Transition.Start, State.Running))         // Fallback
```

Build-time validation (`Build(StateMachineBuildOptions.Validate)`) catches this as an error.

### Avoid Long-Running Operations in Conditions

Conditions are evaluated while the state machine's lock is held. Long-running conditions block all other transitions:

```csharp
// Avoid: Slow condition blocks state machine
.WithTransition(Transition.Start, t => t
    .If(async token => await database.CheckPermissionsAsync(token))
    .To(State.Running))

// Good: Cache results in OnEntry or actions, check in conditions
.WithTransition(Transition.Start, t => t
    .If(() => cachedPermissions.HasAccess)
    .To(State.Running))
```

### Avoid Side Effects in Conditions

Conditions should be pure checks without side effects or stateful changes. Conditions may be evaluated multiple times
across different resolution attempts, and not all conditions on a transition are guaranteed to be checked due to
short-circuit evaluation:

```csharp
// Avoid: Side effects in conditions
.WithTransition(Transition.Start, t => t
    .If(() =>
    {
        attemptCount++;           // Side-effect — may run multiple times
        return attemptCount < 3;
    })
    .To(State.Running))

// Good: Pure condition check
.WithTransition(Transition.Start, t => t
    .If(() => attemptCount < 3)
    .To(State.Running))
```

## Next Steps

- [Parameterized Transitions](./7-parameterized-transitions.md) - Transitions that carry typed data
- [Mapped Transitions](./8-mapped-transitions.md) - Automatic parameter conversion
- [Reentrant Transitions](./9-reentrant-transitions.md) - Same-parameter transitions
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals
- [Exception Handling](./11-exception-handling.md) - Error handling strategies
