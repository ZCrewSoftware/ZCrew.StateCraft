namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata for an initial state whose value is fixed at configuration time. The state value and
///     any parameters were supplied directly when configuring the state machine's initial state.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
public interface IStaticInitialStateInfo<TState> : IInitialStateInfo<TState>
    where TState : notnull
{
    /// <summary>
    ///     The state value the machine will start in. Exposed as <typeparamref name="TState"/> rather than as a
    ///     navigable <see cref="IStateInfo{TState}"/> because the value is not guaranteed to match a configured
    ///     state.
    /// </summary>
    TState InitialStateValue { get; }

    /// <summary>
    ///     The parameter values that will be supplied to the initial state, in declaration order. Empty for a
    ///     parameterless initial state.
    /// </summary>
    IReadOnlyList<object?> InitialParameters { get; }
}
