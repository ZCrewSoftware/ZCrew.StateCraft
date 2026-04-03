using System.Collections.Concurrent;
using ZCrew.StateCraft.Async.Contracts;

namespace ZCrew.StateCraft.Async;

/// <inheritdoc />
internal sealed class BackgroundDispatcher : IBackgroundDispatcher
{
    private readonly ConcurrentBag<IBackgroundWorker> workersPendingDisposal = [];

    /// <inheritdoc />
    public async Task<IBackgroundWorker> Dispatch(Func<CancellationToken, Task> backgroundWork, CancellationToken token)
    {
        var worker = new BackgroundWorker(this, backgroundWork);
        await worker.Activate(token);

        return worker;
    }

    /// <inheritdoc />
    public void Dispose(IBackgroundWorker worker)
    {
        this.workersPendingDisposal.Add(worker);
    }

    /// <inheritdoc />
    public async Task Flush(CancellationToken token)
    {
        while (this.workersPendingDisposal.TryTake(out var worker))
        {
            await worker.DisposeAsync();
        }
    }
}
