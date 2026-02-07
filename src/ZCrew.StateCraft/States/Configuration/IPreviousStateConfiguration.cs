using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States.Configuration;

internal interface IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    bool IsConditional { get; }

    TState StateValue { get; }

    IReadOnlyList<Type> TypeParameters { get; }

    IPreviousState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable);
}
