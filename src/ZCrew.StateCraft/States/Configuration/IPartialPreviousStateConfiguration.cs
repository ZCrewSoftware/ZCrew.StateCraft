namespace ZCrew.StateCraft.States.Configuration;

internal interface IPartialPreviousStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<bool> condition);
    void Add(Func<CancellationToken, Task<bool>> condition);
    void Add(Func<CancellationToken, ValueTask<bool>> condition);
}

internal interface IPartialPreviousStateConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<T, bool> condition);
}

internal interface IPartialPreviousStateConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<T1, T2, bool> condition);
}

internal interface IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<T1, T2, T3, bool> condition);
}

internal interface IPartialPreviousStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<T1, T2, T3, T4, bool> condition);
}
