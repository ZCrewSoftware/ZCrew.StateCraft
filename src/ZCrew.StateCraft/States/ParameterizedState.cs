using System.Diagnostics;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Actions.Contracts;
using ZCrew.StateCraft.Parameters.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.States;

/// <summary>
///     Standard implementation of a state with a single parameter.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T">The type of the parameter for this state.</typeparam>
[DebuggerDisplay("{DisplayString}")]
internal class ParameterizedState<TState, TTransition, T>
    : IState<TState, TTransition>,
        IParameterizedState<TState, TTransition, T>
    where TState : notnull
    where TTransition : notnull
{
    private string DisplayString => $"{StateValue}<{typeof(T).FriendlyName}>";

    private readonly IReadOnlyList<IAsyncAction<TState, T>> onActivateHandlers;
    private readonly IReadOnlyList<IAsyncAction<TState, T>> onDeactivateHandlers;
    private readonly IReadOnlyList<IAsyncAction<TState, TTransition, TState, T>> onStateChangeHandlers;
    private readonly IReadOnlyList<IAsyncAction<T>> onEntryHandlers;
    private readonly IReadOnlyList<IAsyncAction<T>> onExitHandlers;
    private readonly IReadOnlyList<IParameterizedAction<T>> actions;
    private readonly TransitionTable<TState, TTransition> transitionTable;

    public ParameterizedState(
        TState state,
        IReadOnlyList<IAsyncAction<TState, T>> onActivateHandlers,
        IReadOnlyList<IAsyncAction<TState, T>> onDeactivateHandlers,
        IReadOnlyList<IAsyncAction<TState, TTransition, TState, T>> onStateChangeHandlers,
        IReadOnlyList<IAsyncAction<T>> onEntryHandlers,
        IReadOnlyList<IAsyncAction<T>> onExitHandlers,
        IReadOnlyList<IParameterizedAction<T>> actions,
        IStateMachine<TState, TTransition> stateMachine,
        TransitionTable<TState, TTransition> transitionTable
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
        this.transitionTable = transitionTable;
    }

    /// <inheritdoc />
    public TState StateValue { get; }

    /// <inheritdoc />
    public IReadOnlyList<Type> TypeParameters { get; } = [typeof(T)];

    /// <inheritdoc />
    public IStateMachine<TState, TTransition> StateMachine { get; }

    /// <inheritdoc />
    public IEnumerable<ITransition<TState, TTransition>> Transitions => this.transitionTable;

    /// <inheritdoc />
    public async Task Activate(CancellationToken token)
    {
        var parameter = StateMachine.GetNextParameter<TState, TTransition, T>();
        StateMachine.Tracker?.Activated(this, parameter);
        foreach (var handler in this.onActivateHandlers)
        {
            await StateMachine.RunWithExceptionHandling(() => handler.InvokeAsync(StateValue, parameter, token), token);
        }
    }

    /// <inheritdoc />
    public async Task Deactivate(CancellationToken token)
    {
        var parameter = StateMachine.GetPreviousParameter<TState, TTransition, T>();
        StateMachine.Tracker?.Deactivated(this, parameter);
        foreach (var handler in this.onDeactivateHandlers)
        {
            await StateMachine.RunWithExceptionHandling(() => handler.InvokeAsync(StateValue, parameter, token), token);
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
        var parameter = parameters.GetNextParameter<T>(0);
        return StateChange(previousState, transition, parameter, token);
    }

    /// <inheritdoc />
    public async Task StateChange(TState previousState, TTransition transition, T parameter, CancellationToken token)
    {
        foreach (var handler in this.onStateChangeHandlers)
        {
            await StateMachine.RunWithExceptionHandling(
                () => handler.InvokeAsync(previousState, transition, StateValue, parameter, token),
                token
            );
        }
    }

    /// <inheritdoc />
    public async Task Enter(CancellationToken token)
    {
        var parameter = StateMachine.GetNextParameter<TState, TTransition, T>();
        StateMachine.Tracker?.Entered(this, parameter);
        foreach (var handler in this.onEntryHandlers)
        {
            await StateMachine.RunWithExceptionHandling(() => handler.InvokeAsync(parameter, token), token);
        }
    }

    /// <inheritdoc />
    public async Task Exit(CancellationToken token)
    {
        var parameter = StateMachine.GetPreviousParameter<TState, TTransition, T>();
        StateMachine.Tracker?.Exited(this, parameter);
        foreach (var handler in this.onExitHandlers)
        {
            await StateMachine.RunWithExceptionHandling(() => handler.InvokeAsync(parameter, token), token);
        }
    }

    /// <inheritdoc />
    public async Task Action(CancellationToken token)
    {
        var parameter = StateMachine.GetCurrentParameter<TState, TTransition, T>();
        foreach (var action in this.actions)
        {
            await StateMachine.RunWithExceptionHandling(
                () => action.Invoke(parameter, token),
                throwOnCancellation: false,
                token
            );
        }
    }

    /// <inheritdoc />
    public async Task<ITransition<TState, TTransition>> GetTransition(TTransition transition, CancellationToken token)
    {
        var parameter = StateMachine.GetCurrentParameter<TState, TTransition, T>();
        var result = await this.transitionTable.LookupParameterlessTransition(transition, parameter, token);
        if (result == null)
        {
            throw new InvalidOperationException(
                $"No parameterless transition could be found for: Transition={transition}, Previous={typeof(T)}"
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
        var parameter = StateMachine.GetCurrentParameter<TState, TTransition, T>();
        var result = await this.transitionTable.LookupParameterizedTransition(
            transition,
            parameter,
            nextParameter,
            token
        );
        if (result == null)
        {
            throw new InvalidOperationException(
                $"No parameterized transition could be found for: Transition={transition}, Previous={typeof(T)}, Next={typeof(TNext)}"
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
        var parameter = StateMachine.GetCurrentParameter<TState, TTransition, T>();
        return await this.transitionTable.LookupParameterlessTransition(transition, parameter, token);
    }

    /// <inheritdoc />
    public async Task<ITransition<TState, TTransition>?> GetTransitionOrDefault<TNext>(
        TTransition transition,
        TNext nextParameter,
        CancellationToken token
    )
    {
        var parameter = StateMachine.GetCurrentParameter<TState, TTransition, T>();
        return await this.transitionTable.LookupParameterizedTransition(transition, parameter, nextParameter, token);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"State: {DisplayString}";
    }
}
