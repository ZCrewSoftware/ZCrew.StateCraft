using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.Transitions.Contracts;

/// <summary>
///     Represents common functionality to describe a transition, regardless of the parameters or semantics of the
///     transition.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal interface ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The previous state reference.
    /// </summary>
    IPreviousState<TState, TTransition> Previous { get; }

    /// <summary>
    ///     The next state reference.
    /// </summary>
    INextState<TState, TTransition> Next { get; }

    /// <summary>
    ///     The transition value.
    /// </summary>
    TTransition TransitionValue { get; }

    /// <summary>
    ///     The type parameters of the transition. Empty if the transition can be invoked without providing a parameter.
    /// </summary>
    IReadOnlyList<Type> TransitionTypeParameters { get; }

    /// <summary>
    ///     Evaluate if the conditions to perform this transition have been met.
    /// </summary>
    /// <param name="parameters">The state machine parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the conditions have been met; <see langword="false"/> otherwise.</returns>
    Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token);

    /// <summary>
    ///     Performs the transition by invoking the state machine's state change handlers and the next state's state
    ///     change handlers.
    /// </summary>
    /// <param name="parameters">The state machine parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Transition(IStateMachineParameters parameters, CancellationToken token);
}
