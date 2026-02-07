using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

internal class PreviousState<TState, TTransition> : IPreviousState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<bool>> conditions;

    public PreviousState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<bool>> conditions)
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

internal class PreviousState<TState, TTransition, T> : IPreviousState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncFunc<T, bool>> conditions;

    public PreviousState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<T, bool>> conditions)
    {
        State = state;
        this.conditions = conditions;
    }

    public IState<TState, TTransition> State { get; }

    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters, CancellationToken token)
    {
        var parameter = parameters.GetPreviousParameter<T>(0);
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
