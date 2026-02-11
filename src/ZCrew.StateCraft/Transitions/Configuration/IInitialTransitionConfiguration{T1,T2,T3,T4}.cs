namespace ZCrew.StateCraft;

/// <summary>
///     The initial transition configuration for a state with four parameters. Conditions added here receive the
///     previous state's four parameter values.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter for the previous state.</typeparam>
/// <typeparam name="T2">The type of the second parameter for the previous state.</typeparam>
/// <typeparam name="T3">The type of the third parameter for the previous state.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter for the previous state.</typeparam>
public interface IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.If(Func{TPrevious,bool})"/>
    IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(Func<T1, T2, T3, T4, bool> condition);

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.If(Func{TPrevious,CancellationToken,Task{bool}})"/>
    IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(
        Func<T1, T2, T3, T4, CancellationToken, Task<bool>> condition
    );

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.If(Func{TPrevious,CancellationToken,ValueTask{bool}})"/>
    IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(
        Func<T1, T2, T3, T4, CancellationToken, ValueTask<bool>> condition
    );

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithNoParameters"/>
    IDirectTransitionConfiguration<TState, TTransition> WithNoParameters();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameter{TNext}"/>
    IDirectTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2}"/>
    IDirectTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithParameters<TNext1, TNext2>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2,T3}"/>
    IDirectTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithParameters<
        TNext1,
        TNext2,
        TNext3
    >();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2,T3,T4}"/>
    IDirectTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >();

    /// <summary>
    ///     Configures the transition to map the previous four parameters to a single next parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, T3, T4, TNext> map
    );

    /// <summary>
    ///     Configures the transition to map the previous four parameters to a single next parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, T3, T4, CancellationToken, Task<TNext>> map
    );

    /// <summary>
    ///     Configures the transition to map the previous four parameters to a single next parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(
        Func<T1, T2, T3, T4, CancellationToken, ValueTask<TNext>> map
    );

    /// <summary>
    ///     Configures the transition to map the previous four parameters to two next parameters.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T1, T2, T3, T4, (TNext1, TNext2)> map
    );

    /// <summary>
    ///     Configures the transition to map the previous four parameters to two next parameters.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T1, T2, T3, T4, CancellationToken, Task<(TNext1, TNext2)>> map
    );

    /// <summary>
    ///     Configures the transition to map the previous four parameters to two next parameters.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> WithMappedParameters<TNext1, TNext2>(
        Func<T1, T2, T3, T4, CancellationToken, ValueTask<(TNext1, TNext2)>> map
    );

    /// <summary>
    ///     Configures the transition to map the previous four parameters to three next parameters.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TNext3">The type of the third parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, T3, T4, (TNext1, TNext2, TNext3)> map);

    /// <summary>
    ///     Configures the transition to map the previous four parameters to three next parameters.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TNext3">The type of the third parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, T3, T4, CancellationToken, Task<(TNext1, TNext2, TNext3)>> map);

    /// <summary>
    ///     Configures the transition to map the previous four parameters to three next parameters.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TNext3">The type of the third parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3
    >(Func<T1, T2, T3, T4, CancellationToken, ValueTask<(TNext1, TNext2, TNext3)>> map);

    /// <summary>
    ///     Configures the transition to map the previous four parameters to four next parameters.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TNext3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="TNext4">The type of the fourth parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T1, T2, T3, T4, (TNext1, TNext2, TNext3, TNext4)> map);

    /// <summary>
    ///     Configures the transition to map the previous four parameters to four next parameters.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TNext3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="TNext4">The type of the fourth parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T1, T2, T3, T4, CancellationToken, Task<(TNext1, TNext2, TNext3, TNext4)>> map);

    /// <summary>
    ///     Configures the transition to map the previous four parameters to four next parameters.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TNext3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="TNext4">The type of the fourth parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> WithMappedParameters<
        TNext1,
        TNext2,
        TNext3,
        TNext4
    >(Func<T1, T2, T3, T4, CancellationToken, ValueTask<(TNext1, TNext2, TNext3, TNext4)>> map);

    /// <summary>
    ///     Configures the transition to pass the parameters from the previous state to the next state, unchanged.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> WithSameParameters()
    {
        return WithMappedParameters<T1, T2, T3, T4>((a, b, c, d) => (a, b, c, d));
    }

    /// <summary>
    ///     Configure the state to transition to.
    /// </summary>
    /// <param name="state">The next state.</param>
    /// <returns>The final state configuration step.</returns>
    ITransitionConfiguration<TState, TTransition> To(TState state)
    {
        return WithNoParameters().To(state);
    }
}
