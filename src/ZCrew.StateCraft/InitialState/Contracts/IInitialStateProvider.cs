using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.InitialState.Contracts;

/// <summary>
///     Wraps the different techniques for storing the initial state of a state machine.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
internal interface IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Sets the initial state of the state machine.
    /// </summary>
    /// <param name="stateMachine">The state machine.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token);

    /// <summary>
    ///     Query information about the initial state.
    /// </summary>
    /// <returns>Information about the initial state.</returns>
    IInitialStateInfo<TState, TTransition> GetInfo();
}
