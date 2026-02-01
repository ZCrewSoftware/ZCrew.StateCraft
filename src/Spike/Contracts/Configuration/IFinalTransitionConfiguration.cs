namespace Spike.Contracts.Configuration;

public interface IFinalTransitionConfiguration<TState, TTransition> : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    internal ITransition<TState, TTransition> Build();
}
