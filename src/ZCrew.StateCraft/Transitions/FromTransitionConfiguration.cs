using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States;
using ZCrew.StateCraft.States.Configuration;
using ZCrew.StateCraft.Validation;
using ZCrew.StateCraft.Validation.Contracts;
using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc cref="ITransitionConfiguration{TState,TTransition}"/>
internal class FromTransitionConfiguration<TState, TTransition>
    : IFromTransitionConfiguration<TState, TTransition>,
        IFromAllStatesTransitionConfiguration<TState, TTransition>,
        IValidatable<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TTransition transitionValue;
    private readonly INextStateConfiguration<TState, TTransition> nextStateConfiguration;
    private readonly List<ExcludedState> excludedStates = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="FromTransitionConfiguration{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="transitionValue">The transition value that triggers this transition.</param>
    /// <param name="nextStateConfiguration">The next state configuration for this transition.</param>
    public FromTransitionConfiguration(
        TTransition transitionValue,
        INextStateConfiguration<TState, TTransition> nextStateConfiguration
    )
    {
        this.transitionValue = transitionValue;
        this.nextStateConfiguration = nextStateConfiguration;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> AllStates()
    {
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> AllOtherStates()
    {
        return Exclude(this.nextStateConfiguration.StateValue, this.nextStateConfiguration.TypeParameters.ToArray());
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> Except(TState state)
    {
        return Exclude(this.nextStateConfiguration.StateValue, []);
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious>(TState state)
    {
        return Exclude(this.nextStateConfiguration.StateValue, [typeof(TPrevious)]);
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2>(TState state)
    {
        return Exclude(this.nextStateConfiguration.StateValue, [typeof(TPrevious1), typeof(TPrevious2)]);
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2, TPrevious3>(
        TState state
    )
    {
        return Exclude(
            this.nextStateConfiguration.StateValue,
            [typeof(TPrevious1), typeof(TPrevious2), typeof(TPrevious3)]
        );
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> Except<
        TPrevious1,
        TPrevious2,
        TPrevious3,
        TPrevious4
    >(TState state)
    {
        return Exclude(
            this.nextStateConfiguration.StateValue,
            [typeof(TPrevious1), typeof(TPrevious2), typeof(TPrevious3), typeof(TPrevious4)]
        );
    }

    /// <inheritdoc />
    public void Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var nextState = this.nextStateConfiguration.Build(stateMachine.StateTable);
        foreach (var state in stateMachine.StateTable)
        {
            var excluded = this.excludedStates.Any(excludedState =>
                excludedState.Matches(state.StateValue, state.TypeParameters)
            );
            if (excluded)
            {
                continue;
            }

            // Use a dynamic previous state. There are no conditions and so we don't care about the type parameters
            var previousState = new DynamicPreviousState<TState, TTransition>(state);
            var transition = new DirectTransition<TState, TTransition>(
                previousState,
                nextState,
                this.transitionValue,
                stateMachine
            );

            state.AddTransition(transition);
        }
    }

    /// <inheritdoc />
    public void AddToValidationContext(StateMachineValidationContext<TState, TTransition> context)
    {
        // This requires all states to be loaded ahead of time. We can just add all states and then all transitions
        foreach (var state in context.States)
        {
            var excluded = this.excludedStates.Any(excludedState =>
                excludedState.Matches(state.State, state.TypeParameters)
            );
            if (excluded)
            {
                continue;
            }

            var transition = new TransitionValidationModel<TState, TTransition>(
                state.State,
                this.transitionValue,
                this.nextStateConfiguration.StateValue,
                state.TypeParameters,
                this.nextStateConfiguration.TypeParameters,
                this.nextStateConfiguration.TypeParameters,
                this.nextStateConfiguration.IsConditional
            );
            context.Transitions.Add(transition);
        }
    }

    private IFromAllStatesTransitionConfiguration<TState, TTransition> Exclude(TState state, Type[] typeParameters)
    {
        this.excludedStates.Add(new ExcludedState(state, typeParameters));
        return this;
    }

    private readonly record struct ExcludedState(TState State, Type[] TypeParameters)
    {
        public bool Matches(TState state, IReadOnlyList<Type> typeParameters)
        {
            if (!EqualityComparer<TState>.Default.Equals(state, State))
            {
                return false;
            }

            return typeParameters.SequenceEqual(TypeParameters);
        }
    }
}
