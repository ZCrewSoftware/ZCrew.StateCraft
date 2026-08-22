# ZCrew.StateCraft.PlantUml

A PlantUML renderer for [ZCrew.StateCraft](https://www.nuget.org/packages/ZCrew.StateCraft). Render any state machine configuration as a [PlantUML state diagram](https://plantuml.com/state-diagram), so the diagram never drifts from the code.

## Features

- **One-line rendering** - `ToPlantUmlDiagram()` works straight off a configuration; no `Build()` needed
- **[Start marker](https://github.com/ZCrewSoftware/ZCrew.StateCraft/blob/main/docs/plantuml-diagrams.md#the-start-marker)** - Emits `[*] --> State` when the initial state is known up front
- **[Layout direction](https://github.com/ZCrewSoftware/ZCrew.StateCraft/blob/main/docs/plantuml-diagrams.md#direction)** - Top-to-bottom or left-to-right
- **[Newline handling](https://github.com/ZCrewSoftware/ZCrew.StateCraft/blob/main/docs/plantuml-diagrams.md#newline)** - Strip newlines, turn them into spaces, or render them as `\n` line breaks
- **Condition descriptors** - Guarded transitions render with `If` / `And` clauses
- **Parameterized states** - Type parameters appear in the node alias and its label
- **PlantUML-safe escaping** - Angle brackets and runs of spaces are escaped, and node aliases are sanitized and deduplicated

## Installation

This package is available on NuGet as `ZCrew.StateCraft.PlantUml` for these frameworks:

- .NET 8.0
- .NET 9.0
- .NET 10.0

```xml
<PackageReference Include="ZCrew.StateCraft" Version="1.0.0" />
<PackageReference Include="ZCrew.StateCraft.PlantUml" Version="1.0.0" />
```

## Quick Start

`ToPlantUmlDiagram()` is an extension on `IStateMachineConfiguration<TState, TTransition>`:

```csharp
using ZCrew.StateCraft;
using ZCrew.StateCraft.PlantUml;

enum State { Idle, Running, Finished }
enum Trigger { Start, Complete }

static bool QueueIsHealthy() => true;

var configuration = StateMachine
    .Configure<State, Trigger>()
    .WithInitialState(State.Idle)
    .WithState(State.Idle, state => state
        .WithTransition(Trigger.Start, t => t
            .If(QueueIsHealthy)
            .To(State.Running)))
    .WithState(State.Running, state => state
        .WithTransition(Trigger.Complete, State.Finished))
    .WithState(State.Finished, state => state);

var diagram = configuration.ToPlantUmlDiagram();
```

`diagram` is a `string` — write it to a `.puml` file, embed it in a Markdown document, or paste it into the [PlantUML web server](https://www.plantuml.com/plantuml/uml/).

### Sample Output

The configuration above renders as:

```text
@startuml
title State Machine

top to bottom direction

state "Idle" as Idle
state "Running" as Running
state "Finished" as Finished

[*] --> Idle
Idle --> Running : Start\nIf: QueueIsHealthy
Running --> Finished : Complete
@enduml
```

### Options

Three overloads — defaults, a `PlantUmlOptions` instance, or a configure callback:

```csharp
// Defaults
configuration.ToPlantUmlDiagram();

// Explicit options instance
configuration.ToPlantUmlDiagram(new PlantUmlOptions
{
    Direction = PlantUmlDirection.LeftToRight,
    Newline = PlantUmlNewline.LineBreak,
});

// Configure callback against a fresh options instance
configuration.ToPlantUmlDiagram(options =>
{
    options.Direction = PlantUmlDirection.LeftToRight;
    options.Newline = PlantUmlNewline.LineBreak;
});
```

### Readable Conditions

`If(...)` captures the condition's source text by default. Method groups and properties read well on their own; for a large lambda, pass a descriptor instead:

```csharp
.If(() => /* several lines of conditions... */, "ready to process")
```

That renders as `If: ready to process` rather than the whole expression.

## Documentation

See [PlantUML Diagrams](https://github.com/ZCrewSoftware/ZCrew.StateCraft/blob/main/docs/plantuml-diagrams.md) for the full documentation.

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/ZCrewSoftware/ZCrew.StateCraft/blob/main/LICENSE.md) file for details.
