using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.Mapping.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc cref="ITransitionConfiguration{TState,TTransition}"/>
internal class MappedTransitionConfiguration<TState, TTransition> : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration;
    private readonly INextStateConfiguration<TState, TTransition> nextStateConfiguration;
    private readonly TTransition transitionValue;
    private readonly IMappingFunction mappingFunction;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="MappedTransitionConfiguration{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="previousStateConfiguration">The configuration for the previous state.</param>
    /// <param name="nextStateConfiguration">The configuration for the next state.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    /// <param name="mappingFunction">The mapping function that transforms the previous parameter.</param>
    public MappedTransitionConfiguration(
        IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration,
        INextStateConfiguration<TState, TTransition> nextStateConfiguration,
        TTransition transition,
        IMappingFunction mappingFunction
    )
    {
        this.previousStateConfiguration = previousStateConfiguration;
        this.nextStateConfiguration = nextStateConfiguration;
        this.transitionValue = transition;
        this.mappingFunction = mappingFunction;
    }

    /// <inheritdoc />
    public ITransitionInfo<TState, TTransition> GetInfo(IStateMachineInfo<TState, TTransition> stateMachine)
    {
        var previousStateInfo = this.previousStateConfiguration.GetInfo(stateMachine);
        var nextStateInfo = this.nextStateConfiguration.GetInfo(stateMachine);
        var mappingFunctionInfo = this.mappingFunction.GetInfo();
        return new MappedTransitionInfo<TState, TTransition>(
            stateMachine,
            this.transitionValue,
            previousStateInfo,
            nextStateInfo,
            mappingFunctionInfo
        );
    }

    /// <inheritdoc />
    public void Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var previousState = this.previousStateConfiguration.Build(stateMachine.StateTable);
        var nextState = this.nextStateConfiguration.Build(stateMachine.StateTable);
        var transition = new MappedTransition<TState, TTransition>(
            previousState,
            nextState,
            this.transitionValue,
            this.mappingFunction,
            stateMachine
        );

        previousState.State.AddTransition(transition);
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
