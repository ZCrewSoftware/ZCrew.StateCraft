using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.Validation;
using ZCrew.StateCraft.Validation.Contracts;
using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc cref="ITransitionConfiguration{TState,TTransition}"/>
internal class DirectTransitionConfiguration<TState, TTransition>
    : ITransitionConfiguration<TState, TTransition>,
        IValidatable<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration;
    private readonly INextStateConfiguration<TState, TTransition> nextStateConfiguration;
    private readonly TTransition transitionValue;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="DirectTransitionConfiguration{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="previousStateConfiguration">The configuration for the previous state.</param>
    /// <param name="nextStateConfiguration">The configuration for the next state.</param>
    /// <param name="transitionValue">The transition value that triggers this transition.</param>
    public DirectTransitionConfiguration(
        IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration,
        INextStateConfiguration<TState, TTransition> nextStateConfiguration,
        TTransition transitionValue
    )
    {
        this.previousStateConfiguration = previousStateConfiguration;
        this.nextStateConfiguration = nextStateConfiguration;
        this.transitionValue = transitionValue;
    }

    /// <inheritdoc />
    public void Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var previousState = this.previousStateConfiguration.Build(stateMachine.StateTable);
        var nextState = this.nextStateConfiguration.Build(stateMachine.StateTable);
        var transition = new DirectTransition<TState, TTransition>(
            previousState,
            nextState,
            this.transitionValue,
            stateMachine
        );

        previousState.State.AddTransition(transition);
    }

    /// <inheritdoc />
    public void AddToValidationContext(StateMachineValidationContext<TState, TTransition> context)
    {
        var transition = new TransitionValidationModel<TState, TTransition>(
            this.previousStateConfiguration.StateValue,
            this.transitionValue,
            this.nextStateConfiguration.StateValue,
            this.previousStateConfiguration.TypeParameters,
            this.nextStateConfiguration.TypeParameters,
            this.nextStateConfiguration.TypeParameters,
            this.previousStateConfiguration.IsConditional || this.nextStateConfiguration.IsConditional
        );
        context.Transitions.Add(transition);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (
            this.previousStateConfiguration.StateValue.Equals(this.nextStateConfiguration.StateValue)
            && this.previousStateConfiguration.TypeParameters.SequenceEqual(this.nextStateConfiguration.TypeParameters)
        )
        {
            return $"{this.transitionValue}({this.previousStateConfiguration}) ↩";
        }

        return $"{this.transitionValue}({this.previousStateConfiguration}) → {this.nextStateConfiguration}";
    }
}
