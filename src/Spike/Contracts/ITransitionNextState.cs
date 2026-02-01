namespace Spike.Contracts;

public interface ITransitionNextState<TState>
    where TState : notnull
{
    TState StateValue { get; }
    IReadOnlyList<Type> TypeParameters { get; }
    Task<bool> EvaluateConditions(IStateMachineParameters parameters);
    Task ChangeState(IStateMachineParameters parameters);
}
