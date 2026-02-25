using System.Collections.Concurrent;
using OrderProcessor.Application.Context.Contracts;
using OrderProcessor.Domain.Contracts.Data;
using OrderProcessor.Domain.Models;

namespace OrderProcessor.Application.Context;

internal class OrderContextStore : IOrderContextStore
{
    private readonly ConcurrentDictionary<Guid, OrderContext> contexts = new();
    private readonly IOrderDataService dataService;

    public OrderContextStore(IOrderDataService dataService)
    {
        this.dataService = dataService;
    }

    public async ValueTask<IOrderContext> CreateOrderContext(Order order, CancellationToken token)
    {
        var context = new OrderContext(order, this.dataService);
        this.contexts[order.Id] = context;
        await context.ActivateAsync(token);
        return context;
    }

    public ValueTask<IOrderContext> GetOrderContext(Guid orderId, CancellationToken token)
    {
        if (!this.contexts.TryGetValue(orderId, out var context))
        {
            throw new KeyNotFoundException($"Order context not found for order {orderId}.");
        }

        return ValueTask.FromResult<IOrderContext>(context);
    }
}
