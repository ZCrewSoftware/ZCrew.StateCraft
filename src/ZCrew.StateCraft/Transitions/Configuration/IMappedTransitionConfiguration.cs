namespace ZCrew.StateCraft;

/// <summary>
///     Configures a mapped transition from a parameterized state. Conditions added here receive the mapped parameter
///     value (transformed from the previous state's parameter to the next state's parameter type). The next state must
///     be specified using <see cref="To"/>.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TNext">The type of the mapped parameter for the next state.</typeparam>
/// <remarks>
///     <para>
///     Conditions added via <see cref="If(Func{TNext, bool})"/> are evaluated in the order they are registered.
///     All conditions must return <see langword="true"/> for the transition to proceed (logical AND).
///     Evaluation short-circuits on the first <see langword="false"/> result.
///     </para>
///     <para>
///     Conditions added at this stage are evaluated after any conditions added via
///     <see cref="IInitialTransitionConfiguration{TState, TTransition}"/>.
///     </para>
/// </remarks>
public interface IMappedTransitionConfiguration<TState, TTransition, TNext>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    ///     The condition receives the mapped parameter value from the previous that will be passed to the next state.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext> If(Func<TNext, bool> condition);

    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    ///     The condition receives the mapped parameter value from the previous that will be passed to the next state.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext> If(Func<TNext, CancellationToken, Task<bool>> condition);

    /// <summary>
    ///     Configures a <paramref name="condition"/> which will be evaluated when resolving which transition to use.
    ///     The condition receives the mapped parameter value from the previous that will be passed to the next state.
    /// </summary>
    /// <param name="condition">The delegate to check when resolving the transition.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IMappedTransitionConfiguration<TState, TTransition, TNext> If(
        Func<TNext, CancellationToken, ValueTask<bool>> condition
    );

    /// <summary>
    ///     Configure the state to transition to.
    /// </summary>
    /// <param name="state">The next state.</param>
    /// <returns>The final state configuration step.</returns>
    ITransitionConfiguration<TState, TTransition> To(TState state);

    /// <summary>
    ///     Configures this transition to return to the same state.
    /// </summary>
    /// <returns>The final state configuration step.</returns>
    /// <remarks>
    ///     Since a state may have the same <see cref="IState{TState,TTransition}.StateValue"/> but different parameters
    ///     this may point to a different configured state. This is merely shorthand for <see cref="To"/> with the same
    ///     <see cref="IState{TState,TTransition}.StateValue"/>.
    /// </remarks>
    ITransitionConfiguration<TState, TTransition> ToSameState();
}

/// <summary>
///     Configures a mapped transition that produces two parameters for the next state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TNext1">The type of the first mapped parameter for the next state.</typeparam>
/// <typeparam name="TNext2">The type of the second mapped parameter for the next state.</typeparam>
public interface IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,bool})"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> If(Func<TNext1, TNext2, bool> condition);

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,CancellationToken,Task{bool}})"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> If(
        Func<TNext1, TNext2, CancellationToken, Task<bool>> condition
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,CancellationToken,ValueTask{bool}})"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2> If(
        Func<TNext1, TNext2, CancellationToken, ValueTask<bool>> condition
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.To"/>
    ITransitionConfiguration<TState, TTransition> To(TState state);

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.ToSameState"/>
    ITransitionConfiguration<TState, TTransition> ToSameState();
}

/// <summary>
///     Configures a mapped transition that produces three parameters for the next state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TNext1">The type of the first mapped parameter for the next state.</typeparam>
/// <typeparam name="TNext2">The type of the second mapped parameter for the next state.</typeparam>
/// <typeparam name="TNext3">The type of the third mapped parameter for the next state.</typeparam>
public interface IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,bool})"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> If(
        Func<TNext1, TNext2, TNext3, bool> condition
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,CancellationToken,Task{bool}})"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> If(
        Func<TNext1, TNext2, TNext3, CancellationToken, Task<bool>> condition
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,CancellationToken,ValueTask{bool}})"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> If(
        Func<TNext1, TNext2, TNext3, CancellationToken, ValueTask<bool>> condition
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.To"/>
    ITransitionConfiguration<TState, TTransition> To(TState state);

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.ToSameState"/>
    ITransitionConfiguration<TState, TTransition> ToSameState();
}

/// <summary>
///     Configures a mapped transition that produces four parameters for the next state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TNext1">The type of the first mapped parameter for the next state.</typeparam>
/// <typeparam name="TNext2">The type of the second mapped parameter for the next state.</typeparam>
/// <typeparam name="TNext3">The type of the third mapped parameter for the next state.</typeparam>
/// <typeparam name="TNext4">The type of the fourth mapped parameter for the next state.</typeparam>
public interface IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,bool})"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> If(
        Func<TNext1, TNext2, TNext3, TNext4, bool> condition
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,CancellationToken,Task{bool}})"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> If(
        Func<TNext1, TNext2, TNext3, TNext4, CancellationToken, Task<bool>> condition
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.If(Func{TNext,CancellationToken,ValueTask{bool}})"/>
    IMappedTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3, TNext4> If(
        Func<TNext1, TNext2, TNext3, TNext4, CancellationToken, ValueTask<bool>> condition
    );

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.To"/>
    ITransitionConfiguration<TState, TTransition> To(TState state);

    /// <inheritdoc cref="IMappedTransitionConfiguration{TState,TTransition,TNext}.ToSameState"/>
    ITransitionConfiguration<TState, TTransition> ToSameState();
}
