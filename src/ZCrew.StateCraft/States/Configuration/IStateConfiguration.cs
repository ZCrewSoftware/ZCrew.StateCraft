using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Represents common functionality to describe a state configuration, regardless of the parameters or semantics of
///     the state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The state value.
    /// </summary>
    internal TState State { get; }

    /// <summary>
    ///     The type parameters of the state.
    /// </summary>
    internal IReadOnlyList<Type> TypeParameters { get; }

    /// <summary>
    ///     The transitions in this configuration.
    /// </summary>
    internal IEnumerable<ITransitionConfiguration<TState, TTransition>> Transitions { get; }

    /// <summary>
    ///     Build the state based on the configuration.
    /// </summary>
    /// <param name="stateMachine">The parent state machine.</param>
    /// <returns>The state model.</returns>
    internal IState<TState, TTransition> Build(IStateMachine<TState, TTransition> stateMachine);
}
