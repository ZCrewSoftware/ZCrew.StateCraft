using ZCrew.StateCraft.Actions.Contracts;
using ZCrew.StateCraft.Async;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.States;

/// <summary>
///     Standard implementation of a state with four parameters.
/// </summary>
internal class State<TState, TTransition, T1, T2, T3, T4> : IState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly IReadOnlyList<AsyncHandler<TState, T1, T2, T3, T4>> onActivateHandlers;
    private readonly IReadOnlyList<AsyncHandler<TState, T1, T2, T3, T4>> onDeactivateHandlers;
    private readonly IReadOnlyList<AsyncHandler<TState, TTransition, TState, T1, T2, T3, T4>> onStateChangeHandlers;
    private readonly IReadOnlyList<AsyncHandler<T1, T2, T3, T4>> onEntryHandlers;
    private readonly IReadOnlyList<AsyncHandler<T1, T2, T3, T4>> onExitHandlers;
    private readonly IReadOnlyList<IAction<T1, T2, T3, T4>> actions;
    private readonly TransitionTable<TState, TTransition> transitionTable = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="State{TState, TTransition, T1, T2, T3, T4}"/> class.
    /// </summary>
    public State(
        TState state,
        IReadOnlyList<AsyncHandler<TState, T1, T2, T3, T4>> onActivateHandlers,
        IReadOnlyList<AsyncHandler<TState, T1, T2, T3, T4>> onDeactivateHandlers,
        IReadOnlyList<AsyncHandler<TState, TTransition, TState, T1, T2, T3, T4>> onStateChangeHandlers,
        IReadOnlyList<AsyncHandler<T1, T2, T3, T4>> onEntryHandlers,
        IReadOnlyList<AsyncHandler<T1, T2, T3, T4>> onExitHandlers,
        IReadOnlyList<IAction<T1, T2, T3, T4>> actions,
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
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

    /// <inheritdoc />
    public IStateMachine<TState, TTransition> StateMachine { get; }

    /// <inheritdoc />
    public IEnumerable<ITransition<TState, TTransition>> Transitions => this.transitionTable;

    /// <inheritdoc />
    public async Task Activate(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2, p3, p4) = parameters.GetNextParameters<T1, T2, T3, T4>();
        StateMachine.Tracker?.Activated(this, (p1, p2, p3, p4));
        foreach (var handler in this.onActivateHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnActivate(
                t => handler.Invoke(StateValue, p1, p2, p3, p4, t),
                token
            );
        }
    }

    /// <inheritdoc />
    public async Task Deactivate(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2, p3, p4) = parameters.GetPreviousParameters<T1, T2, T3, T4>();
        StateMachine.Tracker?.Deactivated(this, (p1, p2, p3, p4));
        foreach (var handler in this.onDeactivateHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnDeactivate(
                t => handler.Invoke(StateValue, p1, p2, p3, p4, t),
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
        var (p1, p2, p3, p4) = parameters.GetNextParameters<T1, T2, T3, T4>();
        foreach (var handler in this.onStateChangeHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnStateChange(
                t => handler.Invoke(previousState, transition, StateValue, p1, p2, p3, p4, t),
                token
            );
        }
    }

    /// <inheritdoc />
    public async Task Enter(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2, p3, p4) = parameters.GetNextParameters<T1, T2, T3, T4>();
        StateMachine.Tracker?.Entered(this, (p1, p2, p3, p4));
        foreach (var handler in this.onEntryHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnEntry(t => handler.Invoke(p1, p2, p3, p4, t), token);
        }
    }

    /// <inheritdoc />
    public async Task Exit(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2, p3, p4) = parameters.GetPreviousParameters<T1, T2, T3, T4>();
        StateMachine.Tracker?.Exited(this, (p1, p2, p3, p4));
        foreach (var handler in this.onExitHandlers)
        {
            await StateMachine.ExceptionBehavior.CallOnExit(t => handler.Invoke(p1, p2, p3, p4, t), token);
        }
    }

    /// <inheritdoc />
    public async Task Action(IStateMachineParameters parameters, CancellationToken token)
    {
        var (p1, p2, p3, p4) = parameters.GetCurrentParameters<T1, T2, T3, T4>();
        foreach (var action in this.actions)
        {
            await StateMachine.ExceptionBehavior.CallAction(t => action.Invoke(p1, p2, p3, p4, t), token);
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
            var available = this.transitionTable.Select(t => t.ToString()).ToList();

            var availableInfo =
                available.Count > 0
                    ? $" Available from '{StateValue}': {string.Join(", ", available)}."
                    : $" No transitions registered for '{StateValue}'.";

            throw new InvalidOperationException(
                $"No transition could be found for '{transition}' from state '{StateValue}'.{availableInfo}"
            );
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
        return $"{StateValue}<"
            + $"{typeof(T1).FriendlyName}, "
            + $"{typeof(T2).FriendlyName}, "
            + $"{typeof(T3).FriendlyName}, "
            + $"{typeof(T4).FriendlyName}>";
    }
}
