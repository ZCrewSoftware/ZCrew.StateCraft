namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class StateMachineInfo<TState, TTransition> : IStateMachineInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IStateInfo<TState, TTransition>> states = [];
    private readonly List<ITransitionInfo<TState, TTransition>> transitions = [];

    public StateMachineInfo(IInitialStateInfo<TState, TTransition>? initialState)
    {
        InitialState = initialState;
    }

    /// <inheritdoc />
    public IInitialStateInfo<TState, TTransition>? InitialState { get; }

    /// <inheritdoc />
    public IReadOnlyList<IStateInfo<TState, TTransition>> States => this.states;

    /// <inheritdoc />
    public IReadOnlyList<ITransitionInfo<TState, TTransition>> Transitions => this.transitions;

    /// <summary>
    ///     Add the state to this info.
    /// </summary>
    /// <param name="state">The state info.</param>
    /// <remarks>
    ///     This info is modifiable to break the cyclical dependency.
    /// </remarks>
    public void Add(IStateInfo<TState, TTransition> state)
    {
        this.states.Add(state);
    }

    /// <summary>
    ///     Add the transition to this info.
    /// </summary>
    /// <param name="transition">The transition info.</param>
    /// <remarks>
    ///     This info is modifiable to break the cyclical dependency.
    /// </remarks>
    public void Add(ITransitionInfo<TState, TTransition> transition)
    {
        this.transitions.Add(transition);
    }
}
