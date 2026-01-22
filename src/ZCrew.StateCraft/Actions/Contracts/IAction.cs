namespace ZCrew.StateCraft.Actions.Contracts;

/// <summary>
///     Represents common functionality to describe an action, regardless of the parameters or semantics of the action.
/// </summary>
internal interface IAction
{
    /// <summary>
    ///     The type parameters of the action.
    /// </summary>
    internal IReadOnlyList<Type> TypeParameters { get; }
}
