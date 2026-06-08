namespace ZCrew.StateCraft;

/// <summary>
///     Configures which previous states participate in an inverted transition whose destination state has a single
///     parameter. An inverted transition defines a destination state and then specifies which states can transition
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
/// <typeparam name="TNext">The type of the parameter for the destination state.</typeparam>
/// <remarks>
///     This interface is reached via <see cref="IInitialTransitionConfiguration{TState, TTransition, T}.From"/> when
///     configuring a transition on a parameterized state. The state being configured becomes the destination, and
///     <see cref="AllStates"/> or <see cref="AllOtherStates"/> determines which states can transition to it.
/// </remarks>
public interface IFromTransitionConfiguration<TState, TTransition, TNext>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IFromTransitionConfiguration{TState, TTransition}.AllStates"/>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> AllStates();

    /// <inheritdoc cref="IFromTransitionConfiguration{TState, TTransition}.AllOtherStates"/>
    IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> AllOtherStates();
}
