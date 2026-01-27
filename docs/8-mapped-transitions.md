# Mapped Transitions

Mapped transitions automatically derive the next state's parameter from the previous state's parameter using a mapping
function. They are only available from parameterized states and do not require a parameter when calling
`StateMachine.Transition(...)` at runtime.

## Configuration

Use `WithMappedParameter<TNext>(Func<TPrevious, TNext> map)` to define the mapping:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Complete, t => t
        .WithMappedParameter<JobResult>(job => new JobResult(job.ItemsProcessed))
        .To(State.Completed)))

.WithState(State.Completed, state => state
    .WithParameter<JobResult>()
    .OnEntry(result => Console.WriteLine($"Done: {result.ItemsProcessed} items")))
```

## No Runtime Parameter Required

Because the mapping function derives the parameter from the current state, no parameter is needed when triggering the
transition:

```csharp
// Mapped transitions use the parameterless Transition overload
await machine.Transition(Transition.Complete, cancellationToken);

// Do NOT pass a parameter — the mapping function provides it
```

This is the key distinction from [parameterized transitions](./7-parameterized-transitions.md), which require a
parameter at runtime.

## Conditional Transitions

### Conditions Before WithMappedParameter

Conditions added before the mapping receive the previous state's parameter:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Complete, t => t
        .If(job => job.IsFinished)
        .WithMappedParameter<JobResult>(job => new JobResult(job.ItemsProcessed))
        .To(State.Completed)))
```

### Conditions After WithMappedParameter

Conditions added after the mapping receive the mapped (next) parameter:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Complete, t => t
        .WithMappedParameter<JobResult>(job => new JobResult(job.ItemsProcessed))
        .If(result => result.ItemsProcessed > 0)
        .To(State.Completed)))
```

### Combining Conditions

Conditions can be placed both before and after the mapping.
All conditions are evaluated in order: conditions added before are evaluated first, followed by conditions added after.
The mapping function is only invoked if conditions after the mapping exist:

```csharp
.WithState(State.Processing, state => state
    .WithParameter<JobData>()
    .WithTransition(Transition.Complete, t => t
        .If(job => job.IsFinished)                                        // Evaluated first (receives JobData)
        .WithMappedParameter<JobResult>(job => new JobResult(job.Count))
        .If(result => result.ItemsProcessed > 0)                          // Evaluated second (receives JobResult)
        .To(State.Completed)))
```

If the first condition returns `false`, the mapping function is never called and the second condition is not evaluated.

## Example

```csharp
enum State { Idle, Processing, Completed }
enum Transition { Start, Complete }

record JobData(string Name, IReadOnlyList<string> Items);
record JobResult(int ItemsProcessed);

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
        // Mapped: convert JobData to JobResult
        .WithTransition(Transition.Complete, t => t
            .WithMappedParameter<JobResult>(job => new JobResult(job.Items.Count))
            .To(State.Completed)))

    .WithState(State.Completed, state => state
        .WithParameter<JobResult>()
        .OnEntry(result => Console.WriteLine($"Done: {result.ItemsProcessed} items")))

    .Build();

await machine.Activate(cancellationToken);

// Parameterized: provide JobData
await machine.Transition(Transition.Start, new JobData("Import", ["a.csv", "b.csv"]), cancellationToken);

// Mapped: no parameter needed, mapping function converts JobData to JobResult
await machine.Transition(Transition.Complete, cancellationToken);

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
    .WithTransition(Transition.Complete, t => t
        .WithMappedParameter<JobResult>(job => new JobResult(job.Items.Count))
        .To(State.Completed))                                                // Always matches
    .WithTransition(Transition.Complete, t => t
        .WithMappedParameter<JobResult>(job => new JobResult(job.Items.Count))
        .If(result => result.ItemsProcessed > 10)
        .To(State.FastCompleted)))                                           // Never reached
```

Build-time validation (`Build(StateMachineBuildOptions.Validate)`) catches this as an error.

### Avoid Long-Running Operations in Conditions

Conditions are evaluated while the state machine's lock is held. Long-running conditions block all other transitions:

```csharp
// Avoid: Slow condition blocks state machine
.WithTransition(Transition.Complete, t => t
    .If(async (job, token) => await database.IsJobFinishedAsync(job.Id, token))
    .WithMappedParameter<JobResult>(job => new JobResult(job.Items.Count))
    .To(State.Completed))

// Good: Cache results in OnEntry or actions, check in conditions
.WithTransition(Transition.Complete, t => t
    .If(job => job.IsFinished)
    .WithMappedParameter<JobResult>(job => new JobResult(job.Items.Count))
    .To(State.Completed))
```

### Avoid Side Effects in Conditions

Conditions should be pure checks without side effects or stateful changes. Conditions may be evaluated multiple times
across different resolution attempts, and not all conditions on a transition are guaranteed to be checked due to
short-circuit evaluation:

```csharp
// Avoid: Side effects in conditions
.WithTransition(Transition.Complete, t => t
    .WithMappedParameter<JobResult>(job => new JobResult(job.Items.Count))
    .If(result =>
    {
        completionCount++;                 // Side-effect — may run multiple times
        return result.ItemsProcessed > 0;
    })
    .To(State.Completed))

// Good: Pure condition check
.WithTransition(Transition.Complete, t => t
    .WithMappedParameter<JobResult>(job => new JobResult(job.Items.Count))
    .If(result => result.ItemsProcessed > 0)
    .To(State.Completed))
```

### Avoid Side Effects in Mapping Functions

Mapping functions should be pure transformations. They may be called during condition evaluation and again during the
actual transition. Avoid modifying external state:

```csharp
// Avoid: Side effects in mapping
.WithMappedParameter<JobResult>(job =>
{
    metricsService.RecordCompletion(job);  // Side-effect — may run more than once
    return new JobResult(job.ItemsProcessed);
})

// Good: Pure transformation
.WithMappedParameter<JobResult>(job => new JobResult(job.ItemsProcessed))
```

Use lifecycle handlers like `OnEntry` or `OnStateChange` for side effects instead.

## Next Steps

- [Reentrant Transitions](./9-reentrant-transitions.md) - Same-parameter transitions
- [Parameterless Transitions](./6-parameterless-transitions.md) - Transitions without data
- [Parameterized Transitions](./7-parameterized-transitions.md) - Transitions that carry typed data
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals
- [Exception Handling](./11-exception-handling.md) - Error handling strategies
