using System.Runtime.CompilerServices;
using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Extensions;
using ZCrew.StateCraft.InitialState;
using ZCrew.StateCraft.StateMachines;

namespace ZCrew.StateCraft;

/// <inheritdoc/>
internal sealed class InitialStateMachineConfiguration<TState, TTransition>
    : IInitialStateMachineConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState(TState state)
    {
        var initialStateProvider = new StaticInitialStateProvider<TState, TTransition>(state);
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<TState> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<CancellationToken, Task<TState>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState(
        Func<CancellationToken, ValueTask<TState>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(TState state, T parameter)
    {
        var initialStateProvider = new StaticInitialStateProvider<TState, TTransition, T>(state, parameter);
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<(TState, T)> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<CancellationToken, Task<(TState, T)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T>(
        Func<CancellationToken, ValueTask<(TState, T)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        TState state,
        T1 parameter1,
        T2 parameter2
    )
    {
        var initialStateProvider = new StaticInitialStateProvider<TState, TTransition, T1, T2>(
            state,
            parameter1,
            parameter2
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<(TState, T1, T2)> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T1, T2>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<CancellationToken, Task<(TState, T1, T2)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T1, T2>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2>(
        Func<CancellationToken, ValueTask<(TState, T1, T2)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T1, T2>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        TState state,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3
    )
    {
        var initialStateProvider = new StaticInitialStateProvider<TState, TTransition, T1, T2, T3>(
            state,
            parameter1,
            parameter2,
            parameter3
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<(TState, T1, T2, T3)> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T1, T2, T3>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<CancellationToken, Task<(TState, T1, T2, T3)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T1, T2, T3>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3>(
        Func<CancellationToken, ValueTask<(TState, T1, T2, T3)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T1, T2, T3>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        TState state,
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4
    )
    {
        var initialStateProvider = new StaticInitialStateProvider<TState, TTransition, T1, T2, T3, T4>(
            state,
            parameter1,
            parameter2,
            parameter3,
            parameter4
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<(TState, T1, T2, T3, T4)> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T1, T2, T3, T4>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<CancellationToken, Task<(TState, T1, T2, T3, T4)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T1, T2, T3, T4>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }

    /// <inheritdoc/>
    public IStateMachineConfiguration<TState, TTransition> WithInitialState<T1, T2, T3, T4>(
        Func<CancellationToken, ValueTask<(TState, T1, T2, T3, T4)>> stateProvider,
        [CallerArgumentExpression(nameof(stateProvider))] string? descriptor = null
    )
    {
        var initialStateProvider = new DynamicInitialStateProvider<TState, TTransition, T1, T2, T3, T4>(
            stateProvider.AsAsyncFunc().AsAsyncStateProvider(descriptor)
        );
        return new StateMachineConfiguration<TState, TTransition>(initialStateProvider);
    }
}
