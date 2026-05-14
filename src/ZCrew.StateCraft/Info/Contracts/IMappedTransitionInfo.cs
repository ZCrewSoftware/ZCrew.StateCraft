namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata for a mapped transition: a transition that produces the next state's parameters from
///     the source state's parameters using a mapping function. The caller does not supply parameters when invoking
///     it.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IMappedTransitionInfo<TState, TTransition> : ITransitionInfo<TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The state the transition is taken from.
    /// </summary>
    IStateInfo<TState> PreviousState { get; }

    /// <summary>
    ///     The state the transition moves to.
    /// </summary>
    IStateInfo<TState> NextState { get; }

    /// <summary>
    ///     Conditions evaluated against the source state's current parameters before <see cref="MappingFunction"/>
    ///     runs. Evaluated in registration order; all must return <see langword="true"/> for the transition to
    ///     proceed. Empty when no source-side conditions are configured.
    /// </summary>
    IReadOnlyList<IConditionInfo> PreviousParameterConditions { get; }

    /// <summary>
    ///     Conditions evaluated against the mapped values produced by <see cref="MappingFunction"/> before the
    ///     transition is taken. Evaluated in registration order; all must return <see langword="true"/> for the
    ///     transition to proceed. Empty when no destination-side conditions are configured.
    /// </summary>
    IReadOnlyList<IConditionInfo> NextParameterConditions { get; }

    /// <summary>
    ///     The mapping function that transforms the source state's parameters into the next state's parameters.
    /// </summary>
    IMappingFunctionInfo MappingFunction { get; }
}
