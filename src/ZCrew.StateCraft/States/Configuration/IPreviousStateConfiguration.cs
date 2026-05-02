using ZCrew.StateCraft.Rendering.Contracts;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States.Configuration;

/// <summary>
///     Configuration for a previous state in a transition, including its conditions and source state value.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal interface IPreviousStateConfiguration<TState, TTransition> : IRenderableConditions
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Gets a value indicating whether this previous state has any conditions that must be evaluated.
    /// </summary>
    bool IsConditional { get; }

    /// <summary>
    ///     Gets the state value that this configuration references.
    /// </summary>
    TState StateValue { get; }

    /// <summary>
    ///     Gets the type parameters associated with the source state.
    /// </summary>
    IReadOnlyList<Type> TypeParameters { get; }

    /// <summary>
    ///     Builds the runtime <see cref="IPreviousState{TState, TTransition}"/> from this configuration by resolving
    ///     the source state from the provided state table.
    /// </summary>
    /// <param name="stateTable">The state table to resolve the source state from.</param>
    /// <returns>A new <see cref="IPreviousState{TState, TTransition}"/> instance.</returns>
    IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable);

    /// <summary>
    ///     Transforms this state from a previous state to a next state. This is used for inverted transitions, which
    ///     start with a <see cref="IPreviousStateConfiguration{TState,TTransition}"/> but need to invert the direction
    ///     of the transition.
    /// </summary>
    /// <returns>The same previous state information representing a next state.</returns>
    INextStateConfiguration<TState, TTransition> ToNextStateConfiguration();
}
