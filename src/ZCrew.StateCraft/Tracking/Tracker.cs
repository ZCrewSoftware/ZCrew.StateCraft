using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.Tracking.Contracts;

namespace ZCrew.StateCraft.Tracking;

/// <summary>
///     A base <see cref="ITracker{TState, TTransition}"/> whose events all do nothing. Derive from this and override
///     only the events of interest.
/// </summary>
/// <typeparam name="TState">The type representing state identifiers.</typeparam>
/// <typeparam name="TTransition">The type representing transition identifiers.</typeparam>
internal abstract class Tracker<TState, TTransition> : ITracker<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc />
    public virtual void Activating(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void Activated(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void Deactivating(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void Deactivated(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void Entering(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void Entered(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void Exiting(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void Exited(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void Transitioning(ITransitionIdentity<TTransition> transition, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void Transitioned(ITransitionIdentity<TTransition> transition, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void StateChanging(
        IStateIdentity<TState> from,
        ITransitionIdentity<TTransition> transition,
        IStateIdentity<TState> to,
        IParameters parameters
    ) { }

    /// <inheritdoc />
    public virtual void StateChanged(
        IStateIdentity<TState> from,
        ITransitionIdentity<TTransition> transition,
        IStateIdentity<TState> to,
        IParameters parameters
    ) { }

    /// <inheritdoc />
    public virtual void ActionStarting(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void ActionCompleted(IStateIdentity<TState> state, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void TransitionQuerying(
        TransitionQueryKind kind,
        TTransition transition,
        IStateIdentity<TState> from,
        IParameters parameters
    ) { }

    /// <inheritdoc />
    public virtual void TransitionFound(ITransitionIdentity<TTransition> transition, IParameters parameters) { }

    /// <inheritdoc />
    public virtual void TransitionNotFound(
        TransitionQueryKind kind,
        TTransition transition,
        IStateIdentity<TState> from,
        IParameters parameters
    ) { }

    /// <inheritdoc />
    public virtual void TransitionSkipped(
        ITransitionIdentity<TTransition> candidate,
        TransitionSkippedReason reason,
        IParameters parameters
    ) { }

    /// <inheritdoc />
    public virtual void RolledBack(IStateIdentity<TState> restoredState, Exception? exception) { }

    /// <inheritdoc />
    public virtual void HandlerFailed(ExceptionCallSite callSite, Exception exception) { }
}
