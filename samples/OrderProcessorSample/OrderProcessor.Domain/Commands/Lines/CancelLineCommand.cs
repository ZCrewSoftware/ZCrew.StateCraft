namespace OrderProcessor.Domain.Commands.Lines;

public record CancelLineCommand(Guid OrderId, Guid LineId, string? CancellationReason);
