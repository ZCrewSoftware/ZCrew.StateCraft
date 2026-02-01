namespace Spike.Contracts;

public interface ITransitionPreviousState<TState>
    where TState : notnull
{
    TState StateValue { get; }
    IReadOnlyList<Type> TypeParameters { get; }
    Task<bool> EvaluateConditions(IStateMachineParameters parameters);
}
