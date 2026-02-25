namespace OrderProcessor.Domain.Commands.Lines;

public record SuspendLineCommand(Guid OrderId, Guid LineId, string? SuspensionReason);
