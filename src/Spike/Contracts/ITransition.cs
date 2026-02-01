namespace Spike.Contracts;

public interface ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IReadOnlyList<Type> PreviousStateTypeParameters { get; }
    IReadOnlyList<Type> TransitionTypeParameters { get; }
    IReadOnlyList<Type> NextStateTypeParameters { get; }
    Task<bool> EvaluateConditions(IStateMachineParameters parameters);
    Task Transition(IStateMachineParameters parameters);
}
