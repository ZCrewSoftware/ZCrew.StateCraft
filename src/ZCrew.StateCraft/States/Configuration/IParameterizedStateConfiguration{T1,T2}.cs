namespace ZCrew.StateCraft;

/// <summary>
///     Configures a state with two parameters.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
/// <typeparam name="T1">The type of the first parameter for this state.</typeparam>
/// <typeparam name="T2">The type of the second parameter for this state.</typeparam>
public interface IParameterizedStateConfiguration<TState, TTransition, T1, T2>
    : IStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnActivate(Action{TState})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnActivate(Action<TState, T1, T2> handler);

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnActivate(Func{TState,CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnActivate(
        Func<TState, T1, T2, CancellationToken, Task> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnActivate(Func{TState,CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnActivate(
        Func<TState, T1, T2, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnDeactivate(Action{TState})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnDeactivate(Action<TState, T1, T2> handler);

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnDeactivate(Func{TState,CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnDeactivate(
        Func<TState, T1, T2, CancellationToken, Task> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnDeactivate(Func{TState,CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnDeactivate(
        Func<TState, T1, T2, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnStateChange(Action{TState,TTransition,TState})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnStateChange(
        Action<TState, TTransition, TState, T1, T2> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnStateChange(Func{TState,TTransition,TState,CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnStateChange(
        Func<TState, TTransition, TState, T1, T2, CancellationToken, Task> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnStateChange(Func{TState,TTransition,TState,CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnStateChange(
        Func<TState, TTransition, TState, T1, T2, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnEntry(Action)"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnEntry(Action<T1, T2> handler);

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnEntry(Func{CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnEntry(
        Func<T1, T2, CancellationToken, Task> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnEntry(Func{CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnEntry(
        Func<T1, T2, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnExit(Action)"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnExit(Action<T1, T2> handler);

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnExit(Func{CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnExit(Func<T1, T2, CancellationToken, Task> handler);

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnExit(Func{CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> OnExit(
        Func<T1, T2, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.WithAction"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> WithAction(
        Func<IInitialActionConfiguration<T1, T2>, IActionConfiguration<T1, T2>> configureAction
    );
}
