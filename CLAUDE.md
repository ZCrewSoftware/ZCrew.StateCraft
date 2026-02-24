# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Build all projects
dotnet build
```

## Project Structure

- `src/ZCrew.StateCraft/` - Main library (targets net8.0, net9.0, net10.0)
- `tests/ZCrew.StateCraft.UnitTests/` - Unit tests with NSubstitute mocks
- `tests/ZCrew.StateCraft.IntegrationTests/` - Integration tests for full state machine behavior
- `docs/` - User-facing documentation (numbered markdown files)

## Architecture

StateCraft is a fluent async state machine library. The entry point is `StateMachine.Configure<TState, TTransition>()`.

**Key Components:**

- **StateMachine** (`StateMachine.cs`) - Static entry point for fluent configuration
- **StateMachineConfiguration** (`StateMachines/StateMachineConfiguration.cs`) - Builds state machines from configuration
- **StateMachine<TState, TTransition>** (`StateMachines/StateMachine.cs`) - Runtime state machine with thread-safe transitions using AsyncLock

**State/Transition Pattern:**
- States and transitions are configured via `IStateConfiguration` interfaces in `States/Configuration/`
- Parameterless states (`ParameterlessState`) and parameterized states (`ParameterizedState<T>`, `State<T1,T2>`, `State<T1,T2,T3>`, `State<T1,T2,T3,T4>`) in `States/`
- States support up to 4 typed parameters configured via `WithParameter<T>()` (single) or `WithParameters<T1,T2>()` etc. (multi)
- Transitions can be parameterless, parameterized, mapped (type conversion), or reentrant (same parameter) in `Transitions/`
- All handlers (lifecycle, actions, triggers, conditions) support three signatures: sync (`Action`), async `Task`, and async `ValueTask`

**Internal State Flow:**
The state machine tracks internal state through: `Inactive -> Idle -> Active` with transition states: `Exiting -> Exited -> Transitioning -> Transitioned -> Entering -> Entered`. A `Recovery` state handles rollback on failures.

**Validation:** Build-time validators in `Validation/` check for duplicate states, unreachable transitions, and invalid transition targets. Enable with `Build(StateMachineBuildOptions.Validate)`.

**Configuration Reuse:** `IStateMachineConfiguration` is reusable; each `Build()` creates an independent state machine instance.

**Exception Handling:** `IExceptionBehavior` (`Exceptions/Contracts/`) wraps each call site (lifecycle, conditions, actions, triggers, mapping) so implementations can intercept exceptions. `DefaultExceptionBehavior` (`Exceptions/`) is used by default and routes caught exceptions through registered `OnException` handlers that return an `ExceptionResult` (`Continue`, `Rethrow`, or `Throw`). Custom implementations can be provided via `.WithExceptionBehavior(handlers => ...)` on the configuration; the provider is called per `Build()`. `DefaultExceptionBehavior` has `virtual` methods and can be subclassed. `OperationCanceledException` when the token is canceled bypasses handlers — rethrown for lifecycle/conditions/mapping, suppressed for actions/triggers.

**Triggers:** Autonomous transition initiators (`RunOnceTrigger`, `RepeatingTrigger`) configured at the machine level (not on individual states) that activate/deactivate with the state machine.

## API Quick Reference

**Runtime methods on `IStateMachine<TState, TTransition>`:**
- `Activate()` / `Deactivate()` - start/stop the state machine (not StartAsync/StopAsync)
- `Transition(transition)` / `Transition(transition, param)` / `Transition<T1,T2>(transition, p1, p2)` etc. up to 4 params
- `CanTransition(transition)` / `CanTransition<T1,T2>(transition, p1, p2)` etc. up to 4 params
- `TryTransition(transition)` / `TryTransition<T1,T2>(transition, p1, p2)` etc. up to 4 params

**Configuration patterns:**
- Initial state (required): `.WithInitialState(State.X)` or `.WithInitialState<T>(State.X, param)` or `.WithInitialState<T1,T2>(State.X, p1, p2)` etc.
- Single-parameter states: `.WithState(State.X, s => s.WithParameter<T>()...)` (no generic on WithState)
- Multi-parameter states: `.WithState(State.X, s => s.WithParameters<T1,T2>()...)` (up to 4 type params)
- Parameterless transitions: `.WithTransition(Trigger.X, t => t.To(State.Y))` (`WithNoParameters()` is implied)
- Single-param transitions: `.WithTransition(Trigger.X, t => t.WithParameter<T>().If(...).To(...))`
- Multi-param transitions: `.WithTransition(Trigger.X, t => t.WithParameters<T1,T2>().To(...))` (up to 4)
- Mapped transitions (single): `.WithTransition(Trigger.X, t => t.WithMappedParameter<TNext>(prev => ...).To(State.Y))`
- Mapped transitions (multi): `.WithTransition(Trigger.X, t => t.WithMappedParameters<T1,T2>((prev) => ...).To(State.Y))`
- Reentrant transitions (single): `.WithTransition(Trigger.X, t => t.WithSameParameter().To(State.Y))`
- Reentrant transitions (multi): `.WithTransition(Trigger.X, t => t.WithSameParameters().To(State.Y))`
- Transition shortcuts: `.WithTransition(Trigger.X, State.Y)` — always implies `WithNoParameters()` regardless of source state parameters
- Actions (parameterless): `.WithAction(a => a.Invoke(async token => ...))`
- Actions (parameterized): `.WithAction(a => a.Invoke(async (param, token) => ...))` — receives all state parameters
- Async actions: `.WithAsynchronousActions()` — makes `Transition` return before actions complete
- Exception behavior: `.WithExceptionBehavior(handlers => new CustomBehavior(handlers))` — replaces default `IExceptionBehavior`; provider called per `Build()`
- Exception handlers: `.OnException(ex => ExceptionResult.Continue())` — registers handler invoked by the exception behavior
- Build: `.Build()` or `.Build(StateMachineBuildOptions.Validate)` to validate configuration

**Trigger configuration (on `IStateMachineConfiguration`, not state):**
```csharp
.WithTrigger(trigger => trigger
    .Once()  // or .Repeat()
    .Await(token => signal.WaitAsync(token))
    .ThenInvoke(sm => sm.Transition(Trigger.X)))
```

## Code Style

- Uses CSharpier for formatting (enforced via pre-commit hook and does not ever need to be ran manually)
- C# 14.0 language version with nullable reference types enabled
- Test projects use xUnit v3 with NSubstitute for mocking
- Internal classes are visible to test projects via `InternalsVisibleTo`

**XML Documentation:**
- XML documentation should be indented (4 spaces for content within tags)
- Lines should not exceed 120 characters; wrap long descriptions across multiple lines
- Always document `CancellationToken` parameters as: `<param name="token">The token to monitor for cancellation requests.</param>`
