namespace ZCrew.StateCraft.Info;

internal sealed class DynamicInitialStateInfo<TState> : IDynamicInitialStateInfo<TState>
    where TState : notnull
{
    public DynamicInitialStateInfo(IReadOnlyList<Type> initialParameterTypes, string? descriptor)
    {
        InitialParameterTypes = initialParameterTypes;
        Descriptor = descriptor;
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> InitialParameterTypes { get; }

    /// <inheritdoc />
    public string? Descriptor { get; }
}
