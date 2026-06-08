# Migrating from v1 to v2

## Breaking Changes

### Inverted ("From All") transitions

- **`From()` return type.** On a parameterized state, `From()` now returns the typed
  `IFromTransitionConfiguration<TState, TTransition, T…>` instead of the non-generic
  `IFromTransitionConfiguration<TState, TTransition>` (parameterless states are unchanged). The fluent
  chain still compiles; only explicitly typed variables or method return types need updating.
- **`IFromAllStatesTransitionConfiguration<TState, TTransition>`** gained three `OnTransition(...)`
  overloads (`Action`, `Func<CancellationToken, Task>`, `Func<CancellationToken, ValueTask>`).
  Custom implementations of this interface must add them.
