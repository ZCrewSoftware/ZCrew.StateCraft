using ZCrew.StateCraft.Tracking.Contracts;

namespace ZCrew.StateCraft.Tracking;

/// <summary>
///     Wraps another <see cref="IExceptionBehavior"/> and reports every handler failure to a tracker before the inner
///     behavior decides what to do with it.
/// </summary>
/// <remarks>
///     The handler itself is wrapped rather than the inner behavior, so the exception is observed at the point it is
///     thrown and then rethrown untouched. That keeps the inner behavior's semantics intact and works for any
///     implementation, including custom ones.
/// </remarks>
/// <typeparam name="TState">The type representing state identifiers.</typeparam>
/// <typeparam name="TTransition">The type representing transition identifiers.</typeparam>
internal sealed class TrackingExceptionBehavior<TState, TTransition> : IExceptionBehavior
    where TState : notnull
    where TTransition : notnull
{
    private readonly ITracker<TState, TTransition> tracker;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TrackingExceptionBehavior{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="inner">The behavior to delegate to.</param>
    /// <param name="tracker">The tracker to report handler failures to.</param>
    public TrackingExceptionBehavior(IExceptionBehavior inner, ITracker<TState, TTransition> tracker)
    {
        Inner = inner;
        this.tracker = tracker;
    }

    /// <summary>
    ///     The behavior this one delegates to. Decorating is transparent to the machine, so this is how the
    ///     configured behavior is recovered.
    /// </summary>
    public IExceptionBehavior Inner { get; }

    /// <inheritdoc />
    public Task CallOnEntry(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Inner.CallOnEntry(t => Report(handler, ExceptionCallSite.OnEntry, t), token);
    }

    /// <inheritdoc />
    public Task CallOnExit(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Inner.CallOnExit(t => Report(handler, ExceptionCallSite.OnExit, t), token);
    }

    /// <inheritdoc />
    public Task CallOnStateChange(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Inner.CallOnStateChange(t => Report(handler, ExceptionCallSite.OnStateChange, t), token);
    }

    /// <inheritdoc />
    public Task CallOnActivate(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Inner.CallOnActivate(t => Report(handler, ExceptionCallSite.OnActivate, t), token);
    }

    /// <inheritdoc />
    public Task CallOnDeactivate(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Inner.CallOnDeactivate(t => Report(handler, ExceptionCallSite.OnDeactivate, t), token);
    }

    /// <inheritdoc />
    public Task CallOnTransition(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Inner.CallOnTransition(t => Report(handler, ExceptionCallSite.OnTransition, t), token);
    }

    /// <inheritdoc />
    public Task<bool> CallCondition(Func<CancellationToken, Task<bool>> handler, CancellationToken token = default)
    {
        return Inner.CallCondition(t => Report(handler, ExceptionCallSite.Condition, t), token);
    }

    /// <inheritdoc />
    public Task CallMap(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Inner.CallMap(t => Report(handler, ExceptionCallSite.Map, t), token);
    }

    /// <inheritdoc />
    public Task CallAction(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Inner.CallAction(t => Report(handler, ExceptionCallSite.Action, t), token);
    }

    /// <inheritdoc />
    public Task CallTrigger(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Inner.CallTrigger(t => Report(handler, ExceptionCallSite.Trigger, t), token);
    }

    private async Task Report(
        Func<CancellationToken, Task> handler,
        ExceptionCallSite callSite,
        CancellationToken token
    )
    {
        try
        {
            await handler(token);
        }
        catch (Exception exception)
        {
            this.tracker.HandlerFailed(callSite, exception);
            throw;
        }
    }

    private async Task<T> Report<T>(
        Func<CancellationToken, Task<T>> handler,
        ExceptionCallSite callSite,
        CancellationToken token
    )
    {
        try
        {
            return await handler(token);
        }
        catch (Exception exception)
        {
            this.tracker.HandlerFailed(callSite, exception);
            throw;
        }
    }
}
