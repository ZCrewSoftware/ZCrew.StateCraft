using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.States.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.UnitTests.Stubs;

/// <remarks>
///     This stub provides common behavior for a parameterless-to-parameterized transition implementing
///     <see cref="ITransition{TState,TTransition}"/>. To verify method behavior you should mock the
///     methods as necessary.
/// </remarks>
internal class StubParameterizedTransition<TState, TTransition, TNext> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubParameterizedTransition(TState previousState, TTransition transition, TState nextState)
    {
        Previous = new StubStateRef<TState, TTransition>(
            new StubParameterlessState<TState, TTransition>(previousState)
        );
        TransitionValue = transition;
        Next = new StubStateRef<TState, TTransition>(new StubParameterizedState<TState, TTransition, TNext>(nextState));
    }

    public IPreviousState<TState, TTransition> Previous { get; }

    public INextState<TState, TTransition> Next { get; }

    public TTransition TransitionValue { get; }

    public IReadOnlyList<Type> TransitionTypeParameters => [typeof(TNext)];

    public virtual Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.FromResult(true);
    }

    public virtual Task Transition(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}

/// <remarks>
///     This stub provides common behavior for a parameterized-to-parameterized transition implementing
///     <see cref="ITransition{TState,TTransition}"/>. To verify method behavior you should mock the
///     methods as necessary.
/// </remarks>
internal class StubParameterizedTransition<TState, TTransition, TPrevious, TNext> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubParameterizedTransition(TState previousState, TTransition transition, TState nextState)
    {
        Previous = new StubStateRef<TState, TTransition>(
            new StubParameterizedState<TState, TTransition, TPrevious>(previousState)
        );
        TransitionValue = transition;
        Next = new StubStateRef<TState, TTransition>(new StubParameterizedState<TState, TTransition, TNext>(nextState));
    }

    public IPreviousState<TState, TTransition> Previous { get; }

    public INextState<TState, TTransition> Next { get; }

    public TTransition TransitionValue { get; }

    public IReadOnlyList<Type> TransitionTypeParameters => [typeof(TNext)];

    public virtual Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.FromResult(true);
    }

    public virtual Task Transition(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
