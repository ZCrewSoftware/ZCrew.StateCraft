using Microsoft.EntityFrameworkCore;
using OrderProcessor.Domain.Contracts.Data;
using OrderProcessor.Domain.Models;

namespace OrderProcessor.Infrastructure.Services;

internal class OrderDataService : IOrderDataService
{
    private readonly IDbContextFactory<OrderProcessorDbContext> dbContextFactory;

    public OrderDataService(IDbContextFactory<OrderProcessorDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async ValueTask<Order> CreateOrder(Order order, CancellationToken token)
    {
        await using var dbContext = await this.dbContextFactory.CreateDbContextAsync(token);
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(token);
        return order;
    }

    public async ValueTask<Order> GetOrder(Guid orderId, CancellationToken token)
    {
        await using var dbContext = await this.dbContextFactory.CreateDbContextAsync(token);
        return await dbContext.Orders.AsNoTracking().FirstAsync(o => o.Id == orderId, token);
    }

    public async ValueTask SaveOrder(Order order, CancellationToken token)
    {
        await using var dbContext = await this.dbContextFactory.CreateDbContextAsync(token);
        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(token);
    }
}
