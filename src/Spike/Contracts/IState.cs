namespace Spike.Contracts;

public interface IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull { }

public interface IState<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull { }

public interface IState<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull { }

public interface IState<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull { }

public interface IState<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull { }
