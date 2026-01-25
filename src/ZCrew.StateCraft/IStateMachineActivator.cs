using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Wraps the different techniques for storing the activating a state machine.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
internal interface IStateMachineActivator<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Retrieves the initial state, and activates the <paramref name="stateMachine"/>.
    /// </summary>
    /// <param name="stateMachine">The state machine.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token);
}
