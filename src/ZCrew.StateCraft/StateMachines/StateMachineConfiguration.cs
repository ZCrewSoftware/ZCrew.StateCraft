using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States;
using ZCrew.StateCraft.Triggers;
using ZCrew.StateCraft.Triggers.Contracts;
using ZCrew.StateCraft.Validation;

namespace ZCrew.StateCraft.StateMachines;

/// <inheritdoc/>
internal class StateMachineConfiguration<TState, TTransition> : IStateMachineConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private IStateMachineActivator<TState, TTransition>? initialStateProducer;
    private readonly List<IAsyncAction<TState, TTransition, TState>> onStateChanges = [];
    private readonly List<IAsyncFunc<Exception, ExceptionResult>> onExceptionHandlers = [];
    private readonly List<IStateConfiguration<TState, TTransition>> stateConfigurations = [];
    private readonly List<IFinalTriggerConfiguration<TState, TTransition>> triggerConfigurations = [];
    private StateMachineOptions stateMachineOptions = StateMachineOptions.None;

    /// <inheritdoc/>
    public IEnumerable<IStateConfiguration<TState, TTransition>> States => this.stateConfigurations;

    /// <inheritdoc/>
    public IStateMachine<TState, TTransition> Build()
    {
        return Build(StateMachineBuildOptions.None);
    }

    /// <inheritdoc/>
    public IStateMachine<TState, TTransition> Build(StateMachineBuildOptions options)
    {
        if (this.initialStateProducer is null)
        {
            throw new InvalidOperationException("Initial state must be configured before building the state machine.");
        }

        // Ensure that only defined options are set. New entries to StateMachineBuildOptions will have to be added here
        if ((options & ~StateMachineBuildOptions.Validate) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Invalid build options were specified.");
        }

        if (options.HasFlag(StateMachineBuildOptions.Validate))
        {
            StateMachineValidation.Validate(this);
        }

        var triggers = new List<ITrigger>();
        var stateMachine = new StateMachine<TState, TTransition>(
            this.initialStateProducer,
            this.onStateChanges,
            this.onExceptionHandlers,
            triggers,
            this.stateMachineOptions
        );

        foreach (var triggerConfiguration in this.triggerConfigurations)
        {
            var trigger = triggerConfiguration.Build(stateMachine);
            triggers.Add(trigger);
        }

        // Defer transition configurations until all the states are populated so lookups can be made during building
        var transitionConfigurations = new List<ITransitionConfiguration<TState, TTransition>>();
        foreach (var stateConfiguration in this.stateConfigurations)
        {
            stateConfiguration.Build(stateMachine);
            transitionConfigurations.AddRange(stateConfiguration.Transitions);
        }

        foreach (var transitionConfiguration in transitionConfigurations)
        {
            transitionConfiguration.Build(stateMachine);
        }

        return stateMachine;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithAsynchronousActions()
    {
        this.stateMachineOptions |= StateMachineOptions.RunActionsAsynchronously;
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState(TState state)
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition>(state);
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState(Func<TState> stateProvider)
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition>(stateProvider.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<CancellationToken, Task<TState>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition>(stateProvider.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<CancellationToken, ValueTask<TState>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition>(stateProvider.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(TState state, T parameter)
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T>(state, parameter);
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(Func<(TState, T)> stateProvider)
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T>(stateProvider.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<CancellationToken, Task<(TState, T)>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T>(stateProvider.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<CancellationToken, ValueTask<(TState, T)>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T>(stateProvider.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        TState state,
        T1 parameter1,
        T2 parameter2
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2>(
            state,
            parameter1,
            parameter2
        );
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<(TState, T1, T2)> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2>(stateProvider.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<CancellationToken, Task<(TState, T1, T2)>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2>(stateProvider.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<CancellationToken, ValueTask<(TState, T1, T2)>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2>(stateProvider.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        TState state,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2, T3>(
            state,
            parameter1,
            parameter2,
            parameter3
        );
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<(TState, T1, T2, T3)> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2, T3>(
            stateProvider.AsAsyncFunc()
        );
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<CancellationToken, Task<(TState, T1, T2, T3)>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2, T3>(
            stateProvider.AsAsyncFunc()
        );
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<CancellationToken, ValueTask<(TState, T1, T2, T3)>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2, T3>(
            stateProvider.AsAsyncFunc()
        );
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        TState state,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2, T3, T4>(
            state,
            parameter1,
            parameter2,
            parameter3,
            parameter4
        );
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<(TState, T1, T2, T3, T4)> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2, T3, T4>(
            stateProvider.AsAsyncFunc()
        );
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<CancellationToken, Task<(TState, T1, T2, T3, T4)>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2, T3, T4>(
            stateProvider.AsAsyncFunc()
        );
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<CancellationToken, ValueTask<(TState, T1, T2, T3, T4)>> stateProvider
    )
    {
        this.initialStateProducer = new StateMachineActivator<TState, TTransition, T1, T2, T3, T4>(
            stateProvider.AsAsyncFunc()
        );
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> OnStateChange(Action<TState, TTransition, TState> handler)
    {
        this.onStateChanges.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, Task> handler
    )
    {
        this.onStateChanges.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, ValueTask> handler
    )
    {
        this.onStateChanges.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> OnException(Func<Exception, ExceptionResult> handler)
    {
        this.onExceptionHandlers.Add(handler.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> OnException(
        Func<Exception, CancellationToken, Task<ExceptionResult>> handler
    )
    {
        this.onExceptionHandlers.Add(handler.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> OnException(
        Func<Exception, CancellationToken, ValueTask<ExceptionResult>> handler
    )
    {
        this.onExceptionHandlers.Add(handler.AsAsyncFunc());
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithState(
        TState state,
        Func<IInitialStateConfiguration<TState, TTransition>, IStateConfiguration<TState, TTransition>> configureState
    )
    {
        var initialStateConfiguration = new ParameterlessStateConfiguration<TState, TTransition>(state);
        var finalStateConfiguration = configureState(initialStateConfiguration);
        this.stateConfigurations.Add(finalStateConfiguration);
        return this;
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithTrigger(
        Func<
            IInitialTriggerConfiguration<TState, TTransition>,
            IFinalTriggerConfiguration<TState, TTransition>
        > configureTrigger
    )
    {
        var initialTriggerConfiguration = new InitialTriggerConfiguration<TState, TTransition>();
        var finalTriggerConfiguration = configureTrigger(initialTriggerConfiguration);
        this.triggerConfigurations.Add(finalTriggerConfiguration);
        return this;
    }
}
