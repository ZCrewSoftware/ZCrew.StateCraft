using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

internal class NextState<TState, TTransition> : INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<bool>> conditions;

    public NextState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<bool>> conditions)
    {
        State = state;
        this.conditions = conditions;
    }

    public IState<TState, TTransition> State { get; }

    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        foreach (var condition in this.conditions)
        {
            var result = await condition.InvokeAsync(token);
            if (!result)
            {
                return false;
            }
        }

        return true;
    }
}

internal class NextState<TState, TTransition, T> : INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T, bool>> conditions;

    public NextState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<T, bool>> conditions)
    {
        State = state;
        this.conditions = conditions;
    }

    public IState<TState, TTransition> State { get; }

    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter = parameters.GetNextParameter<T>(0);
        foreach (var condition in this.conditions)
        {
            var result = await condition.InvokeAsync(parameter, token);
            if (!result)
            {
                return false;
            }
        }

        return true;
    }
}
