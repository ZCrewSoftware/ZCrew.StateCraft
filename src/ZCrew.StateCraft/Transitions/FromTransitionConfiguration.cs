using System.Text;
using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc cref="ITransitionConfiguration{TState,TTransition}"/>
internal class FromTransitionConfiguration<TState, TTransition>
    : IFromTransitionConfiguration<TState, TTransition>,
        IFromAllStatesTransitionConfiguration<TState, TTransition>
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
        return Exclude(state, []);
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious>(TState state)
    {
        return Exclude(state, [typeof(TPrevious)]);
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2>(TState state)
    {
        return Exclude(state, [typeof(TPrevious1), typeof(TPrevious2)]);
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> Except<TPrevious1, TPrevious2, TPrevious3>(
        TState state
    )
    {
        return Exclude(state, [typeof(TPrevious1), typeof(TPrevious2), typeof(TPrevious3)]);
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition> Except<
        TPrevious1,
        TPrevious2,
        TPrevious3,
        TPrevious4
    >(TState state)
    {
        return Exclude(state, [typeof(TPrevious1), typeof(TPrevious2), typeof(TPrevious3), typeof(TPrevious4)]);
    }

    /// <inheritdoc />
    public ITransitionInfo<TState, TTransition> GetInfo(IStateMachineInfo<TState, TTransition> stateMachine)
    {
        var nextStateInfo = this.nextStateConfiguration.GetInfo(stateMachine);
        var excludedStateInfo = this
            .excludedStates.Select(excludedState => excludedState.GetInfo(stateMachine))
            .ToArray();
        return new FromTransitionInfo<TState, TTransition>(
            stateMachine,
            this.transitionValue,
            this.nextStateConfiguration.TypeParameters,
            nextStateInfo,
            excludedStateInfo
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

            // Use a dynamic previous state. There are no conditions, so we don't care about the type parameters
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

        public IStateInfo<TState, TTransition> GetInfo(IStateMachineInfo<TState, TTransition> stateMachine)
        {
            return new StateInfo<TState, TTransition>(stateMachine, State, TypeParameters);
        }
    }
}
