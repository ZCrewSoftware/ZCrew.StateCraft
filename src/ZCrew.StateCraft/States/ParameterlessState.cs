using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;
using ZCrew.StateCraft.States.Contracts;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft.States;

/// <summary>
///     Standard implementation of a state with no parameters.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
[DebuggerDisplay("{DisplayString}")]
internal class ParameterlessState<TState, TTransition>
    : IState<TState, TTransition>,
        IParameterlessState<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString => $"{StateValue}";

    private readonly IReadOnlyList<IAsyncAction<TState>> onActivateHandlers;
    private readonly IReadOnlyList<IAsyncAction<TState>> onDeactivateHandlers;
    private readonly IReadOnlyList<IAsyncAction<TState, TTransition, TState>> onStateChangeHandlers;
    private readonly IReadOnlyList<IAsyncAction> onEntryHandlers;
    private readonly IReadOnlyList<IAsyncAction> onExitHandlers;
    private readonly IReadOnlyList<IParameterlessAction> actions;
    private readonly TransitionTable<TState, TTransition> transitionTable = [];

    public ParameterlessState(
        TState state,
        IReadOnlyList<IAsyncAction<TState>> onActivateHandlers,
        IReadOnlyList<IAsyncAction<TState>> onDeactivateHandlers,
        IReadOnlyList<IAsyncAction<TState, TTransition, TState>> onStateChangeHandlers,
        IReadOnlyList<IAsyncAction> onEntryHandlers,
        IReadOnlyList<IAsyncAction> onExitHandlers,
        IReadOnlyList<IParameterlessAction> actions,
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
    public IReadOnlyList<Type> TypeParameters { get; } = [];

    /// <inheritdoc />
    public IStateMachine<TState, TTransition> StateMachine { get; }

    /// <inheritdoc />
    public IEnumerable<ITransition<TState, TTransition>> Transitions => this.transitionTable;

    /// <inheritdoc />
    public async Task Activate(CancellationToken token)
    {
        StateMachine.Tracker?.Activated(this);
        foreach (var handler in this.onActivateHandlers)
        {
            await StateMachine.RunWithExceptionHandling(() => handler.InvokeAsync(StateValue, token), token);
        }
    }

    /// <inheritdoc />
    public async Task Deactivate(CancellationToken token)
    {
        StateMachine.Tracker?.Deactivated(this);
        foreach (var handler in this.onDeactivateHandlers)
        {
            await StateMachine.RunWithExceptionHandling(() => handler.InvokeAsync(StateValue, token), token);
        }
    }

    /// <inheritdoc />
    public Task StateChange(
        TState previousState,
        TTransition transition,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        return StateChange(previousState, transition, token);
    }

    /// <inheritdoc />
    public async Task StateChange(TState previousState, TTransition transition, CancellationToken token)
    {
        foreach (var handler in this.onStateChangeHandlers)
        {
            await StateMachine.RunWithExceptionHandling(
                () => handler.InvokeAsync(previousState, transition, StateValue, token),
                token
            );
        }
    }

    /// <inheritdoc />
    public async Task Enter(CancellationToken token)
    {
        StateMachine.Tracker?.Entered(this);
        foreach (var handler in this.onEntryHandlers)
        {
            await StateMachine.RunWithExceptionHandling(() => handler.InvokeAsync(token), token);
        }
    }

    /// <inheritdoc />
    public async Task Exit(CancellationToken token)
    {
        StateMachine.Tracker?.Exited(this);
        foreach (var handler in this.onExitHandlers)
        {
            await StateMachine.RunWithExceptionHandling(() => handler.InvokeAsync(token), token);
        }
    }

    /// <inheritdoc />
    public async Task Action(CancellationToken token)
    {
        foreach (var action in this.actions)
        {
            await StateMachine.RunWithExceptionHandling(() => action.Invoke(token), throwOnCancellation: false, token);
        }
    }

    /// <inheritdoc />
    public void AddTransition(ITransition<TState, TTransition> transition)
    {
        this.transitionTable.Add(transition);
    }

    /// <inheritdoc />
    public async Task<ITransition<TState, TTransition>> GetTransition(TTransition transition, CancellationToken token)
    {
        var result = await this.transitionTable.LookupParameterlessTransition(transition, token);
        if (result == null)
        {
            throw new InvalidOperationException(
                $"No parameterless transition could be found for: Transition={transition}"
            );
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<ITransition<TState, TTransition>> GetTransition<TNext>(
        TTransition transition,
        TNext nextParameter,
        CancellationToken token
    )
    {
        var result = await this.transitionTable.LookupParameterizedTransition(transition, nextParameter, token);
        if (result == null)
        {
            throw new InvalidOperationException(
                $"No parameterized transition could be found for: Transition={transition}, Next={typeof(TNext)}"
            );
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<ITransition<TState, TTransition>?> GetTransitionOrDefault(
        TTransition transition,
        CancellationToken token
    )
    {
        return await this.transitionTable.LookupParameterlessTransition(transition, token);
    }

    /// <inheritdoc />
    public async Task<ITransition<TState, TTransition>?> GetTransitionOrDefault<TNext>(
        TTransition transition,
        TNext nextParameter,
        CancellationToken token
    )
    {
        return await this.transitionTable.LookupParameterizedTransition(transition, nextParameter, token);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"State: {DisplayString}";
    }
}
