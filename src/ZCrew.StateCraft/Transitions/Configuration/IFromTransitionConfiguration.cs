namespace ZCrew.StateCraft;

/// <summary>
///     Configures which previous states participate in an inverted transition. An inverted transition defines a
///     destination state and then specifies which states can transition to it, rather than configuring transitions
///     from each source state individually.
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
///     This interface is reached via <see cref="IInitialTransitionConfiguration{TState, TTransition}.From"/> when
///     configuring a transition on a state. The state being configured becomes the destination, and
///     <see cref="AllStates"/> or <see cref="AllOtherStates"/> determines which states can transition to it.
/// </remarks>
public interface IFromTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures the transition to be available from all configured states, including the destination state itself
    ///     (reentrant).
    /// </summary>
    /// <returns>
    ///     A reference to the configuration after the configuration was updated. Use
    ///     <see cref="IFromAllStatesTransitionConfiguration{TState, TTransition}.Except(TState)"/> to exclude
    ///     specific states.
    /// </returns>
    IFromAllStatesTransitionConfiguration<TState, TTransition> AllStates();

    /// <summary>
    ///     Configures the transition to be available from all configured states except the destination state. This
    ///     prevents a reentrant transition where the state transitions to itself.
    /// </summary>
    /// <returns>
    ///     A reference to the configuration after the configuration was updated. Use
    ///     <see cref="IFromAllStatesTransitionConfiguration{TState, TTransition}.Except(TState)"/> to exclude
    ///     additional states.
    /// </returns>
    /// <remarks>
    ///     "Other" is determined by comparing both the state value and its type parameters. A state with the same
    ///     value but different type parameters is considered a different state and will be included.
    /// </remarks>
    IFromAllStatesTransitionConfiguration<TState, TTransition> AllOtherStates();
}
