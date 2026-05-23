namespace ZCrew.StateCraft;

/// <summary>
///
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IConditionalStateInfo<TState, TTransition> : IStateInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Conditions evaluated against the caller-supplied parameters. If this state is the previous state, then these
    ///     conditions are evaluated first. If this state is the next state then these conditions are evaluated second.
    ///     Regardless, all conditions are evaluated in registration order; all must return <see langword="true"/>
    ///     for the transition to proceed. Empty when no conditions are configured.
    /// </summary>
    IReadOnlyList<IConditionInfo> Conditions { get; }
}
