namespace ZCrew.StateCraft;

public interface IFromAllStatesTransitionConfiguration<TState, TTransition>
    : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except(TState state);
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious>(TState state);
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2>(TState state);
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2, TPrevious3>(TState state);
    IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2, TPrevious3, TPrevious4>(
        TState state
    );
}
