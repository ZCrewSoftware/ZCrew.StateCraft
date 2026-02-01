namespace Spike.Contracts.Configuration;

public interface ITransitionNextStateConfiguration<TState>
    where TState : notnull
{
    internal TState StateValue { get; }

    internal IReadOnlyList<Type> TypeParameters { get; }

    ITransitionNextState<TState> Build();
}

public interface ITransitionNextStateConfigurationBuilder<TState>
    where TState : notnull
{
    internal IReadOnlyList<Type> TypeParameters { get; }

    ITransitionNextStateConfiguration<TState> WithState(TState stateValue);
}

public interface ITransitionNextStateConfigurationBuilder<TState, T> : ITransitionNextStateConfigurationBuilder<TState>
    where TState : notnull
{
    void Add(Func<T, bool> condition);
}

public interface ITransitionNextStateConfigurationBuilder<TState, T1, T2>
    : ITransitionNextStateConfigurationBuilder<TState>
    where TState : notnull
{
    void Add(Func<T1, T2, bool> condition);
}

public interface ITransitionNextStateConfigurationBuilder<TState, T1, T2, T3>
    : ITransitionNextStateConfigurationBuilder<TState>
    where TState : notnull
{
    void Add(Func<T1, T2, T3, bool> condition);
}

public interface ITransitionNextStateConfigurationBuilder<TState, T1, T2, T3, T4>
    : ITransitionNextStateConfigurationBuilder<TState>
    where TState : notnull
{
    void Add(Func<T1, T2, T3, T4, bool> condition);
}

public interface ITransitionParameterlessNextStateConfigurationBuilder<TState>
    : ITransitionNextStateConfigurationBuilder<TState>
    where TState : notnull
{
    void Add(Func<bool> condition);
}
