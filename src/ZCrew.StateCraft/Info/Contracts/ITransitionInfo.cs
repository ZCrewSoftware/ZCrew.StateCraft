namespace ZCrew.StateCraft;

/// <summary>
///     Common shape for introspection metadata describing a transition between states.
/// </summary>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface ITransitionInfo<TTransition>
    where TTransition : notnull
{
    /// <summary>
    ///     The transition value that triggers this transition.
    /// </summary>
    TTransition TransitionValue { get; }

    /// <summary>
    ///     The types of the parameters the caller must supply when invoking this transition. Empty when the
    ///     transition is parameterless.
    /// </summary>
    /// <remarks>
    ///     Always empty for <see cref="IMappedTransitionInfo{TState, TTransition}"/> (parameters are produced by
    ///     the mapping function, not the caller).
    /// </remarks>
    IReadOnlyList<Type> TransitionParameterTypes { get; }

    /// <summary>
    ///     Whether this transition has any conditions gating it. A conditional transition has at least one
    ///     condition that must return <see langword="true"/> for the transition to be taken.
    /// </summary>
    bool IsConditional { get; }
}
