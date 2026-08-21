# Introduction to StateCraft

## Why StateCraft?

- **Type Safety** - Generic state and transition types with compile-time checking
- **Thread Safety** - Prevents concurrent state updates to guarantee reliability
- **Async-First** - Native `async` / `await` throughout with cancellation support
- **Parameterized States** - States can carry up to 4 typed parameters, with parameter mapping between transitions
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
- Optional typed parameters (up to 4)

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

- [Getting Started](./getting-started.md) - Installation and first state machine
- [General Concepts](./general-concepts.md) - Handler signatures, initial state, and build validation
- [State Machine Lifecycle](./state-machine-lifecycle.md) - Detailed lifecycle documentation
- [Actions](./actions.md) - Long-running interruptible state work
- [Parameterless Transitions](./parameterless-transitions.md) - Simple state-to-state transitions
- [Parameterized Transitions](./parameterized-transitions.md) - Transitions that carry typed data
- [Mapped Transitions](./mapped-transitions.md) - Automatic parameter conversion
- [Reentrant Transitions](./reentrant-transitions.md) - Same-parameter transitions
- [Triggers](./triggers.md) - Autonomous transitions based on signals
- [Exception Handling](./exception-handling.md) - Error handling strategies
- [Exception Behavior](./exception-behavior.md) - Custom exception handling implementations
- [Inverted Transitions](./inverted-transitions.md) - Define transitions by destination instead of source
- [Mermaid Diagrams](./mermaid-diagrams.md) - Render a configuration as a Mermaid `stateDiagram-v2`
