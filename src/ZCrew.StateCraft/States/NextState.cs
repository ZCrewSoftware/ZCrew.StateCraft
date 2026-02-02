using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft.States;

internal class NextState<TState, TTransition> : INextState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IState<TState, TTransition> state;
    private readonly IReadOnlyList<IAsyncFunc<bool>> conditions;

    public NextState(IState<TState, TTransition> state, IReadOnlyList<IAsyncFunc<bool>> conditions)
    {
        this.state = state;
        this.conditions = conditions;
    }

    public TState StateValue => this.state.StateValue;

    public IReadOnlyList<Type> TypeParameters { get; } = [];

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

    public Task ChangeState(
        TState previousState,
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        return this.state.StateChange(previousState, transition, parameters, token);
    }
}
