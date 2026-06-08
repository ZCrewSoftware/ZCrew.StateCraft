namespace ZCrew.StateCraft;

/// <summary>
///     Configures which previous states participate in an inverted transition whose destination state has three
///     parameters. An inverted transition defines a destination state and then specifies which states can transition
///     to it, rather than configuring transitions from each source state individually.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="TNext1">The type of the first parameter for the destination state.</typeparam>
/// <typeparam name="TNext2">The type of the second parameter for the destination state.</typeparam>
/// <typeparam name="TNext3">The type of the third parameter for the destination state.</typeparam>
/// <remarks>
///     This interface is reached via
///     <see cref="IInitialTransitionConfiguration{TState, TTransition, T1, T2, T3}.From"/> when configuring a
///     transition on a state with three parameters. The state being configured becomes the destination, and
///     <see cref="AllStates"/> or <see cref="AllOtherStates"/> determines which states can transition to it.
/// </remarks>
public interface IFromTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IFromTransitionConfiguration{TState, TTransition}.AllStates"/>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> AllStates();

    /// <inheritdoc cref="IFromTransitionConfiguration{TState, TTransition}.AllOtherStates"/>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext1, TNext2, TNext3> AllOtherStates();
}
