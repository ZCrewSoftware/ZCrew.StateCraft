namespace ZCrew.StateCraft;

/// <summary>
///     Configures an inverted transition from all (or most) states, with the ability to exclude specific states.
///     This interface also extends <see cref="ITransitionConfiguration{TState, TTransition}"/> so the configuration
///     chain can terminate without calling <see cref="Except(TState)"/>.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IFromAllStatesTransitionConfiguration<TState, TTransition>
    : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Excludes a parameterless state from the inverted transition. The excluded state will not have this
    ///     transition available.
    /// </summary>
    /// <param name="state">The state value to exclude.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except(TState state);

    /// <inheritdoc cref="Except(TState)"/>
    /// <typeparam name="TPrevious">
    ///     The parameter type of the state to exclude. This narrows the exclusion to the specific state configured
    ///     with this parameter type.
    /// </typeparam>
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious>(TState state);

    /// <inheritdoc cref="Except(TState)"/>
    /// <typeparam name="TPrevious1">The type of the first parameter of the state to exclude.</typeparam>
    /// <typeparam name="TPrevious2">The type of the second parameter of the state to exclude.</typeparam>
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2>(TState state);

    /// <inheritdoc cref="Except(TState)"/>
    /// <typeparam name="TPrevious1">The type of the first parameter of the state to exclude.</typeparam>
    /// <typeparam name="TPrevious2">The type of the second parameter of the state to exclude.</typeparam>
    /// <typeparam name="TPrevious3">The type of the third parameter of the state to exclude.</typeparam>
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2, TPrevious3>(TState state);

    /// <inheritdoc cref="Except(TState)"/>
    /// <typeparam name="TPrevious1">The type of the first parameter of the state to exclude.</typeparam>
    /// <typeparam name="TPrevious2">The type of the second parameter of the state to exclude.</typeparam>
    /// <typeparam name="TPrevious3">The type of the third parameter of the state to exclude.</typeparam>
    /// <typeparam name="TPrevious4">The type of the fourth parameter of the state to exclude.</typeparam>
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2, TPrevious3, TPrevious4>(
        TState state
    );
}
