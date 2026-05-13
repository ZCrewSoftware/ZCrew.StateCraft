namespace ZCrew.StateCraft.Info;

internal sealed class ExcludedStateInfo<TState> : IExcludedStateInfo<TState>
    where TState : notnull
{
    public ExcludedStateInfo(TState stateValue, IReadOnlyList<Type> stateParameterTypes)
    {
        StateValue = stateValue;
        StateParameterTypes = stateParameterTypes;
    }

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> StateParameterTypes { get; }
}
