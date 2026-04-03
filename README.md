# ZCrew.StateCraft

A fluent, async-first state machine library for .NET. StateCraft provides a clean, type-safe API for defining states, transitions, and lifecycle handlers with full support for parameterized states, conditional transitions, and cancellation.

## Features

- **[Fluent Configuration API](docs/2-getting-started.md)** - Intuitive builder pattern for defining state machines
- **Async/Await Support** - First-class support for async lifecycle handlers and actions
- **[Parameterized States](docs/7-parameterized-transitions.md)** - States can carry typed data (up to 4 parameters) that flows through entry, action, and exit handlers
- **[Conditional Transitions](docs/7-parameterized-transitions.md)** - Guard conditions that determine if a transition should proceed
- **[Mapped Transitions](docs/8-mapped-transitions.md)** - Transform parameters during transitions between states
- **[Reentrant Transitions](docs/9-reentrant-transitions.md)** - Preserve parameters when transitioning between states of the same type
- **[Lifecycle Handlers](docs/4-state-machine-lifecycle.md)** - OnEntry, OnExit, and OnStateChange hooks for state management
- **[Triggers](docs/10-triggers.md)** - Autonomous transition initiators (one-shot and repeating) that activate with the state machine
- **[Actions](docs/5-actions.md)** - Long-running actions that states perform, which can transition the state machine
- **[Exception Handling](docs/11-exception-handling.md)** - Configurable exception handlers and [custom exception behavior](docs/12-exception-behavior.md)
- **Thread-Safe** - Internal locking ensures safe concurrent access
- **[Validation](docs/3-general-concepts.md)** - Detects configuration errors like duplicate states and invalid transitions

## Installation

This package is available on NuGet as `ZCrew.StateCraft` for these frameworks:

- .NET 8.0
- .NET 9.0
- .NET 10.0

```xml
<PackageReference Include="ZCrew.StateCraft" Version="1.0.0" />
```

## Quick Start

```csharp
enum ScvState { Idle, Harvesting, Returning }
enum ScvTrigger { Harvest, CargoFull, DepositComplete }

var stateMachine = StateMachine
    .Configure<ScvState, ScvTrigger>()
    .WithInitialState(ScvState.Idle)

    .WithState(ScvState.Idle, state => state
        .OnEntry(() => Console.WriteLine("SCV ready."))
        .WithTransition(ScvTrigger.Harvest, ScvState.Harvesting))

    .WithState(ScvState.Harvesting, state => state
        .OnEntry(() => Console.WriteLine("Harvesting minerals..."))
        .WithTransition(ScvTrigger.CargoFull, ScvState.Returning))

    .WithState(ScvState.Returning, state => state
        .OnEntry(() => Console.WriteLine("Returning cargo."))
        .WithTransition(ScvTrigger.DepositComplete, ScvState.Idle))

    .Build();

await stateMachine.Activate();                       // SCV ready.
await stateMachine.Transition(ScvTrigger.Harvest);   // Idle -> Harvesting
await stateMachine.Transition(ScvTrigger.CargoFull); // Harvesting -> Returning
```

### Conditional Transitions

Guard conditions control when transitions are allowed:

```csharp
.WithTransition(ScvTrigger.Harvest, t => t
    .WithParameter<MineralPatch>()
    .If(patch => patch.HasMinerals)
    .To(ScvState.Harvesting))
```

### Parameterized States

States can carry typed data through their lifecycle:

```csharp
.WithState(ScvState.Harvesting, state => state
    .WithParameter<MineralPatch>()
    .OnEntry(patch => Console.WriteLine($"Harvesting from {patch.Id}..."))
    .OnExit(patch => Console.WriteLine($"Cargo full from {patch.Id}.")))

// Transition with parameter
await stateMachine.Transition(ScvTrigger.Harvest, nearestPatch);
```

### Async Actions

States can run async work while active, including transitioning the state machine:

```csharp
.WithState(ScvState.Harvesting, state => state
    .WithParameter<MineralPatch>()
    .WithAction(action => action
        .Invoke(async (patch, token) =>
        {
            await patch.HarvestAsync(token);
            await stateMachine.Transition(ScvTrigger.CargoFull, token);
        })))
```

### Triggers

Triggers automatically fire transitions based on async signals:

```csharp
var depositSignal = new SemaphoreSlim(0, 1);

.WithTrigger(trigger => trigger
    .Repeat()
    .Await(token => depositSignal.WaitAsync(token))
    .ThenInvoke(async (sm, token) => await sm.Transition(ScvTrigger.DepositComplete, token)))
```

### Full Example

Combining all features into a complete SCV harvesting state machine:

```csharp
enum ScvState { Idle, Harvesting, Returning }
enum ScvTrigger { Harvest, CargoFull, DepositComplete }

var depositSignal = new SemaphoreSlim(0, 1);

// Declared first so the action closure can reference the state machine
IStateMachine<ScvState, ScvTrigger> stateMachine = null!;

stateMachine = StateMachine
    .Configure<ScvState, ScvTrigger>()
    .WithInitialState(ScvState.Idle)

    // Idle - waiting for harvest command
    .WithState(ScvState.Idle, state => state
        .OnEntry(() => Console.WriteLine("SCV ready."))
        .WithTransition(ScvTrigger.Harvest, t => t
            .WithParameter<MineralPatch>()
            .If(patch => patch.HasMinerals)
            .To(ScvState.Harvesting)))

    // Harvesting - actively mining from the patch
    .WithState(ScvState.Harvesting, state => state
        .WithParameter<MineralPatch>()
        .OnEntry(patch => Console.WriteLine($"Harvesting from {patch.Id}..."))
        .WithAction(action => action
            .Invoke(async (patch, token) =>
            {
                await patch.HarvestAsync(token);
                await stateMachine.Transition(ScvTrigger.CargoFull, token);
            }))
        .OnExit(patch => Console.WriteLine($"Cargo full from {patch.Id}."))
        .WithTransition(ScvTrigger.CargoFull, ScvState.Returning))

    // Returning - heading back to command center
    .WithState(ScvState.Returning, state => state
        .OnEntry(() => Console.WriteLine("Returning cargo."))
        .WithTransition(ScvTrigger.DepositComplete, ScvState.Idle))

    // Trigger - automatically transitions when deposit is signaled
    .WithTrigger(trigger => trigger
        .Repeat()
        .Await(token => depositSignal.WaitAsync(token))
        .ThenInvoke(async (sm, token) => await sm.Transition(ScvTrigger.DepositComplete, token)))

    .Build();

// Start harvesting
await stateMachine.Activate();
await stateMachine.Transition(ScvTrigger.Harvest, nearestPatch);

// Later, when cargo has been deposited...
depositSignal.Release(); // Automatically transitions to Idle
```

## Documentation

For detailed documentation on each feature, see the [docs](docs/) folder.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
