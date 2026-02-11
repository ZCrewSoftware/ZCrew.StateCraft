using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.Tracking.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.StateMachines.Contracts;

/// <summary>
///     A state machine built from a <see cref="IStateMachineConfiguration{TState,TTransition}"/>.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IStateMachine<TState, TTransition> : IDisposable
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The current state of the state machine, or <see langword="null"/> if the state machine has not been
    ///     activated.
    /// </summary>
    internal IState<TState, TTransition>? CurrentState { get; }

    /// <summary>
    ///     The previous state of the state machine, or <see langword="null"/> if the state machine has not been
    ///     activated or is not transitioning.
    /// </summary>
    internal IState<TState, TTransition>? PreviousState { get; }

    /// <summary>
    ///     The next state of the state machine, or <see langword="null"/> if the state machine has not been
    ///     activated or is not transitioning.
    /// </summary>
    internal IState<TState, TTransition>? NextState { get; set; }

    /// <summary>
    ///     The parameters for the state machine when in a state or transitioning between states.
    /// </summary>
    internal IStateMachineParameters Parameters { get; }

    /// <summary>
    ///     The current transition that has been started. This will only be present when transitioning.
    /// </summary>
    internal ITransition<TState, TTransition>? CurrentTransition { get; }

    /// <summary>
    ///     The states in this state machine.
    /// </summary>
    internal StateTable<TState, TTransition> StateTable { get; }

    /// <summary>
    ///     The state machine tracker.
    /// </summary>
    internal ITracker<TState, TTransition>? Tracker { get; }

    /// <summary>
    ///     Activates the state machine by setting the initial state and invoking the activation handlers, entry
    ///     handlers, and state actions for that state. This method must be called exactly once before any transitions
    ///     are made.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">The state machine has already been activated.</exception>
    Task Activate(CancellationToken token = default);

    /// <summary>
    ///     Deactivates the state machine by invoking the exit handlers and deactivation handlers for the current
    ///     state. After deactivation, the state machine returns to an inactive state.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">The state machine has not been activated.</exception>
    Task Deactivate(CancellationToken token = default);

    /// <summary>
    ///     Transitions the state machine moving from the current state to a new state.
    /// </summary>
    /// <param name="transition">The transition to apply.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Transition(TTransition transition, CancellationToken token = default);

    /// <summary>
    ///     Transitions the state machine moving from the current state to a new state with a parameter.
    /// </summary>
    /// <param name="transition">The transition to apply.</param>
    /// <param name="parameter">The parameter for the next state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Transition<T>(TTransition transition, T parameter, CancellationToken token = default);

    /// <summary>
    ///     Transitions the state machine moving from the current state to a new state with two parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for the next state.</typeparam>
    /// <param name="transition">The transition to apply.</param>
    /// <param name="parameter1">The first parameter for the next state.</param>
    /// <param name="parameter2">The second parameter for the next state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Transition<T1, T2>(TTransition transition, T1 parameter1, T2 parameter2, CancellationToken token = default);

    /// <summary>
    ///     Transitions the state machine moving from the current state to a new state with three parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for the next state.</typeparam>
    /// <param name="transition">The transition to apply.</param>
    /// <param name="parameter1">The first parameter for the next state.</param>
    /// <param name="parameter2">The second parameter for the next state.</param>
    /// <param name="parameter3">The third parameter for the next state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Transition<T1, T2, T3>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        CancellationToken token = default
    );

    /// <summary>
    ///     Transitions the state machine moving from the current state to a new state with four parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for the next state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for the next state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for the next state.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter for the next state.</typeparam>
    /// <param name="transition">The transition to apply.</param>
    /// <param name="parameter1">The first parameter for the next state.</param>
    /// <param name="parameter2">The second parameter for the next state.</param>
    /// <param name="parameter3">The third parameter for the next state.</param>
    /// <param name="parameter4">The fourth parameter for the next state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Transition<T1, T2, T3, T4>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        CancellationToken token = default
    );

    /// <summary>
    ///     Determines whether a transition to a parameterless next state can be made from the current state.
    /// </summary>
    /// <param name="transition">The transition to check.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     <see langword="true"/> if a matching transition exists and its conditions are satisfied.
    ///     Otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    ///     Exceptions thrown by user-configured delegates (such as transition conditions) are propagated to the caller.
    ///     This method only returns <see langword="false"/> when no matching transition exists or when conditions
    ///     evaluate to <see langword="false"/>.
    /// </remarks>
    Task<bool> CanTransition(TTransition transition, CancellationToken token = default);

    /// <summary>
    ///     Determines whether a transition to a parameterized next state can be made from the current state.
    /// </summary>
    /// <typeparam name="T">The type of the next state's parameter.</typeparam>
    /// <param name="transition">The transition to check.</param>
    /// <param name="parameter">The parameter for the next state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     <see langword="true"/> if a matching transition exists and its conditions are satisfied.
    ///     Otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    ///     Exceptions thrown by user-configured delegates (such as transition conditions) are propagated to the caller.
    ///     This method only returns <see langword="false"/> when no matching transition exists or when conditions
    ///     evaluate to <see langword="false"/>.
    /// </remarks>
    Task<bool> CanTransition<T>(TTransition transition, T parameter, CancellationToken token = default);

    /// <summary>
    ///     Attempts to transition the state machine to a parameterless next state if a valid transition exists.
    /// </summary>
    /// <param name="transition">The transition to apply.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     <see langword="true"/> if the transition was performed successfully.
    ///     Otherwise, <see langword="false"/> if no matching transition exists or the transition's conditions are not
    ///     satisfied.
    /// </returns>
    /// <remarks>
    ///     Exceptions thrown by user-configured delegates (such as transition conditions or lifecycle handlers) are
    ///     propagated to the caller. This method only returns <see langword="false"/> when no matching transition
    ///     exists or when conditions evaluate to <see langword="false"/>. If an exception occurs during the state
    ///     lifecycle (exit, state change, or entry handlers), the state machine rolls back to the previous state before
    ///     rethrowing.
    /// </remarks>
    Task<bool> TryTransition(TTransition transition, CancellationToken token = default);

    /// <summary>
    ///     Attempts to transition the state machine to a parameterized next state if a valid transition exists.
    /// </summary>
    /// <typeparam name="T">The type of the next state's parameter.</typeparam>
    /// <param name="transition">The transition to apply.</param>
    /// <param name="parameter">The parameter for the next state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     <see langword="true"/> if the transition was performed successfully.
    ///     Otherwise, <see langword="false"/> if no matching transition exists or the transition's conditions are not
    ///     satisfied.
    /// </returns>
    /// <remarks>
    ///     Exceptions thrown by user-configured delegates (such as transition conditions or lifecycle handlers) are
    ///     propagated to the caller. This method only returns <see langword="false"/> when no matching transition
    ///     exists or when conditions evaluate to <see langword="false"/>. If an exception occurs during the state
    ///     lifecycle (exit, state change, or entry handlers), the state machine rolls back to the previous state before
    ///     rethrowing.
    /// </remarks>
    Task<bool> TryTransition<T>(TTransition transition, T parameter, CancellationToken token = default);

    /// <summary>
    ///     Invokes the state machine's state change handlers.
    /// </summary>
    /// <param name="previousState">The state being transitioned from.</param>
    /// <param name="transition">The transition being applied.</param>
    /// <param name="nextState">The state being transitioned to.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    internal Task StateChange(TState previousState, TTransition transition, TState nextState, CancellationToken token);

    /// <summary>
    ///     Executes an action with exception handling. Any exceptions thrown by the action are passed through
    ///     the registered exception handlers. If all handlers return <see cref="ExceptionResult.Continue"/>
    ///     or a handler returns <see cref="ExceptionResult.Rethrow"/>, the exception is rethrown.
    ///     If a handler returns <see cref="ExceptionResult.Throw"/>, the replacement exception is thrown.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    internal Task RunWithExceptionHandling(Func<Task> action, CancellationToken token);

    /// <summary>
    ///     Executes an action with exception handling. Any exceptions thrown by the action are passed through
    ///     the registered exception handlers. If all handlers return <see cref="ExceptionResult.Continue"/>
    ///     or a handler returns <see cref="ExceptionResult.Rethrow"/>, the exception is rethrown.
    ///     If a handler returns <see cref="ExceptionResult.Throw"/>, the replacement exception is thrown.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="throwOnCancellation">
    ///     If <see langword="true"/> then this will not throw <see cref="OperationCanceledException"/> when the
    ///     <paramref name="token"/> is canceled.
    /// </param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    internal Task RunWithExceptionHandling(Func<Task> action, bool throwOnCancellation, CancellationToken token);

    /// <summary>
    ///     Executes an action with exception handling and returns the result. Any exceptions thrown by the action
    ///     are passed through the registered exception handlers. If all handlers return
    ///     <see cref="ExceptionResult.Continue"/> or a handler returns <see cref="ExceptionResult.Rethrow"/>, the
    ///     exception is rethrown. If a handler returns <see cref="ExceptionResult.Throw"/>, the replacement exception
    ///     is thrown.
    /// </summary>
    /// <typeparam name="T">The return type of the action.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>The result of the action.</returns>
    internal Task<T> RunWithExceptionHandling<T>(Func<Task<T>> action, CancellationToken token);

    /// <summary>
    ///     Adds the <paramref name="state"/> to this machine.
    /// </summary>
    /// <param name="state">The state to add.</param>
    internal void AddState(IState<TState, TTransition> state);
}
