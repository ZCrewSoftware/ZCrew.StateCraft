using System.Runtime.ExceptionServices;
using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft;

/// <inheritdoc />
/// <remarks>
///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when the token is canceled.
///     With <see cref="CallAction"/> or <see cref="CallTrigger"/> that exception is suppressed since it is how those
///     background tasks are stopped. For other methods that exception is rethrown and the handlers are not called.
/// </remarks>
public class RethrowExceptionBehavior : IExceptionBehavior
{
    /// <summary>
    ///     The list of exception handlers passed in.
    /// </summary>
    protected readonly IReadOnlyList<IAsyncAction<ExceptionContext>> OnExceptionHandlers;

    /// <summary>
    ///     Constructs a new instance with the <paramref name="onExceptionHandlers"/> which will be called first-to-last
    ///     when an exception is thrown.
    /// </summary>
    /// <param name="onExceptionHandlers">The handlers to invoke when an exception is thrown.</param>
    public RethrowExceptionBehavior(IEnumerable<IAsyncAction<ExceptionContext>> onExceptionHandlers)
    {
        this.OnExceptionHandlers = onExceptionHandlers.ToArray();
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnEntry(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, ExceptionCallSite.OnEntry, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnExit(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, ExceptionCallSite.OnExit, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnStateChange(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, ExceptionCallSite.OnStateChange, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnActivate(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, ExceptionCallSite.OnActivate, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnDeactivate(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, ExceptionCallSite.OnDeactivate, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task<bool> CallCondition(
        Func<CancellationToken, Task<bool>> handler,
        CancellationToken token = default
    )
    {
        return Execute(handler, ExceptionCallSite.Condition, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallMap(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, ExceptionCallSite.Map, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception is suppressed.
    /// </remarks>
    public virtual Task CallAction(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return ExecuteCancelable(handler, ExceptionCallSite.Action, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The special case is <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception is suppressed.
    /// </remarks>
    public virtual Task CallTrigger(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return ExecuteCancelable(handler, ExceptionCallSite.Trigger, token);
    }

    /// <summary>
    ///     Invokes all exception handlers in-order. If any handler throws an exception then the remaining handlers will
    ///     not be called. This is called by all call sites except <see cref="CallCondition"/>.
    /// </summary>
    /// <param name="exceptionContext">The context including the call site and exception.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    protected virtual async Task OnException(ExceptionContext exceptionContext, CancellationToken token)
    {
        foreach (var handler in this.OnExceptionHandlers)
        {
            await handler.InvokeAsync(exceptionContext, token);
        }

        // After calling each handler, rethrow
        exceptionContext.ThrowException();
    }

    /// <summary>
    ///     Invokes all exception handlers in-order. If any handler throws an exception then the remaining handlers will
    ///     not be called. This is called by <see cref="CallCondition"/>.
    /// </summary>
    /// <param name="exceptionContext">The context including the callsite and exception.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    protected virtual async Task<T> OnException<T>(ExceptionContext exceptionContext, CancellationToken token)
    {
        foreach (var handler in this.OnExceptionHandlers)
        {
            await handler.InvokeAsync(exceptionContext, token);
        }

        // After calling each handler, rethrow
        exceptionContext.ThrowException();

        // Never hit, the above call always throws
        return default;
    }

    private async Task Execute(
        Func<CancellationToken, Task> handler,
        ExceptionCallSite callSite,
        CancellationToken token
    )
    {
        try
        {
            await handler(token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Skip calling handlers but still throw
            throw;
        }
        catch (Exception ex)
        {
            var exceptionContext = new ExceptionContext(ExceptionDispatchInfo.Capture(ex), callSite);
            await OnException(exceptionContext, CancellationToken.None);
        }
    }

    private async Task ExecuteCancelable(
        Func<CancellationToken, Task> handler,
        ExceptionCallSite callSite,
        CancellationToken token
    )
    {
        try
        {
            await handler(token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Suppress as this was expected
        }
        catch (Exception ex)
        {
            var exceptionContext = new ExceptionContext(ExceptionDispatchInfo.Capture(ex), callSite);
            await OnException(exceptionContext, CancellationToken.None);
        }
    }

    private async Task<T> Execute<T>(
        Func<CancellationToken, Task<T>> handler,
        ExceptionCallSite callSite,
        CancellationToken token
    )
    {
        try
        {
            return await handler(token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Skip calling handlers but still throw
            throw;
        }
        catch (Exception ex)
        {
            var exceptionContext = new ExceptionContext(ExceptionDispatchInfo.Capture(ex), callSite);
            return await OnException<T>(exceptionContext, CancellationToken.None);
        }
    }
}
