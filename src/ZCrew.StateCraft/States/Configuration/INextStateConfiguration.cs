using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States.Configuration;

internal interface INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    bool IsConditional { get; }

    TState StateValue { get; }

    IReadOnlyList<Type> TypeParameters { get; }

    INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable);
}
