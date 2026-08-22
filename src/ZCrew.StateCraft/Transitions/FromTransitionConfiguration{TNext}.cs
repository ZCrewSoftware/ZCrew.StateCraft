using System.Runtime.CompilerServices;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Async.Contracts;
using ZCrew.StateCraft.Extensions;
using ZCrew.StateCraft.Identities.Extensions;
using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States;
using ZCrew.StateCraft.States.Configuration;

namespace ZCrew.StateCraft.Transitions;

/// <inheritdoc cref="IFromTransitionConfiguration{TState, TTransition, TNext}"/>
internal class FromTransitionConfiguration<TState, TTransition, TNext>
    : IFromTransitionConfiguration<TState, TTransition, TNext>,
        IFromAllStatesTransitionConfiguration<TState, TTransition, TNext>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TTransition transitionValue;
    private readonly INextStateConfiguration<TState, TTransition> nextStateConfiguration;
    private readonly List<IStateIdentity<TState>> excludedStates = [];
    private readonly List<INextParametersHandler> onTransitionHandlers = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="FromTransitionConfiguration{TState, TTransition, TNext}"/> class.
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
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> AllStates()
    {
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> AllOtherStates()
    {
        this.excludedStates.Add(
            StateIdentity.For(this.nextStateConfiguration.StateValue, this.nextStateConfiguration.TypeParameters)
        );
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> Except(TState state)
    {
        this.excludedStates.Add(StateIdentity.For(state));
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> Except<TPrevious>(TState state)
    {
        this.excludedStates.Add(StateIdentity.For<TState, TPrevious>(state));
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> Except<TPrevious1, TPrevious2>(
        TState state
    )
    {
        this.excludedStates.Add(StateIdentity.For<TState, TPrevious1, TPrevious2>(state));
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> Except<TPrevious1, TPrevious2, TPrevious3>(
        TState state
    )
    {
        this.excludedStates.Add(StateIdentity.For<TState, TPrevious1, TPrevious2, TPrevious3>(state));
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> Except<
        TPrevious1,
        TPrevious2,
        TPrevious3,
        TPrevious4
    >(TState state)
    {
        this.excludedStates.Add(StateIdentity.For<TState, TPrevious1, TPrevious2, TPrevious3, TPrevious4>(state));
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> OnTransition(
        Action<TNext> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onTransitionHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor).AsNextParametersHandler());
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> OnTransition(
        Func<TNext, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onTransitionHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor).AsNextParametersHandler());
        return this;
    }

    /// <inheritdoc />
    public IFromAllStatesTransitionConfiguration<TState, TTransition, TNext> OnTransition(
        Func<TNext, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onTransitionHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor).AsNextParametersHandler());
        return this;
    }

    /// <inheritdoc />
    public ITransitionInfo<TState, TTransition> GetInfo(IStateMachineInfo<TState, TTransition> stateMachine)
    {
        var nextStateInfo = this.nextStateConfiguration.GetInfo(stateMachine);
        var excludedStateInfo = this
            .excludedStates.Select(excludedState => new StateInfo<TState, TTransition>(
                stateMachine,
                excludedState.StateValue,
                excludedState.StateParameterTypes
            ))
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
                excludedState.Matches(state.StateValue, state.StateParameterTypes)
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
                stateMachine,
                this.onTransitionHandlers
            );

            state.AddTransition(transition);
        }
    }
}
