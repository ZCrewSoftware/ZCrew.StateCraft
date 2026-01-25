namespace ZCrew.StateCraft;

/// <summary>
///     Marks the end of the parameterized action configuration.
/// </summary>
/// <typeparam name="T">The type of the parameter passed to the action.</typeparam>
/// <remarks>
///     This should remain empty of public configuration members. This allows configuration steps to stop further
///     configuration by returning this type.
/// </remarks>
public interface IFinalParameterizedActionConfiguration<T> : IParameterizedActionConfiguration<T>;
