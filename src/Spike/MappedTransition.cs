using Spike.Contracts;

namespace Spike;

public class MappedTransition<TState, TTransition> : ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly ITransitionPreviousState<TState> previousState;
    private readonly ITransitionNextState<TState> nextState;
    private readonly IMappingFunction function;

    public MappedTransition(
        ITransitionPreviousState<TState> previousState,
        ITransitionNextState<TState> nextState,
        IMappingFunction function
    )
    {
        this.previousState = previousState;
        this.nextState = nextState;
        this.function = function;
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
        this.function.Map(parameters);
        var nextStateCondition = await this.nextState.EvaluateConditions(parameters);
        return nextStateCondition;
    }

    public async Task Transition(IStateMachineParameters parameters)
    {
        // TODO MWZ: how do we guarantee that the mapping already took place, i.e: the conditions already ran and mapped

        // Call state machine OnStateChange
        await this.nextState.ChangeState(parameters);
    }
}
