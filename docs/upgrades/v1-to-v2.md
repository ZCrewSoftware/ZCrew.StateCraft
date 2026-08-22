# Migrating from v1 to v2

## Breaking Changes

### Initial state must be configured first

- **`StateMachine.Configure<TState, TTransition>()` now returns
  `IInitialStateMachineConfiguration<TState, TTransition>`.** This interface only exposes the
  `WithInitialState(...)` overloads. The rest of the fluent API (`WithState`, `WithTransition`,
  `WithTrigger`, `OnStateChange`, `Build`, …) lives on `IStateMachineConfiguration<TState, TTransition>`,
  which you only receive *after* calling `WithInitialState(...)`. Configuration order now matters: the
  initial state must be specified first before the state machine can be configured further. Move any
  `WithInitialState(...)` call to the front of the chain.

  ```csharp
  // v2 — WithInitialState(...) must come first
  var stateMachine = StateMachine
      .Configure<MarineState, MarineTransition>()
      .WithInitialState(MarineState.Idle)
      .WithState(MarineState.Idle, state => /* ... */)
      .Build();
  ```

### Inverted ("From All") transitions

- **`From()` return type.** On a parameterized state, `From()` now returns the typed
  `IFromTransitionConfiguration<TState, TTransition, T…>` instead of the non-generic
  `IFromTransitionConfiguration<TState, TTransition>` (parameterless states are unchanged). The fluent
  chain still compiles; only explicitly typed variables or method return types need updating.
- **`IFromAllStatesTransitionConfiguration<TState, TTransition>`** gained three `OnTransition(...)`
  overloads (`Action`, `Func<CancellationToken, Task>`, `Func<CancellationToken, ValueTask>`).
  Custom implementations of this interface must add them.

### Removed Legacy Exception Handler

- The exception handler was marked `[Obsolete]` for a few releases now. It is finally removed.
