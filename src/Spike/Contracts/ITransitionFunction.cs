namespace Spike.Contracts;

public interface ITransitionFunction<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    Task Apply();
}
