using ZCrew.StateCraft.Async;
using ZCrew.StateCraft.Info;
using ZCrew.StateCraft.InitialState.Contracts;
using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft.InitialState;

/// <inheritdoc/>
/// <remarks>
///     Used for parameterless initial states fetched from a provider.
/// </remarks>
internal sealed class DynamicInitialStateProvider<TState, TTransition> : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly AsyncStateProvider<TState> provider;

    /// <summary>
    ///     Creates a <see cref="DynamicInitialStateProvider{TState,TTransition}"/> with a function to fetch the
    ///     initial state.
    /// </summary>
    /// <param name="provider">The initial state provider.</param>
    public DynamicInitialStateProvider(AsyncStateProvider<TState> provider)
    {
        this.provider = provider;
    }

    /// <inheritdoc/>
    public async Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var fetchedStateValue = await this.provider.Evaluate(token);
        var parameterlessState = stateMachine.StateTable.LookupState(fetchedStateValue);
        stateMachine.Parameters.SetEmptyNextParameters();
        stateMachine.NextState = parameterlessState;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new DynamicInitialStateInfo<TState, TTransition>(this.provider.Descriptor, []);
    }
}

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T">The initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states fetched from a provider.
/// </remarks>
internal sealed class DynamicInitialStateProvider<TState, TTransition, T> : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly AsyncStateProvider<TState, T> provider;

    /// <summary>
    ///     Creates a <see cref="DynamicInitialStateProvider{TState,TTransition,T}"/> with a function to fetch the
    ///     initial state.
    /// </summary>
    /// <param name="provider">The initial state provider.</param>
    public DynamicInitialStateProvider(AsyncStateProvider<TState, T> provider)
    {
        this.provider = provider;
    }

    /// <inheritdoc/>
    public async Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var (fetchedStateValue, fetchedParameter) = await this.provider.Evaluate(token);
        var parameterizedState = stateMachine.StateTable.LookupState<T>(fetchedStateValue);
        stateMachine.Parameters.SetNextParameter(fetchedParameter);
        stateMachine.NextState = parameterizedState;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new DynamicInitialStateInfo<TState, TTransition>(this.provider.Descriptor, [typeof(T)]);
    }
}

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T1">The first initial state parameter.</typeparam>
/// <typeparam name="T2">The second initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states with two parameters fetched from a provider.
/// </remarks>
internal sealed class DynamicInitialStateProvider<TState, TTransition, T1, T2>
    : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly AsyncStateProvider<TState, T1, T2> provider;

    /// <summary>
    ///     Creates a <see cref="DynamicInitialStateProvider{TState,TTransition,T1,T2}"/> with a function to fetch
    ///     the initial state.
    /// </summary>
    /// <param name="provider">The initial state provider.</param>
    public DynamicInitialStateProvider(AsyncStateProvider<TState, T1, T2> provider)
    {
        this.provider = provider;
    }

    /// <inheritdoc/>
    public async Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var (fetchedStateValue, fetchedP1, fetchedP2) = await this.provider.Evaluate(token);
        var parameterizedState = stateMachine.StateTable.LookupState<T1, T2>(fetchedStateValue);
        stateMachine.Parameters.SetNextParameters(fetchedP1, fetchedP2);
        stateMachine.NextState = parameterizedState;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new DynamicInitialStateInfo<TState, TTransition>(this.provider.Descriptor, [typeof(T1), typeof(T2)]);
    }
}

/// <inheritdoc/>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
/// <typeparam name="T1">The first initial state parameter.</typeparam>
/// <typeparam name="T2">The second initial state parameter.</typeparam>
/// <typeparam name="T3">The third initial state parameter.</typeparam>
/// <remarks>
///     Used for parameterized initial states with three parameters fetched from a provider.
/// </remarks>
internal sealed class DynamicInitialStateProvider<TState, TTransition, T1, T2, T3>
    : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly AsyncStateProvider<TState, T1, T2, T3> provider;

    /// <summary>
    ///     Creates a <see cref="DynamicInitialStateProvider{TState,TTransition,T1,T2,T3}"/> with a function to
    ///     fetch the initial state.
    /// </summary>
    /// <param name="provider">The initial state provider.</param>
    public DynamicInitialStateProvider(AsyncStateProvider<TState, T1, T2, T3> provider)
    {
        this.provider = provider;
    }

    /// <inheritdoc/>
    public async Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var (fetchedStateValue, fetchedP1, fetchedP2, fetchedP3) = await this.provider.Evaluate(token);
        var parameterizedState = stateMachine.StateTable.LookupState<T1, T2, T3>(fetchedStateValue);
        stateMachine.Parameters.SetNextParameters(fetchedP1, fetchedP2, fetchedP3);
        stateMachine.NextState = parameterizedState;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new DynamicInitialStateInfo<TState, TTransition>(
            this.provider.Descriptor,
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
///     Used for parameterized initial states with four parameters fetched from a provider.
/// </remarks>
internal sealed class DynamicInitialStateProvider<TState, TTransition, T1, T2, T3, T4>
    : IInitialStateProvider<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    private readonly AsyncStateProvider<TState, T1, T2, T3, T4> provider;

    /// <summary>
    ///     Creates a <see cref="DynamicInitialStateProvider{TState,TTransition,T1,T2,T3,T4}"/> with a function to
    ///     fetch the initial state.
    /// </summary>
    /// <param name="provider">The initial state provider.</param>
    public DynamicInitialStateProvider(AsyncStateProvider<TState, T1, T2, T3, T4> provider)
    {
        this.provider = provider;
    }

    /// <inheritdoc/>
    public async Task Activate(IStateMachine<TState, TTransition> stateMachine, CancellationToken token)
    {
        var (fetchedStateValue, fetchedP1, fetchedP2, fetchedP3, fetchedP4) = await this.provider.Evaluate(token);
        var parameterizedState = stateMachine.StateTable.LookupState<T1, T2, T3, T4>(fetchedStateValue);
        stateMachine.Parameters.SetNextParameters(fetchedP1, fetchedP2, fetchedP3, fetchedP4);
        stateMachine.NextState = parameterizedState;
    }

    /// <inheritdoc/>
    public IInitialStateInfo<TState, TTransition> GetInfo()
    {
        return new DynamicInitialStateInfo<TState, TTransition>(
            this.provider.Descriptor,
            [typeof(T1), typeof(T2), typeof(T3), typeof(T4)]
        );
    }
}
