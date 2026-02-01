using Spike.Contracts;
using Spike.Contracts.Configuration;

namespace Spike.Configuration;

public class TransitionNextStateConfigurationBuilder<TState>
    : ITransitionParameterlessNextStateConfigurationBuilder<TState>
    where TState : notnull
{
    private readonly List<Func<bool>> conditions = [];

    public IReadOnlyList<Type> TypeParameters { get; } = [];

    public ITransitionNextStateConfiguration<TState> WithState(TState stateValue)
    {
        return new TransitionNextStateConfiguration<TState>(stateValue, this.conditions);
    }

    public void Add(Func<bool> condition)
    {
        this.conditions.Add(condition);
    }
}

public class TransitionNextStateConfiguration<TState> : ITransitionNextStateConfiguration<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<bool>> conditions;

    public TransitionNextStateConfiguration(TState stateValue, IReadOnlyList<Func<bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [];

    public ITransitionNextState<TState> Build()
    {
        return new TransitionNextState<TState>(StateValue, this.conditions);
    }
}

public class TransitionNextStateConfigurationBuilder<TState, T> : ITransitionNextStateConfigurationBuilder<TState, T>
    where TState : notnull
{
    private readonly List<Func<T, bool>> conditions = [];

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    public ITransitionNextStateConfiguration<TState> WithState(TState stateValue)
    {
        return new TransitionNextStateConfiguration<TState, T>(stateValue, this.conditions);
    }

    public void Add(Func<T, bool> condition)
    {
        this.conditions.Add(condition);
    }
}

public class TransitionNextStateConfiguration<TState, T> : ITransitionNextStateConfiguration<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T, bool>> conditions;

    public TransitionNextStateConfiguration(TState stateValue, IReadOnlyList<Func<T, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    public ITransitionNextState<TState> Build()
    {
        return new TransitionNextState<TState, T>(StateValue, this.conditions);
    }
}

public class TransitionNextStateConfigurationBuilder<TState, T1, T2>
    : ITransitionNextStateConfigurationBuilder<TState, T1, T2>
    where TState : notnull
{
    private readonly List<Func<T1, T2, bool>> conditions = [];

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2)];

    public ITransitionNextStateConfiguration<TState> WithState(TState stateValue)
    {
        return new TransitionNextStateConfiguration<TState, T1, T2>(stateValue, this.conditions);
    }

    public void Add(Func<T1, T2, bool> condition)
    {
        this.conditions.Add(condition);
    }
}

public class TransitionNextStateConfiguration<TState, T1, T2> : ITransitionNextStateConfiguration<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T1, T2, bool>> conditions;

    public TransitionNextStateConfiguration(TState stateValue, IReadOnlyList<Func<T1, T2, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2)];

    public ITransitionNextState<TState> Build()
    {
        return new TransitionNextState<TState, T1, T2>(StateValue, this.conditions);
    }
}

public class TransitionNextStateConfigurationBuilder<TState, T1, T2, T3>
    : ITransitionNextStateConfigurationBuilder<TState, T1, T2, T3>
    where TState : notnull
{
    private readonly List<Func<T1, T2, T3, bool>> conditions = [];

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3)];

    public ITransitionNextStateConfiguration<TState> WithState(TState stateValue)
    {
        return new TransitionNextStateConfiguration<TState, T1, T2, T3>(stateValue, this.conditions);
    }

    public void Add(Func<T1, T2, T3, bool> condition)
    {
        this.conditions.Add(condition);
    }
}

public class TransitionNextStateConfiguration<TState, T1, T2, T3> : ITransitionNextStateConfiguration<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T1, T2, T3, bool>> conditions;

    public TransitionNextStateConfiguration(TState stateValue, IReadOnlyList<Func<T1, T2, T3, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3)];

    public ITransitionNextState<TState> Build()
    {
        return new TransitionNextState<TState, T1, T2, T3>(StateValue, this.conditions);
    }
}

public class TransitionNextStateConfigurationBuilder<TState, T1, T2, T3, T4>
    : ITransitionNextStateConfigurationBuilder<TState, T1, T2, T3, T4>
    where TState : notnull
{
    private readonly List<Func<T1, T2, T3, T4, bool>> conditions = [];

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    public ITransitionNextStateConfiguration<TState> WithState(TState stateValue)
    {
        return new TransitionNextStateConfiguration<TState, T1, T2, T3, T4>(stateValue, this.conditions);
    }

    public void Add(Func<T1, T2, T3, T4, bool> condition)
    {
        this.conditions.Add(condition);
    }
}

public class TransitionNextStateConfiguration<TState, T1, T2, T3, T4> : ITransitionNextStateConfiguration<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T1, T2, T3, T4, bool>> conditions;

    public TransitionNextStateConfiguration(TState stateValue, IReadOnlyList<Func<T1, T2, T3, T4, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    public ITransitionNextState<TState> Build()
    {
        return new TransitionNextState<TState, T1, T2, T3, T4>(StateValue, this.conditions);
    }
}
