# Exception Handling

> **Preview Notice:** Exception handling behavior is under active discussion and may change in a future release.
> See [Discussion #21](https://github.com/ZCrewSoftware/ZCrew.StateCraft/discussions/21) for details.

StateCraft provides a configurable exception handling mechanism for errors that occur during state lifecycle
operations. Exception handlers are configured at the machine level and are invoked for exceptions thrown in
`OnActivate`, `OnEntry`, `OnExit`, `OnStateChange`, `OnDeactivate`, actions, and triggers.

## OnException Handlers

Exception handlers are configured using `OnException` on the state machine configuration:

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .OnException(ex =>
    {
        logger.LogError(ex, "State machine error");
        return ExceptionResult.Continue();
    })
    .WithState(State.Idle, state => state
        .WithTransition(Transition.Start, State.Running))
    .WithState(State.Running, state => state)
    .Build();
```

Multiple handlers can be registered and are invoked in registration order. Each handler receives the exception
and returns an `ExceptionResult` that determines what happens next.

### Handler Signatures

Exception handlers support three signatures, consistent with all StateCraft handlers:

```csharp
// Synchronous
.OnException(ex => ExceptionResult.Continue())

// Async Task
.OnException(async (ex, token) =>
{
    await LogErrorAsync(ex, token);
    return ExceptionResult.Continue();
})

// Async ValueTask
.OnException((ex, token) =>
{
    LogError(ex);
    return ValueTask.FromResult(ExceptionResult.Continue());
})
```

### ExceptionResult

The `ExceptionResult` type controls what happens after a handler runs. It has three possible values:

| Value                              | Behavior                                                                                                                      |
|------------------------------------|-------------------------------------------------------------------------------------------------------------------------------|
| `ExceptionResult.Continue()`       | Passes the exception to the next handler. If no more handlers exist, the exception is rethrown with its original stack trace. |
| `ExceptionResult.Rethrow()`        | Immediately rethrows the original exception with its original stack trace. Remaining handlers are skipped.                    |
| `ExceptionResult.Throw(exception)` | Throws a different exception instead of the original. Remaining handlers are skipped.                                         |

`Continue()` is a safe default when you are unsure which value to use. It ensures every registered handler
is invoked and the exception is still thrown afterward.

### Example

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)

    // First handler: log everything
    .OnException(ex =>
    {
        logger.LogError(ex, "State machine error");
        return ExceptionResult.Continue();
    })

    // Second handler: wrap known exceptions
    .OnException(ex => ex switch
    {
        TimeoutException timeout => ExceptionResult.Throw(
            new StateTransitionException("Transition timed out", timeout)),

        InvalidOperationException => ExceptionResult.Rethrow(),

        _ => ExceptionResult.Continue()
    })

    .WithState(State.Idle, state => state
        .WithTransition(Transition.Start, State.Running))
    .WithState(State.Running, state => state
        .OnEntry(async token => await ConnectAsync(token)))
    .Build();
```

In this example, if `ConnectAsync` throws a `TimeoutException`:

1. The first handler logs the error and returns `Continue()`, so the next handler is invoked.
2. The second handler matches `TimeoutException` and returns `Throw(...)`, so a `StateTransitionException`
   is thrown instead of the original `TimeoutException`.

If the exception handler itself throws, that exception propagates immediately. Remaining handlers are not invoked.

## OperationCanceledException

`OperationCanceledException` is handled differently from other exceptions. When an `OperationCanceledException`
is thrown and the cancellation token provided to the operation has been canceled, the exception is **not** routed
through exception handlers. It is either rethrown or suppressed depending on the context.

| Context                                                                                 | Behavior                                                    |
|-----------------------------------------------------------------------------------------|-------------------------------------------------------------|
| Lifecycle handlers (`OnActivate`, `OnEntry`, `OnExit`, `OnStateChange`, `OnDeactivate`) | Rethrown without invoking exception handlers                |
| Triggers, state actions                                                                 | Suppressed during deactivation (expected cancellation path) |

This design treats cancellation as a normal control flow signal rather than an error. During transitions and
deactivation, the state machine cancels the action's cancellation token to interrupt long-running work. The
resulting `OperationCanceledException` is expected and suppressed:

```csharp
.WithState(State.Running, state => state
    .WithAction(action => action
        .Invoke(async token =>
        {
            // When a transition occurs, the token is canceled
            // and the resulting OperationCanceledException is suppressed
            await Task.Delay(Timeout.Infinite, token);
        })))
```

If an `OperationCanceledException` is thrown but the cancellation token has **not** been canceled, it is treated
as a regular exception and routed through exception handlers.
This happens if the work being performed is canceled through some other means and is probably a genuine error.

## Exceptions During Transitions

When a transition fails, the state machine attempts to roll back to the previous state. The behavior depends
on where in the transition the exception occurs. Here is the transition flow for reference:

1. Cancel and await previous state (State A) actions
2. `OnExit` - Clean up the previous state (State A)
3. `OnStateChange` (machine-level)
4. `OnStateChange` (state-level, State B)
5. `OnEntry` - Initialize the next state (State B)
6. Start next state (State B) actions

### Before or During OnStateChange

An exception during steps 1 through 4 means the state change has not yet been persisted by your handlers.

**When this can happen:**

- An `OnExit` handler throws while cleaning up the previous state
- A machine-level `OnStateChange` handler throws (e.g., persistence fails)
- A state-level `OnStateChange` handler throws

**What to expect:**

The state machine rolls back to the previous state and enters the `Recovery` state. The previous state's reference is
restored, but `OnEntry` is **not** called again and actions are **not** restarted. The state machine is in a degraded
state where the previous state's exit handlers have already run, but it has not been re-entered.

The exception is routed through `OnException` handlers before being rethrown to the caller.

**Corrective steps:**

Since the `OnStateChange` handlers did not complete, external state (databases, files) should not have been updated, or
may have been partially updated if the exception occurred mid-handler. You can:

- Attempt another transition to move the state machine to a valid state
- Deactivate the state machine

It is recommended to only have one `OnStateChange` that modifies external state. Performing the state change in a
transaction would guarantee the external state is rolled-back and can be retried.

```csharp
try
{
    await machine.Transition(Transition.Start, cancellationToken);
}
catch (Exception ex)
{
    logger.LogError(ex, "Transition failed before state change committed");

    // Attempt a recovery transition
    await machine.Transition(Transition.Reset, cancellationToken);
}
```

### After OnStateChange

An exception during steps 5 or 6 means the `OnStateChange` handlers have already completed. If your`OnStateChange`
handler persisted the state change to an external store, that write has already succeeded.

**When this can happen:**

- An `OnEntry` handler throws while initializing the next state
- An action throws synchronously during startup (before yielding)

**What to expect:**

The state machine does **not** roll back to the previous state. The current state is 'locked in'. `OnEntry` handlers and
actions may not have been called.

The exception is routed through `OnException` handlers before being rethrown to the caller.

**Corrective steps:**

Since the `OnStateChange` handlers completed successfully, external state has already been updated to reflect the new
state. You can:

- Attempt another transition to bring the state machine to a valid state
- Deactivate the state machine, optionally reload the external state, and reactivate the state machine

### Recovery State

After a failed transition, the state machine is in the `Recovery` state. In this state:

- The state machine can still accept new transitions
- The state machine can be deactivated
- The previous state was restored but was **not** re-entered — its `OnEntry` handlers were not called and its
  actions were not started

Design `OnExit`, `OnStateChange`, and `OnEntry` handlers to be idempotent where possible to simplify recovery:

```csharp
.WithState(State.Running, state => state
    .OnEntry(() =>
    {
        // Idempotent: safe if called again after recovery and re-transition
        _cache.AddOrUpdate(key, value);
    })
    .OnStateChange(async (from, transition, to, token) =>
    {
        // Idempotent: upsert rather than insert
        await repository.UpsertState(to, token);
    }))
```

## Exceptions During Activation

When `Activate()` fails, the behavior depends on whether the state machine had begun entering the initial state.

**Activation flow:**

1. State machine activator runs (resolves initial state)
2. `OnActivate` - Activate the initial state
3. Triggers are activated
4. `OnEntry` - Enter the initial state
5. Start initial state actions

If an exception occurs during steps 1 through 4 (before the state is fully entered), the state machine:

- Rolls back to `Inactive`
- Deactivates any triggers that were activated
- Rethrows the exception

The exception is routed through `OnException` handlers.

After a failed activation, the state machine is back in the `Inactive` state and `Activate()` can be called
again:

```csharp
try
{
    await machine.Activate(cancellationToken);
}
catch (Exception ex)
{
    logger.LogError(ex, "Activation failed");

    // The state machine is Inactive — retry is safe
    await machine.Activate(cancellationToken);
}
```

## Exceptions During Deactivation

When `Deactivate()` fails, the state machine enters the `Recovery` state.

**Deactivation flow:**

1. Cancel and await final state actions
2. `OnExit` - Clean up the final state
3. Triggers are deactivated
4. `OnDeactivate` - Deactivate the final state

If an exception occurs during any of these steps, the state machine:

- Rolls back to the previous state (restores `CurrentState`)
- Enters the `Recovery` state
- Rethrows the exception

The exception is routed through `OnException` handlers.

After a failed deactivation, the state machine is in `Recovery` with the previous state restored. You can
attempt to deactivate again or transition to a different state:

```csharp
try
{
    await machine.Deactivate(cancellationToken);
}
catch (Exception ex)
{
    logger.LogError(ex, "Deactivation failed");

    // The state machine is in Recovery — try deactivating again
    await machine.Deactivate(cancellationToken);
}
```

## Next Steps

- [State Lifecycle](./4-state-lifecycle.md) - Lifecycle handler documentation
- [Actions](./5-actions.md) - Long-running interruptible state work
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals
