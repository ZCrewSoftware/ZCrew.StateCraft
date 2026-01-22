namespace ZCrew.StateCraft.Actions.Contracts;

/// <summary>
///     Represents an action with a parameter.
/// </summary>
internal interface IParameterizedAction<in T> : IAction
{
    /// <summary>
    ///     Invoke the action. Awaiting this will await the full completion of the action.
    /// </summary>
    /// <param name="parameter">The action parameter.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Invoke(T parameter, CancellationToken token);
}
