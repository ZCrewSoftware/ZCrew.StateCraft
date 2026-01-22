namespace ZCrew.StateCraft.Actions.Contracts;

/// <summary>
///     Represents an action with no parameters.
/// </summary>
internal interface IParameterlessAction : IAction
{
    /// <summary>
    ///     Invoke the action. Awaiting this will await the full completion of the action.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Invoke(CancellationToken token);
}
