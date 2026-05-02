using System.Runtime.CompilerServices;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions;
using ZCrew.StateCraft.Async;
using ZCrew.StateCraft.Extensions;
using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.Rendering.Contracts;
using ZCrew.StateCraft.Rendering.Models;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Transitions;
using ZCrew.StateCraft.Triggers;
using ZCrew.StateCraft.Validation;
using ZCrew.StateCraft.Validation.Contracts;
using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.States;

/// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}"/>
internal class StateConfiguration<TState, TTransition>
    : IInitialStateConfiguration<TState, TTransition>,
        IRenderable<TState, TTransition>,
        IValidatable<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<AsyncHandler<TState>> onActivateHandlers = [];
    private readonly List<AsyncHandler<TState>> onDeactivateHandlers = [];
    private readonly List<AsyncHandler<TState, TTransition, TState>> onStateChangeHandlers = [];
    private readonly List<AsyncHandler> onEntryHandlers = [];
    private readonly List<AsyncHandler> onExitHandlers = [];
    private readonly List<IActionConfiguration> actionConfigurations = [];
    private readonly List<ITriggerConfiguration<TState, TTransition>> triggerConfigurations = [];
    private readonly List<ITransitionConfiguration<TState, TTransition>> transitionConfigurations = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="StateConfiguration{TState, TTransition}"/> class.
    /// </summary>
    /// <param name="state">The state value that identifies this state configuration.</param>
    public StateConfiguration(TState state)
    {
        State = state;
    }

    /// <inheritdoc />
    public TState State { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public IEnumerable<ITransitionConfiguration<TState, TTransition>> Transitions => this.transitionConfigurations;

    /// <inheritdoc />
    public void Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var actions = this.actionConfigurations.Select(action => action.Build()).ToList();
        var triggers = this.triggerConfigurations.Select(trigger => trigger.Build(stateMachine)).ToList();
        var state = new State<TState, TTransition>(
            State,
            this.onActivateHandlers,
            this.onDeactivateHandlers,
            this.onStateChangeHandlers,
            this.onEntryHandlers,
            this.onExitHandlers,
            actions,
            triggers,
            stateMachine
        );

        stateMachine.AddState(state);
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> WithAction(
        Func<IInitialActionConfiguration, IActionConfiguration> configureAction
    )
    {
        var initialActionConfiguration = new InitialActionConfiguration();
        var finalActionConfiguration = configureAction(initialActionConfiguration);
        this.actionConfigurations.Add(finalActionConfiguration);
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> WithTrigger(
        Func<
            IInitialTriggerConfiguration<TState, TTransition>,
            ITriggerConfiguration<TState, TTransition>
        > configureTrigger
    )
    {
        var initialTriggerConfiguration = new InitialTriggerConfiguration<TState, TTransition>();
        var triggerConfiguration = configureTrigger(initialTriggerConfiguration);
        this.triggerConfigurations.Add(triggerConfiguration);
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> WithTransition(
        TTransition transition,
        Func<
            IInitialTransitionConfiguration<TState, TTransition>,
            ITransitionConfiguration<TState, TTransition>
        > configureTransition
    )
    {
        var initialTransitionConfiguration = new InitialTransitionConfiguration<TState, TTransition>(State, transition);
        var finalTransitionConfiguration = configureTransition(initialTransitionConfiguration);
        this.transitionConfigurations.Add(finalTransitionConfiguration);
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnActivate(
        Action<TState> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnActivate(
        Func<TState, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnActivate(
        Func<TState, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnDeactivate(
        Action<TState> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnDeactivate(
        Func<TState, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnDeactivate(
        Func<TState, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnStateChange(
        Action<TState, TTransition, TState> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnEntry(
        Action handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnEntry(
        Func<CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnEntry(
        Func<CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnExit(
        Action handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnExit(
        Func<CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnExit(
        Func<CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> WithNoParameters()
    {
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> WithParameter<T>()
    {
        return new StateConfiguration<TState, TTransition, T>(State);
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2> WithParameters<T1, T2>()
    {
        return new StateConfiguration<TState, TTransition, T1, T2>(State);
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithParameters<T1, T2, T3>()
    {
        return new StateConfiguration<TState, TTransition, T1, T2, T3>(State);
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> WithParameters<T1, T2, T3, T4>()
    {
        return new StateConfiguration<TState, TTransition, T1, T2, T3, T4>(State);
    }

    /// <inheritdoc />
    public void AddToValidationContext(StateMachineValidationContext<TState, TTransition> context)
    {
        var state = new StateValidationModel<TState, TTransition>(State, TypeParameters);
        context.States.Add(state);
    }

    /// <inheritdoc />
    public void AddToRenderingContext(StateMachineRenderingContext<TState, TTransition> context)
    {
        var id = $"{State}";
        var descriptor = ToString();
        var state = new StateRenderingModel<TState, TTransition>(State, TypeParameters, id, descriptor);
        context.States.Add(state);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{State}";
    }
}
