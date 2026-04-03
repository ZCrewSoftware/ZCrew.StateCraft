using System.Diagnostics;
using ZCrew.StateCraft.Async.Contracts;

namespace ZCrew.StateCraft.Async;

/// <summary>
///     Runs a delegate on a fire-and-forget task and detects self-referential deactivation using an
///     <see cref="AsyncLocal{T}"/> call chain. Each worker pushes its ID onto the chain in <see cref="Activate"/>
///     so that <see cref="Deactivate"/> can check whether it is being called from within its own work. This
///     happens when a trigger or action calls <c>Transition</c>/<c>Deactivate</c> on the owning state machine,
///     causing the state machine to try to stop the very worker whose execution context is on the stack.
/// </summary>
internal class BackgroundWorker : IBackgroundWorker
{
    private static int workerId;

    /// <summary>
    ///     Per-execution-context stack of active worker IDs. Each fire-and-forget task inherits its own copy
    ///     via <see cref="AsyncLocal{T}"/> flow, so workers on independent tasks never see each other's IDs.
    ///     <see cref="Deactivate"/> checks whether this worker's ID is at the top of the stack to decide
    ///     whether cancellation and awaiting must be deferred.
    /// </summary>
    private static readonly AsyncLocal<int[]?> workerIds = new();

    private readonly IBackgroundDispatcher dispatcher;
    private readonly Func<CancellationToken, Task> backgroundWork;
    private readonly int id;
    private readonly object executeLock = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();

    private Task? executeTask;

    internal BackgroundWorker(IBackgroundDispatcher dispatcher, Func<CancellationToken, Task> backgroundWork)
    {
        this.dispatcher = dispatcher;
        this.backgroundWork = backgroundWork;
        this.id = Interlocked.Increment(ref workerId);
    }

    /// <inheritdoc />
    public Task Activate(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        lock (this.executeLock)
        {
            if (this.executeTask != null)
            {
                return Task.CompletedTask;
            }

            // Push this worker's ID before invoking the delegate so that the AsyncLocal value flows into the
            // fire-and-forget task's execution context. Any synchronous code in the delegate runs here under
            // executeLock; async continuations carry the ID forward independently.
            AddToAsynchronousCallChain();
            this.executeTask = this.backgroundWork(this.cancellationTokenSource.Token);
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task Deactivate(CancellationToken token)
    {
        Task task;
        lock (this.executeLock)
        {
            if (this.executeTask == null)
            {
                return;
            }

            task = this.executeTask;
            this.executeTask = null;

            // Self-referential: this worker's delegate (directly or via Transition/Deactivate) is trying to
            // stop itself. We cannot cancel-and-await our own task here without deadlocking, so defer the CTS
            // cleanup to the dispatcher. The task will observe cancellation once Flush runs at a safe point.
            if (RemoveFromAsynchronousCallChain())
            {
                this.dispatcher.Dispose(this);
                return;
            }
        }

        // Normal path: cancel the token, dispose the source, then await completion. SuppressThrowing avoids
        // re-raising exceptions that were already routed through the IExceptionBehavior by the caller's lambda.
        await DisposeAsync();
        await task.WaitAsync(token).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }

    /// <summary>
    ///     Pushes this worker's ID onto the <see cref="AsyncLocal{T}"/> call chain. Must be called on the
    ///     thread that will invoke the background delegate so the ID flows into its execution context.
    /// </summary>
    private void AddToAsynchronousCallChain()
    {
        var ids = workerIds.Value ?? [];
        workerIds.Value = ids.Append(this.id).ToArray();
    }

    /// <summary>
    ///     Checks whether this worker's ID is at the top of the call chain, indicating self-referential
    ///     deactivation. If found, pops the ID and returns <see langword="true"/>. Only the top element is
    ///     checked — nested workers deeper in the stack are not considered self-referential.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if this worker is being deactivated from within its own execution context.
    /// </returns>
    private bool RemoveFromAsynchronousCallChain()
    {
        var ids = workerIds.Value?.ToArray();
        if (ids == null || ids.Length == 0 || ids[^1] != this.id)
        {
            return false;
        }

        workerIds.Value = ids[..^1];
        return true;
    }

    /// <summary>
    ///     Cancels the internal <see cref="CancellationTokenSource"/> without awaiting the background task.
    ///     Used both in the normal deactivation path (before awaiting the task) and by
    ///     <see cref="IBackgroundDispatcher.Flush"/> for deferred cleanup of self-referential workers.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (this.cancellationTokenSource is { IsCancellationRequested: false })
        {
            try
            {
                await this.cancellationTokenSource.CancelAsync();
                this.cancellationTokenSource.Dispose();
            }
            catch (ObjectDisposedException)
            {
                Debug.Assert(false, "BackgroundWorker double-disposed");
            }
        }
    }
}
