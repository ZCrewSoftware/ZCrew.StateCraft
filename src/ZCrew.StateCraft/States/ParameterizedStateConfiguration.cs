using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Transitions;

namespace ZCrew.StateCraft.States;

/// <inheritdoc />
[DebuggerDisplay("{DisplayString}")]
internal class ParameterizedStateConfiguration<TState, TTransition, T>
    : IParameterizedStateConfiguration<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString => $"{State}<{typeof(T).FriendlyName}>";

    private readonly List<IAsyncAction<TState, T>> onActivateHandlers = [];
    private readonly List<IAsyncAction<TState, T>> onDeactivateHandlers = [];
    private readonly List<IAsyncAction<TState, TTransition, TState, T>> onStateChangeHandlers = [];
    private readonly List<IAsyncAction<T>> onEntryHandlers = [];
    private readonly List<IAsyncAction<T>> onExitHandlers = [];
    private readonly List<IParameterizedActionConfiguration<T>> actionConfigurations = [];
    private readonly List<ITransitionConfiguration<TState, TTransition>> transitionConfigurations = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="ParameterizedStateConfiguration{TState, TTransition, T}"/> class.
    /// </summary>
    /// <param name="state">The state value that identifies this state configuration.</param>
    public ParameterizedStateConfiguration(TState state)
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
    public IState<TState, TTransition> Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var actions = this.actionConfigurations.Select(action => action.Build()).ToList();
        var state = new ParameterizedState<TState, TTransition, T>(
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

        return state;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> WithAction(
        Func<IInitialParameterizedActionConfiguration<T>, IFinalParameterizedActionConfiguration<T>> configureAction
    )
    {
        var initialActionConfiguration = new InitialParameterizedActionConfiguration<T>();
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
    public IParameterizedStateConfiguration<TState, TTransition, T> OnActivate(Action<TState, T> handler)
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnActivate(
        Func<TState, T, CancellationToken, Task> handler
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnActivate(
        Func<TState, T, CancellationToken, ValueTask> handler
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnDeactivate(Action<TState, T> handler)
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnDeactivate(
        Func<TState, T, CancellationToken, Task> handler
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnDeactivate(
        Func<TState, T, CancellationToken, ValueTask> handler
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnStateChange(
        Action<TState, TTransition, TState, T> handler
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnStateChange(
        Func<TState, TTransition, TState, T, CancellationToken, Task> handler
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnStateChange(
        Func<TState, TTransition, TState, T, CancellationToken, ValueTask> handler
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnEntry(Action<T> handler)
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnEntry(Func<T, CancellationToken, Task> handler)
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnEntry(
        Func<T, CancellationToken, ValueTask> handler
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnExit(Action<T> handler)
    {
        this.onExitHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnExit(Func<T, CancellationToken, Task> handler)
    {
        this.onExitHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T> OnExit(
        Func<T, CancellationToken, ValueTask> handler
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"State: {DisplayString}";
    }
}
