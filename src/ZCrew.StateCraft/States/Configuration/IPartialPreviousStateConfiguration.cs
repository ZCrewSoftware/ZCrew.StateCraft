using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.States.Configuration;

internal interface IPartialPreviousStateConfiguration<TState, TTransition>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    void Add(IAsyncFunc<bool> condition);
}

internal interface IPartialPreviousStateConfiguration<TState, TTransition, T>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    void Add(IAsyncFunc<T, bool> condition);
}

internal interface IPartialPreviousStateConfiguration<TState, TTransition, T1, T2>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    void Add(IAsyncFunc<T1, T2, bool> condition);
}

internal interface IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    void Add(IAsyncFunc<T1, T2, T3, bool> condition);
}

internal interface IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    : IPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    void Add(IAsyncFunc<T1, T2, T3, T4, bool> condition);
}
