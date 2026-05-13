namespace ZCrew.StateCraft.Info;

internal sealed class StateMachineInfo<TState, TTransition> : IStateMachineInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StateMachineInfo(
        IInitialStateInfo<TState> initialState,
        IReadOnlyList<IStateInfo<TState>> states,
        IReadOnlyList<ITransitionInfo<TTransition>> transitions
    )
    {
        InitialState = initialState;
        States = states;
        Transitions = transitions;
    }

    /// <inheritdoc />
    public IInitialStateInfo<TState> InitialState { get; }

    /// <inheritdoc />
    public IReadOnlyList<IStateInfo<TState>> States { get; }

    /// <inheritdoc />
    public IReadOnlyList<ITransitionInfo<TTransition>> Transitions { get; }
}
