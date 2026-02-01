namespace Spike.Contracts.Configuration;

public interface ITransitionPreviousStateConfiguration<TState>
    where TState : notnull
{
    internal TState StateValue { get; }

    internal IReadOnlyList<Type> TypeParameters { get; }

    ITransitionPreviousState<TState> Build();
}

public interface ITransitionPreviousStateConfiguration<TState, T> : ITransitionPreviousStateConfiguration<TState>
    where TState : notnull
{
    void Add(Func<T, bool> condition);
}

public interface ITransitionPreviousStateConfiguration<TState, T1, T2> : ITransitionPreviousStateConfiguration<TState>
    where TState : notnull
{
    void Add(Func<T1, T2, bool> condition);
}

public interface ITransitionPreviousStateConfiguration<TState, T1, T2, T3>
    : ITransitionPreviousStateConfiguration<TState>
    where TState : notnull
{
    void Add(Func<T1, T2, T3, bool> condition);
}

public interface ITransitionPreviousStateConfiguration<TState, T1, T2, T3, T4>
    : ITransitionPreviousStateConfiguration<TState>
    where TState : notnull
{
    void Add(Func<T1, T2, T3, T4, bool> condition);
}

public interface ITransitionParameterlessPreviousStateConfiguration<TState>
    : ITransitionPreviousStateConfiguration<TState>
    where TState : notnull
{
    void Add(Func<bool> condition);
}
