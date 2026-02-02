namespace ZCrew.StateCraft;

/// <summary>
///     Configures a state with a single parameter.
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
/// <example>
///     To create a state with an on-entry and on-exit action:
///     <code>
///     StateMachine
///         .Configure&lt;State, Transition&gt;()
///         .WithState(State.Open, state => state
///             .WithParameter&lt;string&gt;()
///             .OnEntry(arg => Console.WriteLine($"Entering 'Open' state with {arg}"))
///             .OnExit(arg => Console.WriteLine($"Exiting 'Open' state with {arg}")));
///     </code>
/// </example>
public interface IParameterizedStateConfiguration<TState, TTransition, T> : IStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when the state machine is activated
    ///     with this state as the initial state. This is only called during activation, not during transitions.
    /// </summary>
    /// <param name="handler">The delegate to call when the state machine is activated.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnActivate(Action<TState, T> handler);

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when the state machine is activated
    ///     with this state as the initial state. This is only called during activation, not during transitions.
    /// </summary>
    /// <param name="handler">The delegate to call when the state machine is activated.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnActivate(
        Func<TState, T, CancellationToken, Task> handler
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when the state machine is activated
    ///     with this state as the initial state. This is only called during activation, not during transitions.
    /// </summary>
    /// <param name="handler">The delegate to call when the state machine is activated.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnActivate(
        Func<TState, T, CancellationToken, ValueTask> handler
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when the state machine is deactivated
    ///     with this state as the current state. This is only called during deactivation, not during transitions.
    /// </summary>
    /// <param name="handler">The delegate to call when the state machine is deactivated.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnDeactivate(Action<TState, T> handler);

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when the state machine is deactivated
    ///     with this state as the current state. This is only called during deactivation, not during transitions.
    /// </summary>
    /// <param name="handler">The delegate to call when the state machine is deactivated.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnDeactivate(
        Func<TState, T, CancellationToken, Task> handler
    );

    /// <summary>
    ///     Configures a <paramref name="handler"/> delegate which will be called when the state machine is deactivated
    ///     with this state as the current state. This is only called during deactivation, not during transitions.
    /// </summary>
    /// <param name="handler">The delegate to call when the state machine is deactivated.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnDeactivate(
        Func<TState, T, CancellationToken, ValueTask> handler
    );

    /// <summary>
    ///     <para>
    ///     Configures a <see cref="handler"/> delegate which will be called when the state changes. The parameters to
    ///     the <paramref name="handler"/> are: the previous state, the transition, the next state, and the transition
    ///     parameter.
    ///     </para>
    ///     <para>
    ///     Since this is configured for a specific state, the 'next state' will always be 'this state'.
    ///     </para>
    /// </summary>
    /// <param name="handler">The delegate to call as a state is changed.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnStateChange(
        Action<TState, TTransition, TState, T> handler
    );

    /// <summary>
    ///     <para>
    ///     Configures a <see cref="handler"/> delegate which will be called when the state changes. The parameters to
    ///     the <paramref name="handler"/> are: the previous state, the transition, the next state, the transition
    ///     parameter, and a token to monitor for cancellation.
    ///     </para>
    ///     <para>
    ///     Since this is configured for a specific state, the 'next state' will always be 'this state'.
    ///     </para>
    /// </summary>
    /// <param name="handler">The delegate to call as a state is changed.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnStateChange(
        Func<TState, TTransition, TState, T, CancellationToken, Task> handler
    );

    /// <summary>
    ///     <para>
    ///     Configures a <see cref="handler"/> delegate which will be called when the state changes. The parameters to
    ///     the <paramref name="handler"/> are: the previous state, the transition, the next state, the transition
    ///     parameter, and a token to monitor for cancellation.
    ///     parameter.
    ///     </para>
    ///     <para>
    ///     Since this is configured for a specific state, the 'next state' will always be 'this state'.
    ///     </para>
    /// </summary>
    /// <param name="handler">The delegate to call as a state is changed.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnStateChange(
        Func<TState, TTransition, TState, T, CancellationToken, ValueTask> handler
    );

    /// <summary>
    ///     Configures a <see cref="handler"/> delegate which will be called when this state is entered. This action can
    ///     not be interrupted by other state changes and should not perform state changes.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is entered.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnEntry(Action<T> handler);

    /// <summary>
    ///     Configures a <see cref="handler"/> delegate which will be called when this state is entered. This action can
    ///     not be interrupted by other state changes and should not perform state changes.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is entered.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnEntry(Func<T, CancellationToken, Task> handler);

    /// <summary>
    ///     Configures a <see cref="handler"/> delegate which will be called when this state is entered. This action can
    ///     not be interrupted by other state changes and should not perform state changes.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is entered.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnEntry(Func<T, CancellationToken, ValueTask> handler);

    /// <summary>
    ///     Configures a <see cref="handler"/> delegate which will be called when this state is exited. This action can
    ///     not be interrupted by other state changes and should not perform state changes.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is exited.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnExit(Action<T> handler);

    /// <summary>
    ///     Configures a <see cref="handler"/> delegate which will be called when this state is exited. This action can
    ///     not be interrupted by other state changes and should not perform state changes.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is exited.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnExit(Func<T, CancellationToken, Task> handler);

    /// <summary>
    ///     Configures a <see cref="handler"/> delegate which will be called when this state is exited. This action can
    ///     not be interrupted by other state changes and should not perform state changes.
    /// </summary>
    /// <param name="handler">The delegate to call as a state is exited.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> OnExit(Func<T, CancellationToken, ValueTask> handler);

    /// <summary>
    ///     Configures a new action representing the main functionality of the state. This action can be interrupted by
    ///     other state changes and can perform state changes.
    /// </summary>
    /// <param name="configureAction">The delegate to call representing the primary function of the state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> WithAction(
        Func<IInitialParameterizedActionConfiguration<T>, IFinalParameterizedActionConfiguration<T>> configureAction
    );

    /// <summary>
    ///     Configures a new transition for this state.
    /// </summary>
    /// <param name="transition">The transition to configure.</param>
    /// <param name="configureTransition">The configuration setup.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> WithTransition(
        TTransition transition,
        Func<
            IInitialTransitionConfiguration<TState, TTransition, T>,
            ITransitionConfiguration<TState, TTransition>
        > configureTransition
    );

    /// <summary>
    ///     Configures a new transition for this state that keeps the same parameter type. This is a shortcut for
    ///     <c>.WithTransition(transition, t =&gt; t.WithSameParameter().To(to))</c>.
    /// </summary>
    /// <param name="transition">The transition to configure.</param>
    /// <param name="to">The target state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> WithTransition(TTransition transition, TState to)
    {
        return WithTransition(transition, t => t.WithSameParameter().To(to));
    }

    /// <summary>
    ///     Configures a new transition for this state with a different parameter type. This is a shortcut for
    ///     <c>.WithTransition(transition, t =&gt; t.WithParameter&lt;TNext&gt;().To(to))</c>.
    /// </summary>
    /// <typeparam name="TNext">The type of the parameter for the target state.</typeparam>
    /// <param name="transition">The transition to configure.</param>
    /// <param name="to">The target state.</param>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> WithTransition<TNext>(TTransition transition, TState to)
    {
        return WithTransition(transition, t => t.WithParameter<TNext>().To(to));
    }
}
