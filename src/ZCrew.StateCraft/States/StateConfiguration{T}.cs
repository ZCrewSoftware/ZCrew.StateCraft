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

/// <inheritdoc cref="IParameterizedStateConfiguration{TState,TTransition,T}" />
internal class StateConfiguration<TState, TTransition, T>
    : IParameterizedStateConfiguration<TState, TTransition, T>,
        IRenderable<TState, TTransition>,
        IValidatable<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<AsyncHandler<TState, T>> onActivateHandlers = [];
    private readonly List<AsyncHandler<TState, T>> onDeactivateHandlers = [];
    private readonly List<AsyncHandler<TState, TTransition, TState, T>> onStateChangeHandlers = [];
    private readonly List<AsyncHandler<T>> onEntryHandlers = [];
    private readonly List<AsyncHandler<T>> onExitHandlers = [];
    private readonly List<IActionConfiguration<T>> actionConfigurations = [];
    private readonly List<ITransitionConfiguration<TState, TTransition>> transitionConfigurations = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="StateConfiguration{TState, TTransition, T}"/> class.
    /// </summary>
    /// <param name="state">The state value that identifies this state configuration.</param>
    public StateConfiguration(TState state)
    {
        State = state;
    }

    /// <inheritdoc />
    public TState State { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    /// <inheritdoc />
    public IEnumerable<ITransitionConfiguration<TState, TTransition>> Transitions => this.transitionConfigurations;

    /// <inheritdoc />
    public void Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var actions = this.actionConfigurations.Select(action => action.Build()).ToList();
        var state = new State<TState, TTransition, T>(
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
    public IParameterizedStateConfiguration<TState, TTransition, T> WithAction(
        Func<IInitialActionConfiguration<T>, IActionConfiguration<T>> configureAction
    )
    {
        var initialActionConfiguration = new InitialActionConfiguration<T>();
        var finalActionConfiguration = configureAction(initialActionConfiguration);
        this.actionConfigurations.Add(finalActionConfiguration);
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> WithTransition(
        TTransition transition,
        Func<
            IInitialTransitionConfiguration<TState, TTransition, T>,
            ITransitionConfiguration<TState, TTransition>
        > configureTransition
    )
    {
        var initialTransitionConfiguration = new InitialTransitionConfiguration<TState, TTransition, T>(
            State,
            transition
        );
        var finalTransitionConfiguration = configureTransition(initialTransitionConfiguration);
        this.transitionConfigurations.Add(finalTransitionConfiguration);
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnActivate(
        Action<TState, T> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnActivate(
        Func<TState, T, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnActivate(
        Func<TState, T, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnDeactivate(
        Action<TState, T> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnDeactivate(
        Func<TState, T, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnDeactivate(
        Func<TState, T, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnStateChange(
        Action<TState, TTransition, TState, T> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnStateChange(
        Func<TState, TTransition, TState, T, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnStateChange(
        Func<TState, TTransition, TState, T, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnEntry(
        Action<T> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnEntry(
        Func<T, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnEntry(
        Func<T, CancellationToken, ValueTask> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnExit(
        Action<T> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnExit(
        Func<T, CancellationToken, Task> handler,
        [CallerArgumentExpression(nameof(handler))] string? descriptor = null
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction().AsAsyncHandler(descriptor));
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnExit(
        Func<T, CancellationToken, ValueTask> handler,
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
        var id = $"{State}_{typeof(T).RenderingId}";
        var descriptor = ToString();
        var state = new StateRenderingModel<TState, TTransition>(State, TypeParameters, id, descriptor);
        context.States.Add(state);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{State}<{typeof(T).FriendlyName}>";
    }
}
