# Exception Behavior

The `IExceptionBehavior` interface controls how each call site in the state machine — lifecycle handlers, conditions,
actions, triggers, and mapping functions — is wrapped with exception handling. By default, StateCraft provides a
`RethrowExceptionBehavior` that routes exceptions through `OnException` handlers and then rethrows. You can replace
or extend this behavior using `WithExceptionBehavior`.

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

When no custom behavior is configured, StateCraft uses `RethrowExceptionBehavior`. This class:

1. Wraps each call site (`OnEntry`, `OnExit`, `OnActivate`, `OnDeactivate`, `OnStateChange`, conditions, mapping,
   actions, and triggers) in a `try`/`catch` block.
2. Routes caught exceptions through the registered `OnException` handlers in order, providing an `ExceptionContext`
   that includes the exception and the call site where it was thrown.
3. After all handlers have been called, rethrows the original exception with its original stack trace.
4. Handles `OperationCanceledException` specially — rethrown for lifecycle/conditions/mapping, suppressed for
   actions/triggers (see [Exception Handling](./11-exception-handling.md#operationcanceledexception)).

This is the same behavior described in the [Exception Handling](./11-exception-handling.md) documentation.

## Overriding the Default Behavior

`RethrowExceptionBehavior` has `virtual` methods, so you can subclass it to override behavior for specific call sites
without reimplementing everything:

```csharp
public class LoggingExceptionBehavior : RethrowExceptionBehavior
{
    private readonly ILogger logger;

    public LoggingExceptionBehavior(
        IEnumerable<IAsyncAction<ExceptionContext>> onExceptionHandlers,
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

You can also override the `OnException` and `OnException<T>` methods to customize how exception handlers are invoked.
For example, to suppress exceptions for a specific call site:

```csharp
public class SuppressActionExceptionBehavior : RethrowExceptionBehavior
{
    public SuppressActionExceptionBehavior(
        IEnumerable<IAsyncAction<ExceptionContext>> onExceptionHandlers)
        : base(onExceptionHandlers) { }

    protected override async Task OnException(ExceptionContext exceptionContext, CancellationToken token)
    {
        // Call handlers but don't rethrow for action exceptions
        if (exceptionContext.CallSite == ExceptionCallSite.Action)
        {
            foreach (var handler in OnExceptionHandlers)
            {
                await handler.InvokeAsync(exceptionContext, token);
            }

            return;
        }

        await base.OnException(exceptionContext, token);
    }
}
```

### Overridable Methods

| Method              | Call Site                                                                 |
|---------------------|---------------------------------------------------------------------------|
| `CallOnActivate`    | State `OnActivate` handler                                                |
| `CallOnEntry`       | State `OnEntry` handler                                                   |
| `CallOnExit`        | State `OnExit` handler                                                    |
| `CallOnDeactivate`  | State `OnDeactivate` handler                                              |
| `CallOnStateChange` | `OnStateChange` handler                                                   |
| `CallCondition`     | Transition condition evaluation                                           |
| `CallMap`           | Mapped transition mapping function                                        |
| `CallAction`        | State action invocation                                                   |
| `CallTrigger`       | Trigger invocation                                                        |
| `OnException`       | Exception handler invocation                                              |
| `OnException<T>`    | Exception handler invocation (with return value, used by `CallCondition`) |

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
.OnException(ctx =>
{
    logger.LogError(ctx.Exception, "State machine error at {CallSite}", ctx.CallSite);
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

- [Exception Handling](./11-exception-handling.md) - OnException handlers and ExceptionContext
- [State Machine Lifecycle](./4-state-machine-lifecycle.md) - Lifecycle handler documentation
- [Actions](./5-actions.md) - Long-running interruptible state work
- [Triggers](./10-triggers.md) - Autonomous transitions based on signals
