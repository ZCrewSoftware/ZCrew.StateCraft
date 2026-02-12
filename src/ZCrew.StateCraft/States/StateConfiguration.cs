using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Transitions;

namespace ZCrew.StateCraft.States;

/// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}"/>
internal class StateConfiguration<TState, TTransition>
    : IInitialStateConfiguration<TState, TTransition>,
        IParameterlessStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IAsyncAction<TState>> onActivateHandlers = [];
    private readonly List<IAsyncAction<TState>> onDeactivateHandlers = [];
    private readonly List<IAsyncAction<TState, TTransition, TState>> onStateChangeHandlers = [];
    private readonly List<IAsyncAction> onEntryHandlers = [];
    private readonly List<IAsyncAction> onExitHandlers = [];
    private readonly List<IActionConfiguration> actionConfigurations = [];
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
    public IState<TState, TTransition> Build(IStateMachine<TState, TTransition> stateMachine)
    {
        var actions = this.actionConfigurations.Select(action => action.Build()).ToList();
        var state = new State<TState, TTransition>(
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
    public IParameterlessStateConfiguration<TState, TTransition> OnActivate(Action<TState> handler)
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnActivate(
        Func<TState, CancellationToken, Task> handler
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnActivate(
        Func<TState, CancellationToken, ValueTask> handler
    )
    {
        this.onActivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnDeactivate(Action<TState> handler)
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnDeactivate(
        Func<TState, CancellationToken, Task> handler
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnDeactivate(
        Func<TState, CancellationToken, ValueTask> handler
    )
    {
        this.onDeactivateHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnStateChange(
        Action<TState, TTransition, TState> handler
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, Task> handler
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnStateChange(
        Func<TState, TTransition, TState, CancellationToken, ValueTask> handler
    )
    {
        this.onStateChangeHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnEntry(Action handler)
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnEntry(Func<CancellationToken, Task> handler)
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnEntry(Func<CancellationToken, ValueTask> handler)
    {
        this.onEntryHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnExit(Action handler)
    {
        this.onExitHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnExit(Func<CancellationToken, Task> handler)
    {
        this.onExitHandlers.Add(handler.AsAsyncAction());
        return this;
    }

    /// <inheritdoc />
    public IParameterlessStateConfiguration<TState, TTransition> OnExit(Func<CancellationToken, ValueTask> handler)
    {
        this.onExitHandlers.Add(handler.AsAsyncAction());
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
    public override string ToString()
    {
        return $"{State}";
    }
}
