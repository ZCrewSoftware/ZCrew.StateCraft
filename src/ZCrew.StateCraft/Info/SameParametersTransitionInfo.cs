namespace ZCrew.StateCraft.Info;

internal sealed class SameParametersTransitionInfo<TState, TTransition>
    : ISameParametersTransitionInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public SameParametersTransitionInfo(
        TTransition transitionValue,
        IReadOnlyList<Type> transitionParameterTypes,
        IStateInfo<TState> previousState,
        IStateInfo<TState> nextState,
        IReadOnlyList<IConditionInfo> previousParameterConditions,
        IReadOnlyList<IConditionInfo> nextParameterConditions
    )
    {
        TransitionValue = transitionValue;
        TransitionParameterTypes = transitionParameterTypes;
        PreviousState = previousState;
        NextState = nextState;
        PreviousParameterConditions = previousParameterConditions;
        NextParameterConditions = nextParameterConditions;
    }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionParameterTypes { get; }

    /// <inheritdoc />
    public bool IsConditional => PreviousParameterConditions.Count > 0 || NextParameterConditions.Count > 0;

    /// <inheritdoc />
    public IStateInfo<TState> PreviousState { get; }

    /// <inheritdoc />
    public IStateInfo<TState> NextState { get; }

    /// <inheritdoc />
    public IReadOnlyList<IConditionInfo> PreviousParameterConditions { get; }

    /// <inheritdoc />
    public IReadOnlyList<IConditionInfo> NextParameterConditions { get; }
}
