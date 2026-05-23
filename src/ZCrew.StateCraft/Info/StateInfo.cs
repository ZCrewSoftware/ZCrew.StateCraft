namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class StateInfo<TState, TTransition> : IStateInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StateInfo(TState stateValue, IReadOnlyList<Type> stateParameterTypes)
    {
        StateValue = stateValue;
        StateParameterTypes = stateParameterTypes;
    }

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> StateParameterTypes { get; }
}
