namespace ZCrew.StateCraft;

/// <summary>
///     Represents common functionality to describe a transition, regardless of the parameters or semantics of the
///     transition.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
internal interface ITransition<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The previous state value.
    /// </summary>
    TState PreviousStateValue { get; }

    /// <summary>
    ///     The previous state and owner of this transition.
    /// </summary>
    IState<TState, TTransition> PreviousState { get; }

    /// <summary>
    ///     The transition value.
    /// </summary>
    TTransition TransitionValue { get; }

    /// <summary>
    ///     The next state value.
    /// </summary>
    TState NextStateValue { get; }

    /// <summary>
    ///     The next state.
    /// </summary>
    IState<TState, TTransition> NextState { get; }

    /// <summary>
    ///     The type parameters of the previous state. Empty if the previous state has no parameters.
    /// </summary>
    internal IReadOnlyList<Type> PreviousStateTypeParameters { get; }

    /// <summary>
    ///     The type parameters of the transition. Empty if the transition can be invoked without providing a parameter.
    /// </summary>
    internal IReadOnlyList<Type> TransitionTypeParameters { get; }

    /// <summary>
    ///     The type parameters of the next state. Empty if the next state has no parameters.
    /// </summary>
    internal IReadOnlyList<Type> NextStateTypeParameters { get; }

    /// <summary>
    ///     Performs the transition by invoking the state machine's state change handlers and the next state's state
    ///     change handlers.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Transition(CancellationToken token);
}
