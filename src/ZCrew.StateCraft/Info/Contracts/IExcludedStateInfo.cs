namespace ZCrew.StateCraft;

/// <summary>
///     A state explicitly excluded from the source set of an inverted (<c>From</c>) transition. See
///     <see cref="IFromTransitionInfo{TState, TTransition}.ExcludedStates"/>.
/// </summary>
/// <remarks>
///     An excluded state is identified by its value and parameter types rather than by a reference to an
///     <see cref="IStateInfo{TState}"/> because the excluded state may not be configured on the machine. This lets
///     validators flag exclusions that target a state which does not exist.
/// </remarks>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
public interface IExcludedStateInfo<TState>
    where TState : notnull
{
    /// <summary>
    ///     The excluded state's value.
    /// </summary>
    TState StateValue { get; }

    /// <summary>
    ///     The parameter types of the excluded state, in declaration order. Empty when excluding a parameterless
    ///     state.
    /// </summary>
    IReadOnlyList<Type> StateParameterTypes { get; }
}
