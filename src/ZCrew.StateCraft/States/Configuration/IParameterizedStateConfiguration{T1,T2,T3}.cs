namespace ZCrew.StateCraft;

/// <summary>
///     Configures a state with three parameters.
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
/// <typeparam name="T3">The type of the third parameter for this state.</typeparam>
public interface IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3>
    : IStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnActivate(Action{TState})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnActivate(Action<TState, T1, T2, T3> handler);

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnActivate(Func{TState,CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnActivate(
        Func<TState, T1, T2, T3, CancellationToken, Task> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnActivate(Func{TState,CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnActivate(
        Func<TState, T1, T2, T3, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnDeactivate(Action{TState})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnDeactivate(Action<TState, T1, T2, T3> handler);

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnDeactivate(Func{TState,CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnDeactivate(
        Func<TState, T1, T2, T3, CancellationToken, Task> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnDeactivate(Func{TState,CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnDeactivate(
        Func<TState, T1, T2, T3, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnStateChange(Action{TState,TTransition,TState})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnStateChange(
        Action<TState, TTransition, TState, T1, T2, T3> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnStateChange(Func{TState,TTransition,TState,CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnStateChange(
        Func<TState, TTransition, TState, T1, T2, T3, CancellationToken, Task> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnStateChange(Func{TState,TTransition,TState,CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnStateChange(
        Func<TState, TTransition, TState, T1, T2, T3, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnEntry(Action)"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnEntry(Action<T1, T2, T3> handler);

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnEntry(Func{CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnEntry(
        Func<T1, T2, T3, CancellationToken, Task> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnEntry(Func{CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnEntry(
        Func<T1, T2, T3, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnExit(Action)"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnExit(Action<T1, T2, T3> handler);

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnExit(Func{CancellationToken,Task})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnExit(
        Func<T1, T2, T3, CancellationToken, Task> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.OnExit(Func{CancellationToken,ValueTask})"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> OnExit(
        Func<T1, T2, T3, CancellationToken, ValueTask> handler
    );

    /// <inheritdoc cref="IParameterlessStateConfiguration{TState,TTransition}.WithAction"/>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithAction(
        Func<IInitialActionConfiguration<T1, T2, T3>, IActionConfiguration<T1, T2, T3>> configureAction
    );

    /// <summary>
    ///     Configures a new transition for this state.
    /// </summary>
    /// <param name="transition">The transition to configure.</param>
    /// <param name="configureTransition">The configuration setup.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithTransition(
        TTransition transition,
        Func<
            IInitialTransitionConfiguration<TState, TTransition, T1, T2, T3>,
            ITransitionConfiguration<TState, TTransition>
        > configureTransition
    );

    /// <summary>
    ///     Configures a new transition for this state that keeps the same parameter types. This is a shortcut for
    ///     <c>.WithTransition(transition, t =&gt; t.WithSameParameters().To(to))</c>.
    /// </summary>
    /// <param name="transition">The transition to configure.</param>
    /// <param name="to">The target state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithTransition(TTransition transition, TState to)
    {
        return WithTransition(transition, t => t.WithSameParameters().To(to));
    }

    /// <summary>
    ///     Configures a new single-parameter transition for this state. This is a shortcut for
    ///     <c>.WithTransition(transition, t =&gt; t.WithParameter&lt;TNext&gt;().To(to))</c>.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the target state.</typeparam>
    /// <param name="transition">The transition to configure.</param>
    /// <param name="to">The target state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithTransition<TNext>(
        TTransition transition,
        TState to
    )
    {
        return WithTransition(transition, t => t.WithParameter<TNext>().To(to));
    }

    /// <summary>
    ///     Configures a new two-parameter transition for this state. This is a shortcut for
    ///     <c>.WithTransition(transition, t =&gt; t.WithParameters&lt;TNext1, TNext2&gt;().To(to))</c>.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the target state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the target state.</typeparam>
    /// <param name="transition">The transition to configure.</param>
    /// <param name="to">The target state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithTransition<TNext1, TNext2>(
        TTransition transition,
        TState to
    )
    {
        return WithTransition(transition, t => t.WithParameters<TNext1, TNext2>().To(to));
    }

    /// <summary>
    ///     Configures a new three-parameter transition for this state. This is a shortcut for
    ///     <c>.WithTransition(transition, t =&gt; t.WithParameters&lt;TNext1, TNext2, TNext3&gt;().To(to))</c>.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the target state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the target state.</typeparam>
    /// <typeparam name="TNext3">The type of the third parameter for the target state.</typeparam>
    /// <param name="transition">The transition to configure.</param>
    /// <param name="to">The target state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithTransition<TNext1, TNext2, TNext3>(
        TTransition transition,
        TState to
    )
    {
        return WithTransition(transition, t => t.WithParameters<TNext1, TNext2, TNext3>().To(to));
    }

    /// <summary>
    ///     Configures a new four-parameter transition for this state. This is a shortcut for
    ///     <c>.WithTransition(transition, t =&gt; t.WithParameters&lt;TNext1, TNext2, TNext3, TNext4&gt;().To(to))</c>.
    /// </summary>
    /// <typeparam name="TNext1">The type of the first parameter for the target state.</typeparam>
    /// <typeparam name="TNext2">The type of the second parameter for the target state.</typeparam>
    /// <typeparam name="TNext3">The type of the third parameter for the target state.</typeparam>
    /// <typeparam name="TNext4">The type of the fourth parameter for the target state.</typeparam>
    /// <param name="transition">The transition to configure.</param>
    /// <param name="to">The target state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithTransition<TNext1, TNext2, TNext3, TNext4>(
        TTransition transition,
        TState to
    )
    {
        return WithTransition(transition, t => t.WithParameters<TNext1, TNext2, TNext3, TNext4>().To(to));
    }
}
