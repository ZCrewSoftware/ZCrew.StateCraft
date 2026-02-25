using OrderProcessor.Application.Context.Contracts;
using OrderProcessor.Domain.Commands.Orders;
using OrderProcessor.Domain.Contracts;
using OrderProcessor.Domain.Contracts.Data;
using OrderProcessor.Domain.Models;

namespace OrderProcessor.Application.Services;

internal class OrderCommandService : IOrderCommandService
{
    private readonly IOrderContextStore orderContextStore;
    private readonly IOrderDataService orderDataService;

    public OrderCommandService(IOrderContextStore orderContextStore, IOrderDataService orderDataService)
    {
        this.orderContextStore = orderContextStore;
        this.orderDataService = orderDataService;
    }

    public async Task<Order> CreateOrder(CreateOrderCommand command, CancellationToken token)
    {
        var order = await this.orderDataService.CreateOrder(command.Order, token);
        await this.orderContextStore.CreateOrderContext(order, token);
        return order;
    }

    public async Task<Order> OpenOrder(OpenOrderCommand command, CancellationToken token)
    {
        var context = await this.orderContextStore.GetOrderContext(command.OrderId, token);
        await context.Open(token);
        return await this.orderDataService.GetOrder(command.OrderId, token);
    }

    public async Task<Order> SuspendOrder(SuspendOrderCommand command, CancellationToken token)
    {
        var context = await this.orderContextStore.GetOrderContext(command.OrderId, token);
        await context.Suspend(command.SuspensionReason ?? "Suspended", token);
        return await this.orderDataService.GetOrder(command.OrderId, token);
    }

    public async Task<Order> ResumeOrder(ResumeOrderCommand command, CancellationToken token)
    {
        var context = await this.orderContextStore.GetOrderContext(command.OrderId, token);
        await context.Resume(token);
        return await this.orderDataService.GetOrder(command.OrderId, token);
    }

    public async Task<Order> CancelOrder(CancelOrderCommand command, CancellationToken token)
    {
        var context = await this.orderContextStore.GetOrderContext(command.OrderId, token);
        await context.Cancel(command.CancellationReason ?? "Canceled", token);
        return await this.orderDataService.GetOrder(command.OrderId, token);
    }
}
