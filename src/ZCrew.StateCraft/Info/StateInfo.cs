namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class StateInfo<TState> : IStateInfo<TState>
    where TState : notnull
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
