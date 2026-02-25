namespace OrderProcessor.Application.Context.Contracts;

internal interface IOrderContext
{
    IEnumerable<ILineContext> LineContexts { get; }

    Task Open(CancellationToken token);
    Task Suspend(string reason, CancellationToken token);
    Task Resume(CancellationToken token);
    Task Cancel(string reason, CancellationToken token);
}
