namespace OrderProcessor.Domain.Commands.Orders;

public record CancelOrderCommand(Guid OrderId, string? CancellationReason);
