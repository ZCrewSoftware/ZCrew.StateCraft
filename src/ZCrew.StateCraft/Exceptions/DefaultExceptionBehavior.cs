using System.Runtime.ExceptionServices;
using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Exceptions;

/// <inheritdoc />
/// <remarks>
///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
///     exception is thrown. The exception is when an <see cref="OperationCanceledException"/> is thrown. With
///     <see cref="CallAction"/> or <see cref="CallTrigger"/> that exception is how those background tasks are stopped.
///     For other methods that exception is just rethrown and the handlers are not called.
/// </remarks>
public class DefaultExceptionBehavior : IExceptionBehavior
{
    protected readonly IReadOnlyList<IAsyncFunc<Exception, ExceptionResult>> OnExceptionHandlers;

    /// <summary>
    ///     Constructs a new instance with the <paramref name="onExceptionHandlers"/> which will be called first-to-last
    ///     when an exception is thrown.
    /// </summary>
    /// <param name="onExceptionHandlers">The handlers to invoke when an exception is thrown.</param>
    public DefaultExceptionBehavior(IReadOnlyList<IAsyncFunc<Exception, ExceptionResult>> onExceptionHandlers)
    {
        this.OnExceptionHandlers = onExceptionHandlers;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The exception is for <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnEntry(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The exception is for <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnExit(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The exception is for <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnStateChange(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The exception is for <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnActivate(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The exception is for <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallOnDeactivate(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The exception is for <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task<bool> CallCondition(
        Func<CancellationToken, Task<bool>> handler,
        CancellationToken token = default
    )
    {
        return Execute(handler, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The exception is for <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception handlers will not be called.
    /// </remarks>
    public virtual Task CallMap(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return Execute(handler, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The exception is for <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception is suppressed.
    /// </remarks>
    public virtual Task CallAction(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return ExecuteCancelable(handler, token);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     By default, this will invoke the <see cref="OnExceptionHandlers"/> using <see cref="OnException"/> when any
    ///     exception is thrown. The exception is for <see cref="OperationCanceledException"/> when
    ///     <paramref name="token"/> has been canceled, in which case the exception is suppressed.
    /// </remarks>
    public virtual Task CallTrigger(Func<CancellationToken, Task> handler, CancellationToken token = default)
    {
        return ExecuteCancelable(handler, token);
    }

    protected virtual async Task OnException(ExceptionDispatchInfo exceptionInfo, CancellationToken token)
    {
        foreach (var handler in this.OnExceptionHandlers)
        {
            var result = await handler.InvokeAsync(exceptionInfo.SourceException, token);
            switch (result)
            {
                case ExceptionResult.RethrowResult:
                    exceptionInfo.Throw();
                    break;
                case ExceptionResult.ThrowResult { Exception: var exception }:
                    throw exception;
                case ExceptionResult.ContinueResult:
                    continue;
            }
        }

        // No handler made a decision, rethrow with original stack trace
        exceptionInfo.Throw();
    }

    private async Task Execute(Func<CancellationToken, Task> handler, CancellationToken token)
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
            await OnException(ExceptionDispatchInfo.Capture(ex), CancellationToken.None);
        }
    }

    private async Task ExecuteCancelable(Func<CancellationToken, Task> handler, CancellationToken token)
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
            await OnException(ExceptionDispatchInfo.Capture(ex), CancellationToken.None);
        }
    }

    private async Task<T> Execute<T>(Func<CancellationToken, Task<T>> handler, CancellationToken token)
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
            await OnException(ExceptionDispatchInfo.Capture(ex), CancellationToken.None);
            throw;
        }
    }
}
