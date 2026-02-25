namespace OrderProcessor.Domain.Commands.Orders;

public record SuspendOrderCommand(Guid OrderId, string? SuspensionReason);
