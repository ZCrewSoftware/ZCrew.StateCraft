# PlantUML Diagrams

The `ZCrew.StateCraft.PlantUml` package renders a state machine configuration as a
[PlantUML state diagram](https://plantuml.com/state-diagram), so the diagram never drifts from the code. For
Mermaid output instead, see [Mermaid Diagrams](./mermaid-diagrams.md).

## Installation

Add a reference to the package alongside the core one:

```xml
<PackageReference Include="ZCrew.StateCraft" />
<PackageReference Include="ZCrew.StateCraft.PlantUml" />
```

Then add the namespace:

```csharp
using ZCrew.StateCraft.PlantUml;
```

## Rendering a Diagram

`ToPlantUmlDiagram()` is an extension on `IStateMachineConfiguration<TState, TTransition>`, so you render straight
from the configuration — there is no need to `Build()` first:

```csharp
enum State { Idle, Running, Finished }
enum Transition { Start, Complete }

static bool QueueIsHealthy() => true;

var configuration = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithState(State.Idle, state => state
        .WithTransition(Transition.Start, t => t
            .If(QueueIsHealthy)
            .To(State.Running)))
    .WithState(State.Running, state => state
        .WithTransition(Transition.Complete, State.Finished))
    .WithState(State.Finished, state => state);

var diagram = configuration.ToPlantUmlDiagram();
```

`diagram` is a `string`. Write it to a `.puml` file, embed it in a Markdown document, or paste it into the
[PlantUML web server](https://www.plantuml.com/plantuml/uml/).

### Sample Output

The configuration above renders as:

````text
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
````

## The Start Marker

PlantUML marks the starting state with `[*] --> State`. It is emitted whenever the initial state is known up
front:

```csharp
// Renders: [*] --> Idle
.WithInitialState(State.Idle)
```

It is left out when there is nothing to point at:

- `WithInitialState(() => ...)` resolves when the machine activates, so the value is unknown while rendering.
- The initial state names a state that was never configured — something
  `Build(StateMachineBuildOptions.Validate)` would reject.

## Node Aliases

Each state renders as `state "<label>" as <alias>`. The label is the display name; the alias is the identifier
PlantUML uses for the node, limited to letters, digits, and underscores:

```text
state "Idle" as Idle
state "Working<U+003C>int<U+003E>" as Working_int
state "Order.Placed" as Order_Placed
```

Any other character becomes `_`, a leading digit gets an `_` prefix, and aliases that would collide are suffixed
`_2`, `_3`, and so on. A state's identity includes its parameter types, so `Working<int>` and `Working<string>`
are separate nodes.

## Options

`ToPlantUmlDiagram` has three overloads — defaults, an options instance, or a configure callback:

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

### `Direction`

The layout direction emitted near the top of the diagram.

| Value         | PlantUML token            | Description                          |
|---------------|---------------------------|--------------------------------------|
| `TopToBottom` | `top to bottom direction` | States flow top to bottom (default). |
| `LeftToRight` | `left to right direction` | States flow left to right.           |

### `Newline`

How newlines inside a descriptor are rendered. PlantUML does not allow raw newlines in a label, so they have to
become something else:

| Value       | Behavior                                                                   |
|-------------|----------------------------------------------------------------------------|
| `Ignore`    | Strip them; the surrounding text runs together (default).                  |
| `Space`     | Replace each one with a single space.                                      |
| `LineBreak` | Replace each one with PlantUML's `\n` escape so the label spans lines.     |

Use `LineBreak` when a descriptor carries multi-line text worth keeping — a block-bodied lambda condition, for
example.

### Other Encoding

Descriptors are always escaped, whatever the options:

- `<` and `>` become `<U+003C>` and `<U+003E>`. PlantUML reads `<b>`, `<code>`, `<size:…>` and friends as
  formatting tags, so a parameter type named after one would otherwise vanish. The escape looks noisy in the raw
  text but renders as a plain `<`.
- The second and later spaces in a run become `<U+00A0>`, since PlantUML would collapse them.

Conditions are always joined onto the transition label with `\n`. The `Newline` option only affects newlines
inside a descriptor's own text.

## Tips

### Name Your Conditions

`If(...)` captures the condition's source text by default, via `[CallerArgumentExpression]`. A method group or
property reads well on its own:

```csharp
.If(QueueIsHealthy)     // If: QueueIsHealthy
.If(policy.CanCancel)   // If: policy.CanCancel
```

A large lambda does not, so pass a descriptor instead:

```csharp
.If(() => /* several lines of conditions... */, "ready to process")
```

That renders as `If: ready to process` rather than the whole expression.

### Inverted Transitions Fan Out

An [inverted transition](./inverted-transitions.md) is declared once but emits one edge per source state. A global
`Reset` across many states makes for a busy diagram — use `Except(...)` to trim the sources that do not matter.
