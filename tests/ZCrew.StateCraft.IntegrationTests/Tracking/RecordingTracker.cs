using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.Tracking;

namespace ZCrew.StateCraft.IntegrationTests.Tracking;

/// <summary>
///     Captures every tracking event in order so tests can assert on the trace a real state machine produces.
/// </summary>
internal sealed class RecordingTracker : Tracker<string, string>
{
    private readonly List<TrackedEvent> events = [];

    public IReadOnlyList<TrackedEvent> Events => this.events;

    /// <summary>
    ///     The names of the recorded events, in order.
    /// </summary>
    public IReadOnlyList<string> Names => this.events.Select(e => e.Name).ToList();

    /// <summary>
    ///     The single recorded event with the given name.
    /// </summary>
    public TrackedEvent Single(string name)
    {
        return this.events.Single(e => e.Name == name);
    }

    /// <summary>
    ///     Every recorded event with the given name, in order.
    /// </summary>
    public IReadOnlyList<TrackedEvent> All(string name)
    {
        return this.events.Where(e => e.Name == name).ToList();
    }

    public override void Activating(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(Activating), state: state, parameters: parameters);
    }

    public override void Activated(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(Activated), state: state, parameters: parameters);
    }

    public override void Deactivating(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(Deactivating), state: state, parameters: parameters);
    }

    public override void Deactivated(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(Deactivated), state: state, parameters: parameters);
    }

    public override void Entering(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(Entering), state: state, parameters: parameters);
    }

    public override void Entered(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(Entered), state: state, parameters: parameters);
    }

    public override void Exiting(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(Exiting), state: state, parameters: parameters);
    }

    public override void Exited(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(Exited), state: state, parameters: parameters);
    }

    public override void Transitioning(ITransitionIdentity<string> transition, IParameters parameters)
    {
        Record(nameof(Transitioning), transition: transition, parameters: parameters);
    }

    public override void Transitioned(ITransitionIdentity<string> transition, IParameters parameters)
    {
        Record(nameof(Transitioned), transition: transition, parameters: parameters);
    }

    public override void StateChanging(
        IStateIdentity<string> from,
        ITransitionIdentity<string> transition,
        IStateIdentity<string> to,
        IParameters parameters
    )
    {
        Record(nameof(StateChanging), state: from, transition: transition, target: to, parameters: parameters);
    }

    public override void StateChanged(
        IStateIdentity<string> from,
        ITransitionIdentity<string> transition,
        IStateIdentity<string> to,
        IParameters parameters
    )
    {
        Record(nameof(StateChanged), state: from, transition: transition, target: to, parameters: parameters);
    }

    public override void ActionStarting(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(ActionStarting), state: state, parameters: parameters);
    }

    public override void ActionCompleted(IStateIdentity<string> state, IParameters parameters)
    {
        Record(nameof(ActionCompleted), state: state, parameters: parameters);
    }

    public override void TransitionQuerying(
        TransitionQueryKind kind,
        string transition,
        IStateIdentity<string> from,
        IParameters parameters
    )
    {
        Record(nameof(TransitionQuerying), state: from, kind: kind, value: transition, parameters: parameters);
    }

    public override void TransitionFound(ITransitionIdentity<string> transition, IParameters parameters)
    {
        Record(nameof(TransitionFound), transition: transition, parameters: parameters);
    }

    public override void TransitionNotFound(
        TransitionQueryKind kind,
        string transition,
        IStateIdentity<string> from,
        IParameters parameters
    )
    {
        Record(nameof(TransitionNotFound), state: from, kind: kind, value: transition, parameters: parameters);
    }

    public override void TransitionSkipped(
        ITransitionIdentity<string> candidate,
        TransitionSkippedReason reason,
        IParameters parameters
    )
    {
        Record(nameof(TransitionSkipped), transition: candidate, reason: reason, parameters: parameters);
    }

    public override void RolledBack(IStateIdentity<string> restoredState, Exception? exception)
    {
        Record(nameof(RolledBack), state: restoredState, exception: exception);
    }

    public override void HandlerFailed(ExceptionCallSite callSite, Exception exception)
    {
        Record(nameof(HandlerFailed), callSite: callSite, exception: exception);
    }

    private void Record(
        string name,
        IStateIdentity<string>? state = null,
        ITransitionIdentity<string>? transition = null,
        IStateIdentity<string>? target = null,
        IParameters? parameters = null,
        TransitionQueryKind? kind = null,
        TransitionSkippedReason? reason = null,
        ExceptionCallSite? callSite = null,
        string? value = null,
        Exception? exception = null
    )
    {
        this.events.Add(
            new TrackedEvent
            {
                Name = name,
                State = state,
                Transition = transition,
                Target = target,
                Parameters = parameters,
                Kind = kind,
                Reason = reason,
                CallSite = callSite,
                Value = value,
                Exception = exception,
            }
        );
    }

    /// <summary>
    ///     One recorded tracking event.
    /// </summary>
    internal sealed record TrackedEvent
    {
        public required string Name { get; init; }
        public IStateIdentity<string>? State { get; init; }
        public ITransitionIdentity<string>? Transition { get; init; }
        public IStateIdentity<string>? Target { get; init; }
        public IParameters? Parameters { get; init; }
        public TransitionQueryKind? Kind { get; init; }
        public TransitionSkippedReason? Reason { get; init; }
        public ExceptionCallSite? CallSite { get; init; }
        public string? Value { get; init; }
        public Exception? Exception { get; init; }

        public override string ToString()
        {
            return $"{Name}({State?.StateValue ?? Transition?.TransitionValue}) {Parameters}";
        }
    }
}
