# Getting Started with StateCraft

## Installation

Add a reference to the `ZCrew.StateCraft` project:

```xml
TODO NUGET PACKAGE
```

Then add the namespace:

```csharp
using ZCrew.StateCraft;
```

## Define Your States and Transitions

States and transitions are typically defined as enums:

```csharp
enum State
{
    Idle,
    Running,
    Finished
}

enum Transition
{
    Start,
    Stop,
    Complete
}
```

Any non-nullable type works, but enums provide type safety and readability.

## Configure the State Machine

Use the fluent API to define states, transitions, and lifecycle handlers:

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)

    .WithState(State.Idle, state => state
        .OnEntry(() => Console.WriteLine("Ready"))
        .WithTransition(Transition.Start, State.Running))

    .WithState(State.Running, state => state
        .WithAction(action => action
            .Invoke(async token => await DoWorkAsync(token)))
        .WithTransition(Transition.Stop, State.Idle)
        .WithTransition(Transition.Complete, State.Finished))

    .WithState(State.Finished, state => state
        .OnEntry(() => Console.WriteLine("Done")))

    .Build();
```

### Key Configuration Methods

| Method                               | Purpose                            |
|--------------------------------------|------------------------------------|
| `WithInitialState(state)`            | Set the starting state (required)  |
| `WithState(state, config)`           | Configure a state's behavior       |
| `OnStateChange(handler)`             | Global handler for all transitions |
| `OnException(handler)`               | Global exception handler           |
| `Build()`                            | Create the state machine instance  |

## Use the State Machine

### Activation

Before using the state machine, activate it to enter the initial state:

```csharp
await machine.Activate(cancellationToken);
```

This runs `OnActivate` and `OnEntry` handlers for the initial state.

### Triggering Transitions

Move between states by triggering transitions:

```csharp
// Transition (throws if invalid)
await machine.Transition(Transition.Start, cancellationToken);

// Check if a transition is valid
bool canStop = await machine.CanTransition(Transition.Stop, cancellationToken);

// Try a transition (returns false instead of throwing if invalid)
bool success = await machine.TryTransition(Transition.Stop, cancellationToken);
```

### Deactivation

When finished, deactivate the state machine:

```csharp
await machine.Deactivate(cancellationToken);
```

This runs `OnExit` and `OnDeactivate` handlers for the current state.

## State Lifecycle Handlers

Each state supports lifecycle handlers:

```csharp
.WithState(State.Running, state => state
    .OnActivate(currentState => { })   // Only during Activate()
    .OnEntry(() => { })                // When entering this state
    .OnExit(() => { })                 // When exiting this state
    .OnDeactivate(currentState => { }) // Only during Deactivate()
    .WithAction(action => action
        .Invoke(async token => { })))  // Work performed in this state
```

All handlers support both synchronous and asynchronous signatures.

## Conditional Transitions

Transitions can include conditions:

```csharp
.WithTransition(Transition.Start, t => t
    .If(() => isReady)
    .To(State.Running))
```

The transition only proceeds if the condition returns `true`.

## Complete Example

```csharp
using ZCrew.StateCraft;

enum OrderState { Pending, Processing, Shipped, Delivered }
enum OrderTransition { Process, Ship, Deliver }

// Configure
var order = StateMachine
    .Configure<OrderState, OrderTransition>()
    .WithInitialState(OrderState.Pending)

    .WithState(OrderState.Pending, state => state
        .OnEntry(() => Console.WriteLine("Order received"))
        .WithTransition(OrderTransition.Process, OrderState.Processing))

    .WithState(OrderState.Processing, state => state
        .OnEntry(() => Console.WriteLine("Processing order..."))
        .WithAction(action => action
            .Invoke(async token =>
            {
                await Task.Delay(1000, token); // Simulate work
                Console.WriteLine("Order processed");
            }))
        .WithTransition(OrderTransition.Ship, OrderState.Shipped))

    .WithState(OrderState.Shipped, state => state
        .OnEntry(() => Console.WriteLine("Order shipped"))
        .WithTransition(OrderTransition.Deliver, OrderState.Delivered))

    .WithState(OrderState.Delivered, state => state
        .OnEntry(() => Console.WriteLine("Order delivered")))

    .OnStateChange((from, transition, to) =>
        Console.WriteLine($"  [{from}] --{transition}--> [{to}]"))

    .Build();

// Use
await order.Activate(cancellationToken);
await order.Transition(OrderTransition.Process, cancellationToken);
await order.Transition(OrderTransition.Ship, cancellationToken);
await order.Transition(OrderTransition.Deliver, cancellationToken);
await order.Deactivate(cancellationToken);
```

Output:
```
Order received
  [Pending] --Process--> [Processing]
Processing order...
Order processed
  [Processing] --Ship--> [Shipped]
Order shipped
  [Shipped] --Deliver--> [Delivered]
Order delivered
```

## Next Steps

- [General Concepts](./3-general-concepts.md) - Handler signatures, initial state, and build validation
- [State Lifecycle](./4-state-lifecycle.md) - Detailed lifecycle documentation
- [Triggers](./5-triggers.md) - Autonomous transitions
- [Exception Handling](./6-exception-handling.md) - Error handling strategies
