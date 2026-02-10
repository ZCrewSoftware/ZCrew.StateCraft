namespace ZCrew.StateCraft.Actions.Contracts;

/// <summary>
///     Represents common functionality to describe an action, regardless of the parameters or semantics of the action.
/// </summary>
internal interface IActionBase
{
    /// <summary>
    ///     The type parameters of the action.
    /// </summary>
    internal IReadOnlyList<Type> TypeParameters { get; }
}

/// <summary>
///     Represents an action with no parameters.
/// </summary>
internal interface IAction : IActionBase
{
    /// <summary>
    ///     Invoke the action. Awaiting this will await the full completion of the action.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Invoke(CancellationToken token);
}

/// <summary>
///     Represents an action with a parameter.
/// </summary>
internal interface IAction<in T> : IActionBase
{
    /// <summary>
    ///     Invoke the action. Awaiting this will await the full completion of the action.
    /// </summary>
    /// <param name="parameter">The action parameter.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Invoke(T parameter, CancellationToken token);
}

/// <summary>
///     Represents an action with two parameters.
/// </summary>
internal interface IAction<in T1, in T2> : IActionBase
{
    /// <summary>
    ///     Invoke the action. Awaiting this will await the full completion of the action.
    /// </summary>
    /// <param name="parameter1">The first action parameter.</param>
    /// <param name="parameter2">The second action parameter.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Invoke(T1 parameter1, T2 parameter2, CancellationToken token);
}

/// <summary>
///     Represents an action with three parameters.
/// </summary>
internal interface IAction<in T1, in T2, in T3> : IActionBase
{
    /// <summary>
    ///     Invoke the action. Awaiting this will await the full completion of the action.
    /// </summary>
    /// <param name="parameter1">The first action parameter.</param>
    /// <param name="parameter2">The second action parameter.</param>
    /// <param name="parameter3">The third action parameter.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Invoke(T1 parameter1, T2 parameter2, T3 parameter3, CancellationToken token);
}

/// <summary>
///     Represents an action with four parameters.
/// </summary>
internal interface IAction<in T1, in T2, in T3, in T4> : IActionBase
{
    /// <summary>
    ///     Invoke the action. Awaiting this will await the full completion of the action.
    /// </summary>
    /// <param name="parameter1">The first action parameter.</param>
    /// <param name="parameter2">The second action parameter.</param>
    /// <param name="parameter3">The third action parameter.</param>
    /// <param name="parameter4">The fourth action parameter.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Invoke(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, CancellationToken token);
}
