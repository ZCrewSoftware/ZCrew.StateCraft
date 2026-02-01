using Spike.Contracts;
using Spike.Contracts.Configuration;

namespace Spike.Configuration;

public class TransitionPreviousStateConfiguration<TState> : ITransitionParameterlessPreviousStateConfiguration<TState>
    where TState : notnull
{
    private readonly List<Func<bool>> conditions;

    public TransitionPreviousStateConfiguration(TState stateValue)
        : this(stateValue, []) { }

    public TransitionPreviousStateConfiguration(TState stateValue, List<Func<bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [];

    public ITransitionPreviousState<TState> Build()
    {
        return new TransitionPreviousState<TState>(StateValue, this.conditions);
    }

    public void Add(Func<bool> condition)
    {
        this.conditions.Add(condition);
    }
}

public class TransitionPreviousStateConfiguration<TState, T> : ITransitionPreviousStateConfiguration<TState, T>
    where TState : notnull
{
    private readonly List<Func<T, bool>> conditions;

    public TransitionPreviousStateConfiguration(TState stateValue)
        : this(stateValue, []) { }

    public TransitionPreviousStateConfiguration(TState stateValue, List<Func<T, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    public ITransitionPreviousState<TState> Build()
    {
        return new TransitionPreviousState<TState, T>(StateValue, this.conditions);
    }

    public void Add(Func<T, bool> condition)
    {
        this.conditions.Add(condition);
    }
}

public class TransitionPreviousStateConfiguration<TState, T1, T2>
    : ITransitionPreviousStateConfiguration<TState, T1, T2>
    where TState : notnull
{
    private readonly List<Func<T1, T2, bool>> conditions;

    public TransitionPreviousStateConfiguration(TState stateValue)
        : this(stateValue, []) { }

    public TransitionPreviousStateConfiguration(TState stateValue, List<Func<T1, T2, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2)];

    public ITransitionPreviousState<TState> Build()
    {
        return new TransitionPreviousState<TState, T1, T2>(StateValue, this.conditions);
    }

    public void Add(Func<T1, T2, bool> condition)
    {
        this.conditions.Add(condition);
    }
}

public class TransitionPreviousStateConfiguration<TState, T1, T2, T3>
    : ITransitionPreviousStateConfiguration<TState, T1, T2, T3>
    where TState : notnull
{
    private readonly List<Func<T1, T2, T3, bool>> conditions;

    public TransitionPreviousStateConfiguration(TState stateValue)
        : this(stateValue, []) { }

    public TransitionPreviousStateConfiguration(TState stateValue, List<Func<T1, T2, T3, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3)];

    public ITransitionPreviousState<TState> Build()
    {
        return new TransitionPreviousState<TState, T1, T2, T3>(StateValue, this.conditions);
    }

    public void Add(Func<T1, T2, T3, bool> condition)
    {
        this.conditions.Add(condition);
    }
}

public class TransitionPreviousStateConfiguration<TState, T1, T2, T3, T4>
    : ITransitionPreviousStateConfiguration<TState, T1, T2, T3, T4>
    where TState : notnull
{
    private readonly List<Func<T1, T2, T3, T4, bool>> conditions;

    public TransitionPreviousStateConfiguration(TState stateValue)
        : this(stateValue, []) { }

    public TransitionPreviousStateConfiguration(TState stateValue, List<Func<T1, T2, T3, T4, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    public ITransitionPreviousState<TState> Build()
    {
        return new TransitionPreviousState<TState, T1, T2, T3, T4>(StateValue, this.conditions);
    }

    public void Add(Func<T1, T2, T3, T4, bool> condition)
    {
        this.conditions.Add(condition);
    }
}
