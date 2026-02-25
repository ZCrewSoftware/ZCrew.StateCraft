using OrderProcessor.Domain.Commands.Lines;
using OrderProcessor.Domain.Models;

namespace OrderProcessor.Domain.Contracts;

public interface ILineCommandService
{
    Task<Line> SuspendLine(SuspendLineCommand command, CancellationToken token);
    Task<Line> ResumeLine(ResumeLineCommand command, CancellationToken token);
    Task<Line> CancelLine(CancelLineCommand command, CancellationToken token);
    Task<Line> CompleteLine(CompleteLineCommand command, CancellationToken token);
}
