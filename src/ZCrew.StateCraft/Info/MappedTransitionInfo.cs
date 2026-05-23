namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class MappedTransitionInfo<TState, TTransition> : IMappedTransitionInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public MappedTransitionInfo(
        TTransition transitionValue,
        IConditionalStateInfo<TState, TTransition> previousState,
        IConditionalStateInfo<TState, TTransition> nextState,
        IMappingFunctionInfo mappingFunction
    )
    {
        TransitionValue = transitionValue;
        PreviousState = previousState;
        NextState = nextState;
        MappingFunction = mappingFunction;
    }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionParameterTypes { get; } = [];

    /// <inheritdoc />
    public bool IsConditional => PreviousState.Conditions.Count > 0 || NextState.Conditions.Count > 0;

    /// <inheritdoc />
    public IConditionalStateInfo<TState, TTransition> PreviousState { get; }

    /// <inheritdoc />
    public IConditionalStateInfo<TState, TTransition> NextState { get; }

    /// <inheritdoc />
    public IMappingFunctionInfo MappingFunction { get; }
}
