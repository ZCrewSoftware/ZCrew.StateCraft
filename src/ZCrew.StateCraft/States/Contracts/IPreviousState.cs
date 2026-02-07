using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.States.Contracts;

internal interface IPreviousState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IState<TState, TTransition> State { get; }
    Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token);
}
