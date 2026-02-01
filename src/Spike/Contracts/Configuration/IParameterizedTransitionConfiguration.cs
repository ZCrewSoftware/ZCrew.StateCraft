namespace Spike.Contracts.Configuration;

public interface IParameterlessTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    IParameterlessTransitionConfiguration<TState, TTransition> If(Func<bool> condition);

    IFinalTransitionConfiguration<TState, TTransition> To(TState state);

    IFinalTransitionConfiguration<TState, TTransition> ToSameState();
}

public interface IParameterizedTransitionConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    IParameterizedTransitionConfiguration<TState, TTransition, T> If(Func<T, bool> condition);

    IFinalTransitionConfiguration<TState, TTransition> To(TState state);

    IFinalTransitionConfiguration<TState, TTransition> ToSameState();
}

public interface IParameterizedTransitionConfiguration<TState, TTransition, T1, T2>
    where TState : notnull
    where TTransition : notnull
{
    IParameterizedTransitionConfiguration<TState, TTransition, T1, T2> If(Func<T1, T2, bool> condition);

    IFinalTransitionConfiguration<TState, TTransition> To(TState state);

    IFinalTransitionConfiguration<TState, TTransition> ToSameState();
}

public interface IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3>
    where TState : notnull
    where TTransition : notnull
{
    IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3> If(Func<T1, T2, T3, bool> condition);

    IFinalTransitionConfiguration<TState, TTransition> To(TState state);

    IFinalTransitionConfiguration<TState, TTransition> ToSameState();
}

public interface IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    IParameterizedTransitionConfiguration<TState, TTransition, T1, T2, T3, T4> If(Func<T1, T2, T3, T4, bool> condition);

    IFinalTransitionConfiguration<TState, TTransition> To(TState state);

    IFinalTransitionConfiguration<TState, TTransition> ToSameState();
}
