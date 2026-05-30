namespace ZCrew.StateCraft.Info;

/// <summary>
///     An ordered path through the transition graph, made up of the <see cref="PathStepInfo{TState, TTransition}"/>
///     steps taken to get from a starting state to a destination state.
/// </summary>
/// <remarks>
///     The first step leaves the starting state and the last step lands on the destination state. An empty step list
///     represents a path of no transitions (the start and destination are the same state).
/// </remarks>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public class PathInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PathInfo{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="steps">The ordered steps that make up the path.</param>
    public PathInfo(IReadOnlyList<PathStepInfo<TState, TTransition>> steps)
    {
        Steps = steps;
    }

    /// <summary>
    ///     The ordered steps that make up the path, from the starting state to the destination state.
    /// </summary>
    public IReadOnlyList<PathStepInfo<TState, TTransition>> Steps { get; }
}
