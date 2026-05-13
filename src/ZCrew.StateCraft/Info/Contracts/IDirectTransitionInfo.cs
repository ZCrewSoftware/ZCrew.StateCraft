namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata for a direct transition: a transition declared on a source state with an explicit
///     destination state. The caller supplies the next state's parameters (if any) when invoking the transition.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IDirectTransitionInfo<TState, TTransition> : ITransitionInfo<TTransition>
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
    ///     Conditions evaluated against the source state's current parameters before the transition is taken.
    ///     Evaluated in registration order; all must return <see langword="true"/> for the transition to proceed.
    ///     Empty when no source-side conditions are configured.
    /// </summary>
    IReadOnlyList<IConditionInfo> PreviousParameterConditions { get; }

    /// <summary>
    ///     Conditions evaluated against the caller-supplied next-state parameters. Evaluated after
    ///     <see cref="PreviousParameterConditions"/> in registration order; all must return <see langword="true"/>
    ///     for the transition to proceed. Empty when no destination-side conditions are configured.
    /// </summary>
    IReadOnlyList<IConditionInfo> NextParameterConditions { get; }
}
