using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States.Configuration;

internal interface INextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    TState StateValue { get; }

    IReadOnlyList<Type> TypeParameters { get; }

    INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable);
}

internal interface INextStateConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    TState StateValue { get; }

    IReadOnlyList<Type> TypeParameters { get; }

    INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable);
}

internal interface INextStateConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    TState StateValue { get; }

    IReadOnlyList<Type> TypeParameters { get; }

    INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable);
}

internal interface INextStateConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    TState StateValue { get; }

    IReadOnlyList<Type> TypeParameters { get; }

    INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable);
}

internal interface INextStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    TState StateValue { get; }

    IReadOnlyList<Type> TypeParameters { get; }

    INextState<TState, TTransition> Build(StateTable<TState, TTransition> stateTable);
}
