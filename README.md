# ZCrew.StateCraft

A fluent, async-first state machine library for .NET. StateCraft provides a clean, type-safe API for defining states, transitions, and lifecycle handlers with full support for parameterized states, conditional transitions, and cancellation.

## Features

- **Fluent Configuration API** - Intuitive builder pattern for defining state machines
- **Async/Await Support** - First-class support for async lifecycle handlers and actions
- **Parameterized States** - States can carry typed data that flows through entry, action, and exit handlers
- **Conditional Transitions** - Guard conditions that determine if a transition should proceed
- **Mapped Transitions** - Transform parameters during transitions between states
- **Lifecycle Handlers** - OnEntry, OnExit, and OnStateChange hooks for state management
- **Triggers** - Autonomous transition initiators (one-shot and repeating) that activate with the state machine
- **Actions** - Long-running actions that state performs, which can transition the state machine
- **Thread-Safe** - Internal locking ensures safe concurrent access
- **Validation** - Detects configuration errors like duplicate states and invalid transitions

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

States can run async work while active:

```csharp
.WithState(ScvState.Harvesting, state => state
    .WithParameter<MineralPatch>()
    .WithAction(action => action
        .Invoke(async (patch, token) => await patch.HarvestAsync(token))))
```

### Triggers

Triggers automatically fire transitions based on async signals:

```csharp
.WithTrigger(trigger => trigger
    .Repeat()
    .Await(token => cargoFullSignal.WaitAsync(token))
    .ThenInvoke(async (sm, token) => await sm.Transition(ScvTrigger.CargoFull, token)))
```

### Full Example

Combining all features into a complete SCV harvesting state machine:

```csharp
enum ScvState { Idle, Harvesting, Returning }
enum ScvTrigger { Harvest, CargoFull, DepositComplete }

// Some other service has to set this but the idea is there
var cargoFullSignal = new AsyncManualResetEvent();

var stateMachine = StateMachine
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
            .Invoke(async (patch, token) => await patch.HarvestAsync(token)))
        .OnExit(patch => Console.WriteLine($"Cargo full from {patch.Id}."))
        .WithTransition(ScvTrigger.CargoFull, ScvState.Returning))

    // Returning - heading back to command center
    .WithState(ScvState.Returning, state => state
        .OnEntry(() => Console.WriteLine("Returning cargo."))
        .OnExit(() => cargoFullSignal.Reset())
        .WithTransition(ScvTrigger.DepositComplete, ScvState.Idle))

    // Trigger to automatically transition the machine
    .WithTrigger(trigger => trigger
        .Repeat()
        .Await(token => cargoFullSignal.WaitAsync(token))
        .ThenInvoke(async (sm, token) => await sm.Transition(ScvTrigger.CargoFull, token)))

    .Build();

// Start harvesting
await stateMachine.Activate();
await stateMachine.Transition(ScvTrigger.Harvest, nearestPatch);

// Later, when cargo is full...
cargoFullSignal.Set(); // Automatically transitions to Returning
```

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
