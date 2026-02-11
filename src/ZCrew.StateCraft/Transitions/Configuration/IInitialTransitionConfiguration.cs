using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     The initial transition configuration which provides an opportunity to specify parameters and add conditions
///     based on the previous state. Conditions added here are evaluated before the transition parameter type is known.
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
///     <para>
///     Conditions added via <see cref="If(Func{bool})"/> are evaluated in the order they are registered.
///     All conditions must return <see langword="true"/> for the transition to proceed (logical AND).
///     Evaluation short-circuits on the first <see langword="false"/> result.
///     </para>
///     <para>
///     Conditions added at this stage (before calling <see cref="WithParameter{TNext}"/> or
///     <see cref="WithNoParameters"/>) are evaluated before any conditions added afterward.
///     </para>
/// </remarks>
public interface IInitialTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IInitialTransitionConfiguration<TState, TTransition> If(Func<bool> condition);

    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IInitialTransitionConfiguration<TState, TTransition> If(Func<CancellationToken, Task<bool>> condition);

    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IInitialTransitionConfiguration<TState, TTransition> If(Func<CancellationToken, ValueTask<bool>> condition);

    /// <summary>
    ///     Configures the transition to have no parameters. This is optional and is implied if no parameter
    ///     configuration is explicitly made, such as calling <see cref="WithParameter{TNext}"/>.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition> WithNoParameters();

    /// <summary>
    ///     Configures the transition to have one parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>();

    /// <summary>
    ///     Configures the transition to have two parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition, T1, T2> WithParameters<T1, T2>();

    /// <summary>
    ///     Configures the transition to have three parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition, T1, T2, T3> WithParameters<T1, T2, T3>();

    /// <summary>
    ///     Configures the transition to have four parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> WithParameters<T1, T2, T3, T4>();

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

/// <summary>
///     The initial transition configuration which provides an opportunity to specify parameters and add conditions
///     based on the previous state's parameter. Conditions added here receive the previous state's parameter value.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TPrevious">The type of the parameter for the previous state.</typeparam>
/// <remarks>
///     <para>
///     Conditions added via <see cref="If(Func{TPrevious, bool})"/> are evaluated in the order they are registered.
///     All conditions must return <see langword="true"/> for the transition to proceed (logical AND).
///     Evaluation short-circuits on the first <see langword="false"/> result.
///     </para>
///     <para>
///     Conditions added at this stage (before calling <see cref="WithParameter{TNext}"/> or
///     <see cref="WithNoParameters"/>) are evaluated before any conditions added afterward.
///     </para>
/// </remarks>
public interface IInitialTransitionConfiguration<TState, TTransition, TPrevious>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    ///     The condition receives the previous state's parameter value.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IInitialTransitionConfiguration<TState, TTransition, TPrevious> If(Func<TPrevious, bool> condition);

    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    ///     The condition receives the previous state's parameter value.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IInitialTransitionConfiguration<TState, TTransition, TPrevious> If(
        Func<TPrevious, CancellationToken, Task<bool>> condition
    );

    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    ///     The condition receives the previous state's parameter value.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IInitialTransitionConfiguration<TState, TTransition, TPrevious> If(
        Func<TPrevious, CancellationToken, ValueTask<bool>> condition
    );

    /// <summary>
    ///     Configures the transition to have no parameters. This is optional and is implied if no parameter
    ///     configuration is explicitly made, such as calling <see cref="WithParameter{TNext}"/>.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition> WithNoParameters();

    /// <summary>
    ///     Configures the transition to have one parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>();

    /// <summary>
    ///     Configures the transition to have two parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition, T1, T2> WithParameters<T1, T2>();

    /// <summary>
    ///     Configures the transition to have three parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition, T1, T2, T3> WithParameters<T1, T2, T3>();

    /// <summary>
    ///     Configures the transition to have four parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IDirectTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> WithParameters<T1, T2, T3, T4>();

    /// <summary>
    ///     Configures the transition to map the previous parameter of type <typeparamref name="TPrevious"/> to the next
    ///     parameter of type <typeparamref name="TNext"/>. The state machine should be activated with
    ///     <see cref="IStateMachine{TState,TTransition}.Transition(TTransition,CancellationToken)"/> to perform this
    ///     transition.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <param name="map">
    ///     The mapping function to apply to the parameter from the previous state to generate the parameter for the
    ///     next state.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(Func<TPrevious, TNext> map);

    /// <summary>
    ///     Configures the transition to map the previous parameter of type <typeparamref name="TPrevious"/> to two
    ///     parameters for the next state.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <param name="map">
    ///     The mapping function to apply to the parameter from the previous state to generate the parameters
    ///     for the next state.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2> WithMappedParameters<TN1, TN2>(
        Func<TPrevious, (TN1, TN2)> map
    );

    /// <summary>
    ///     Configures the transition to map the previous parameter of type <typeparamref name="TPrevious"/> to three
    ///     parameters for the next state.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TN3">The type of the third parameter for the next state.</typeparam>
    /// <param name="map">
    ///     The mapping function to apply to the parameter from the previous state to generate the parameters
    ///     for the next state.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithMappedParameters<TN1, TN2, TN3>(
        Func<TPrevious, (TN1, TN2, TN3)> map
    );

    /// <summary>
    ///     Configures the transition to map the previous parameter of type <typeparamref name="TPrevious"/> to four
    ///     parameters for the next state.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TN3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="TN4">The type of the fourth parameter for the next state.</typeparam>
    /// <param name="map">
    ///     The mapping function to apply to the parameter from the previous state to generate the parameters
    ///     for the next state.
    /// </param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithMappedParameters<TN1, TN2, TN3, TN4>(
        Func<TPrevious, (TN1, TN2, TN3, TN4)> map
    );

    /// <summary>
    ///     Configures the transition to pass the parameter from the previous state to the next state, unchanged.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TPrevious> WithSameParameter()
    {
        return WithMappedParameter(previous => previous);
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

/// <summary>
///     The initial transition configuration for a state with two parameters. Conditions added here receive the
///     previous state's two parameter values.
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
public interface IInitialTransitionConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.If(Func{TPrevious,bool})"/>
    IInitialTransitionConfiguration<TState, TTransition, T1, T2> If(Func<T1, T2, bool> condition);

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.If(Func{TPrevious,CancellationToken,Task{bool}})"/>
    IInitialTransitionConfiguration<TState, TTransition, T1, T2> If(
        Func<T1, T2, CancellationToken, Task<bool>> condition
    );

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.If(Func{TPrevious,CancellationToken,ValueTask{bool}})"/>
    IInitialTransitionConfiguration<TState, TTransition, T1, T2> If(
        Func<T1, T2, CancellationToken, ValueTask<bool>> condition
    );

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithNoParameters"/>
    IDirectTransitionConfiguration<TState, TTransition> WithNoParameters();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameter{TNext}"/>
    IDirectTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2}"/>
    IDirectTransitionConfiguration<TState, TTransition, TN1, TN2> WithParameters<TN1, TN2>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2,T3}"/>
    IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithParameters<TN1, TN2, TN3>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2,T3,T4}"/>
    IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithParameters<TN1, TN2, TN3, TN4>();

    /// <summary>
    ///     Configures the transition to map the previous two parameters to a single next parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(Func<T1, T2, TNext> map);

    /// <summary>
    ///     Configures the transition to map the previous two parameters to two next parameters.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2> WithMappedParameters<TN1, TN2>(
        Func<T1, T2, (TN1, TN2)> map
    );

    /// <summary>
    ///     Configures the transition to map the previous two parameters to three next parameters.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TN3">The type of the third parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithMappedParameters<TN1, TN2, TN3>(
        Func<T1, T2, (TN1, TN2, TN3)> map
    );

    /// <summary>
    ///     Configures the transition to map the previous two parameters to four next parameters.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TN3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="TN4">The type of the fourth parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithMappedParameters<TN1, TN2, TN3, TN4>(
        Func<T1, T2, (TN1, TN2, TN3, TN4)> map
    );

    /// <summary>
    ///     Configures the transition to pass the parameters from the previous state to the next state, unchanged.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, T1, T2> WithSameParameters()
    {
        return WithMappedParameters<T1, T2>((a, b) => (a, b));
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

/// <summary>
///     The initial transition configuration for a state with three parameters. Conditions added here receive the
///     previous state's three parameter values.
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
public interface IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.If(Func{TPrevious,bool})"/>
    IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3> If(Func<T1, T2, T3, bool> condition);

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.If(Func{TPrevious,CancellationToken,Task{bool}})"/>
    IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3> If(
        Func<T1, T2, T3, CancellationToken, Task<bool>> condition
    );

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.If(Func{TPrevious,CancellationToken,ValueTask{bool}})"/>
    IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3> If(
        Func<T1, T2, T3, CancellationToken, ValueTask<bool>> condition
    );

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithNoParameters"/>
    IDirectTransitionConfiguration<TState, TTransition> WithNoParameters();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameter{TNext}"/>
    IDirectTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2}"/>
    IDirectTransitionConfiguration<TState, TTransition, TN1, TN2> WithParameters<TN1, TN2>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2,T3}"/>
    IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithParameters<TN1, TN2, TN3>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2,T3,T4}"/>
    IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithParameters<TN1, TN2, TN3, TN4>();

    /// <summary>
    ///     Configures the transition to map the previous three parameters to a single next parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext> WithMappedParameter<TNext>(Func<T1, T2, T3, TNext> map);

    /// <summary>
    ///     Configures the transition to map the previous three parameters to two next parameters.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2> WithMappedParameters<TN1, TN2>(
        Func<T1, T2, T3, (TN1, TN2)> map
    );

    /// <summary>
    ///     Configures the transition to map the previous three parameters to three next parameters.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TN3">The type of the third parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithMappedParameters<TN1, TN2, TN3>(
        Func<T1, T2, T3, (TN1, TN2, TN3)> map
    );

    /// <summary>
    ///     Configures the transition to map the previous three parameters to four next parameters.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TN3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="TN4">The type of the fourth parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithMappedParameters<TN1, TN2, TN3, TN4>(
        Func<T1, T2, T3, (TN1, TN2, TN3, TN4)> map
    );

    /// <summary>
    ///     Configures the transition to pass the parameters from the previous state to the next state, unchanged.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, T1, T2, T3> WithSameParameters()
    {
        return WithMappedParameters<T1, T2, T3>((a, b, c) => (a, b, c));
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
    IDirectTransitionConfiguration<TState, TTransition, TN1, TN2> WithParameters<TN1, TN2>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2,T3}"/>
    IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithParameters<TN1, TN2, TN3>();

    /// <inheritdoc cref="IInitialTransitionConfiguration{TState,TTransition,TPrevious}.WithParameters{T1,T2,T3,T4}"/>
    IDirectTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithParameters<TN1, TN2, TN3, TN4>();

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
    ///     Configures the transition to map the previous four parameters to two next parameters.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2> WithMappedParameters<TN1, TN2>(
        Func<T1, T2, T3, T4, (TN1, TN2)> map
    );

    /// <summary>
    ///     Configures the transition to map the previous four parameters to three next parameters.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TN3">The type of the third parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3> WithMappedParameters<TN1, TN2, TN3>(
        Func<T1, T2, T3, T4, (TN1, TN2, TN3)> map
    );

    /// <summary>
    ///     Configures the transition to map the previous four parameters to four next parameters.
    /// </summary>
    /// <typeparam name="TN1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="TN2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="TN3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="TN4">The type of the fourth parameter for the next state.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TN1, TN2, TN3, TN4> WithMappedParameters<TN1, TN2, TN3, TN4>(
        Func<T1, T2, T3, T4, (TN1, TN2, TN3, TN4)> map
    );

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
