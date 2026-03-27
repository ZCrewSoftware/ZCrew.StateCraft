using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace ZCrew.StateCraft;

public readonly record struct ExceptionContext
{
    public ExceptionContext(ExceptionDispatchInfo exceptionInfo, ExceptionCallSite callSite)
    {
        ExceptionInfo = exceptionInfo;
        CallSite = callSite;
    }

    public ExceptionDispatchInfo ExceptionInfo { get; }

    public Exception Exception => ExceptionInfo.SourceException;

    public ExceptionCallSite CallSite { get; }

    [DoesNotReturn]
    [StackTraceHidden]
    public void ThrowException()
    {
        ExceptionInfo.Throw();
    }
}
