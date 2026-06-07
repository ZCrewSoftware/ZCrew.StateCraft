namespace ZCrew.StateCraft;

/// <summary>
///     Common shape for introspection metadata describing a transition between states.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface ITransitionInfo<TState, TTransition> : ITransitionIdentity<TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The owning state machine. The same <see cref="IStateMachineInfo{TState, TTransition}"/> instance that
    ///     exposes this transition via <see cref="IStateMachineInfo{TState, TTransition}.Transitions"/>, providing a
    ///     back reference for navigation from a transition to its peers.
    /// </summary>
    IStateMachineInfo<TState, TTransition> StateMachine { get; }

    /// <summary>
    ///     Whether this transition has any conditions gating it. A conditional transition has at least one
    ///     condition that must return <see langword="true"/> for the transition to be taken.
    /// </summary>
    bool IsConditional { get; }
}
