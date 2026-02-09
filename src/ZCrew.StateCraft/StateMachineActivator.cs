using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <inheritdoc/>
/// <remarks>
///     Used for parameterless initial states.
/// </remarks>
internal sealed class StateMachineActivator<TState, TTransition> : IStateMachineActivator<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly bool isValueSet;

    // Only one of these two should be set - either it is already available or needs to be fetched
    private readonly TState? stateValue;
    private readonly IAsyncFunc<TState>? func;

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition}"/> with a predetermined state.
    /// </summary>
    /// <param name="state">The initial state.</param>
    public StateMachineActivator(TState state)
    {
        this.isValueSet = true;
        this.stateValue = state;
    }

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition}"/> with a function to fetch the initial
    ///     state.
    /// </summary>
    /// <param name="func">The initial state function.</param>
    public StateMachineActivator(IAsyncFunc<TState> func)
    {
        this.isValueSet = false;
        this.func = func;
    }

    /// <inheritdoc/>
    public async Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        // State is already set and doesn't need to be fetched
        if (this.isValueSet)
        {
            var parameterlessState = stateMachine.StateTable.LookupState(this.stateValue!);
            stateMachine.Parameters.SetEmptyNextParameters();
            stateMachine.NextState = parameterlessState;
        }
        else
        {
            var fetchedStateValue = await this.func!.InvokeAsync(token);
            var parameterlessState = stateMachine.StateTable.LookupState(fetchedStateValue);
            stateMachine.Parameters.SetEmptyNextParameters();
            stateMachine.NextState = parameterlessState;
        }
    }
}

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T">The initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states.
/// </remarks>
internal sealed class StateMachineActivator<TState, TTransition, T> : IStateMachineActivator<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly bool isValueSet;

    // Only one of these two should be set - either it is already available (with a parameter) or needs to be fetched
    private readonly TState? stateValue;
    private readonly T? parameter;

    private readonly IAsyncFunc<(TState, T)>? func;

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition}"/> with a predetermined state and parameter.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter">The initial parameter.</param>
    public StateMachineActivator(TState state, T parameter)
    {
        this.isValueSet = true;
        this.stateValue = state;
        this.parameter = parameter;
    }

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition}"/> with a function to fetch the initial
    ///     state.
    /// </summary>
    /// <param name="func">The initial state function.</param>
    public StateMachineActivator(IAsyncFunc<(TState, T)> func)
    {
        this.isValueSet = false;
        this.func = func;
    }

    /// <inheritdoc/>
    public async Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        // State is already set and doesn't need to be fetched
        if (this.isValueSet)
        {
            var parameterizedState = stateMachine.StateTable.LookupState<T>(this.stateValue!);
            stateMachine.Parameters.SetNextParameter(this.parameter);
            stateMachine.NextState = parameterizedState;
        }
        else
        {
            var (fetchedStateValue, fetchedParameter) = await this.func!.InvokeAsync(token);
            var parameterizedState = stateMachine.StateTable.LookupState<T>(fetchedStateValue);
            stateMachine.Parameters.SetNextParameter(fetchedParameter);
            stateMachine.NextState = parameterizedState;
        }
    }
}
