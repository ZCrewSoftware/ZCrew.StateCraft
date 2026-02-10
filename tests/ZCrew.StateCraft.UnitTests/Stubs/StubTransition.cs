using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.States.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.UnitTests.Stubs;

/// <remarks>
///     This stub provides common behavior for a parameterless-to-parameterless transition implementing
///     <see cref="ITransition{TState,TTransition}"/>. To verify method behavior you should mock the
///     methods as necessary.
/// </remarks>
internal class StubTransition<TState, TTransition> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubTransition(TState previousState, TTransition transition, TState nextState)
        : this(previousState, transition, nextState, []) { }

    public StubTransition(TState previousState, TTransition transition, TState nextState, params Type[] typeParameters)
    {
        Previous = new StubStateRef<TState, TTransition>(new StubState<TState, TTransition>(previousState));
        TransitionValue = transition;
        Next = new StubStateRef<TState, TTransition>(new StubState<TState, TTransition>(nextState));
        TransitionTypeParameters = typeParameters;
    }

    public IPreviousState<TState, TTransition> Previous { get; }

    public INextState<TState, TTransition> Next { get; }

    public TTransition TransitionValue { get; }

    public IReadOnlyList<Type> TransitionTypeParameters { get; }

    public virtual Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.FromResult(true);
    }

    public virtual Task Transition(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
