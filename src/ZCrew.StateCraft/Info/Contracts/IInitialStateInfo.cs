namespace ZCrew.StateCraft;

/// <summary>
///     Common shape for introspection metadata describing the initial state of a state machine. Variants are
///     <see cref="IStaticInitialStateInfo{TState, TTransition}"/> for an initial state whose value is fixed at
///     configuration time and <see cref="IDynamicInitialStateInfo{TState, TTransition}"/> for one resolved at
///     activation time.
/// </summary>
/// <remarks>
///     The initial state value is not validated against the configured states list when the state machine is built.
///     For <see cref="IStaticInitialStateInfo{TState, TTransition}"/>, validators can verify the value matches a
///     configured <see cref="IStateInfo{TState, TTransition}"/> because the value is known. For
///     <see cref="IDynamicInitialStateInfo{TState, TTransition}"/>, the provider's result can only be checked at
///     runtime when the state machine activates.
/// </remarks>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IInitialStateInfo<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The types of the parameters supplied to the initial state, in declaration order. Empty for a
    ///     parameterless initial state.
    /// </summary>
    IReadOnlyList<Type> InitialParameterTypes { get; }
}
