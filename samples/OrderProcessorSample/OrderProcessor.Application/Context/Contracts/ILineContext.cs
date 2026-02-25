using OrderProcessor.Domain.Models;

namespace OrderProcessor.Application.Context.Contracts;

internal interface ILineContext
{
    Guid LineId { get; }
    Line Line { get; }

    Task Suspend(string? reason, CancellationToken token);
    Task Resume(CancellationToken token);
    Task Cancel(string? reason, CancellationToken token);
    Task Complete(CancellationToken token);
}
