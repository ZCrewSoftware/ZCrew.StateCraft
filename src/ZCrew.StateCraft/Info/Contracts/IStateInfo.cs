namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata describing a single state configured on a state machine. Captures the state's
///     identity — its value and parameter shape.
/// </summary>
/// <remarks>
///     <para>
///     Two states with the same <see cref="StateValue"/> but different <see cref="StateParameterTypes"/> are
///     distinct configured states.
///     </para>
///     <para>
///     Transitions are not exposed per-state — <see cref="IStateMachineInfo{TState, TTransition}.Transitions"/> is
///     the canonical transition list. Consumers wanting the transitions whose source is a particular state filter
///     that collection and handle each variant's notion of previous state (see
///     <see cref="ITransitionInfo{TTransition}"/> subtypes).
///     </para>
/// </remarks>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
public interface IStateInfo<TState>
    where TState : notnull
{
    /// <summary>
    ///     The state value.
    /// </summary>
    TState StateValue { get; }

    /// <summary>
    ///     The parameter types declared on this state, in declaration order. Empty for a parameterless state.
    /// </summary>
    IReadOnlyList<Type> StateParameterTypes { get; }
}
