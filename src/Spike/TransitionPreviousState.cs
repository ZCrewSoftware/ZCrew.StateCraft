using Spike.Contracts;

namespace Spike;

public class TransitionPreviousState<TState> : ITransitionPreviousState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<bool>> conditions;

    public TransitionPreviousState(TState stateValue, IReadOnlyList<Func<bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [];

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        return Task.FromResult(this.conditions.All(condition => condition()));
    }
}

public class TransitionPreviousState<TState, T> : ITransitionPreviousState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T, bool>> conditions;

    public TransitionPreviousState(TState stateValue, IReadOnlyList<Func<T, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        var parameter = parameters.GetPreviousParameter<T>(0);
        return Task.FromResult(this.conditions.All(condition => condition(parameter)));
    }
}

public class TransitionPreviousState<TState, T1, T2> : ITransitionPreviousState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T1, T2, bool>> conditions;

    public TransitionPreviousState(TState stateValue, IReadOnlyList<Func<T1, T2, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2)];

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<T1>(0);
        var parameter2 = parameters.GetPreviousParameter<T2>(1);
        return Task.FromResult(this.conditions.All(condition => condition(parameter1, parameter2)));
    }
}

public class TransitionPreviousState<TState, T1, T2, T3> : ITransitionPreviousState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T1, T2, T3, bool>> conditions;

    public TransitionPreviousState(TState stateValue, IReadOnlyList<Func<T1, T2, T3, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3)];

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<T1>(0);
        var parameter2 = parameters.GetPreviousParameter<T2>(1);
        var parameter3 = parameters.GetPreviousParameter<T3>(2);
        return Task.FromResult(this.conditions.All(condition => condition(parameter1, parameter2, parameter3)));
    }
}

public class TransitionPreviousState<TState, T1, T2, T3, T4> : ITransitionPreviousState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T1, T2, T3, T4, bool>> conditions;

    public TransitionPreviousState(TState stateValue, IReadOnlyList<Func<T1, T2, T3, T4, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetPreviousParameter<T1>(0);
        var parameter2 = parameters.GetPreviousParameter<T2>(1);
        var parameter3 = parameters.GetPreviousParameter<T3>(2);
        var parameter4 = parameters.GetPreviousParameter<T4>(3);
        return Task.FromResult(
            this.conditions.All(condition => condition(parameter1, parameter2, parameter3, parameter4))
        );
    }
}
