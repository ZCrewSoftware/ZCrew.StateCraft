namespace ZCrew.StateCraft.Async.Contracts;

/// <summary>
///     A background unit of work with self-referential call detection. Used by both state actions
///     (when <see cref="StateMachineOptions.RunActionsAsynchronously"/> is set) and state-scoped
///     triggers. Disposing cancels the internal <see cref="CancellationTokenSource"/> without awaiting the
///     background task — used for deferred cleanup of self-referential workers.
/// </summary>
internal interface IBackgroundWorker : IAsyncDisposable
{
    /// <summary>
    ///     Starts the background work on a fire-and-forget task. The work delegate receives a cancellation token
    ///     from an internally-managed <see cref="CancellationTokenSource"/>; <paramref name="token"/> only guards
    ///     against cancellation before the work begins.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Activate(CancellationToken token);

    /// <summary>
    ///     Stops the background work. If the worker is being deactivated from within its own work
    ///     (e.g., a trigger calling <c>Transition</c> which exits the state that owns the trigger), deactivation defers
    ///     <see cref="CancellationTokenSource"/> cleanup to <see cref="IBackgroundDispatcher.Flush"/> and returns
    ///     immediately to avoid deadlocking. Otherwise, cancels the token, disposes the source, and awaits the
    ///     background task.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Deactivate(CancellationToken token);
}
