namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class DynamicInitialStateInfo<TState> : IDynamicInitialStateInfo<TState>
    where TState : notnull
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
