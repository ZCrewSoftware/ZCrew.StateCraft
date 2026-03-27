namespace ZCrew.StateCraft;

/// <summary>
///     Identifies the state machine call site where an exception was thrown.
/// </summary>
/// <seealso cref="ExceptionContext"/>
/// <seealso cref="IExceptionBehavior"/>
public enum ExceptionCallSite
{
    /// <summary>
    ///     The exception was thrown during a state's <c>OnEntry</c> handler.
    /// </summary>
    /// <seealso cref="IExceptionBehavior.CallOnEntry"/>
    OnEntry,

    /// <summary>
    ///     The exception was thrown during a state's <c>OnExit</c> handler.
    /// </summary>
    /// <seealso cref="IExceptionBehavior.CallOnExit"/>
    OnExit,

    /// <summary>
    ///     The exception was thrown during the state machine's <c>OnStateChange</c> handler.
    /// </summary>
    /// <seealso cref="IExceptionBehavior.CallOnStateChange"/>
    OnStateChange,

    /// <summary>
    ///     The exception was thrown during a state's <c>OnActivate</c> handler.
    /// </summary>
    /// <seealso cref="IExceptionBehavior.CallOnActivate"/>
    OnActivate,

    /// <summary>
    ///     The exception was thrown during a state's <c>OnDeactivate</c> handler.
    /// </summary>
    /// <seealso cref="IExceptionBehavior.CallOnDeactivate"/>
    OnDeactivate,

    /// <summary>
    ///     The exception was thrown during a transition's condition evaluation.
    /// </summary>
    /// <seealso cref="IExceptionBehavior.CallCondition"/>
    Condition,

    /// <summary>
    ///     The exception was thrown during a mapped transition's mapping function.
    /// </summary>
    /// <seealso cref="IExceptionBehavior.CallMap"/>
    Map,

    /// <summary>
    ///     The exception was thrown during a state's action invocation.
    /// </summary>
    /// <seealso cref="IExceptionBehavior.CallAction"/>
    Action,

    /// <summary>
    ///     The exception was thrown during a trigger's invocation.
    /// </summary>
    /// <seealso cref="IExceptionBehavior.CallTrigger"/>
    Trigger,
}
