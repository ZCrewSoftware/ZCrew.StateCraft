using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     <para>
///     Top-level introspection metadata describing a configured state machine. Captures the initial state, every
///     configured state, and every declared transition.
///     </para>
///     <para>
///     Intended for build-time validation, diagram rendering, and user-defined inspection. All transitions are
///     surfaced from <see cref="Transitions"/>; there is no per-state transition list on
///     <see cref="IStateInfo{TState}"/>.
///     </para>
/// </summary>
/// <remarks>
///     Consumers wanting "the transitions whose source is state X" filter <see cref="Transitions"/> and handle
///     each variant's notion of previous state (see <see cref="ITransitionInfo{TTransition}"/> subtypes). Inverted
///     transitions appear once per declaration, not once per matching source state — see
///     <see cref="IFromTransitionInfo{TState, TTransition}"/>.
/// </remarks>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IStateMachineInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The initial state configured on the machine. See <see cref="IStaticInitialStateInfo{TState}"/> and
    ///     <see cref="IDynamicInitialStateInfo{TState}"/> for the variants.
    /// </summary>
    /// <remarks>
    ///     This is only <see langword="null"/> if it has not been configured yet. If this information came from a
    ///     <see cref="IStateMachine{TState,TTransition}"/> then this will never be <see langword="null"/>.
    /// </remarks>
    IInitialStateInfo<TState>? InitialState { get; }

    /// <summary>
    ///     Every state configured on the machine, in declaration order.
    /// </summary>
    IReadOnlyList<IStateInfo<TState>> States { get; }

    /// <summary>
    ///     Every transition declared on the machine, in declaration order. Inverted (<c>From</c>) transitions
    ///     appear as a single <see cref="IFromTransitionInfo{TState, TTransition}"/> entry per declaration rather
    ///     than expanded per source state.
    /// </summary>
    IReadOnlyList<ITransitionInfo<TTransition>> Transitions { get; }
}
