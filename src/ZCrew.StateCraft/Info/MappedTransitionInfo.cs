namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class MappedTransitionInfo<TState, TTransition> : IMappedTransitionInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public MappedTransitionInfo(
        IStateMachineInfo<TState, TTransition> stateMachine,
        TTransition transitionValue,
        IConditionalStateInfo<TState, TTransition> previousState,
        IConditionalStateInfo<TState, TTransition> nextState,
        IMappingFunctionInfo mappingFunction
    )
    {
        StateMachine = stateMachine;
        TransitionValue = transitionValue;
        PreviousState = previousState;
        NextState = nextState;
        MappingFunction = mappingFunction;
    }

    /// <inheritdoc />
    public IStateMachineInfo<TState, TTransition> StateMachine { get; }

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
