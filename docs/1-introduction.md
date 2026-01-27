# Introduction to StateCraft

## Why StateCraft?

- **Type Safety** - Generic state and transition types with compile-time checking
- **Thread Safety** - Prevents concurrent state updates to guarantee reliability
- **Async-First** - Native `async` / `await` throughout with cancellation support
- **Parameterized States** - States can carry typed data, with parameter mapping between transitions
- **Rich Lifecycle** - Hooks for activation, entry, exit, deactivation, and state changes
- **Triggers** - Autonomous transitions based on signals or timers
- **Exception Handling** - Configurable handlers with automatic partial rollback on failures

## Core Concepts

### States

States represent distinct modes of operation.
Each state can have:
- Entry / exit handlers
- An action (the work performed while in the state)
- Outgoing transitions
- An optional typed parameter

### Transitions

Transitions define how to move between states. They can:
- Be conditional (only proceed if conditions are met)
- Carry parameters to the next state
- Map the previous state's parameter to a new type

### Triggers

Triggers autonomously initiate transitions based on external signals or timers.
They activate and deactivate with the state machine.

## Quick Example

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

// Activate to enter the initial state
await machine.Activate(cancellationToken);

// Trigger a transition
await machine.Transition(Transition.Start, cancellationToken);

// Deactivate when finished
await machine.Deactivate(cancellationToken);
```

## Next Steps

- [Getting Started](./2-getting-started.md) - Installation and first state machine
- [General Concepts](./3-general-concepts.md) - Handler signatures, initial state, and build validation
- [State Lifecycle](./4-state-lifecycle.md) - Detailed lifecycle documentation
- [Actions](./5-actions.md) - Long-running interruptible state work
- [Parameterless Transitions](./6-parameterless-transitions.md) - Simple state-to-state transitions
- [Parameterized Transitions](./7-parameterized-transitions.md) - Transitions that carry typed data
- [Mapped Transitions](./8-mapped-transitions.md) - Automatic parameter conversion
- [Reentrant Transitions](./9-reentrant-transitions.md) - Same-parameter transitions
- [Triggers](./10-triggers.md) - Autonomous transitions
- [Exception Handling](./11-exception-handling.md) - Error handling strategies
