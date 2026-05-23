namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class StateMachineInfo<TState, TTransition> : IStateMachineInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StateMachineInfo(
        IInitialStateInfo<TState, TTransition>? initialState,
        IReadOnlyList<IStateInfo<TState, TTransition>> states,
        IReadOnlyList<ITransitionInfo<TState, TTransition>> transitions
    )
    {
        InitialState = initialState;
        States = states;
        Transitions = transitions;
    }

    /// <inheritdoc />
    public IInitialStateInfo<TState, TTransition>? InitialState { get; }

    /// <inheritdoc />
    public IReadOnlyList<IStateInfo<TState, TTransition>> States { get; }

    /// <inheritdoc />
    public IReadOnlyList<ITransitionInfo<TState, TTransition>> Transitions { get; }
}
