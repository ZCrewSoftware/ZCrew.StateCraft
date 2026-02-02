namespace ZCrew.StateCraft.States.Configuration;

internal interface IPartialNextStateConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<T, bool> condition);
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}

internal interface IPartialNextStateConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<T1, T2, bool> condition);
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}

internal interface IPartialNextStateConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<T1, T2, T3, bool> condition);
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}

internal interface IPartialNextStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<T1, T2, T3, T4, bool> condition);
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}

internal interface IPartialNextStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    void Add(Func<bool> condition);
    INextStateConfiguration<TState, TTransition> WithState(TState stateValue);
}
