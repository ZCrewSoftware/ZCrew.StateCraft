# Exception Behavior

> **Preview Notice:** Exception handling behavior is under active discussion and may change in a future release.
> See [Discussion #21](https://github.com/ZCrewSoftware/ZCrew.StateCraft/discussions/21) for details.

The `IExceptionBehavior` interface controls how each call site in the state machine — lifecycle handlers, conditions,
actions, triggers, and mapping functions — is wrapped with exception handling. By default, StateCraft provides a
`DefaultExceptionBehavior` that routes exceptions through `OnException` handlers. You can replace or extend this
behavior using `WithExceptionBehavior`.

## Configuring Exception Behavior

Use `WithExceptionBehavior` on the state machine configuration to provide a custom `IExceptionBehavior` implementation.
The provider function receives the registered `OnException` handlers and returns the behavior instance:

```csharp
var machine = StateMachine
    .Configure<State, Transition>()
    .WithInitialState(State.Idle)
    .WithExceptionBehavior(handlers => new CustomExceptionBehavior(handlers))
    .WithState(State.Idle, state => state
        .WithTransition(Transition.Start, State.Running))
    .WithState(State.Running, state => state)
    .Build();
```

The provider is called each time `Build()` is called, so each state machine instance receives its own
`IExceptionBehavior` instance. This is important when the behavior maintains state (e.g., counters or circuit
breakers).

## Default Behavior

When no custom behavior is configured, StateCraft uses `DefaultExceptionBehavior`. This class:

1. Wraps each call site (`OnEntry`, `OnExit`, `OnActivate`, `OnDeactivate`, `OnStateChange`, conditions, mapping,
   actions, and triggers) in a `try`/`catch` block.
2. Routes caught exceptions through the registered `OnException` handlers in order.
3. Handles `OperationCanceledException` specially — rethrown for lifecycle/conditions/mapping, suppressed for
   actions/triggers (see [Exception Handling](./11-exception-handling.md#operationcanceledexception)).

This is the same behavior described in the [Exception Handling](./11-exception-handling.md) documentation.

## Overriding the Default Behavior

`DefaultExceptionBehavior` has `virtual` methods, so you can subclass it to override behavior for specific call sites
without reimplementing everything:

```csharp
public class LoggingExceptionBehavior : DefaultExceptionBehavior
{
    private readonly ILogger logger;

    public LoggingExceptionBehavior(
        IEnumerable<IAsyncFunc<Exception, ExceptionResult>> onExceptionHandlers,
        ILogger logger)
        : base(onExceptionHandlers)
    {
        this.logger = logger;
    }

    public override async Task CallOnEntry(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        logger.LogDebug("Entering state");
        await base.CallOnEntry(handler, token);
    }
}
```

Register it with the provider:

```csharp
.WithExceptionBehavior(handlers => new LoggingExceptionBehavior(handlers, logger))
```

### Overridable Methods

| Method             | Call Site                           |
|--------------------|-------------------------------------|
| `CallOnActivate`   | State `OnActivate` handler          |
| `CallOnEntry`      | State `OnEntry` handler             |
| `CallOnExit`       | State `OnExit` handler              |
| `CallOnDeactivate` | State `OnDeactivate` handler        |
| `CallOnStateChange`| `OnStateChange` handler             |
| `CallCondition`    | Transition condition evaluation     |
| `CallMap`          | Mapped transition mapping function  |
| `CallAction`       | State action invocation             |
| `CallTrigger`      | Trigger invocation                  |

All methods are `virtual` and can be overridden independently.

## Implementing IExceptionBehavior

For full control, implement `IExceptionBehavior` directly:

```csharp
public class CircuitBreakerBehavior : IExceptionBehavior
{
    private int failureCount;

    public async Task CallOnEntry(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        try
        {
            await handler(token);
            Interlocked.Exchange(ref failureCount, 0);
        }
        catch (Exception)
        {
            Interlocked.Increment(ref failureCount);
            throw;
        }
    }

    // ... implement remaining methods
}
```

The registered `OnException` handlers are passed to the provider but are not required to be used. A custom
implementation can ignore them entirely and handle exceptions through its own mechanism.

## Best Practices

### Use OnException for Exception Handling

`WithExceptionBehavior` is intended for cross-cutting infrastructure concerns — observability, circuit breaking, or
wrapping call sites with custom execution logic. For handling exceptions themselves (logging, rethrowing, wrapping),
use `OnException` handlers instead:

```csharp
// Good: Use OnException for handling exceptions
.OnException(ex =>
{
    logger.LogError(ex, "State machine error");
    return ExceptionResult.Continue();
})

// Avoid: Using WithExceptionBehavior just to handle exceptions
.WithExceptionBehavior(handlers => new MyBehaviorThatJustLogsAndRethrows(handlers))
```

`OnException` handlers are simpler to write, composable (multiple handlers can be registered), and work with
the default behavior out of the box.

### Suppressing Exceptions May Skip Lifecycle Events

If a custom `IExceptionBehavior` suppresses an exception (catches it without rethrowing), the state machine may
end up in an inconsistent state. For example, if `CallOnEntry` suppresses an exception thrown by an `OnEntry`
handler, the state machine will consider the entry successful even though the handler did not complete. Subsequent
lifecycle events (transitions out of that state, `OnExit`, etc.) will proceed as if entry succeeded. 

The same applies to other call sites:

- Suppressing `CallOnExit` may leave a state that was never properly cleaned up.
- Suppressing `CallOnStateChange` may cause external state to diverge from the state machine.
- Suppressing `CallCondition` may cause a transition to proceed when it should not have.
- Suppressing `CallMap` may cause a transition to proceed with default or stale parameter values.

Only suppress exceptions when you are certain the state machine can safely continue.

## Next Steps

- [Exception Handling](./11-exception-handling.md) - OnException handlers and ExceptionResult
- [State Lifecycle](./4-state-lifecycle.md) - Lifecycle handler documentation
- [Actions](./5-actions.md) - Long-running interruptible state work
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals