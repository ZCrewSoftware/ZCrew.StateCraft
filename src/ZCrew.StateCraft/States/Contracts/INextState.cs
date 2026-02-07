using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.States.Contracts;

/// <summary>
///     Represents the next state that a transition will move to, along with any conditions
///     that must be satisfied before the transition is allowed.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal interface INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Gets the state that this next state references.
    /// </summary>
    IState<TState, TTransition> State { get; }

    /// <summary>
    ///     Gets a value indicating whether this next state has any conditions that must be evaluated.
    /// </summary>
    bool IsConditional { get; }

    /// <summary>
    ///     Evaluates any conditions associated with this next state.
    /// </summary>
    /// <param name="parameters">The parameters for the current transition.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     <see langword="true"/> if all conditions are satisfied or there are no conditions;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token);
}
