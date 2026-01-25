namespace ZCrew.StateCraft;

/// <summary>
///     Represents common functionality to describe a transition configuration, regardless of the parameters or
///     semantics of the transition.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface ITransitionConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The previous state value.
    /// </summary>
    internal TState PreviousStateValue { get; }

    /// <summary>
    ///     The transition value.
    /// </summary>
    internal TTransition TransitionValue { get; }

    /// <summary>
    ///     The next state value. This will be <see langword="null"/> until it is set. <see cref="Build"/> should not
    ///     be called until this value is set.
    /// </summary>
    internal TState? NextStateValue { get; }

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
    ///     Indicates whether the transition is conditional. A conditional transition has one or more conditions that
    ///     must be satisfied for the transition to be taken.
    /// </summary>
    internal bool IsConditional { get; }
}
