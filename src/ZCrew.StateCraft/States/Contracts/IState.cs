using ZCrew.StateCraft.StateMachines.Contracts;

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
    internal TState StateValue { get; }

    /// <summary>
    ///     The type parameters of the state.
    /// </summary>
    internal IReadOnlyList<Type> TypeParameters { get; }

    /// <summary>
    ///     The state machine that contains this state.
    /// </summary>
    internal IStateMachine<TState, TTransition> StateMachine { get; }

    /// <summary>
    ///     The transitions from this state.
    /// </summary>
    internal IEnumerable<ITransition<TState, TTransition>> Transitions { get; }

    /// <summary>
    ///     Gets the transition to a parameterless next state.
    /// </summary>
    /// <param name="transition">The transition to look up.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>The first matching transition whose conditions evaluate to true.</returns>
    /// <exception cref="InvalidOperationException">No matching transition was found.</exception>
    internal Task<ITransition<TState, TTransition>> GetTransition(TTransition transition, CancellationToken token);

    /// <summary>
    ///     Gets the transition to a parameterized next state.
    /// </summary>
    /// <typeparam name="TNext">The type of the next state's parameter.</typeparam>
    /// <param name="transition">The transition to look up.</param>
    /// <param name="nextParameter">
    ///     The parameter for the next state (used for type matching and condition evaluation).
    /// </param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>The first matching transition whose type and conditions match.</returns>
    /// <exception cref="InvalidOperationException">No matching transition was found.</exception>
    internal Task<ITransition<TState, TTransition>> GetTransition<TNext>(
        TTransition transition,
        TNext nextParameter,
        CancellationToken token
    );

    /// <summary>
    ///     Gets the transition to a parameterless next state, or <see langword="null"/> if no matching transition was
    ///     found.
    /// </summary>
    /// <param name="transition">The transition to look up.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose whose type and conditions.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    internal Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        CancellationToken token
    );

    /// <summary>
    ///     Gets the transition to a parameterized next state, or <see langword="null"/> if no matching transition was
    ///     found.
    /// </summary>
    /// <typeparam name="TNext">The type of the next state's parameter.</typeparam>
    /// <param name="transition">The transition to look up.</param>
    /// <param name="nextParameter">
    ///     The parameter for the next state (used for type matching and condition evaluation).
    /// </param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose whose type and conditions.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    internal Task<ITransition<TState, TTransition>?> GetTransitionOrDefault<TNext>(
        TTransition transition,
        TNext nextParameter,
        CancellationToken token
    );

    /// <summary>
    ///     Invoke the activation handlers when the state machine is activated with this state.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Activate(CancellationToken token);

    /// <summary>
    ///     Invoke the deactivation handlers when the state machine is deactivated with this state.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Deactivate(CancellationToken token);

    /// <summary>
    ///     Invoke the actions when entering this state after the state has been changed successfully.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Enter(CancellationToken token);

    /// <summary>
    ///     Invoke the actions when exiting this state before the state has been changed and after the next transition
    ///     has been identified successfully.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Exit(CancellationToken token);

    /// <summary>
    ///     Invoke the actions representing the main functionality of the state.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Action(CancellationToken token);
}
