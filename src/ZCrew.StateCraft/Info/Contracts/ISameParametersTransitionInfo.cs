namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata for a reentrant-style transition that reuses the source state's parameters as the
///     next state's parameters without requiring the caller to supply them.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface ISameParametersTransitionInfo<TState, TTransition> : ITransitionInfo<TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The state the transition is taken from.
    /// </summary>
    IStateInfo<TState> PreviousState { get; }

    /// <summary>
    ///     The state the transition moves to. Its parameter types match <see cref="PreviousState"/>'s parameter
    ///     types.
    /// </summary>
    IStateInfo<TState> NextState { get; }

    /// <summary>
    ///     Conditions evaluated against the source state's current parameters. Evaluated in registration order;
    ///     all must return <see langword="true"/> for the transition to proceed. Empty when no source-side
    ///     conditions are configured.
    /// </summary>
    IReadOnlyList<IConditionInfo> PreviousParameterConditions { get; }

    /// <summary>
    ///     Conditions evaluated against the next-state parameters (which equal the source-state parameters).
    ///     Evaluated in registration order; all must return <see langword="true"/> for the transition to proceed.
    ///     Empty when no destination-side conditions are configured.
    /// </summary>
    IReadOnlyList<IConditionInfo> NextParameterConditions { get; }
}
