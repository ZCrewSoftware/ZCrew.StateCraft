namespace ZCrew.StateCraft.UnitTests.Stubs;

/// <remarks>
///     This stub provides common behavior for the properties of
///     <see cref="IParameterizedTransition{TState,TTransition,TNext}"/>. To verify method behavior you should mock the
///     methods as necessary.
/// </remarks>
internal class StubParameterizedTransition<TState, TTransition, TNext>
    : IParameterizedTransition<TState, TTransition, TNext>
    where TState : notnull
    where TTransition : notnull
{
    public StubParameterizedTransition(TState previousState, TTransition transition, TState nextState)
    {
        PreviousStateValue = previousState;
        PreviousState = new StubParameterlessState<TState, TTransition>(previousState);
        TransitionValue = transition;
        NextStateValue = nextState;
        NextState = new StubParameterizedState<TState, TTransition, TNext>(nextState);
    }

    public TState PreviousStateValue { get; }

    public IState<TState, TTransition> PreviousState { get; }

    public TTransition TransitionValue { get; }

    public TState NextStateValue { get; }

    public IState<TState, TTransition> NextState { get; }

    public IReadOnlyList<Type> PreviousStateTypeParameters => [];
    public IReadOnlyList<Type> TransitionTypeParameters => [typeof(TNext)];
    public IReadOnlyList<Type> NextStateTypeParameters => [typeof(TNext)];

    public virtual Task Transition(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task<bool> EvaluateConditions(TNext nextParameter, CancellationToken token)
    {
        return Task.FromResult(true);
    }
}

/// <remarks>
///     This stub provides common behavior for the properties of
///     <see cref="IParameterizedTransition{TState,TTransition,TPrevious,TNext}"/>. To verify method behavior you should
///     mock the methods as necessary.
/// </remarks>
internal class StubParameterizedTransition<TState, TTransition, TPrevious, TNext>
    : IParameterizedTransition<TState, TTransition, TPrevious, TNext>
    where TState : notnull
    where TTransition : notnull
{
    public StubParameterizedTransition(TState previousState, TTransition transition, TState nextState)
    {
        PreviousStateValue = previousState;
        PreviousState = new StubParameterizedState<TState, TTransition, TPrevious>(previousState);
        TransitionValue = transition;
        NextStateValue = nextState;
        NextState = new StubParameterizedState<TState, TTransition, TNext>(nextState);
    }

    public TState PreviousStateValue { get; }

    public IState<TState, TTransition> PreviousState { get; }

    public TTransition TransitionValue { get; }

    public TState NextStateValue { get; }

    public IState<TState, TTransition> NextState { get; }

    public IReadOnlyList<Type> PreviousStateTypeParameters => [typeof(TPrevious)];
    public IReadOnlyList<Type> TransitionTypeParameters => [typeof(TNext)];
    public IReadOnlyList<Type> NextStateTypeParameters => [typeof(TNext)];

    public virtual Task Transition(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task<bool> EvaluateConditions(
        TPrevious previousParameter,
        TNext nextParameter,
        CancellationToken token
    )
    {
        return Task.FromResult(true);
    }
}
