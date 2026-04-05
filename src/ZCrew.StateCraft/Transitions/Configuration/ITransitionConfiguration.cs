using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Represents common functionality to describe a transition configuration, regardless of the parameters or
///     semantics of the transition.
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
public interface ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Build the transition based on the configuration.
    /// </summary>
    /// <param name="stateMachine">The parent state machine.</param>
    internal void Build(IStateMachine<TState, TTransition> stateMachine);
}
