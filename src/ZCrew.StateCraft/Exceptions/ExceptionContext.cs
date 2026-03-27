using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace ZCrew.StateCraft;

/// <summary>
///     Provides contextual information about an exception that was thrown during a state machine operation.
///     This is passed to exception handlers registered via
///     <see cref="IStateMachineConfiguration{TState,TTransition}.OnException(Action{ExceptionContext})"/>.
/// </summary>
/// <seealso cref="ExceptionCallSite"/>
/// <seealso cref="RethrowExceptionBehavior"/>
public readonly record struct ExceptionContext
{
    /// <summary>
    ///     Constructs a new instance with the captured exception and the call site where it was thrown.
    /// </summary>
    /// <param name="exceptionInfo">The captured exception with its original stack trace.</param>
    /// <param name="callSite">The call site where the exception was thrown.</param>
    public ExceptionContext(ExceptionDispatchInfo exceptionInfo, ExceptionCallSite callSite)
    {
        ExceptionInfo = exceptionInfo;
        CallSite = callSite;
    }

    /// <summary>
    ///     The captured exception dispatch info, preserving the original stack trace.
    /// </summary>
    public ExceptionDispatchInfo ExceptionInfo { get; }

    /// <summary>
    ///     The exception that was thrown.
    /// </summary>
    public Exception Exception => ExceptionInfo.SourceException;

    /// <summary>
    ///     The state machine call site where the exception was thrown.
    /// </summary>
    public ExceptionCallSite CallSite { get; }

    /// <summary>
    ///     Rethrows the original exception, preserving the original stack trace.
    /// </summary>
    [DoesNotReturn]
    [StackTraceHidden]
    public void ThrowException()
    {
        ExceptionInfo.Throw();
    }
}
