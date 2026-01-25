namespace ZCrew.StateCraft;

/// <summary>
///     Marks the end of the parameterless action configuration.
/// </summary>
/// <remarks>
///     This should remain empty of public configuration members. This allows configuration steps to stop further
///     configuration by returning this type.
/// </remarks>
public interface IFinalParameterlessActionConfiguration : IParameterlessActionConfiguration;
