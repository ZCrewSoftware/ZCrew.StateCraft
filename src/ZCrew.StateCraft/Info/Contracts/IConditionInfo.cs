namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata describing a transition condition. Conditions gate whether a transition can be taken;
///     see <see cref="IDirectTransitionInfo{TState, TTransition}.PreviousParameterConditions"/> and similar members
///     for where they are surfaced.
/// </summary>
/// <remarks>
///     This interface adds no members beyond <see cref="IDelegateInfo"/>. It exists to distinguish condition
///     delegates from other delegate kinds (such as <see cref="IMappingFunctionInfo"/>) when transition info is
///     consumed.
/// </remarks>
public interface IConditionInfo : IDelegateInfo;
