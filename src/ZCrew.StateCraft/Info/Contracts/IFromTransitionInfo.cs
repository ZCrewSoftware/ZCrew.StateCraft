namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata for an inverted (<c>From</c>) transition: a transition declared on a destination
///     state and made available from every configured state except those listed in <see cref="ExcludedStates"/>.
/// </summary>
/// <remarks>
///     Unlike other variants, an inverted transition has no singular previous state — it spans many. Expansion to
///     per-source-state edges happens internally when the state machine is built; this interface surfaces only the
///     original declaration.
/// </remarks>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IFromTransitionInfo<TState, TTransition> : ITransitionInfo<TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The destination state for the transition.
    /// </summary>
    IStateInfo<TState> NextState { get; }

    /// <summary>
    ///     Conditions evaluated against the next-state parameters before the transition is taken. Evaluated in
    ///     registration order; all must return <see langword="true"/> for the transition to proceed. Empty when no
    ///     destination-side conditions are configured.
    /// </summary>
    IReadOnlyList<IConditionInfo> NextParameterConditions { get; }

    /// <summary>
    ///     Source states explicitly excluded from this transition. Every configured state not listed here is a
    ///     valid source.
    /// </summary>
    IReadOnlyList<IExcludedStateInfo<TState>> ExcludedStates { get; }
}
