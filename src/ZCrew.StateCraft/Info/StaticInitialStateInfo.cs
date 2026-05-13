namespace ZCrew.StateCraft.Info;

internal sealed class StaticInitialStateInfo<TState> : IStaticInitialStateInfo<TState>
    where TState : notnull
{
    public StaticInitialStateInfo(
        IReadOnlyList<Type> initialParameterTypes,
        TState initialStateValue,
        IReadOnlyList<object?> initialParameters
    )
    {
        InitialParameterTypes = initialParameterTypes;
        InitialStateValue = initialStateValue;
        InitialParameters = initialParameters;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> InitialParameterTypes { get; }

    /// <inheritdoc />
    public TState InitialStateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<object?> InitialParameters { get; }
}
