namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class MappingFunctionInfo : IMappingFunctionInfo
{
    public MappingFunctionInfo(string? descriptor, IReadOnlyList<Type> typeParameters, IReadOnlyList<Type> resultTypes)
    {
        Descriptor = descriptor;
        TypeParameters = typeParameters;
        ResultTypes = resultTypes;
    }

    /// <inheritdoc />
    public string? Descriptor { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> ResultTypes { get; }
}
