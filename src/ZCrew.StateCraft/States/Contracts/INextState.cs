using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.States.Contracts;

internal interface INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    TState StateValue { get; }
    IReadOnlyList<Type> TypeParameters { get; }
    Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token);
    Task ChangeState(
        TState previousState,
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    );
}
