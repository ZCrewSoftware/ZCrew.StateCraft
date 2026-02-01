using Spike.Contracts;

namespace Spike;

public class TransitionNextState<TState> : ITransitionNextState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<bool>> conditions;

    public TransitionNextState(TState stateValue, IReadOnlyList<Func<bool>> conditions)
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

    public Task ChangeState(IStateMachineParameters parameters)
    {
        return Task.CompletedTask;
    }
}

public class TransitionNextState<TState, T> : ITransitionNextState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T, bool>> conditions;

    public TransitionNextState(TState stateValue, IReadOnlyList<Func<T, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        var parameter = parameters.GetNextParameter<T>(0);
        return Task.FromResult(this.conditions.All(condition => condition(parameter)));
    }

    public Task ChangeState(IStateMachineParameters parameters)
    {
        return Task.CompletedTask;
    }
}

public class TransitionNextState<TState, T1, T2> : ITransitionNextState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T1, T2, bool>> conditions;

    public TransitionNextState(TState stateValue, IReadOnlyList<Func<T1, T2, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2)];

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetNextParameter<T1>(0);
        var parameter2 = parameters.GetNextParameter<T2>(1);
        return Task.FromResult(this.conditions.All(condition => condition(parameter1, parameter2)));
    }

    public Task ChangeState(IStateMachineParameters parameters)
    {
        return Task.CompletedTask;
    }
}

public class TransitionNextState<TState, T1, T2, T3> : ITransitionNextState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T1, T2, T3, bool>> conditions;

    public TransitionNextState(TState stateValue, IReadOnlyList<Func<T1, T2, T3, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3)];

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetNextParameter<T1>(0);
        var parameter2 = parameters.GetNextParameter<T2>(1);
        var parameter3 = parameters.GetNextParameter<T3>(2);
        return Task.FromResult(this.conditions.All(condition => condition(parameter1, parameter2, parameter3)));
    }

    public Task ChangeState(IStateMachineParameters parameters)
    {
        return Task.CompletedTask;
    }
}

public class TransitionNextState<TState, T1, T2, T3, T4> : ITransitionNextState<TState>
    where TState : notnull
{
    private readonly IReadOnlyList<Func<T1, T2, T3, T4, bool>> conditions;

    public TransitionNextState(TState stateValue, IReadOnlyList<Func<T1, T2, T3, T4, bool>> conditions)
    {
        StateValue = stateValue;
        this.conditions = conditions;
    }

    public TState StateValue { get; }

    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    public Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        var parameter1 = parameters.GetNextParameter<T1>(0);
        var parameter2 = parameters.GetNextParameter<T2>(1);
        var parameter3 = parameters.GetNextParameter<T3>(2);
        var parameter4 = parameters.GetNextParameter<T4>(3);
        return Task.FromResult(
            this.conditions.All(condition => condition(parameter1, parameter2, parameter3, parameter4))
        );
    }

    public Task ChangeState(IStateMachineParameters parameters)
    {
        return Task.CompletedTask;
    }
}
