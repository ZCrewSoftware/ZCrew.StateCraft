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
    IParameterlessTransitionConfiguration<TState, TTransition> WithNoParameters();

    /// <summary>
    ///     Configures the transition to have one parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedTransitionConfiguration<TState, TTransition, TNext> WithParameter<TNext>();

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
    IParameterlessTransitionConfiguration<TState, TTransition, TPrevious> WithNoParameters();

    /// <summary>
    ///     Configures the transition to have one parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the next state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedTransitionConfiguration<TState, TTransition, TPrevious, TNext> WithParameter<TNext>();

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
    IMappedTransitionConfiguration<TState, TTransition, TPrevious, TNext> WithMappedParameter<TNext>(
        Func<TPrevious, TNext> map
    );

    /// <summary>
    ///     Configures the transition to pass the parameter from the previous state to the next state, unchanged.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TPrevious, TPrevious> WithSameParameter()
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
