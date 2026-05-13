namespace ZCrew.StateCraft.Info;

internal sealed class ConditionInfo : IConditionInfo
{
    public ConditionInfo(string? descriptor, IReadOnlyList<Type> typeParameters)
    {
        Descriptor = descriptor;
        TypeParameters = typeParameters;
    }

    /// <inheritdoc />
    public string? Descriptor { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; }
}
