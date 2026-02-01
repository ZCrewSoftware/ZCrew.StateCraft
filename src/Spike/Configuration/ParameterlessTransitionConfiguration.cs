using Spike.Contracts.Configuration;

namespace Spike.Configuration;

public class ParameterlessTransitionConfiguration<TState, TTransition>
    : IParameterlessTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TransitionNextStateConfigurationBuilder<TState> nextStateBuilder = new();
    private readonly ITransitionPreviousStateConfiguration<TState> previousState;
    private readonly TTransition transitionValue;

    public ParameterlessTransitionConfiguration(
        ITransitionPreviousStateConfiguration<TState> previousState,
        TTransition transitionValue
    )
    {
        this.previousState = previousState;
        this.transitionValue = transitionValue;
    }

    public IParameterlessTransitionConfiguration<TState, TTransition> If(Func<bool> condition)
    {
        this.nextStateBuilder.Add(condition);
        return this;
    }

    public IFinalTransitionConfiguration<TState, TTransition> To(TState state)
    {
        var nextState = this.nextStateBuilder.WithState(state);
        return new FinalDirectTransitionConfiguration<TState, TTransition>(
            this.previousState,
            nextState,
            this.transitionValue
        );
    }

    public IFinalTransitionConfiguration<TState, TTransition> ToSameState()
    {
        return To(this.previousState.StateValue);
    }
}
