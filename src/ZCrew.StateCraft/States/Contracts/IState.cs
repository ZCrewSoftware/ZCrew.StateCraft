using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Represents common functionality to describe a state, regardless of the parameters or semantics of the state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal interface IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The state value.
    /// </summary>
    TState StateValue { get; }

    /// <summary>
    ///     The type parameters of the state.
    /// </summary>
    IReadOnlyList<Type> TypeParameters { get; }

    /// <summary>
    ///     The state machine that contains this state.
    /// </summary>
    IStateMachine<TState, TTransition> StateMachine { get; }

    /// <summary>
    ///     The transitions from this state.
    /// </summary>
    IEnumerable<ITransition<TState, TTransition>> Transitions { get; }

    /// <summary>
    ///     Adds the <paramref name="transition"/> to this state.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    void AddTransition(ITransition<TState, TTransition> transition);

    /// <summary>
    ///     Gets the transition to the next state.
    /// </summary>
    /// <param name="transition">The transition to look up.</param>
    /// <param name="parameters">The current parameters and transition parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>The first matching transition whose type and conditions match.</returns>
    /// <exception cref="InvalidOperationException">No matching transition was found.</exception>
    Task<ITransition<TState, TTransition>> GetTransition(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    );

    /// <summary>
    ///     Gets the transition to a parameterless next state, or <see langword="null"/> if no matching transition was
    ///     found.
    /// </summary>
    /// <param name="transition">The transition to look up.</param>
    /// <param name="parameters">The current parameters and transition parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition with passing conditions.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    );

    /// <summary>
    ///     Invoke the activation handlers when the state machine is activated with this state.
    /// </summary>
    /// <param name="parameters">The current parameters and transition parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Activate(IStateMachineParameters parameters, CancellationToken token);

    /// <summary>
    ///     Invoke the deactivation handlers when the state machine is deactivated with this state.
    /// </summary>
    /// <param name="parameters">The current parameters and transition parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Deactivate(IStateMachineParameters parameters, CancellationToken token);

    /// <summary>
    ///     Invoke the actions when entering this state after the state has been changed successfully.
    /// </summary>
    /// <param name="parameters">The current parameters and transition parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Enter(IStateMachineParameters parameters, CancellationToken token);

    /// <summary>
    ///     Invoke the actions when exiting this state before the state has been changed and after the next transition
    ///     has been identified successfully.
    /// </summary>
    /// <param name="parameters">The current parameters and transition parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Exit(IStateMachineParameters parameters, CancellationToken token);

    /// <summary>
    ///     Invoke the actions representing the main functionality of the state.
    /// </summary>
    /// <param name="parameters">The current parameters and transition parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Action(IStateMachineParameters parameters, CancellationToken token);

    /// <summary>
    ///     Invoke the state change handlers when transitioning into this state.
    /// </summary>
    /// <param name="previousState">The state being transitioned from.</param>
    /// <param name="transition">The transition being applied.</param>
    /// <param name="parameters">The state machine parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task StateChange(
        TState previousState,
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    );
}
