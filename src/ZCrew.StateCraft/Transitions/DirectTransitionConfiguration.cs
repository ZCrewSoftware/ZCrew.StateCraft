using System.Diagnostics;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc />
[DebuggerDisplay("{ToDisplayString()}")]
internal class DirectTransitionConfiguration<TState, TTransition> : ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration;
    private readonly INextStateConfiguration<TState, TTransition> nextStateConfiguration;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="DirectTransitionConfiguration{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="previousStateConfiguration">The configuration for the previous state.</param>
    /// <param name="nextStateConfiguration">The configuration for the next state.</param>
    /// <param name="transition">The transition value that triggers this transition.</param>
    public DirectTransitionConfiguration(
        IPreviousStateConfiguration<TState, TTransition> previousStateConfiguration,
        INextStateConfiguration<TState, TTransition> nextStateConfiguration,
        TTransition transition
    )
    {
        this.previousStateConfiguration = previousStateConfiguration;
        this.nextStateConfiguration = nextStateConfiguration;
        TransitionValue = transition;
    }

    /// <inheritdoc />
    public TState PreviousStateValue => this.previousStateConfiguration.StateValue;

    /// <inheritdoc />
    public TTransition TransitionValue { get; }

    /// <inheritdoc />
    public TState NextStateValue => this.nextStateConfiguration.StateValue;

    /// <inheritdoc />
    public IReadOnlyList<Type> PreviousStateTypeParameters => this.previousStateConfiguration.TypeParameters;

    /// <inheritdoc/>
    public IReadOnlyList<Type> TransitionTypeParameters => this.nextStateConfiguration.TypeParameters;

    /// <inheritdoc />
    public IReadOnlyList<Type> NextStateTypeParameters => this.nextStateConfiguration.TypeParameters;

    /// <inheritdoc />
    public bool IsConditional =>
        this.previousStateConfiguration.IsConditional || this.nextStateConfiguration.IsConditional;

    /// <inheritdoc />
    public ITransition<TState, TTransition> Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var previousState = this.previousStateConfiguration.Build(stateMachine.StateTable);
        var nextState = this.nextStateConfiguration.Build(stateMachine.StateTable);
        var transition = new DirectTransition<TState, TTransition>(
            previousState,
            nextState,
            TransitionValue,
            stateMachine
        );

        previousState.State.AddTransition(transition);

        return transition;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Transition: {ToDisplayString()}";
    }

    private string ToDisplayString()
    {
        // TODO MWZ: this needs refactoring since #31 really botched it
        var previousStateParameters =
            PreviousStateTypeParameters.Count == 0
                ? string.Empty
                : $"<{string.Join(", ", PreviousStateTypeParameters.Select(type => type.FriendlyName))}>";
        if (
            PreviousStateValue.Equals(NextStateValue)
            && PreviousStateTypeParameters.SequenceEqual(NextStateTypeParameters)
        )
        {
            return $"{TransitionValue}({PreviousStateValue}{previousStateParameters}) ↩";
        }
        var nextStateParameters =
            NextStateTypeParameters.Count == 0
                ? string.Empty
                : $"<{string.Join(", ", NextStateTypeParameters.Select(type => type.FriendlyName))}>";
        return $"{TransitionValue}({PreviousStateValue}{previousStateParameters}) → {NextStateValue}{nextStateParameters}";
    }
}
