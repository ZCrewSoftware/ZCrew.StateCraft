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
public interface IMappedTransitionInfo<TState, TTransition> : ITransitionInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The state the transition is taken from.
    /// </summary>
    IConditionalStateInfo<TState, TTransition> PreviousState { get; }

    /// <summary>
    ///     The state the transition moves to.
    /// </summary>
    IConditionalStateInfo<TState, TTransition> NextState { get; }

    /// <summary>
    ///     The mapping function that transforms the source state's parameters into the next state's parameters.
    /// </summary>
    IMappingFunctionInfo MappingFunction { get; }
}
