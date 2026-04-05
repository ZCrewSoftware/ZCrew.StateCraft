# Inverted Transitions

Inverted transitions define a destination state and configure which states can transition to it, rather than
configuring the transition on each source state individually. This is useful when many states share the same transition
to a common destination.

For example, an order management system might have a `Canceled` state that any non-terminal state can transition to.
Instead of adding a `Cancel` transition to every source state one by one, you configure it once on the `Canceled` state
using `From()`.

## Configuration

### From All Other States

Use `From().AllOtherStates()` to allow every configured state (except the destination itself) to transition to this
state. This is the most common pattern:

```csharp
.WithState(State.Canceled, state => state
    .WithTransition(Transition.Cancel, t => t
        .From()
        .AllOtherStates()))
```

This creates a `Cancel` transition on every state except `Canceled`. "Other" is determined by comparing both the state
value and its type parameters, so a state with the same value but different type parameters is considered a different
state.

### From All States

Use `From().AllStates()` to include the destination state itself (a reentrant transition):

```csharp
.WithState(State.Retry, state => state
    .WithTransition(Transition.Retry, t => t
        .From()
        .AllStates()))
```

This creates a `Retry` transition on every state, including `Retry` itself.

### Excluding States

Use `Except(state)` to exclude specific states. Multiple exclusions can be chained:

```csharp
.WithState(State.Canceled, state => state
    .WithTransition(Transition.Cancel, t => t
        .From()
        .AllOtherStates()
        .Except(State.Completed))
        .Except(State.Failed)))
```

This creates a `Cancel` transition on every state except `Canceled` (excluded by `AllOtherStates()`) and `Completed`
(excluded by `Except()`).

### Excluding Parameterized States

When a state has parameters, the `Except` overloads must specify the type parameters to match the state configuration
exactly:

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Pending)

    .WithState(State.Pending, state => state
        .WithTransition<OrderData>(Transition.Open, State.Open))

    // Parameterized state: must use Except<OrderData> to exclude
    .WithState(State.Open, state => state
        .WithParameter<OrderData>()
        .WithTransition(Transition.Ship, t => t
            .WithSameParameter()
            .To(State.Shipped)))

    .WithState(State.Shipped, state => state
        .WithParameter<OrderData>())

    .WithState(State.Completed, state => state)

    .WithState(State.Canceled, state => state
        .WithTransition(Transition.Cancel, t => t
            .From()
            .AllOtherStates()
            .Except(State.Completed)
            .Except<OrderData>(State.Shipped)))

    .Build();
```

Here `Except(State.Completed)` excludes the parameterless `Completed` state, while `Except<OrderData>(State.Shipped)`
excludes the `Shipped` state that was configured with an `OrderData` parameter. Using `Except(State.Shipped)` without
the type parameter would not match, since `Shipped` was configured with a parameter.

Overloads are available for states with up to four parameters:

| State Parameters | Except Overload                 |
|------------------|---------------------------------|
| None             | `Except(state)`                 |
| 1                | `Except<T>(state)`              |
| 2                | `Except<T1, T2>(state)`         |
| 3                | `Except<T1, T2, T3>(state)`     |
| 4                | `Except<T1, T2, T3, T4>(state)` |

### Parameterized Destination States

`From()` is available on parameterized destination states. The destination state's parameters are preserved in the
inverted transition configuration, and the caller must provide the parameters when triggering the transition:

```csharp
.WithState(State.Error, state => state
    .WithParameter<ErrorInfo>()
    .WithTransition(Transition.Fail, t => t
        .From()
        .AllOtherStates()
        .Except(State.Completed)))
```

The transition is created on every non-excluded state, targeting the parameterized `Error` state. Since the destination
requires an `ErrorInfo` parameter, the caller must provide it:

```csharp
await machine.Transition(
    Transition.Fail,
    new ErrorInfo("something went wrong"),
    cancellationToken);
```

## Conditional Transitions

Conditions can be added before `From()` using `If(...)`. When the destination state is parameterless, these conditions
do not receive any parameters:

```csharp
.WithState(State.Canceled, state => state
    .WithTransition(Transition.Cancel, t => t
        .If(() => cancellationPolicy.IsEnabled)
        .From()
        .AllOtherStates()
        .Except(State.Completed)))
```

When the destination state has parameters, conditions receive the destination state's parameter values:

```csharp
.WithState(State.Error, state => state
    .WithParameter<ErrorInfo>()
    .WithTransition(Transition.Fail, t => t
        .If((error) => error.IsFatal)
        .From()
        .AllOtherStates()))
```

The condition is evaluated for every source state when the transition is triggered. If the condition returns `false`,
the transition does not proceed.

Async conditions are also supported:

```csharp
.WithState(State.Canceled, state => state
    .WithTransition(Transition.Cancel, t => t
        .If(token => policy.CanCancelAsync(token))
        .From()
        .AllOtherStates()))
```

There is no way to add conditions based on the *source* state's parameters, since the source state is dynamic and its
parameter types are not known at compile time.

## Example

```csharp
enum State { Pending, Open, Processing, Completed, Canceled }
enum Transition { Open, Process, Complete, Cancel }

record OrderData(string OrderId, string Customer);

var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Pending)
    .OnStateChange((previous, next) =>
        Console.WriteLine($"{previous} -> {next}"))

    .WithState(State.Pending, state => state
        .WithTransition<OrderData>(Transition.Open, State.Open))

    .WithState(State.Open, state => state
        .WithParameter<OrderData>()
        .WithTransition(Transition.Process, t => t
            .WithSameParameter()
            .To(State.Processing)))

    .WithState(State.Processing, state => state
        .WithParameter<OrderData>()
        .WithAction(action => action
            .Invoke(async Task (order, token) =>
            {
                Console.WriteLine($"Processing order {order.OrderId}");
                await Task.Delay(1000, token);
            }))
        .WithTransition(Transition.Complete, State.Completed))

    .WithState(State.Completed, state => state)

    // Canceled: reachable from all states except Completed and Canceled
    .WithState(State.Canceled, state => state
        .WithTransition(Transition.Cancel, t => t
            .From()
            .AllOtherStates()
            .Except(State.Completed)))

    .Build();

await machine.Activate(cancellationToken);

// Open an order
await machine.Transition(
    Transition.Open,
    new OrderData("ORD-001", "Alice"),
    cancellationToken);

// Cancel from Open state
await machine.Transition(Transition.Cancel, cancellationToken);

await machine.Deactivate(cancellationToken);
```

Output:

```
Pending -> Open
Processing order ORD-001
Open -> Canceled
```

The `Cancel` transition was configured once on the `Canceled` state, but is available from `Pending`, `Open`, and
`Processing`. Without inverted transitions, the same `Cancel` transition would need to be added to each of those
states individually.

## Best Practices

### Prefer AllOtherStates Over AllStates

In most cases, `AllOtherStates()` is the correct choice. `AllStates()` includes the destination state itself, creating
a reentrant transition that runs the full exit/entry lifecycle on every trigger. Use `AllStates()` only when reentrant
behavior is intentional:

```csharp
// Good: No reentrant Cancel transition
.WithState(State.Canceled, state => state
    .WithTransition(Transition.Cancel, t => t
        .From()
        .AllOtherStates()))

// Caution: Canceling while already Canceled re-enters the state
.WithState(State.Canceled, state => state
    .WithTransition(Transition.Cancel, t => t
        .From()
        .AllStates()))
```

### Match Type Parameters on Except

When excluding a parameterized state, the type parameters must match exactly. A mismatched `Except` call will silently
fail to exclude the intended state:

```csharp
// Bad: Shipped has an OrderData parameter, so this does not exclude it
.Except(State.Shipped)

// Good: Type parameters match the state configuration
.Except<OrderData>(State.Shipped)
```

Build-time validation (`Build(StateMachineBuildOptions.Validate)`) can help catch configuration issues.

> Note: currently no validation option marks invalid excludes. There is no planned work item for this feature yet.

### Use Inverted Transitions for Shared Destinations

Inverted transitions reduce repetition when multiple states share the same transition to a common destination. If only
one or two states transition to a destination, a regular transition is simpler and more explicit:

```csharp
// Good: Only two states transition to Completed — use regular transitions
.WithState(State.Processing, state => state
    .WithParameter<OrderData>()
    .WithTransition(Transition.Complete, State.Completed))

.WithState(State.Reviewing, state => state
    .WithParameter<OrderData>()
    .WithTransition(Transition.Complete, State.Completed))

// Good: Many states transition to Canceled — use an inverted transition
.WithState(State.Canceled, state => state
    .WithTransition(Transition.Cancel, t => t
        .From()
        .AllOtherStates()
        .Except(State.Completed)))
```

## Next Steps

- [Parameterless Transitions](./6-parameterless-transitions.md) - Simple state-to-state transitions
- [Parameterized Transitions](./7-parameterized-transitions.md) - Transitions that carry typed data
- [Mapped Transitions](./8-mapped-transitions.md) - Parameter type conversion between states
- [Reentrant Transitions](./9-reentrant-transitions.md) - Pass parameters unchanged between states
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals
- [Exception Handling](./11-exception-handling.md) - Error handling strategies
