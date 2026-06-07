using System.Text;
using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc/>
internal class DirectTransitionConfiguration<TState, TTransition> : ITransitionConfiguration<TState, TTransition>
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
    public ITransitionInfo<TState, TTransition> GetInfo(IStateMachineInfo<TState, TTransition> stateMachine)
    {
        var previousStateInfo = this.previousStateConfiguration.GetInfo(stateMachine);
        var nextStateInfo = this.nextStateConfiguration.GetInfo(stateMachine);
        return new DirectTransitionInfo<TState, TTransition>(
            stateMachine,
            this.transitionValue,
            this.nextStateConfiguration.TypeParameters,
            previousStateInfo,
            nextStateInfo
        );
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
