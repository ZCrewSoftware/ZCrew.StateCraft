using Spike.Contracts;

namespace Spike;

public class DirectTransition<TState, TTransition> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly ITransitionPreviousState<TState> previousState;
    private readonly ITransitionNextState<TState> nextState;

    public DirectTransition(ITransitionPreviousState<TState> previousState, ITransitionNextState<TState> nextState)
    {
        this.previousState = previousState;
        this.nextState = nextState;
    }

    public IReadOnlyList<Type> PreviousStateTypeParameters => this.previousState.TypeParameters;

    public IReadOnlyList<Type> TransitionTypeParameters => this.nextState.TypeParameters;

    public IReadOnlyList<Type> NextStateTypeParameters => this.nextState.TypeParameters;

    public async Task<bool> EvaluateConditions(IStateMachineParameters parameters)
    {
        var previousStateCondition = await this.previousState.EvaluateConditions(parameters);
        if (!previousStateCondition)
        {
            return false;
        }
        var nextStateCondition = await this.nextState.EvaluateConditions(parameters);
        return nextStateCondition;
    }

    public async Task Transition(IStateMachineParameters parameters)
    {
        // Call state machine OnStateChange
        await this.nextState.ChangeState(parameters);
    }
}
