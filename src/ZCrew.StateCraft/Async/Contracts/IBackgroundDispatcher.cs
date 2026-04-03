namespace ZCrew.StateCraft.Async.Contracts;

/// <summary>
///     Manages the lifecycle of <see cref="IBackgroundWorker"/> instances. Workers that are deactivated from within
///     their own execution context (self-referential deactivation) cannot safely cancel and await their own task, so
///     the dispatcher holds them for deferred disposal via <see cref="Flush"/>.
/// </summary>
internal interface IBackgroundDispatcher
{
    /// <summary>
    ///     Creates and activates a new <see cref="IBackgroundWorker"/> that runs <paramref name="backgroundWork"/>
    ///     on a fire-and-forget task with its own <see cref="CancellationTokenSource"/>.
    /// </summary>
    /// <param name="backgroundWork">
    ///     The work to execute in the background. Receives the worker's internal cancellation token, which is
    ///     canceled when the worker is deactivated.
    /// </param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A worker that can be used to stop the background work.</returns>
    Task<IBackgroundWorker> Dispatch(Func<CancellationToken, Task> backgroundWork, CancellationToken token);

    /// <summary>
    ///     Registers a worker for deferred disposal. Called by <see cref="IBackgroundWorker.Deactivate"/> when
    ///     self-referential deactivation is detected and the worker cannot safely dispose its own
    ///     <see cref="CancellationTokenSource"/> inline.
    /// </summary>
    /// <param name="worker">The worker to defer disposal for.</param>
    void Dispose(IBackgroundWorker worker);

    /// <summary>
    ///     Cancels and disposes all workers registered via <see cref="Dispose"/>. Called at safe points in the
    ///     state machine lifecycle (after entering a state, during deactivation) when no self-referential
    ///     execution contexts are on the call stack.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    Task Flush(CancellationToken token);
}
