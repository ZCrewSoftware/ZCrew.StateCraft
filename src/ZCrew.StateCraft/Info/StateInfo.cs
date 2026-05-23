namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal class StateInfo<TState, TTransition> : IStateInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StateInfo(
        IStateMachineInfo<TState, TTransition> stateMachine,
        TState stateValue,
        IReadOnlyList<Type> stateParameterTypes
    )
    {
        StateMachine = stateMachine;
        StateValue = stateValue;
        StateParameterTypes = stateParameterTypes;
    }

    /// <inheritdoc />
    public IStateMachineInfo<TState, TTransition> StateMachine { get; }

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> StateParameterTypes { get; }
}
