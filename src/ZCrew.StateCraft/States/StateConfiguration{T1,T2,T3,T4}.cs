using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Transitions;

namespace ZCrew.StateCraft.States;

/// <inheritdoc />
[DebuggerDisplay("{DisplayString}")]
internal class StateConfiguration<TState, TTransition, T1, T2, T3, T4>
    : IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString =>
        $"{State}<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}, {typeof(T3).FriendlyName}, {typeof(T4).FriendlyName}>";

    private readonly List<IAsyncAction<TState, T1, T2, T3, T4>> onActivateHandlers = [];
    private readonly List<IAsyncAction<TState, T1, T2, T3, T4>> onDeactivateHandlers = [];
    private readonly List<IAsyncAction<TState, TTransition, TState, T1, T2, T3, T4>> onStateChangeHandlers = [];
    private readonly List<IAsyncAction<T1, T2, T3, T4>> onEntryHandlers = [];
    private readonly List<IAsyncAction<T1, T2, T3, T4>> onExitHandlers = [];
    private readonly List<IActionConfiguration<T1, T2, T3, T4>> actionConfigurations = [];
    private readonly List<ITransitionConfiguration<TState, TTransition>> transitionConfigurations = [];

    public StateConfiguration(TState state)
    {
        State = state;
    }

    /// <inheritdoc />
    public TState State { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    /// <inheritdoc />
    public IEnumerable<ITransitionConfiguration<TState, TTransition>> Transitions => this.transitionConfigurations;

    /// <inheritdoc />
    public IState<TState, TTransition> Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var actions = this.actionConfigurations.Select(action => action.Build()).ToList();
        var state = new State<TState, TTransition, T1, T2, T3, T4>(
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
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> WithAction(
        Func<IInitialActionConfiguration<T1, T2, T3, T4>, IActionConfiguration<T1, T2, T3, T4>> configureAction
    )
    {
        var initialActionConfiguration = new InitialActionConfiguration<T1, T2, T3, T4>();
        var finalActionConfiguration = configureAction(initialActionConfiguration);
        this.actionConfigurations.Add(finalActionConfiguration);
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> WithTransition(
        TTransition transition,
        Func<
            IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>,
            ITransitionConfiguration<TState, TTransition>
        > configureTransition
    )
    {
        var initialTransitionConfiguration = new InitialTransitionConfiguration<TState, TTransition, T1, T2, T3, T4>(
            State,
            transition
        );
        var finalTransitionConfiguration = configureTransition(initialTransitionConfiguration);
        this.transitionConfigurations.Add(finalTransitionConfiguration);
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnActivate(
        Action<TState, T1, T2, T3, T4> handler
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnActivate(
        Func<TState, T1, T2, T3, T4, CancellationToken, Task> handler
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnActivate(
        Func<TState, T1, T2, T3, T4, CancellationToken, ValueTask> handler
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnDeactivate(
        Action<TState, T1, T2, T3, T4> handler
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnDeactivate(
        Func<TState, T1, T2, T3, T4, CancellationToken, Task> handler
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnDeactivate(
        Func<TState, T1, T2, T3, T4, CancellationToken, ValueTask> handler
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnStateChange(
        Action<TState, TTransition, TState, T1, T2, T3, T4> handler
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnStateChange(
        Func<TState, TTransition, TState, T1, T2, T3, T4, CancellationToken, Task> handler
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnStateChange(
        Func<TState, TTransition, TState, T1, T2, T3, T4, CancellationToken, ValueTask> handler
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnEntry(Action<T1, T2, T3, T4> handler)
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnEntry(
        Func<T1, T2, T3, T4, CancellationToken, Task> handler
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnEntry(
        Func<T1, T2, T3, T4, CancellationToken, ValueTask> handler
    )
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnExit(Action<T1, T2, T3, T4> handler)
    {
        this.onExitHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnExit(
        Func<T1, T2, T3, T4, CancellationToken, Task> handler
    )
    {
        this.onExitHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> OnExit(
        Func<T1, T2, T3, T4, CancellationToken, ValueTask> handler
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
