using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.UnitTests.Stubs;

/// <remarks>
///     This stub provides common behavior for the properties of
///     <see cref="IParameterizedState{TState,TTransition,T}"/>. To verify method behavior you should mock the
///     methods as necessary.
/// </remarks>
internal class StubParameterizedState<TState, TTransition, T> : IParameterizedState<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    public StubParameterizedState(TState state)
    {
        StateValue = state;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters => [typeof(T)];

    public IStateMachine<TState, TTransition> StateMachine => null!;

    public IEnumerable<ITransition<TState, TTransition>> Transitions { get; } = [];

    public virtual Task<ITransition<TState, TTransition>> GetTransition(TTransition transition, CancellationToken token)
    {
        throw new NotImplementedException("Mock this method to verify call behavior");
    }

    public virtual Task<ITransition<TState, TTransition>> GetTransition<TNext>(
        TTransition transition,
        TNext nextParameter,
        CancellationToken token
    )
    {
        throw new NotImplementedException("Mock this method to verify call behavior");
    }

    public virtual Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        CancellationToken token
    )
    {
        return Task.FromResult<ITransition<TState, TTransition>?>(null);
    }

    public virtual Task<ITransition<TState, TTransition>?> GetTransitionOrDefault<TNext>(
        TTransition transition,
        TNext nextParameter,
        CancellationToken token
    )
    {
        return Task.FromResult<ITransition<TState, TTransition>?>(null);
    }

    public virtual Task StateChange(TState previous, TTransition transition, T parameter, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task StateChange(
        TState previousState,
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        return Task.CompletedTask;
    }

    public virtual Task Activate(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Deactivate(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Enter(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Action(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Exit(CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
