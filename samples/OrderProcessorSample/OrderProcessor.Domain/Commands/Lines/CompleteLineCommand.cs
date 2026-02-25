namespace OrderProcessor.Domain.Commands.Lines;

public record CompleteLineCommand(Guid OrderId, Guid LineId);
