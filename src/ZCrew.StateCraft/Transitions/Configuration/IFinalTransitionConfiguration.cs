using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Marks the end of the transition configuration from a parameterless state.
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
public interface IFinalTransitionConfiguration<TState, TTransition> : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Build the transition based on the configuration.
    /// </summary>
    /// <param name="state">The parent state.</param>
    /// <returns>The transition model.</returns>
    internal ITransition<TState, TTransition> Build(IParameterlessState<TState, TTransition> state);
}

/// <summary>
///     Marks the end of the transition configuration from a parameterized state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T">The parameter type of the state.</typeparam>
/// <remarks>
///     This should remain empty of public configuration members. This allows configuration steps to stop further
///     configuration by returning this type.
/// </remarks>
public interface IFinalTransitionConfiguration<TState, TTransition, T> : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Build the transition based on the configuration.
    /// </summary>
    /// <param name="state">The parent state.</param>
    /// <returns>The transition model.</returns>
    internal ITransition<TState, TTransition> Build(IParameterizedState<TState, TTransition, T> state);
}
