using OrderProcessor.Domain.Models;

namespace OrderProcessor.Domain.Commands.Orders;

public record CreateOrderCommand(Order Order);
