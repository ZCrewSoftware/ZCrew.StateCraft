namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class FromTransitionInfo<TState, TTransition> : IFromTransitionInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public FromTransitionInfo(
        IStateMachineInfo<TState, TTransition> stateMachine,
        TTransition transitionValue,
        IReadOnlyList<Type> transitionParameterTypes,
        IConditionalStateInfo<TState, TTransition> nextState,
        IReadOnlyList<IStateInfo<TState, TTransition>> excludedStates
    )
    {
        StateMachine = stateMachine;
        TransitionValue = transitionValue;
        TransitionParameterTypes = transitionParameterTypes;
        NextState = nextState;
        ExcludedStates = excludedStates;
    }

    /// <inheritdoc />
    public IStateMachineInfo<TState, TTransition> StateMachine { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionParameterTypes { get; }

    /// <inheritdoc />
    public bool IsConditional => NextState.Conditions.Count > 0;

    /// <inheritdoc />
    public IConditionalStateInfo<TState, TTransition> NextState { get; }

    /// <inheritdoc />
    public IReadOnlyList<IStateInfo<TState, TTransition>> ExcludedStates { get; }
}
