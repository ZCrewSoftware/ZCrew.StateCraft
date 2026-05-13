namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class StaticInitialStateInfo<TState> : IStaticInitialStateInfo<TState>
    where TState : notnull
{
    public StaticInitialStateInfo(
        TState initialStateValue,
        IReadOnlyList<object?> initialParameters,
        IReadOnlyList<Type> initialParameterTypes
    )
    {
        InitialStateValue = initialStateValue;
        InitialParameters = initialParameters;
        InitialParameterTypes = initialParameterTypes;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> InitialParameterTypes { get; }

    /// <inheritdoc />
    public TState InitialStateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<object?> InitialParameters { get; }
}
