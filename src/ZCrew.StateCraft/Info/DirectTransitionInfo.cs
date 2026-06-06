using ZCrew.StateCraft.Identities.Extensions;

namespace ZCrew.StateCraft.Info;

/// <inheritdoc />
internal sealed class DirectTransitionInfo<TState, TTransition> : IDirectTransitionInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public DirectTransitionInfo(
        IStateMachineInfo<TState, TTransition> stateMachine,
        TTransition transitionValue,
        IReadOnlyList<Type> transitionParameterTypes,
        IConditionalStateInfo<TState, TTransition> previousState,
        IConditionalStateInfo<TState, TTransition> nextState
    )
    {
        StateMachine = stateMachine;
        TransitionValue = transitionValue;
        TransitionParameterTypes = transitionParameterTypes;
        PreviousState = previousState;
        NextState = nextState;
    }

    /// <inheritdoc />
    public IStateMachineInfo<TState, TTransition> StateMachine { get; }

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TransitionParameterTypes { get; }

    /// <inheritdoc />
    public bool IsConditional => PreviousState.Conditions.Count > 0 || NextState.Conditions.Count > 0;

    /// <inheritdoc />
    public IConditionalStateInfo<TState, TTransition> PreviousState { get; }

    /// <inheritdoc />
    public IConditionalStateInfo<TState, TTransition> NextState { get; }

    /// <inheritdoc cref="IIdentity.ToString" />
    public override string ToString()
    {
        return this.RenderFromOneToOne(PreviousState, NextState);
    }
}
