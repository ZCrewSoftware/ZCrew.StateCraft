namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata describing a transition condition. Conditions gate whether a transition can be taken.
/// </summary>
/// <remarks>
///     This interface adds no members beyond <see cref="IDelegateInfo"/>. It exists to distinguish condition
///     delegates from other delegate kinds (such as <see cref="IMappingFunctionInfo"/>) when transition info is
///     consumed.
/// </remarks>
/// <seealso cref="IConditionalStateInfo{TState, TTransition}.Conditions"/>
public interface IConditionInfo : IDelegateInfo;
