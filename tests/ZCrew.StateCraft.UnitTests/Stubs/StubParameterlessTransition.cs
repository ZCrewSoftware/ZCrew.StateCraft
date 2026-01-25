using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.UnitTests.Stubs;

/// <remarks>
///     This stub provides common behavior for the properties of
///     <see cref="IParameterlessTransition{TState,TTransition}"/>. To verify method behavior you should mock the
///     methods as necessary.
/// </remarks>
internal class StubParameterlessTransition<TState, TTransition> : IParameterlessTransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubParameterlessTransition(TState previousState, TTransition transition, TState nextState)
    {
        PreviousStateValue = previousState;
        PreviousState = new StubParameterlessState<TState, TTransition>(previousState);
        TransitionValue = transition;
        NextStateValue = nextState;
        NextState = new StubParameterlessState<TState, TTransition>(nextState);
    }

    public TState PreviousStateValue { get; }

    public IState<TState, TTransition> PreviousState { get; }

    public TTransition TransitionValue { get; }

    public TState NextStateValue { get; }

    public IState<TState, TTransition> NextState { get; }

    public IReadOnlyList<Type> PreviousStateTypeParameters => [];
    public IReadOnlyList<Type> TransitionTypeParameters => [];
    public IReadOnlyList<Type> NextStateTypeParameters => [];

    public virtual Task Transition(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task<bool> EvaluateConditions(CancellationToken token)
    {
        return Task.FromResult(true);
    }
}

/// <remarks>
///     This stub provides common behavior for the properties of
///     <see cref="IParameterlessTransition{TState,TTransition,TPrevious}"/>. To verify method behavior you should mock
///     the methods as necessary.
/// </remarks>
internal class StubParameterlessTransition<TState, TTransition, TPrevious>
    : IParameterlessTransition<TState, TTransition, TPrevious>
    where TState : notnull
    where TTransition : notnull
{
    public StubParameterlessTransition(TState previousState, TTransition transition, TState nextState)
    {
        PreviousStateValue = previousState;
        PreviousState = new StubParameterizedState<TState, TTransition, TPrevious>(previousState);
        TransitionValue = transition;
        NextStateValue = nextState;
        NextState = new StubParameterlessState<TState, TTransition>(nextState);
    }

    public TState PreviousStateValue { get; }

    public IState<TState, TTransition> PreviousState { get; }

    public TTransition TransitionValue { get; }

    public TState NextStateValue { get; }

    public IState<TState, TTransition> NextState { get; }

    public IReadOnlyList<Type> PreviousStateTypeParameters => [typeof(TPrevious)];
    public IReadOnlyList<Type> TransitionTypeParameters => [];
    public IReadOnlyList<Type> NextStateTypeParameters => [];

    public virtual Task Transition(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task<bool> EvaluateConditions(TPrevious previousParameter, CancellationToken token)
    {
        return Task.FromResult(true);
    }
}
