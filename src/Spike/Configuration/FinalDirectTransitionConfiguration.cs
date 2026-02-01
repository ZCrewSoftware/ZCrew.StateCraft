using Spike.Contracts;
using Spike.Contracts.Configuration;

namespace Spike.Configuration;

public class FinalDirectTransitionConfiguration<TState, TTransition>
    : IFinalTransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly ITransitionPreviousStateConfiguration<TState> previousStateConfiguration;
    private readonly ITransitionNextStateConfiguration<TState> nextStateConfiguration;
    private readonly TTransition transitionValue;

    public FinalDirectTransitionConfiguration(
        ITransitionPreviousStateConfiguration<TState> previousStateConfiguration,
        ITransitionNextStateConfiguration<TState> nextStateConfiguration,
        TTransition transitionValue
    )
    {
        this.previousStateConfiguration = previousStateConfiguration;
        this.nextStateConfiguration = nextStateConfiguration;
        this.transitionValue = transitionValue;
    }

    public TState PreviousStateValue => this.previousStateConfiguration.StateValue;
    public TState NextStateValue => this.nextStateConfiguration.StateValue;
    public TTransition TransitionValue => this.transitionValue;
    public IReadOnlyList<Type> PreviousStateTypeParameters => this.previousStateConfiguration.TypeParameters;
    public IReadOnlyList<Type> NextStateTypeParameters => this.nextStateConfiguration.TypeParameters;

    public ITransition<TState, TTransition> Build()
    {
        var previousState = this.previousStateConfiguration.Build();
        var nextState = this.nextStateConfiguration.Build();
        return new DirectTransition<TState, TTransition>(previousState, nextState);
    }
}
