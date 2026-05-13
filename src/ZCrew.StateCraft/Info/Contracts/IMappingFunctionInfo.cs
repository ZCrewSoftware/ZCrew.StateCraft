namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata describing the mapping function used by a mapped transition (see
///     <see cref="IMappedTransitionInfo{TState, TTransition}.MappingFunction"/>). A mapping function transforms the
///     source state's parameters into the next state's parameters so that the caller does not need to supply them
///     when invoking the transition.
/// </summary>
public interface IMappingFunctionInfo : IDelegateInfo
{
    /// <summary>
    ///     The types of the values produced by the mapping function — the parameter types that will be supplied to
    ///     the next state.
    /// </summary>
    IReadOnlyList<Type> ResultTypes { get; }
}
