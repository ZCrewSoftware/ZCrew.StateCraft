using OrderProcessor.Domain.Commands.Orders;
using OrderProcessor.Domain.Models;

namespace OrderProcessor.Domain.Contracts;

public interface IOrderCommandService
{
    Task<Order> CreateOrder(CreateOrderCommand command, CancellationToken token);
    Task<Order> OpenOrder(OpenOrderCommand command, CancellationToken token);
    Task<Order> SuspendOrder(SuspendOrderCommand command, CancellationToken token);
    Task<Order> ResumeOrder(ResumeOrderCommand command, CancellationToken token);
    Task<Order> CancelOrder(CancelOrderCommand command, CancellationToken token);
}
