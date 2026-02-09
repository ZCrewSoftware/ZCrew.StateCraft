using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.UnitTests.Stubs;

/// <remarks>
///     This stub provides common behavior for the properties of <see cref="IState{TState,TTransition}"/>. To verify
///     method behavior you should mock the methods as necessary.
/// </remarks>
internal class StubState<TState, TTransition> : IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubState(TState state)
    {
        StateValue = state;
    }

    public TState StateValue { get; }

    public IStateMachine<TState, TTransition> StateMachine => null!;

    public IReadOnlyList<Type> TypeParameters => [];

    public IEnumerable<ITransition<TState, TTransition>> Transitions { get; } = [];

    public void AddTransition(ITransition<TState, TTransition> transition) { }

    public virtual Task<ITransition<TState, TTransition>> GetTransition(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        throw new NotImplementedException("Mock this method to verify call behavior");
    }

    public virtual Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        return Task.FromResult<ITransition<TState, TTransition>?>(null);
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

    public virtual Task Activate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Deactivate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Enter(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Action(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Exit(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}

/// <remarks>
///     This stub provides common behavior for the properties of <see cref="IState{TState,TTransition}"/>. To verify
///     method behavior you should mock the methods as necessary.
/// </remarks>
internal class StubState<TState, TTransition, T> : IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubState(TState state)
    {
        StateValue = state;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters => [typeof(T)];

    public IStateMachine<TState, TTransition> StateMachine => null!;

    public IEnumerable<ITransition<TState, TTransition>> Transitions { get; } = [];

    public void AddTransition(ITransition<TState, TTransition> transition) { }

    public virtual Task<ITransition<TState, TTransition>> GetTransition(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        throw new NotImplementedException("Mock this method to verify call behavior");
    }

    public virtual Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        return Task.FromResult<ITransition<TState, TTransition>?>(null);
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

    public virtual Task Activate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Deactivate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Enter(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Action(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Exit(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}

/// <remarks>
///     This stub provides common behavior for the properties of <see cref="IState{TState,TTransition}"/>. To verify
///     method behavior you should mock the methods as necessary.
/// </remarks>
internal class StubState<TState, TTransition, T1, T2> : IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubState(TState state)
    {
        StateValue = state;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters => [typeof(T1), typeof(T2)];

    public IStateMachine<TState, TTransition> StateMachine => null!;

    public IEnumerable<ITransition<TState, TTransition>> Transitions { get; } = [];

    public void AddTransition(ITransition<TState, TTransition> transition) { }

    public virtual Task<ITransition<TState, TTransition>> GetTransition(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        throw new NotImplementedException("Mock this method to verify call behavior");
    }

    public virtual Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        return Task.FromResult<ITransition<TState, TTransition>?>(null);
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

    public virtual Task Activate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Deactivate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Enter(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Action(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Exit(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}

/// <remarks>
///     This stub provides common behavior for the properties of <see cref="IState{TState,TTransition}"/>. To verify
///     method behavior you should mock the methods as necessary.
/// </remarks>
internal class StubState<TState, TTransition, T1, T2, T3> : IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubState(TState state)
    {
        StateValue = state;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters => [typeof(T1), typeof(T2), typeof(T3)];

    public IStateMachine<TState, TTransition> StateMachine => null!;

    public IEnumerable<ITransition<TState, TTransition>> Transitions { get; } = [];

    public void AddTransition(ITransition<TState, TTransition> transition) { }

    public virtual Task<ITransition<TState, TTransition>> GetTransition(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        throw new NotImplementedException("Mock this method to verify call behavior");
    }

    public virtual Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        return Task.FromResult<ITransition<TState, TTransition>?>(null);
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

    public virtual Task Activate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Deactivate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Enter(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Action(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Exit(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}

/// <remarks>
///     This stub provides common behavior for the properties of <see cref="IState{TState,TTransition}"/>. To verify
///     method behavior you should mock the methods as necessary.
/// </remarks>
internal class StubState<TState, TTransition, T1, T2, T3, T4> : IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubState(TState state)
    {
        StateValue = state;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters => [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    public IStateMachine<TState, TTransition> StateMachine => null!;

    public IEnumerable<ITransition<TState, TTransition>> Transitions { get; } = [];

    public void AddTransition(ITransition<TState, TTransition> transition) { }

    public virtual Task<ITransition<TState, TTransition>> GetTransition(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        throw new NotImplementedException("Mock this method to verify call behavior");
    }

    public virtual Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        return Task.FromResult<ITransition<TState, TTransition>?>(null);
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

    public virtual Task Activate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Deactivate(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Enter(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Action(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public virtual Task Exit(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
