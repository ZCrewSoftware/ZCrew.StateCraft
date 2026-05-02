using System.Runtime.CompilerServices;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions;
using ZCrew.StateCraft.Async;
using ZCrew.StateCraft.Extensions;
using ZCrew.StateCraft.Rendering;
using ZCrew.StateCraft.Rendering.Contracts;
using ZCrew.StateCraft.Rendering.Extensions;
using ZCrew.StateCraft.Rendering.Models;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Transitions;
using ZCrew.StateCraft.Validation;
using ZCrew.StateCraft.Validation.Contracts;
using ZCrew.StateCraft.Validation.Models;

namespace ZCrew.StateCraft.States;

/// <inheritdoc cref="IParameterizedStateConfiguration{TState,TTransition,T1,T2,T3,T4}" />
internal class StateConfiguration<TState, TTransition, T1, T2, T3>
    : IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3>,
        IRenderable<TState, TTransition>,
        IValidatable<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<AsyncHandler<TState, T1, T2, T3>> onActivateHandlers = [];
    private readonly List<AsyncHandler<TState, T1, T2, T3>> onDeactivateHandlers = [];
    private readonly List<AsyncHandler<TState, TTransition, TState, T1, T2, T3>> onStateChangeHandlers = [];
    private readonly List<AsyncHandler<T1, T2, T3>> onEntryHandlers = [];
    private readonly List<AsyncHandler<T1, T2, T3>> onExitHandlers = [];
    private readonly List<IActionConfiguration<T1, T2, T3>> actionConfigurations = [];
    private readonly List<ITransitionConfiguration<TState, TTransition>> transitionConfigurations = [];

    public StateConfiguration(TState state)
    {
        State = state;
    }

    /// <inheritdoc />
    public TState State { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3)];

    /// <inheritdoc />
    public IEnumerable<ITransitionConfiguration<TState, TTransition>> Transitions => this.transitionConfigurations;

    /// <inheritdoc />
    public void Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var actions = this.actionConfigurations.Select(action => action.Build()).ToList();
        var state = new State<TState, TTransition, T1, T2, T3>(
            State,
            this.onActivateHandlers,
            this.onDeactivateHandlers,
            this.onStateChangeHandlers,
            this.onEntryHandlers,
            this.onExitHandlers,
            actions,
            stateMachine
        );

        stateMachine.AddState(state);
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithAction(
        Func<IInitialActionConfiguration<T1, T2, T3>, IActionConfiguration<T1, T2, T3>> configureAction
    )
    {
        var initialActionConfiguration = new InitialActionConfiguration<T1, T2, T3>();
        var finalActionConfiguration = configureAction(initialActionConfiguration);
        this.actionConfigurations.Add(finalActionConfiguration);
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithTransition(
        TTransition transition,
        Func<
            IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3>,
            ITransitionConfiguration<TState, TTransition>
        > configureTransition
    )
    {
        var initialTransitionConfiguration = new InitialTransitionConfiguration<TState, TTransition, T1, T2, T3>(
            State,
            transition
        );
        var finalTransitionConfiguration = configureTransition(initialTransitionConfiguration);
        this.transitionConfigurations.Add(finalTransitionConfiguration);
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnActivate(
        Action<TState, T1, T2, T3> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnActivate(
        Func<TState, T1, T2, T3, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnActivate(
        Func<TState, T1, T2, T3, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnDeactivate(
        Action<TState, T1, T2, T3> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnDeactivate(
        Func<TState, T1, T2, T3, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnDeactivate(
        Func<TState, T1, T2, T3, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnStateChange(
        Action<TState, TTransition, TState, T1, T2, T3> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnStateChange(
        Func<TState, TTransition, TState, T1, T2, T3, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnStateChange(
        Func<TState, TTransition, TState, T1, T2, T3, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnEntry(
        Action<T1, T2, T3> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnEntry(
        Func<T1, T2, T3, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnEntry(
        Func<T1, T2, T3, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnExit(
        Action<T1, T2, T3> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnExit(
        Func<T1, T2, T3, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnExit(
        Func<T1, T2, T3, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
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
        var identifier =
            $"{State}"
            + $"_{typeof(T1).RenderingIdentifier}"
            + $"_{typeof(T2).RenderingIdentifier}"
            + $"_{typeof(T3).RenderingIdentifier}";
        var descriptor = ToString();
        var state = new StateRenderingModel<TState, TTransition>(State, TypeParameters, identifier, descriptor);
        context.States.Add(state);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{State}<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}, {typeof(T3).FriendlyName}>";
    }
}
