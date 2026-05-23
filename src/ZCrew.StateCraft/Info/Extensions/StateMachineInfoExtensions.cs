namespace ZCrew.StateCraft.Info.Extensions;

public static class StateMachineInfoExtensions
{
    extension<TState, TTransition>(IStateMachineInfo<TState, TTransition> stateMachineInfo)
        where TState : notnull
        where TTransition : notnull { }
}
