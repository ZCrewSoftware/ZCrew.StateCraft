using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.States.Contracts;

/// <summary>
///     Represents a state with a parameter.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T">The type of the parameter for this state.</typeparam>
internal interface IParameterizedState<TState, TTransition, in T> : IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Invoke the state change handlers when transitioning into this state.
    /// </summary>
    /// <param name="previousState">The state being transitioned from.</param>
    /// <param name="transition">The transition being applied.</param>
    /// <param name="parameter">The parameter for this state.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    [Obsolete($"Use method with {nameof(IStateMachineParameters)}")]
    Task StateChange(TState previousState, TTransition transition, T parameter, CancellationToken token);
}
