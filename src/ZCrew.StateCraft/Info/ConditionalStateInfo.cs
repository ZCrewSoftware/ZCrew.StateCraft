namespace ZCrew.StateCraft.Info;

/// <inheritdoc cref="IConditionalStateInfo{TState, TTransition}" />
internal sealed class ConditionalStateInfo<TState, TTransition>
    : StateInfo<TState, TTransition>,
        IConditionalStateInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public ConditionalStateInfo(
        IStateMachineInfo<TState, TTransition> stateMachine,
        TState stateValue,
        IReadOnlyList<Type> stateParameterTypes,
        IReadOnlyList<IConditionInfo> conditions
    )
        : base(stateMachine, stateValue, stateParameterTypes)
    {
        Conditions = conditions;
    }

    /// <inheritdoc />
    public IReadOnlyList<IConditionInfo> Conditions { get; }
}
