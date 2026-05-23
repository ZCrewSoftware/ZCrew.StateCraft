namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class DynamicInitialStateInfo<TState, TTransition> : IDynamicInitialStateInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public DynamicInitialStateInfo(string? descriptor, IReadOnlyList<Type> initialParameterTypes)
    {
        Descriptor = descriptor;
        InitialParameterTypes = initialParameterTypes;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> InitialParameterTypes { get; }

    /// <inheritdoc />
    public string? Descriptor { get; }
}
