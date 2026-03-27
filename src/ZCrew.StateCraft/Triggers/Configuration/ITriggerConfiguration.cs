using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Triggers.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Base interface for trigger configuration. This interface is used as a marker for the fluent configuration API.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface ITriggerConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Builds the trigger instance for the specified state machine.
    /// </summary>
    /// <param name="stateMachine">The state machine that the trigger will be associated with.</param>
    /// <returns>The constructed trigger instance.</returns>
    internal ITrigger Build(IStateMachine<TState, TTransition> stateMachine);
}
