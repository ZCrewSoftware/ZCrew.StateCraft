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

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T1">The first initial state parameter.</typeparam>
/// <typeparam name="T2">The second initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states with two parameters.
/// </remarks>
internal sealed class StateMachineActivator<TState, TTransition, T1, T2> : IStateMachineActivator<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly bool isValueSet;

    // Only one of these two should be set - either it is already available (with parameters) or needs to be fetched
    private readonly TState? stateValue;
    private readonly T1? parameter1;
    private readonly T2? parameter2;

    private readonly IAsyncFunc<(TState, T1, T2)>? func;

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition,T1,T2}"/> with a predetermined state
    ///     and parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first initial parameter.</param>
    /// <param name="parameter2">The second initial parameter.</param>
    public StateMachineActivator(TState state, T1 parameter1, T2 parameter2)
    {
        this.isValueSet = true;
        this.stateValue = state;
        this.parameter1 = parameter1;
        this.parameter2 = parameter2;
    }

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition,T1,T2}"/> with a function to fetch
    ///     the initial state.
    /// </summary>
    /// <param name="func">The initial state function.</param>
    public StateMachineActivator(IAsyncFunc<(TState, T1, T2)> func)
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
            var parameterizedState = stateMachine.StateTable.LookupState<T1, T2>(this.stateValue!);
            stateMachine.Parameters.SetNextParameters(this.parameter1, this.parameter2);
            stateMachine.NextState = parameterizedState;
        }
        else
        {
            var (fetchedStateValue, fetchedP1, fetchedP2) = await this.func!.InvokeAsync(token);
            var parameterizedState = stateMachine.StateTable.LookupState<T1, T2>(fetchedStateValue);
            stateMachine.Parameters.SetNextParameters(fetchedP1, fetchedP2);
            stateMachine.NextState = parameterizedState;
        }
    }
}

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T1">The first initial state parameter.</typeparam>
/// <typeparam name="T2">The second initial state parameter.</typeparam>
/// <typeparam name="T3">The third initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states with three parameters.
/// </remarks>
internal sealed class StateMachineActivator<TState, TTransition, T1, T2, T3>
    : IStateMachineActivator<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly bool isValueSet;

    // Only one of these two should be set - either it is already available (with parameters) or needs to be fetched
    private readonly TState? stateValue;
    private readonly T1? parameter1;
    private readonly T2? parameter2;
    private readonly T3? parameter3;

    private readonly IAsyncFunc<(TState, T1, T2, T3)>? func;

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition,T1,T2,T3}"/> with a predetermined
    ///     state and parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first initial parameter.</param>
    /// <param name="parameter2">The second initial parameter.</param>
    /// <param name="parameter3">The third initial parameter.</param>
    public StateMachineActivator(TState state, T1 parameter1, T2 parameter2, T3 parameter3)
    {
        this.isValueSet = true;
        this.stateValue = state;
        this.parameter1 = parameter1;
        this.parameter2 = parameter2;
        this.parameter3 = parameter3;
    }

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition,T1,T2,T3}"/> with a function to
    ///     fetch the initial state.
    /// </summary>
    /// <param name="func">The initial state function.</param>
    public StateMachineActivator(IAsyncFunc<(TState, T1, T2, T3)> func)
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
            var parameterizedState = stateMachine.StateTable.LookupState<T1, T2, T3>(this.stateValue!);
            stateMachine.Parameters.SetNextParameters(this.parameter1, this.parameter2, this.parameter3);
            stateMachine.NextState = parameterizedState;
        }
        else
        {
            var (fetchedStateValue, fetchedP1, fetchedP2, fetchedP3) = await this.func!.InvokeAsync(token);
            var parameterizedState = stateMachine.StateTable.LookupState<T1, T2, T3>(fetchedStateValue);
            stateMachine.Parameters.SetNextParameters(fetchedP1, fetchedP2, fetchedP3);
            stateMachine.NextState = parameterizedState;
        }
    }
}

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T1">The first initial state parameter.</typeparam>
/// <typeparam name="T2">The second initial state parameter.</typeparam>
/// <typeparam name="T3">The third initial state parameter.</typeparam>
/// <typeparam name="T4">The fourth initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states with four parameters.
/// </remarks>
internal sealed class StateMachineActivator<TState, TTransition, T1, T2, T3, T4>
    : IStateMachineActivator<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly bool isValueSet;

    // Only one of these two should be set - either it is already available (with parameters) or needs to be fetched
    private readonly TState? stateValue;
    private readonly T1? parameter1;
    private readonly T2? parameter2;
    private readonly T3? parameter3;
    private readonly T4? parameter4;

    private readonly IAsyncFunc<(TState, T1, T2, T3, T4)>? func;

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition,T1,T2,T3,T4}"/> with a predetermined
    ///     state and parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first initial parameter.</param>
    /// <param name="parameter2">The second initial parameter.</param>
    /// <param name="parameter3">The third initial parameter.</param>
    /// <param name="parameter4">The fourth initial parameter.</param>
    public StateMachineActivator(TState state, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
    {
        this.isValueSet = true;
        this.stateValue = state;
        this.parameter1 = parameter1;
        this.parameter2 = parameter2;
        this.parameter3 = parameter3;
        this.parameter4 = parameter4;
    }

    /// <summary>
    ///     Creates a <see cref="StateMachineActivator{TState,TTransition,T1,T2,T3,T4}"/> with a function to
    ///     fetch the initial state.
    /// </summary>
    /// <param name="func">The initial state function.</param>
    public StateMachineActivator(IAsyncFunc<(TState, T1, T2, T3, T4)> func)
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
            var parameterizedState = stateMachine.StateTable.LookupState<T1, T2, T3, T4>(this.stateValue!);
            stateMachine.Parameters.SetNextParameters(
                this.parameter1,
                this.parameter2,
                this.parameter3,
                this.parameter4
            );
            stateMachine.NextState = parameterizedState;
        }
        else
        {
            var (fetchedStateValue, fetchedP1, fetchedP2, fetchedP3, fetchedP4) = await this.func!.InvokeAsync(token);
            var parameterizedState = stateMachine.StateTable.LookupState<T1, T2, T3, T4>(fetchedStateValue);
            stateMachine.Parameters.SetNextParameters(fetchedP1, fetchedP2, fetchedP3, fetchedP4);
            stateMachine.NextState = parameterizedState;
        }
    }
}
