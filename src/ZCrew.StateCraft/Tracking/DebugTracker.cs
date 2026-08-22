using System.Diagnostics;
using ZCrew.StateCraft.Identities.Extensions;
using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Tracking;

/// <summary>
///     Records a rolling window of state machine events as text so the trace can be read from a debugger.
/// </summary>
/// <typeparam name="TState">The type representing state identifiers.</typeparam>
/// <typeparam name="TTransition">The type representing transition identifiers.</typeparam>
[DebuggerDisplay("Count={Count}")]
internal class DebugTracker<TState, TTransition> : Tracker<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private const int MaximumRecords = 100;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private readonly LinkedList<string> records = [];

    /// <summary>
    ///     The number of records currently held.
    /// </summary>
    public int Count => this.records.Count;

    /// <inheritdoc />
    public override void Activating(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Activating {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void Activated(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Activated {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void Deactivating(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Deactivating {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void Deactivated(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Deactivated {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void Entering(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Entering {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void Entered(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Entered {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void Exiting(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Exiting {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void Exited(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Exited {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void Transitioning(ITransitionIdentity<TTransition> transition, IParameters parameters)
    {
        AddRecord($"Transitioning {Describe(transition, parameters)}");
    }

    /// <inheritdoc />
    public override void Transitioned(ITransitionIdentity<TTransition> transition, IParameters parameters)
    {
        AddRecord($"Transitioned {Describe(transition, parameters)}");
    }

    /// <inheritdoc />
    public override void StateChanging(
        IStateIdentity<TState> from,
        ITransitionIdentity<TTransition> transition,
        IStateIdentity<TState> to,
        IParameters parameters
    )
    {
        AddRecord($"State changing {transition.ToDisplayStringFromOneToOne(from, to)} {parameters}");
    }

    /// <inheritdoc />
    public override void StateChanged(
        IStateIdentity<TState> from,
        ITransitionIdentity<TTransition> transition,
        IStateIdentity<TState> to,
        IParameters parameters
    )
    {
        AddRecord($"State changed {transition.ToDisplayStringFromOneToOne(from, to)} {parameters}");
    }

    /// <inheritdoc />
    public override void ActionStarting(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Action starting {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void ActionCompleted(IStateIdentity<TState> state, IParameters parameters)
    {
        AddRecord($"Action completed {Describe(state, parameters)}");
    }

    /// <inheritdoc />
    public override void TransitionQuerying(
        TransitionQueryKind kind,
        TTransition transition,
        IStateIdentity<TState> from,
        IParameters parameters
    )
    {
        AddRecord($"{kind} requested '{transition}' from {Describe(from, parameters)}");
    }

    /// <inheritdoc />
    public override void TransitionFound(ITransitionIdentity<TTransition> transition, IParameters parameters)
    {
        AddRecord($"Resolved {Describe(transition, parameters)}");
    }

    /// <inheritdoc />
    public override void TransitionNotFound(
        TransitionQueryKind kind,
        TTransition transition,
        IStateIdentity<TState> from,
        IParameters parameters
    )
    {
        AddRecord($"{kind} found no match for '{transition}' from {Describe(from, parameters)}");
    }

    /// <inheritdoc />
    public override void TransitionSkipped(
        ITransitionIdentity<TTransition> candidate,
        TransitionSkippedReason reason,
        IParameters parameters
    )
    {
        AddRecord($"Rejected {candidate.ToDisplayString()} ({reason}) {parameters}");
    }

    /// <inheritdoc />
    public override void RolledBack(IStateIdentity<TState> restoredState, Exception? exception)
    {
        var cause = exception == null ? "" : $": {exception.GetType().Name}: {exception.Message}";
        AddRecord($"Rolled back to {restoredState.ToDisplayString()}{cause}");
    }

    /// <inheritdoc />
    public override void HandlerFailed(ExceptionCallSite callSite, Exception exception)
    {
        AddRecord($"{callSite} threw {exception.GetType().Name}: {exception.Message}");
    }

    private static string Describe(IStateIdentity<TState> state, IParameters parameters)
    {
        return $"{state.ToDisplayString()} {parameters}";
    }

    private static string Describe(ITransitionIdentity<TTransition> transition, IParameters parameters)
    {
        return $"{transition.ToDisplayString()} {parameters}";
    }

    private void AddRecord(string message)
    {
        var record = $"[{DateTime.Now:O}]: {message}";
        this.records.AddLast(record);

        // Limit the records to just a constant value for now
        if (this.records.Count > MaximumRecords)
        {
            this.records.RemoveFirst();
        }
    }
}
