using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.UnitTests.Stubs;

/// <remarks>
///     Simple wrapper that adapts an <see cref="IState{TState,TTransition}"/> into
///     <see cref="IPreviousState{TState,TTransition}"/> or <see cref="INextState{TState,TTransition}"/>
///     for use in transition stubs. Conditions always evaluate to <see langword="true"/>.
/// </remarks>
internal class StubStateRef<TState, TTransition> : IPreviousState<TState, TTransition>, INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    public StubStateRef(IState<TState, TTransition> state)
    {
        State = state;
    }

    public IState<TState, TTransition> State { get; }

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        return Task.FromResult(true);
    }
}
