using ZCrew.StateCraft.Parameters;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Tracking.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.UnitTests.Stubs;

internal class StubStateMachine<TState, TTransition> : IStateMachine<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubStateMachine()
    {
        StateTable = new StateTable<TState, TTransition>();
    }

    public StubStateMachine(IState<TState, TTransition> state)
    {
        StateTable = new StateTable<TState, TTransition>([state]);
    }

    public StubStateMachine(IEnumerable<IState<TState, TTransition>> states)
    {
        StateTable = new StateTable<TState, TTransition>(states);
    }

    public IState<TState, TTransition>? CurrentState { get; set; }

    public IState<TState, TTransition>? PreviousState { get; set; }

    public IState<TState, TTransition>? NextState { get; set; }

    public IStateMachineParameters Parameters { get; } = new StateMachineParameters();

    public ITransition<TState, TTransition>? CurrentTransition { get; set; }

    public StateTable<TState, TTransition> StateTable { get; set; }

    public ITracker<TState, TTransition>? Tracker { get; set; }

    public void AddState(IState<TState, TTransition> state) { }

    public virtual Task Activate(CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task Deactivate(CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task Transition(TTransition transition, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task Transition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task Transition<T1, T2>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        CancellationToken token = default
    )
    {
        return Task.CompletedTask;
    }

    public virtual Task Transition<T1, T2, T3>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        CancellationToken token = default
    )
    {
        return Task.CompletedTask;
    }

    public virtual Task Transition<T1, T2, T3, T4>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        CancellationToken token = default
    )
    {
        return Task.CompletedTask;
    }

    public virtual Task<bool> CanTransition(TTransition transition, CancellationToken token = default)
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> CanTransition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> TryTransition(TTransition transition, CancellationToken token = default)
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> TryTransition<T>(TTransition transition, T parameter, CancellationToken token = default)
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> TryTransition<T1, T2>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        CancellationToken token = default
    )
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> TryTransition<T1, T2, T3>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        CancellationToken token = default
    )
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> TryTransition<T1, T2, T3, T4>(
        TTransition transition,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        CancellationToken token = default
    )
    {
        return Task.FromResult(false);
    }

    public virtual Task StateChange(
        TState previousState,
        TTransition transition,
        TState nextState,
        CancellationToken token
    )
    {
        return Task.CompletedTask;
    }

    public virtual async Task RunWithExceptionHandling(Func<Task> action, CancellationToken token)
    {
        await action();
    }

    public virtual async Task RunWithExceptionHandling(
        Func<Task> action,
        bool throwOnCancellation,
        CancellationToken token
    )
    {
        await action();
    }

    public virtual async Task<T> RunWithExceptionHandling<T>(Func<Task<T>> action, CancellationToken token)
    {
        return await action();
    }

    public void Dispose() { }
}
