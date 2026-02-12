using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.States;

/// <summary>
///     Standard implementation of a state with two parameters.
/// </summary>
internal class State<TState, TTransition, T1, T2> : IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<IAsyncAction<TState, T1, T2>> onActivateHandlers;
    private readonly IReadOnlyList<IAsyncAction<TState, T1, T2>> onDeactivateHandlers;
    private readonly IReadOnlyList<IAsyncAction<TState, TTransition, TState, T1, T2>> onStateChangeHandlers;
    private readonly IReadOnlyList<IAsyncAction<T1, T2>> onEntryHandlers;
    private readonly IReadOnlyList<IAsyncAction<T1, T2>> onExitHandlers;
    private readonly IReadOnlyList<IAction<T1, T2>> actions;
    private readonly TransitionTable<TState, TTransition> transitionTable = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="State{TState, TTransition, T1, T2}"/> class.
    /// </summary>
    public State(
        TState state,
        IReadOnlyList<IAsyncAction<TState, T1, T2>> onActivateHandlers,
        IReadOnlyList<IAsyncAction<TState, T1, T2>> onDeactivateHandlers,
        IReadOnlyList<IAsyncAction<TState, TTransition, TState, T1, T2>> onStateChangeHandlers,
        IReadOnlyList<IAsyncAction<T1, T2>> onEntryHandlers,
        IReadOnlyList<IAsyncAction<T1, T2>> onExitHandlers,
        IReadOnlyList<IAction<T1, T2>> actions,
        IStateMachine<TState, TTransition> stateMachine
    )
    {
        StateValue = state;
        this.onActivateHandlers = onActivateHandlers;
        this.onDeactivateHandlers = onDeactivateHandlers;
        this.onStateChangeHandlers = onStateChangeHandlers;
        this.onEntryHandlers = onEntryHandlers;
        this.onExitHandlers = onExitHandlers;
        this.actions = actions;
        StateMachine = stateMachine;
    }

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2)];

    /// <inheritdoc />
    public IStateMachine<TState, TTransition> StateMachine { get; }

    /// <inheritdoc />
    public IEnumerable<ITransition<TState, TTransition>> Transitions => this.transitionTable;

    /// <inheritdoc />
    public async Task Activate(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2) = parameters.GetNextParameters<T1, T2>();
        StateMachine.Tracker?.Activated(this, (p1, p2));
        foreach (var handler in this.onActivateHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnActivate(t => handler.InvokeAsync(StateValue, p1, p2, t), token);
        }
    }

    /// <inheritdoc />
    public async Task Deactivate(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2) = parameters.GetPreviousParameters<T1, T2>();
        StateMachine.Tracker?.Deactivated(this, (p1, p2));
        foreach (var handler in this.onDeactivateHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnDeactivate(
                t => handler.InvokeAsync(StateValue, p1, p2, t),
                token
            );
        }
    }

    /// <inheritdoc />
    public async Task StateChange(
        TState previousState,
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        var (p1, p2) = parameters.GetNextParameters<T1, T2>();
        foreach (var handler in this.onStateChangeHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnStateChange(
                t => handler.InvokeAsync(previousState, transition, StateValue, p1, p2, t),
                token
            );
        }
    }

    /// <inheritdoc />
    public async Task Enter(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2) = parameters.GetNextParameters<T1, T2>();
        StateMachine.Tracker?.Entered(this, (p1, p2));
        foreach (var handler in this.onEntryHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnEntry(t => handler.InvokeAsync(p1, p2, t), token);
        }
    }

    /// <inheritdoc />
    public async Task Exit(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2) = parameters.GetPreviousParameters<T1, T2>();
        StateMachine.Tracker?.Exited(this, (p1, p2));
        foreach (var handler in this.onExitHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnExit(t => handler.InvokeAsync(p1, p2, t), token);
        }
    }

    /// <inheritdoc />
    public async Task Action(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2) = parameters.GetCurrentParameters<T1, T2>();
        foreach (var action in this.actions)
        {
            await StateMachine.ExceptionBehavior.CallAction(t => action.Invoke(p1, p2, t), token);
        }
    }

    /// <inheritdoc />
    public void AddTransition(ITransition<TState, TTransition> transition)
    {
        this.transitionTable.Add(transition);
    }

    /// <inheritdoc />
    public async Task<ITransition<TState, TTransition>> GetTransition(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        var result = await this.transitionTable.LookupTransition(transition, parameters, token);
        if (result == null)
        {
            throw new InvalidOperationException($"No transition could be found for: Transition={transition}");
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        return await this.transitionTable.LookupTransition(transition, parameters, token);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{StateValue}<{typeof(T1).FriendlyName}, {typeof(T2).FriendlyName}>";
    }
}
