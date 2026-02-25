using OrderProcessor.Domain.Models;

namespace OrderProcessor.Domain.Contracts.Data;

public interface IOrderDataService
{
    ValueTask<Order> CreateOrder(Order order, CancellationToken token);
    ValueTask<Order> GetOrder(Guid orderId, CancellationToken token);
    ValueTask SaveOrder(Order order, CancellationToken token);
}
