using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Triggers.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Marks the end of the trigger configuration.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <remarks>
///     This should remain empty of public configuration members. This allows configuration steps to stop further
///     configuration by returning this type.
/// </remarks>
public interface IFinalTriggerConfiguration<TState, TTransition> : ITriggerConfiguration<TState, TTransition>
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
