using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.InitialState.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.InitialState;

/// <inheritdoc/>
/// <remarks>
///     Used for parameterless initial states with a predetermined value.
/// </remarks>
internal sealed class StaticInitialStateProvider<TState, TTransition> : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TState stateValue;

    /// <summary>
    ///     Creates a <see cref="StaticInitialStateProvider{TState,TTransition}"/> with a predetermined state.
    /// </summary>
    /// <param name="state">The initial state.</param>
    public StaticInitialStateProvider(TState state)
    {
        this.stateValue = state;
    }

    /// <inheritdoc/>
    public Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var parameterlessState = stateMachine.StateTable.LookupState(this.stateValue);
        stateMachine.Parameters.SetEmptyNextParameters();
        stateMachine.NextState = parameterlessState;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new StaticInitialStateInfo<TState, TTransition>(this.stateValue, [], []);
    }
}

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T">The initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states with a predetermined value.
/// </remarks>
internal sealed class StaticInitialStateProvider<TState, TTransition, T> : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TState stateValue;
    private readonly T parameter;

    /// <summary>
    ///     Creates a <see cref="StaticInitialStateProvider{TState,TTransition,T}"/> with a predetermined state and
    ///     parameter.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter">The initial parameter.</param>
    public StaticInitialStateProvider(TState state, T parameter)
    {
        this.stateValue = state;
        this.parameter = parameter;
    }

    /// <inheritdoc/>
    public Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var parameterizedState = stateMachine.StateTable.LookupState<T>(this.stateValue);
        stateMachine.Parameters.SetNextParameter(this.parameter);
        stateMachine.NextState = parameterizedState;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new StaticInitialStateInfo<TState, TTransition>(this.stateValue, [this.parameter], [typeof(T)]);
    }
}

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T1">The first initial state parameter.</typeparam>
/// <typeparam name="T2">The second initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states with two predetermined parameters.
/// </remarks>
internal sealed class StaticInitialStateProvider<TState, TTransition, T1, T2>
    : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TState stateValue;
    private readonly T1 parameter1;
    private readonly T2 parameter2;

    /// <summary>
    ///     Creates a <see cref="StaticInitialStateProvider{TState,TTransition,T1,T2}"/> with a predetermined state
    ///     and parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first initial parameter.</param>
    /// <param name="parameter2">The second initial parameter.</param>
    public StaticInitialStateProvider(TState state, T1 parameter1, T2 parameter2)
    {
        this.stateValue = state;
        this.parameter1 = parameter1;
        this.parameter2 = parameter2;
    }

    /// <inheritdoc/>
    public Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var parameterizedState = stateMachine.StateTable.LookupState<T1, T2>(this.stateValue);
        stateMachine.Parameters.SetNextParameters(this.parameter1, this.parameter2);
        stateMachine.NextState = parameterizedState;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new StaticInitialStateInfo<TState, TTransition>(
            this.stateValue,
            [this.parameter1, this.parameter2],
            [typeof(T1), typeof(T2)]
        );
    }
}

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T1">The first initial state parameter.</typeparam>
/// <typeparam name="T2">The second initial state parameter.</typeparam>
/// <typeparam name="T3">The third initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states with three predetermined parameters.
/// </remarks>
internal sealed class StaticInitialStateProvider<TState, TTransition, T1, T2, T3>
    : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TState stateValue;
    private readonly T1 parameter1;
    private readonly T2 parameter2;
    private readonly T3 parameter3;

    /// <summary>
    ///     Creates a <see cref="StaticInitialStateProvider{TState,TTransition,T1,T2,T3}"/> with a predetermined
    ///     state and parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first initial parameter.</param>
    /// <param name="parameter2">The second initial parameter.</param>
    /// <param name="parameter3">The third initial parameter.</param>
    public StaticInitialStateProvider(TState state, T1 parameter1, T2 parameter2, T3 parameter3)
    {
        this.stateValue = state;
        this.parameter1 = parameter1;
        this.parameter2 = parameter2;
        this.parameter3 = parameter3;
    }

    /// <inheritdoc/>
    public Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var parameterizedState = stateMachine.StateTable.LookupState<T1, T2, T3>(this.stateValue);
        stateMachine.Parameters.SetNextParameters(this.parameter1, this.parameter2, this.parameter3);
        stateMachine.NextState = parameterizedState;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new StaticInitialStateInfo<TState, TTransition>(
            this.stateValue,
            [this.parameter1, this.parameter2, this.parameter3],
            [typeof(T1), typeof(T2), typeof(T3)]
        );
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
///     Used for parameterized initial states with four predetermined parameters.
/// </remarks>
internal sealed class StaticInitialStateProvider<TState, TTransition, T1, T2, T3, T4>
    : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly TState stateValue;
    private readonly T1 parameter1;
    private readonly T2 parameter2;
    private readonly T3 parameter3;
    private readonly T4 parameter4;

    /// <summary>
    ///     Creates a <see cref="StaticInitialStateProvider{TState,TTransition,T1,T2,T3,T4}"/> with a predetermined
    ///     state and parameters.
    /// </summary>
    /// <param name="state">The initial state.</param>
    /// <param name="parameter1">The first initial parameter.</param>
    /// <param name="parameter2">The second initial parameter.</param>
    /// <param name="parameter3">The third initial parameter.</param>
    /// <param name="parameter4">The fourth initial parameter.</param>
    public StaticInitialStateProvider(TState state, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
    {
        this.stateValue = state;
        this.parameter1 = parameter1;
        this.parameter2 = parameter2;
        this.parameter3 = parameter3;
        this.parameter4 = parameter4;
    }

    /// <inheritdoc/>
    public Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var parameterizedState = stateMachine.StateTable.LookupState<T1, T2, T3, T4>(this.stateValue);
        stateMachine.Parameters.SetNextParameters(this.parameter1, this.parameter2, this.parameter3, this.parameter4);
        stateMachine.NextState = parameterizedState;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new StaticInitialStateInfo<TState, TTransition>(
            this.stateValue,
            [this.parameter1, this.parameter2, this.parameter3, this.parameter4],
            [typeof(T1), typeof(T2), typeof(T3), typeof(T4)]
        );
    }
}
