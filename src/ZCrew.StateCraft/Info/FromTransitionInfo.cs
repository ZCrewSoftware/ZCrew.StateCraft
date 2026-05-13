namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class FromTransitionInfo<TState, TTransition> : IFromTransitionInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public FromTransitionInfo(
        TTransition transitionValue,
        IReadOnlyList<Type> transitionParameterTypes,
        IStateInfo<TState> nextState,
        IReadOnlyList<IConditionInfo> nextParameterConditions,
        IReadOnlyList<IStateInfo<TState>> excludedStates
    )
    {
        TransitionValue = transitionValue;
        TransitionParameterTypes = transitionParameterTypes;
        NextState = nextState;
        NextParameterConditions = nextParameterConditions;
        ExcludedStates = excludedStates;
    }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionParameterTypes { get; }

    /// <inheritdoc />
    public bool IsConditional => NextParameterConditions.Count > 0;

    /// <inheritdoc />
    public IStateInfo<TState> NextState { get; }

    /// <inheritdoc />
    public IReadOnlyList<IConditionInfo> NextParameterConditions { get; }

    /// <inheritdoc />
    public IReadOnlyList<IStateInfo<TState>> ExcludedStates { get; }
}
