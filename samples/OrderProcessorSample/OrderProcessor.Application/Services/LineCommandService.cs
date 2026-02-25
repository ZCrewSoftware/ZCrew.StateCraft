using OrderProcessor.Application.Context.Contracts;
using OrderProcessor.Domain.Commands.Lines;
using OrderProcessor.Domain.Contracts;
using OrderProcessor.Domain.Models;

namespace OrderProcessor.Application.Services;

internal class LineCommandService : ILineCommandService
{
    private readonly IOrderContextStore orderContextStore;

    public LineCommandService(IOrderContextStore orderContextStore)
    {
        this.orderContextStore = orderContextStore;
    }

    public async Task<Line> SuspendLine(SuspendLineCommand command, CancellationToken token)
    {
        var context = await this.orderContextStore.GetOrderContext(command.OrderId, token);
        var lineContext = context.LineContexts.First(l => l.LineId == command.LineId);
        await lineContext.Suspend(command.SuspensionReason, token);
        return lineContext.Line;
    }

    public async Task<Line> ResumeLine(ResumeLineCommand command, CancellationToken token)
    {
        var context = await this.orderContextStore.GetOrderContext(command.OrderId, token);
        var lineContext = context.LineContexts.First(l => l.LineId == command.LineId);
        await lineContext.Resume(token);
        return lineContext.Line;
    }

    public async Task<Line> CancelLine(CancelLineCommand command, CancellationToken token)
    {
        var context = await this.orderContextStore.GetOrderContext(command.OrderId, token);
        var lineContext = context.LineContexts.First(l => l.LineId == command.LineId);
        await lineContext.Cancel(command.CancellationReason, token);
        return lineContext.Line;
    }

    public async Task<Line> CompleteLine(CompleteLineCommand command, CancellationToken token)
    {
        var context = await this.orderContextStore.GetOrderContext(command.OrderId, token);
        var lineContext = context.LineContexts.First(l => l.LineId == command.LineId);
        await lineContext.Complete(token);
        return lineContext.Line;
    }
}
