using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Tracking.Contracts;

/// <summary>
///     Defines a tracker that receives notifications about state machine lifecycle events.
///     Implementations can be used for logging, debugging, or monitoring state machine behavior.
/// </summary>
/// <typeparam name="TState">The type representing state identifiers.</typeparam>
/// <typeparam name="TTransition">The type representing transition identifiers.</typeparam>
internal interface ITracker<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Called before the <c>OnActivate</c> handlers of a state are invoked.
    /// </summary>
    /// <param name="state">The state being activated.</param>
    /// <param name="parameters">The parameters of the state.</param>
    void Activating(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called after the <c>OnActivate</c> handlers of a state have completed.
    /// </summary>
    /// <param name="state">The state that was activated.</param>
    /// <param name="parameters">The parameters of the state.</param>
    void Activated(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called before the <c>OnDeactivate</c> handlers of a state are invoked.
    /// </summary>
    /// <param name="state">The state being deactivated.</param>
    /// <param name="parameters">The parameters of the state.</param>
    void Deactivating(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called after the <c>OnDeactivate</c> handlers of a state have completed.
    /// </summary>
    /// <param name="state">The state that was deactivated.</param>
    /// <param name="parameters">The parameters of the state.</param>
    void Deactivated(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called before the <c>OnEntry</c> handlers and the triggers of a state are started.
    /// </summary>
    /// <param name="state">The state being entered.</param>
    /// <param name="parameters">The parameters staged for the state.</param>
    void Entering(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called after the <c>OnEntry</c> handlers and the triggers of a state have completed.
    /// </summary>
    /// <param name="state">The state that was entered.</param>
    /// <param name="parameters">The parameters of the state.</param>
    void Entered(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called before the triggers of a state are stopped and its <c>OnExit</c> handlers are invoked.
    /// </summary>
    /// <param name="state">The state being exited.</param>
    /// <param name="parameters">The parameters of the state.</param>
    void Exiting(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called after the <c>OnExit</c> handlers of a state have completed.
    /// </summary>
    /// <param name="state">The state that was exited.</param>
    /// <param name="parameters">The parameters of the state.</param>
    void Exited(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called before the <c>OnTransition</c> handlers of a transition are invoked.
    /// </summary>
    /// <param name="transition">The transition being executed.</param>
    /// <param name="parameters">The parameters staged for the next state.</param>
    void Transitioning(ITransitionIdentity<TTransition> transition, IParameters parameters);

    /// <summary>
    ///     Called after the <c>OnTransition</c> handlers of a transition have completed.
    /// </summary>
    /// <param name="transition">The transition that was executed.</param>
    /// <param name="parameters">The parameters staged for the next state.</param>
    void Transitioned(ITransitionIdentity<TTransition> transition, IParameters parameters);

    /// <summary>
    ///     Called before the machine-level and state-level <c>OnStateChange</c> handlers are invoked.
    /// </summary>
    /// <param name="from">The state being left.</param>
    /// <param name="transition">The transition being applied.</param>
    /// <param name="to">The state being moved to.</param>
    /// <param name="parameters">The parameters staged for the next state.</param>
    void StateChanging(
        IStateIdentity<TState> from,
        ITransitionIdentity<TTransition> transition,
        IStateIdentity<TState> to,
        IParameters parameters
    );

    /// <summary>
    ///     Called after the machine-level and state-level <c>OnStateChange</c> handlers have completed.
    /// </summary>
    /// <param name="from">The state that was left.</param>
    /// <param name="transition">The transition that was applied.</param>
    /// <param name="to">The state that was moved to.</param>
    /// <param name="parameters">The parameters staged for the next state.</param>
    void StateChanged(
        IStateIdentity<TState> from,
        ITransitionIdentity<TTransition> transition,
        IStateIdentity<TState> to,
        IParameters parameters
    );

    /// <summary>
    ///     Called before the actions of a state are invoked.
    /// </summary>
    /// <param name="state">The state whose actions are starting.</param>
    /// <param name="parameters">The parameters of the state.</param>
    void ActionStarting(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called once the actions of a state have completed.
    /// </summary>
    /// <remarks>
    ///     On the synchronous action path this is raised after the state machine lock has been released, so it may
    ///     interleave with events from a transition started by the action itself.
    /// </remarks>
    /// <param name="state">The state whose actions completed.</param>
    /// <param name="parameters">The parameters of the state.</param>
    void ActionCompleted(IStateIdentity<TState> state, IParameters parameters);

    /// <summary>
    ///     Called when a transition is queried for, before the transition table is queried.
    /// </summary>
    /// <param name="kind">Which entry point made the request.</param>
    /// <param name="transition">The requested transition value.</param>
    /// <param name="from">The state the request was made from.</param>
    /// <param name="parameters">The parameters supplied by the caller.</param>
    void TransitionQuerying(
        TransitionQueryKind kind,
        TTransition transition,
        IStateIdentity<TState> from,
        IParameters parameters
    );

    /// <summary>
    ///     Called when a requested transition matched a registered transition whose conditions passed.
    /// </summary>
    /// <param name="transition">The transition that matched.</param>
    /// <param name="parameters">The parameters supplied by the caller.</param>
    void TransitionFound(ITransitionIdentity<TTransition> transition, IParameters parameters);

    /// <summary>
    ///     Called when a requested transition matched no registered transition.
    /// </summary>
    /// <param name="kind">Which entry point made the request.</param>
    /// <param name="transition">The requested transition value.</param>
    /// <param name="from">The state the request was made from.</param>
    /// <param name="parameters">The parameters supplied by the caller.</param>
    void TransitionNotFound(
        TransitionQueryKind kind,
        TTransition transition,
        IStateIdentity<TState> from,
        IParameters parameters
    );

    /// <summary>
    ///     Called for each registered transition that was considered and skipped during a lookup.
    /// </summary>
    /// <param name="candidate">The transition that was skipped.</param>
    /// <param name="reason">Why it was skipped.</param>
    /// <param name="parameters">The parameters supplied by the caller.</param>
    void TransitionSkipped(
        ITransitionIdentity<TTransition> candidate,
        TransitionSkippedReason reason,
        IParameters parameters
    );

    /// <summary>
    ///     Called when a transition in progress was reverted and the machine restored to the state it came from.
    /// </summary>
    /// <param name="restoredState">The state the machine was restored to.</param>
    /// <param name="exception">
    ///     The exception that caused the rollback, or <see langword="null"/> when the rollback was expected, such as a
    ///     <c>CanTransition</c> dry run or a <c>TryTransition</c> that matched nothing.
    /// </param>
    void RolledBack(IStateIdentity<TState> restoredState, Exception? exception);

    /// <summary>
    ///     Called when a user-configured handler threw, before the configured <see cref="IExceptionBehavior"/> decides
    ///     what to do with it.
    /// </summary>
    /// <param name="callSite">The call site the handler was invoked from.</param>
    /// <param name="exception">The exception that was thrown.</param>
    void HandlerFailed(ExceptionCallSite callSite, Exception exception);
}
