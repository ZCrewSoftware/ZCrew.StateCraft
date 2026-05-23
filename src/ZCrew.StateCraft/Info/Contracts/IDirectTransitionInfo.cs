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
public interface IDirectTransitionInfo<TState, TTransition> : ITransitionInfo<TState, TTransition>
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
}
