namespace ZCrew.StateCraft;

/// <summary>
///     A single step along a path through the transition graph: the transition taken and the state it lands on.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public class PathStepInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PathStepInfo{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="transition">The transition taken in this step.</param>
    /// <param name="nextState">The state entered after taking <paramref name="transition"/>.</param>
    public PathStepInfo(ITransitionInfo<TState, TTransition> transition, IStateInfo<TState, TTransition> nextState)
    {
        Transition = transition;
        NextState = nextState;
    }

    /// <summary>
    ///     The transition taken in this step.
    /// </summary>
    public ITransitionInfo<TState, TTransition> Transition { get; }

    /// <summary>
    ///     The state entered after taking <see cref="Transition"/>.
    /// </summary>
    public IStateInfo<TState, TTransition> NextState { get; }
}
