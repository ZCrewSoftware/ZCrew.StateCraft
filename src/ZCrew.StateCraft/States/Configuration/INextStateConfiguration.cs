using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States.Configuration;

/// <summary>
///     Configuration for a next state in a transition, including its conditions and target state value.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal interface INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Gets a value indicating whether this next state has any conditions that must be evaluated.
    /// </summary>
    bool IsConditional { get; }

    /// <summary>
    ///     Gets the state value that this configuration targets.
    /// </summary>
    TState StateValue { get; }

    /// <summary>
    ///     Gets the type parameters associated with the target state.
    /// </summary>
    IReadOnlyList<Type> TypeParameters { get; }

    /// <summary>
    ///     Builds the runtime <see cref="INextState{TState, TTransition}"/> from this configuration by resolving
    ///     the target state from the provided state table.
    /// </summary>
    /// <param name="stateTable">The state table to resolve the target state from.</param>
    /// <returns>A new <see cref="INextState{TState, TTransition}"/> instance.</returns>
    INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable);
}
