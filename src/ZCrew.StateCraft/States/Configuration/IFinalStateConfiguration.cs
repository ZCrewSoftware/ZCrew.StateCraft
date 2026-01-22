namespace ZCrew.StateCraft;

/// <summary>
///     Marks the end of the state configuration.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <remarks>
///     This should remain empty of public configuration members. This allows configuration steps to stop further
///     configuration by returning this type.
/// </remarks>
public interface IFinalStateConfiguration<TState, TTransition> : IStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull;
