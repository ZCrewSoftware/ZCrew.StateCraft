namespace ZCrew.StateCraft;

/// <summary>
///     Represents the identity of a <see cref="IState{TState,TTransition}"/>,
///     <see cref="IStateConfiguration{TState,TTransition}"/>, <see cref="IStateInfo{TState,TTransition}"/>, etc.
/// </summary>
/// <remarks>
///     Two states with the same <see cref="StateValue"/> but different <see cref="StateParameterTypes"/> are
///     distinct configured states.
/// </remarks>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
public interface IStateIdentity<TState> : IIdentity
    where TState : notnull
{
    /// <summary>
    ///     The state value.
    /// </summary>
    TState StateValue { get; }

    /// <summary>
    ///     The parameter types declared on this state, in declaration order. Empty for a parameterless state.
    /// </summary>
    IReadOnlyList<Type> StateParameterTypes { get; }
}
