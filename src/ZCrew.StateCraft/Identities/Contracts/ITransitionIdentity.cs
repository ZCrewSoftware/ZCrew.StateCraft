using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Represents the identity of a <see cref="ITransition{TState,TTransition}"/>,
///     <see cref="ITransitionConfiguration{TState,TTransition}"/>, <see cref="ITransitionInfo{TState,TTransition}"/>,
///     etc.
/// </summary>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface ITransitionIdentity<TTransition> : IIdentity
    where TTransition : notnull
{
    /// <summary>
    ///     The transition value.
    /// </summary>
    TTransition TransitionValue { get; }

    /// <summary>
    ///     The types of the parameters the caller must supply when invoking this transition. Empty when the
    ///     transition is parameterless (the user supplies no parameters). This may be empty even when transitioning to
    ///     a parameterized state if there are no transition parameters.
    /// </summary>
    IReadOnlyList<Type> TransitionParameterTypes { get; }
}
