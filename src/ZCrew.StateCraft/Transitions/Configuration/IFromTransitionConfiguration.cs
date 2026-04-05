namespace ZCrew.StateCraft;

public interface IFromTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IFromAllStatesTransitionConfiguration<TState, TTransition> AllStates();
    IFromAllStatesTransitionConfiguration<TState, TTransition> AllOtherStates();
}
